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
                firstName: u.firstName ?? u.FirstName ?? "",
                lastName: u.lastName ?? u.LastName ?? "",
                userName: u.userName ?? u.UserName ?? u.username,
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
      const items: FeedPostDto[] = Array.isArray(data) ? data : data.items ?? [];
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
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-950 to-slate-900 text-slate-50 transition-colors relative">
      <div className="absolute inset-0 pointer-events-none bg-[radial-gradient(circle_at_20%_20%,rgba(56,189,248,0.12),transparent_25%),radial-gradient(circle_at_80%_0%,rgba(168,85,247,0.14),transparent_25%),radial-gradient(circle_at_50%_80%,rgba(236,72,153,0.1),transparent_22%)]" />
      <div className="container relative max-w-7xl mx-auto px-4 py-10">
        <div className="grid grid-cols-1 lg:grid-cols-12 gap-8">
          {/* Main Feed */}
          <div className="lg:col-span-8 space-y-6">
            <CreatePost onCreated={() => fetchFeed(1, false)} />

            {isLoading && posts.length === 0 ? (
              <div className="space-y-6">
                {[1, 2, 3].map((i) => (
                  <div key={i} className="rounded-3xl p-6 space-y-4 bg-white/5 border border-white/10 shadow-2xl backdrop-blur">
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
              <div className="space-y-6">
                {error && (
                  <div className="rounded-xl border border-destructive/30 bg-destructive/10 px-4 py-3 text-sm text-destructive">
                    {error}
                  </div>
                )}
                {posts.map((post) => (
                  <div
                    key={post.id}
                    className="group relative overflow-hidden rounded-3xl border border-white/10 bg-white/5 p-6 shadow-2xl backdrop-blur transition hover:-translate-y-1 hover:border-white/20 hover:bg-white/10"
                  >
                    <div className="absolute inset-0 opacity-0 group-hover:opacity-100 transition pointer-events-none bg-gradient-to-r from-brand/10 via-transparent to-brand-dark/10" />
                    <div className="relative flex items-start justify-between gap-4">
                      <div className="flex items-start gap-3">
                        <Avatar className="w-12 h-12 bg-white/10 border border-white/10">
                          <AvatarFallback className="bg-brand/20 text-brand">{avatarInitials(post.user)}</AvatarFallback>
                        </Avatar>
                        <div>
                          <div className="flex items-center gap-2">
                            <Link
                              href={
                                post.user.userName
                                  ? `/${post.user.userName}`
                                  : `/${post.user.id}`
                              }
                              className="font-semibold text-lg text-white hover:underline"
                            >
                              {post.user.firstName} {post.user.lastName}
                            </Link>
                            {post.user.userName && (
                              <span className="text-xs text-slate-400">@{post.user.userName}</span>
                            )}
                            <Badge variant="secondary" className="bg-white/10 text-white border border-white/10">
                              Elite
                            </Badge>
                          </div>
                          <div className="text-xs text-slate-300">
                            {new Date(post.createdAt).toLocaleString()}
                          </div>
                        </div>
                      </div>
                      <Button variant="ghost" size="icon" className="text-slate-300 hover:text-white">
                        <MoreHorizontal className="w-5 h-5" />
                      </Button>
                    </div>

                    <div className="relative mt-4 rounded-2xl bg-white/5 p-4 text-base leading-relaxed text-slate-100">
                      {post.content}
                    </div>

                    {(post.postImages?.length || post.postBrutals?.length) ? (
                      <div className="mt-3 grid grid-cols-1 gap-3 rounded-2xl overflow-hidden border border-white/10">
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
                              className="w-full max-h-[420px] rounded-xl bg-black/30"
                              src={src}
                            />
                          ) : (
                            <img
                              key={idx}
                              src={src}
                              alt="Post media"
                              className="w-full max-h-[420px] object-cover rounded-xl"
                            />
                          );
                        })}
                      </div>
                    ) : null}

                    <div className="mt-4 flex items-center justify-between text-sm text-slate-200">
                      <div className="flex items-center gap-3">
                        <Button
                          variant="ghost"
                          size="sm"
                          className={`gap-2 ${
                            likeState[post.id]?.liked
                              ? "text-red-400"
                              : "text-slate-200 hover:text-white hover:bg-white/10"
                          }`}
                          onClick={() => handleLike(post.id)}
                          disabled={likeState[post.id]?.liked}
                        >
                          <Heart className={`w-5 h-5 ${likeState[post.id]?.liked ? "fill-current" : ""}`} />
                          <span>{likeState[post.id]?.count ?? post.likeCount}</span>
                        </Button>
                        <button
                          className="flex items-center gap-2 text-slate-200 hover:text-white transition-colors"
                          onClick={() => toggleComments(post.id)}
                        >
                          <MessageCircle className="w-5 h-5" />
                          <span>{post.commentCount}</span>
                          {openComments[post.id] ? <ChevronUp className="w-4 h-4" /> : <ChevronDown className="w-4 h-4" />}
                        </button>
                      </div>
                      <div className="flex items-center gap-2">
                        <Button variant="ghost" size="sm" className="text-slate-200 hover:text-white hover:bg-white/10">
                          <Bookmark className="w-5 h-5" />
                        </Button>
                      </div>
                    </div>
                    <div className="mt-3 flex items-center gap-2">
                      <input
                        type="text"
                        placeholder="Yorum ekle"
                        className="flex-1 rounded-full border border-white/10 bg-white/5 px-3 py-2 text-sm text-white placeholder:text-slate-400"
                        value={commentInputs[post.id] ?? ""}
                        onChange={(e) =>
                          setCommentInputs((prev) => ({ ...prev, [post.id]: e.target.value }))
                        }
                      />
                      <Button
                        variant="ghost"
                        size="icon"
                        className="text-white hover:bg-white/10"
                        onClick={() => handleComment(post.id)}
                      >
                        <Send className="w-5 h-5" />
                      </Button>
                    </div>

                    {/* Yorum önizlemesi (yorumlar kapalıyken ilk yorum) */}
                    {!openComments[post.id] && (comments[post.id]?.length ?? 0) > 0 && (
                      <div className="mt-3 rounded-2xl border border-white/10 bg-white/5 p-3 text-sm text-slate-200">
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
                              <div className="flex items-center justify-between text-xs text-slate-400">
                                <span>{displayName}</span>
                                {first.createdAt && (
                                  <span>
                                    {formatDistanceToNow(new Date(first.createdAt), { addSuffix: true })}
                                  </span>
                                )}
                              </div>
                              <p className="mt-1">{first.body ?? first.content ?? ""}</p>
                              <button
                                className="mt-2 text-xs text-brand hover:text-brand-dark"
                                onClick={() => toggleComments(post.id)}
                              >
                                Tüm yorumları gör
                              </button>
                            </>
                          );
                        })()}
                      </div>
                    )}

                    {openComments[post.id] && (
                      <div className="mt-3 space-y-3 rounded-2xl border border-white/10 bg-white/5 p-3">
                        {commentsLoading[post.id] ? (
                          <div className="text-slate-300 text-sm">Yorumlar yükleniyor...</div>
                        ) : (comments[post.id]?.length ?? 0) === 0 ? (
                          <div className="text-slate-400 text-sm">Hiç yorum yok. İlk yorumu yaz.</div>
                        ) : (
                          comments[post.id]?.map((c: any) => {
                            const commenter = c.user ?? {};
                            const userId = commenter.userId ?? commenter.UserId ?? commenter.id ?? c.userId ?? c.UserId;
                            const cached = userId ? userCache[userId] : null;
                            const first = commenter.firstName ?? commenter.FirstName ?? cached?.firstName ?? "";
                            const last = commenter.lastName ?? commenter.LastName ?? cached?.lastName ?? "";
                            const userName = commenter.userName ?? commenter.UserName ?? c.userName ?? cached?.userName;
                            const displayName = userName ? `@${userName}` : `${first} ${last}`.trim() || `User #${userId ?? "-"}`;
                            // Sadece username varsa link ver; id fallback'ını kaldırıyoruz (backend username bekliyor)
                            const profileHref = userName ? `/${userName}` : null;
                            return (
                              <div key={c.id} className="text-sm text-slate-100">
                                <div className="flex items-center gap-2 text-xs text-slate-400">
                                  {profileHref ? (
                                    <Link href={profileHref} className="hover:text-white">
                                      {displayName}
                                    </Link>
                                  ) : (
                                    <span>{displayName}</span>
                                  )}
                                  <span>
                                    {c.createdAt
                                      ? formatDistanceToNow(new Date(c.createdAt), { addSuffix: true })
                                      : ""}
                                  </span>
                                </div>
                                <p className="text-slate-100">{c.body ?? c.content ?? ""}</p>
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
                <Button variant="outline" size="lg" onClick={handleLoadMore}>
                  Load More Posts
                </Button>
              </div>
            )}
          </div>

          {/* Sidebar */}
          <div className="lg:col-span-4 space-y-6">
            <RecommendedUsers />

            <div className="rounded-3xl p-6 space-y-4 sticky top-6 border border-white/10 bg-white/5 shadow-2xl backdrop-blur">
              <h3 className="text-lg font-semibold text-white">Trending Topics</h3>
              <div className="space-y-3">
                {["#WebDevelopment", "#Design", "#Technology", "#Startup", "#AI"].map((tag) => (
                  <button
                    key={tag}
                    className="block w-full text-left px-4 py-3 rounded-2xl border border-white/10 bg-white/5 hover:bg-white/10 transition-colors"
                  >
                    <div className="font-medium text-brand">{tag}</div>
                    <div className="text-sm text-slate-200">2.5k posts</div>
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
