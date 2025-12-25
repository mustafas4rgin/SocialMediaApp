"use client";

import { useEffect, useMemo, useState } from "react";
import { formatDistanceToNow } from "date-fns";
import Link from "next/link";
import { CreatePost } from "@/components/feed/create-post";
import { RecommendedUsers } from "@/components/feed/recommended-users";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Badge } from "@/components/ui/badge";
import { Heart, MessageCircle, Bookmark, MoreHorizontal, Send, ChevronDown, ChevronUp } from "lucide-react";
import { postApi, postImageApi, postBrutalApi, likeApi, commentApi, userApi } from "@/lib/queries";
import { setLikeId, getLikeId, clearLikeId } from "@/lib/like-cache";

type FeedUserDto = {
  id: number;
  firstName: string;
  lastName: string;
  userName?: string;
};

type FeedPostDto = {
  id: number;
  content: string;
  createdAt: string;
  user: FeedUserDto;
  likeCount: number;
  commentCount: number;
  postImages: any[];
  postBrutals: any[];
  isLikedByMe: boolean;
};

export default function FeedPage() {
  const [posts, setPosts] = useState<FeedPostDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [pageNumber, setPageNumber] = useState(1);
  const [likeState, setLikeState] = useState<Record<number, { liked: boolean; count: number }>>({});
  const [commentInputs, setCommentInputs] = useState<Record<number, string>>({});
  const [comments, setComments] = useState<Record<number, any[]>>({});
  const [openComments, setOpenComments] = useState<Record<number, boolean>>({});
  const [commentsLoading, setCommentsLoading] = useState<Record<number, boolean>>({});
  const [userCache, setUserCache] = useState<Record<number, { firstName: string; lastName: string; userName?: string }>>({});
  const [error, setError] = useState<string | null>(null);
  const pageSize = 10;

  const avatarInitials = (user: FeedUserDto) =>
    `${user.firstName?.[0] ?? ""}${user.lastName?.[0] ?? ""}`.toUpperCase();

  const storedUser = useMemo(() => {
    if (typeof window === "undefined") return null;
    const raw = localStorage.getItem("user");
    if (!raw || raw === "undefined") return null;
    try {
      return JSON.parse(raw);
    } catch {
      return null;
    }
  }, []);

  const currentUserId = storedUser?.userId ?? storedUser?.id;

  // Yorumları çekip kullanıcı bilgilerini zenginleştiren yardımcı
  const fetchCommentsForPost = async (postId: number) => {
    const data = await commentApi.getComments(String(postId));
    const enriched = await Promise.all(
      (data ?? []).map(async (c: any) => {
        const commenter = c.user ?? {};
        const userId = commenter.userId ?? commenter.UserId ?? commenter.id ?? c.userId ?? c.UserId;
        let first = commenter.firstName ?? commenter.FirstName ?? "";
        let last = commenter.lastName ?? commenter.LastName ?? "";
        let userName = commenter.userName ?? commenter.UserName ?? c.userName;

        const numericId = Number(userId);
        const validId = Number.isFinite(numericId) && numericId > 0;

        if (validId) {
          const cached = userCache[userId];
          if (cached) {
            first = cached.firstName || first;
            last = cached.lastName || last;
            userName = cached.userName || userName;
          } else {
            try {
              const u = await userApi.getUser(String(userId));
              const info = {
                firstName: u.firstName ?? "",
                lastName: u.lastName ?? "",
                userName: u.username ?? "",
              };
              setUserCache((prev) => ({ ...prev, [userId]: info }));
              first = info.firstName || first;
              last = info.lastName || last;
              userName = info.userName || userName;
            } catch {
              // ignore lookup errors
            }
          }
        }

        return {
          ...c,
          userId: validId ? userId : undefined,
          userName,
          user: {
            ...commenter,
            id: validId ? userId : undefined,
            userId: validId ? userId : undefined,
            firstName: first,
            lastName: last,
            userName,
          },
        };
      })
    );
    setComments((prev) => ({ ...prev, [postId]: enriched }));
    return enriched;
  };

  const fetchFeed = async (page = 1, append = false) => {
    setIsLoading(true);
    setError(null);
    try {
      const data = await postApi.getFeed(pageSize, page);

      // Bazı backendler {items: []} döndürebiliyor; ikisini de karşıla
      const items: FeedPostDto[] = Array.isArray(data) ? data : (data as any)?.items ?? [];
      const withMedia = await Promise.all(
        items.map(async (p) => {
          const needsImages = !p.postImages || p.postImages.length === 0;
          const needsVideos = !p.postBrutals || p.postBrutals.length === 0;
          const imgs = needsImages ? await postImageApi.getImages(p.id) : p.postImages;
          const vids = needsVideos ? await postBrutalApi.getVideos(p.id) : p.postBrutals;
          return { ...p, postImages: imgs, postBrutals: vids };
        })
      );

      setPosts((prev) => (append ? [...prev, ...withMedia] : withMedia));
    } catch (err) {
      console.error("Feed error:", err);
      setError("Feed yüklenirken bir sorun oluştu.");
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchFeed(1, false);
  }, []);

  useEffect(() => {
    if (!posts.length) return;
    const next: Record<number, { liked: boolean; count: number }> = {};
    posts.forEach((p) => {
      next[p.id] = { liked: Boolean(p.isLikedByMe), count: p.likeCount ?? 0 };
    });
    setLikeState(next);
  }, [posts]);

  // Yorum önizlemesi için, yüklenen postların yorumlarını (en azından ilkini) otomatik çek
  useEffect(() => {
    if (!posts.length) return;
    const preload = async () => {
      for (const p of posts) {
        if (comments[p.id]) continue;
        try {
          await fetchCommentsForPost(p.id);
        } catch (err) {
          // sessizce geç; toggle sırasında tekrar denenecek
          console.error("Prefetch comments failed", err);
        }
      }
    };
    preload();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [posts]);

  const handleLoadMore = async () => {
    const next = pageNumber + 1;
    setPageNumber(next);
    await fetchFeed(next, true);
  };

  const handleLike = async (postId: number) => {
    if (!currentUserId) {
      setError("Beğenmek için lütfen giriş yapın.");
      return;
    }
    const current = likeState[postId] || { liked: false, count: 0 };

    // Unlike
    if (current.liked) {
      const likeId = getLikeId(postId, currentUserId);
      if (!likeId) return;

      setLikeState((prev) => ({
        ...prev,
        [postId]: { liked: false, count: Math.max(0, current.count - 1) },
      }));

      try {
        await likeApi.unlikePost(likeId);
        clearLikeId(postId, currentUserId);
      } catch (e) {
        setLikeState((prev) => ({
          ...prev,
          [postId]: { liked: true, count: current.count },
        }));
        console.error("Unlike failed:", e);
      }
      return;
    }

    // Like
    setLikeState((prev) => ({
      ...prev,
      [postId]: { liked: true, count: current.count + 1 },
    }));

    try {
      const res = await likeApi.likePost(postId, currentUserId);
      const likeId =
        res?.data?.id ??
        res?.data?.Id ??
        res?.data?.data?.id ??
        res?.data?.data?.Id ??
        res?.id ??
        res?.Id;
      if (likeId) setLikeId(postId, currentUserId, likeId);
    } catch (e) {
      setLikeState((prev) => ({
        ...prev,
        [postId]: { liked: current.liked, count: current.count },
      }));
      console.error("Like failed:", e);
    }
  };

  const handleComment = async (postId: number) => {
    if (!currentUserId) {
      setError("Yorum yapmak için lütfen giriş yapın.");
      return;
    }
    const body = commentInputs[postId]?.trim();
    if (!body) return;

    setCommentInputs((prev) => ({ ...prev, [postId]: "" }));
    try {
      await commentApi.createComment(postId, currentUserId, body);
      setPosts((prev) =>
        prev.map((p) =>
          p.id === postId ? { ...p, commentCount: (p.commentCount ?? 0) + 1 } : p
        )
      );
      // yorum listesini varsa güncelle
      setComments((prev) => ({
        ...prev,
        [postId]: [
          ...(prev[postId] ?? []),
          {
            id: Math.random(),
            content: body,
            body,
            createdAt: new Date().toISOString(),
            userId: currentUserId,
            userName: storedUser?.userName ?? storedUser?.name,
            user: {
              id: currentUserId,
              userId: currentUserId,
              userName: storedUser?.userName ?? storedUser?.name,
              firstName: storedUser?.firstName ?? storedUser?.name ?? "You",
              lastName: storedUser?.lastName ?? "",
            },
          },
        ],
      }));
    } catch (e) {
      console.error("Comment failed:", e);
      setError("Yorum eklenemedi. Lütfen tekrar deneyin.");
    }
  };

  const toggleComments = async (postId: number) => {
    const isOpen = openComments[postId];
    if (isOpen) {
      setOpenComments((prev) => ({ ...prev, [postId]: false }));
      return;
    }
    setOpenComments((prev) => ({ ...prev, [postId]: true }));
    if (comments[postId]) return;
    setCommentsLoading((prev) => ({ ...prev, [postId]: true }));
    try {
      await fetchCommentsForPost(postId);
    } catch (e) {
      console.error("Load comments failed:", e);
    } finally {
      setCommentsLoading((prev) => ({ ...prev, [postId]: false }));
    }
  };

  return (
    <div className="min-h-screen bg-background transition-colors">
      <div className="container max-w-7xl mx-auto px-4 py-6">
        <div className="grid grid-cols-1 lg:grid-cols-12 gap-8">
          {/* Main Feed */}
          <div className="lg:col-span-8 space-y-6">
            <CreatePost onCreated={() => fetchFeed(1, false)} />

            {isLoading && posts.length === 0 ? (
              <div className="space-y-4">
                {[1, 2, 3].map((i) => (
                  <div key={i} className="post-card p-6 space-y-4">
                    <div className="flex items-center gap-3">
                      <Skeleton className="w-12 h-12 rounded-full" />
                      <div className="space-y-2">
                        <Skeleton className="h-4 w-32" />
                        <Skeleton className="h-3 w-24" />
                      </div>
                    </div>
                    <Skeleton className="h-20 w-full" />
                    <Skeleton className="h-64 w-full rounded-xl" />
                  </div>
                ))}
              </div>
            ) : (
              <div className="space-y-4">
                {error && (
                  <div className="rounded-xl border border-destructive/30 bg-destructive/10 px-4 py-3 text-sm text-destructive">
                    {error}
                  </div>
                )}
                {posts.map((post) => (
                  <div
                    key={post.id}
                    className="post-card p-6 space-y-4"
                  >
                    <div className="flex items-start justify-between gap-4">
                      <div className="flex items-start gap-3">
                        <Avatar className="w-12 h-12 border-2 border-border ring-2 ring-background">
                          <AvatarFallback className="bg-gradient-to-br from-brand/20 to-brand-dark/20 text-brand font-semibold">{avatarInitials(post.user)}</AvatarFallback>
                        </Avatar>
                        <div>
                          <div className="flex items-center gap-2 flex-wrap">
                            <Link
                              href={
                                post.user.userName
                                  ? `/${post.user.userName}`
                                  : `/${post.user.id}`
                              }
                              className="font-semibold text-base text-foreground hover:text-primary transition-colors"
                            >
                              {post.user.firstName} {post.user.lastName}
                            </Link>
                            {post.user.userName && (
                              <span className="text-xs text-muted-foreground">@{post.user.userName}</span>
                            )}
                          </div>
                          <div className="text-xs text-muted-foreground">
                            {formatDistanceToNow(new Date(post.createdAt), { addSuffix: true })}
                          </div>
                        </div>
                      </div>
                      <Button variant="ghost" size="icon" className="text-muted-foreground hover:text-foreground hover:bg-muted">
                        <MoreHorizontal className="w-5 h-5" />
                      </Button>
                    </div>

                    <div className="text-base leading-relaxed text-foreground">
                      {post.content}
                    </div>

                    {(post.postImages?.length || post.postBrutals?.length) ? (
                      <div className="grid grid-cols-1 gap-2 rounded-xl overflow-hidden">
                        {[...(post.postImages ?? []), ...(post.postBrutals ?? [])].map((media, idx) => {
                          const src = typeof media === "string" ? media : media.file ?? media.File ?? "";
                          const isVideo =
                            src.endsWith(".mp4") ||
                            src.includes("/video/upload") ||
                            src.includes(".webm") ||
                            src.includes(".mov");
                          return isVideo ? (
                            <video
                              key={idx}
                              controls
                              className="w-full max-h-[500px] rounded-xl bg-muted"
                              src={src}
                            />
                          ) : (
                            <img
                              key={idx}
                              src={src}
                              alt="Post media"
                              className="w-full max-h-[500px] object-cover rounded-xl"
                            />
                          );
                        })}
                      </div>
                    ) : null}

                    <div className="flex items-center justify-between pt-2 border-t border-border/50">
                      <div className="flex items-center gap-1">
                        <Button
                          variant="ghost"
                          size="sm"
                          className={`gap-2 transition-colors ${
                            likeState[post.id]?.liked
                              ? "text-red-500 hover:text-red-600 hover:bg-red-50 dark:hover:bg-red-950/20"
                              : "text-muted-foreground hover:text-red-500 hover:bg-red-50/50 dark:hover:bg-red-950/10"
                          }`}
                          onClick={() => handleLike(post.id)}
                          disabled={likeState[post.id]?.liked}
                        >
                          <Heart className={`w-5 h-5 ${likeState[post.id]?.liked ? "fill-current" : ""}`} />
                          <span className="font-medium">{likeState[post.id]?.count ?? post.likeCount}</span>
                        </Button>
                        <Button
                          variant="ghost"
                          size="sm"
                          className="gap-2 text-muted-foreground hover:text-primary hover:bg-primary/10 transition-colors"
                          onClick={() => toggleComments(post.id)}
                        >
                          <MessageCircle className="w-5 h-5" />
                          <span className="font-medium">{post.commentCount}</span>
                          {openComments[post.id] ? <ChevronUp className="w-4 h-4" /> : <ChevronDown className="w-4 h-4" />}
                        </Button>
                      </div>
                      <div className="flex items-center gap-1">
                        <Button variant="ghost" size="sm" className="text-muted-foreground hover:text-primary hover:bg-primary/10 transition-colors">
                          <Bookmark className="w-5 h-5" />
                        </Button>
                      </div>
                    </div>
                    
                    <div className="flex items-center gap-2">
                      <input
                        type="text"
                        placeholder="Yorum ekle..."
                        className="flex-1 rounded-full border border-input bg-background px-4 py-2 text-sm placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-ring focus:border-transparent transition-all"
                        value={commentInputs[post.id] ?? ""}
                        onChange={(e) =>
                          setCommentInputs((prev) => ({ ...prev, [post.id]: e.target.value }))
                        }
                      />
                      <Button
                        variant="ghost"
                        size="icon"
                        className="text-primary hover:bg-primary/10 transition-colors"
                        onClick={() => handleComment(post.id)}
                      >
                        <Send className="w-5 h-5" />
                      </Button>
                    </div>

                    {/* Yorum önizlemesi (yorumlar kapalıyken ilk yorum) */}
                    {!openComments[post.id] && (comments[post.id]?.length ?? 0) > 0 && (
                      <div className="rounded-xl border border-border/50 bg-muted/30 p-3 text-sm">
                        {(() => {
                          const first = comments[post.id][0];
                          const commenter = first.user ?? {};
                          const userId =
                            commenter.userId ?? commenter.UserId ?? commenter.id ?? first.userId ?? first.UserId;
                          const cached = userId ? userCache[userId] : null;
                          const firstName =
                            commenter.firstName ?? commenter.FirstName ?? cached?.firstName ?? "";
                          const lastName =
                            commenter.lastName ?? commenter.LastName ?? cached?.lastName ?? "";
                          const userName =
                            commenter.userName ?? commenter.UserName ?? first.userName ?? cached?.userName;
                          const displayName = userName
                            ? `@${userName}`
                            : `${firstName} ${lastName}`.trim() || `User #${userId ?? "-"}`;
                          return (
                            <>
                              <div className="flex items-center justify-between text-xs text-muted-foreground">
                                <span className="font-medium">{displayName}</span>
                                {first.createdAt && (
                                  <span>
                                    {formatDistanceToNow(new Date(first.createdAt), { addSuffix: true })}
                                  </span>
                                )}
                              </div>
                              <p className="mt-1 text-foreground">{first.body ?? first.content ?? ""}</p>
                              <button
                                className="mt-2 text-xs text-primary hover:text-primary/80 font-medium transition-colors"
                                onClick={() => toggleComments(post.id)}
                              >
                                Tüm yorumları gör ({comments[post.id]?.length ?? 0})
                              </button>
                            </>
                          );
                        })()}
                      </div>
                    )}

                    {openComments[post.id] && (
                      <div className="space-y-3 rounded-xl border border-border/50 bg-muted/30 p-4">
                        {commentsLoading[post.id] ? (
                          <div className="text-muted-foreground text-sm">Yorumlar yükleniyor...</div>
                        ) : (comments[post.id]?.length ?? 0) === 0 ? (
                          <div className="text-muted-foreground text-sm">Hiç yorum yok. İlk yorumu yaz.</div>
                        ) : (
                          comments[post.id]?.map((c: any) => {
                            const commenter = c.user ?? {};
                            const userId = commenter.userId ?? commenter.UserId ?? commenter.id ?? c.userId ?? c.UserId;
                            const cached = userId ? userCache[userId] : null;
                            const first = commenter.firstName ?? commenter.FirstName ?? cached?.firstName ?? "";
                            const last = commenter.lastName ?? commenter.LastName ?? cached?.lastName ?? "";
                            const userName = commenter.userName ?? commenter.UserName ?? c.userName ?? cached?.userName;
                            const displayName = userName ? `@${userName}` : `${first} ${last}`.trim() || `User #${userId ?? "-"}`;
                            const profileHref = userName ? `/${userName}` : null;
                            return (
                              <div key={c.id} className="text-sm space-y-1">
                                <div className="flex items-center justify-between text-xs text-muted-foreground">
                                  {profileHref ? (
                                    <Link href={profileHref} className="font-medium hover:text-primary transition-colors">
                                      {displayName}
                                    </Link>
                                  ) : (
                                    <span className="font-medium">{displayName}</span>
                                  )}
                                  <span>
                                    {c.createdAt
                                      ? formatDistanceToNow(new Date(c.createdAt), { addSuffix: true })
                                      : ""}
                                  </span>
                                </div>
                                <p className="text-foreground">{c.body ?? c.content ?? ""}</p>
                              </div>
                            );
                          })
                        )}
                      </div>
                    )}

                  </div>
                ))}
              </div>
            )}

            {!isLoading && posts.length > 0 && (
              <div className="flex justify-center py-6">
                <Button 
                  variant="outline" 
                  size="lg" 
                  onClick={handleLoadMore}
                  className="border-border/50 hover:bg-primary hover:text-primary-foreground hover:border-primary transition-colors"
                >
                  Daha Fazla Gönderi Yükle
                </Button>
              </div>
            )}
          </div>

          {/* Sidebar */}
          <div className="lg:col-span-4 space-y-4">
            <RecommendedUsers />

            <div className="post-card p-6 space-y-4 sticky top-20">
              <h3 className="text-lg font-semibold text-heading">Trending Topics</h3>
              <div className="space-y-2">
                {["#WebDevelopment", "#Design", "#Technology", "#Startup", "#AI"].map((tag, idx) => (
                  <button
                    key={tag}
                    className="block w-full text-left px-4 py-3 rounded-xl border border-border/50 bg-muted/30 hover:bg-muted hover:border-primary/30 transition-all group"
                  >
                    <div className="font-medium text-primary group-hover:text-primary/80">{tag}</div>
                    <div className="text-xs text-muted-foreground">{2.5 + idx * 0.3}k posts</div>
                  </button>
                ))}
              </div>
            </div>
          </div>

        </div>
      </div>
    </div>
  );
}
