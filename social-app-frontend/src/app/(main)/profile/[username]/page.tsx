"use client";

import { useEffect, useMemo, useState } from "react";
import { useParams } from "next/navigation";
import { formatDistanceToNow } from "date-fns";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Separator } from "@/components/ui/separator";
import { profileApi, followApi, likeApi, commentApi, postImageApi, postBrutalApi, userApi } from "@/lib/queries";
import { Calendar, Link as LinkIcon, MapPin, Sparkles, Heart, MessageCircle, Send, ChevronDown, ChevronUp } from "lucide-react";
import type { ProfileData } from "@/types";

function initials(firstName?: string, lastName?: string) {
  return `${firstName?.[0] ?? ""}${lastName?.[0] ?? ""}`.toUpperCase();
}

export default function ProfilePage() {
  const params = useParams();
  const [profileData, setProfileData] = useState<ProfileData | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [isFollowing, setIsFollowing] = useState(false);
  const [followLoading, setFollowLoading] = useState(false);
  const [likeState, setLikeState] = useState<Record<number, { liked: boolean; count: number }>>({});
  const [commentInputs, setCommentInputs] = useState<Record<number, string>>({});
  const [comments, setComments] = useState<Record<number, any[]>>({});
  const [openComments, setOpenComments] = useState<Record<number, boolean>>({});
  const [commentsLoading, setCommentsLoading] = useState<Record<number, boolean>>({});
  const [userCache, setUserCache] = useState<Record<number, { firstName: string; lastName: string; userName?: string }>>({});

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
  const routeParam = Array.isArray(params?.username) ? params.username[0] : params?.username;
  const identifier = routeParam ? String(routeParam) : undefined; // username veya id string
  const targetIdentifier = identifier || (currentUserId ? String(currentUserId) : undefined);
  const isOwnProfile = currentUserId && profileData?.header.userId === currentUserId;

  // Yorumları çekip kullanıcı bilgilerini zenginleştiren yardımcı
  const fetchCommentsForPost = async (postId: number) => {
    const data = await commentApi.getComments(String(postId));
    const enriched = await Promise.all(
      (data ?? []).map(async (c: any) => {
        const commenter = c.user ?? {};
        const userId =
          commenter.userId ?? commenter.UserId ?? commenter.id ?? c.userId ?? c.UserId;
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
              // ignore
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

  useEffect(() => {
    const fetchProfile = async () => {
      setLoading(true);
      setError(null);
      try {
        const data = await profileApi.getProfile(targetIdentifier, 10, 1);
        // Fetch images per post
        const postsWithMedia = await Promise.all(
          (data.posts ?? []).map(async (p) => {
            const needsImages = !p.postImages || p.postImages.length === 0;
            const needsVideos = !p.postBrutals || p.postBrutals.length === 0;
            const imgs = needsImages ? await postImageApi.getImages(p.id) : p.postImages;
            const vids = needsVideos ? await postBrutalApi.getVideos(p.id) : p.postBrutals;
            return { ...p, postImages: imgs, postBrutals: vids };
          })
        );
        setProfileData({ ...data, posts: postsWithMedia });

        if (currentUserId && data.header.userId !== currentUserId) {
          const following = await followApi.checkFollowStatus(currentUserId, data.header.userId);
          setIsFollowing(following);
        }
      } catch (e: any) {
        const msg = e?.response?.data?.message ?? e?.message ?? "Failed to load profile.";
        setError(msg);
      } finally {
        setLoading(false);
      }
    };

    if (targetIdentifier) {
      fetchProfile();
    } else {
      setError("Please log in to view profiles.");
      setLoading(false);
    }
  }, [currentUserId, targetIdentifier]);

  useEffect(() => {
    if (!profileData?.posts) return;
    const next: Record<number, { liked: boolean; count: number }> = {};
    profileData.posts.forEach((p) => {
      next[p.id] = {
        liked: Boolean(p.isLikedByMe),
        count: p.likeCount ?? 0,
      };
    });
    setLikeState(next);
  }, [profileData]);

  // Yorum önizlemesi için profildeki postların yorumlarını önceden çek
  useEffect(() => {
    if (!profileData?.posts?.length) return;
    const preload = async () => {
      for (const p of profileData.posts) {
        if (comments[p.id]) continue;
        try {
          await fetchCommentsForPost(p.id);
        } catch (err) {
          console.error("Prefetch comments failed", err);
        }
      }
    };
    preload();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [profileData?.posts]);

  // Yorum önizlemesi için profildeki postların yorumlarını önceden çek
  useEffect(() => {
    if (!profileData?.posts?.length) return;
    const preload = async () => {
      for (const p of profileData.posts) {
        if (comments[p.id]) continue;
        try {
          const data = await commentApi.getComments(String(p.id));
          setComments((prev) => ({ ...prev, [p.id]: data ?? [] }));
        } catch (err) {
          console.error("Prefetch comments failed", err);
        }
      }
    };
    preload();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [profileData?.posts]);

  const handleFollowToggle = async () => {
    if (!profileData || !currentUserId || isOwnProfile) return;

    setFollowLoading(true);
    try {
      if (isFollowing) {
        await followApi.unfollowUser(currentUserId, profileData.header.userId);
        setIsFollowing(false);
        setProfileData((prev) =>
          prev
            ? { ...prev, header: { ...prev.header, followersCount: prev.header.followersCount - 1 } }
            : null
        );
      } else {
        await followApi.followUser(currentUserId, profileData.header.userId);
        setIsFollowing(true);
        setProfileData((prev) =>
          prev
            ? { ...prev, header: { ...prev.header, followersCount: prev.header.followersCount + 1 } }
            : null
        );
      }
    } catch (e: any) {
      console.error("Failed to toggle follow:", e);
    } finally {
      setFollowLoading(false);
    }
  };

  const handleLike = async (postId: number) => {
    if (!currentUserId) {
      setError("Please log in to like posts.");
      return;
    }
    const current = likeState[postId] || { liked: false, count: 0 };
    if (current.liked) return; // şimdilik unlike yok

    setLikeState((prev) => ({
      ...prev,
      [postId]: { liked: true, count: current.count + 1 },
    }));

    try {
      await likeApi.likePost(postId, currentUserId);
    } catch (e: any) {
      // geri al
      setLikeState((prev) => ({
        ...prev,
        [postId]: { liked: current.liked, count: current.count },
      }));
      console.error("Like failed:", e);
    }
  };

  const handleComment = async (postId: number) => {
    if (!currentUserId) {
      setError("Please log in to comment.");
      return;
    }
    const body = commentInputs[postId]?.trim();
    if (!body) return;

    setCommentInputs((prev) => ({ ...prev, [postId]: "" }));

    try {
      await commentApi.createComment(postId, currentUserId, body);
      // Yorum sayısını artır
      setProfileData((prev) =>
        prev
          ? {
              ...prev,
              posts: prev.posts.map((p) =>
                p.id === postId ? { ...p, commentCount: (p.commentCount ?? 0) + 1 } : p
              ),
            }
          : prev
      );
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
    } catch (e: any) {
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
    } catch (e: any) {
      console.error("Load comments failed:", e);
    } finally {
      setCommentsLoading((prev) => ({ ...prev, [postId]: false }));
    }
  };

  const fullName = profileData ? `${profileData.header.firstName} ${profileData.header.lastName}` : "—";

  if (loading && !profileData) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-950 to-slate-900 text-slate-50 flex items-center justify-center">
        <div className="text-slate-300">Loading profile...</div>
      </div>
    );
  }

  if (error && !profileData) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-950 to-slate-900 text-slate-50 flex items-center justify-center">
        <div className="rounded-2xl border border-white/10 bg-white/5 px-6 py-4 text-center text-slate-100">
          <div className="text-lg font-semibold mb-2">Profile not available</div>
          <div className="text-sm text-slate-300">{error}</div>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-950 to-slate-900 text-slate-50">
      <div className="absolute inset-0 pointer-events-none bg-[radial-gradient(circle_at_20%_20%,rgba(56,189,248,0.12),transparent_25%),radial-gradient(circle_at_80%_0%,rgba(168,85,247,0.14),transparent_25%),radial-gradient(circle_at_50%_80%,rgba(236,72,153,0.1),transparent_22%)]" />

      <div className="relative h-64 md:h-72 bg-gradient-to-r from-brand/20 via-brand-dark/10 to-brand/5" />

      <div className="container relative max-w-5xl mx-auto px-4 -mt-20 pb-12">
        <Card className="border-white/10 bg-white/5 backdrop-blur shadow-2xl">
          <CardContent className="p-6 md:p-8">
            <div className="flex flex-col md:flex-row gap-6 md:gap-8">
              <div className="relative">
                <Avatar className="w-28 h-28 md:w-32 md:h-32 border-4 border-slate-900">
                  <AvatarImage
                    src={`https://api.dicebear.com/8.x/initials/svg?seed=${encodeURIComponent(fullName)}`}
                    alt={fullName}
                  />
                  <AvatarFallback className="bg-brand/20 text-brand text-2xl">
                    {initials(profileData?.header.firstName, profileData?.header.lastName)}
                  </AvatarFallback>
                </Avatar>
                <span className="absolute -bottom-3 left-1/2 -translate-x-1/2 rounded-full bg-emerald-500 px-3 py-1 text-xs font-semibold text-white shadow-lg flex items-center gap-1">
                  <Sparkles className="w-3 h-3" />
                  Elite
                </span>
              </div>

              <div className="flex-1 space-y-4">
                <div className="flex flex-col md:flex-row md:items-start md:justify-between gap-4">
                  <div>
                    <h1 className="text-2xl md:text-3xl font-semibold text-white">{fullName}</h1>
                    <p className="text-slate-300 text-sm">
                      {profileData?.header.userName
                        ? `@${profileData.header.userName}`
                        : profileData
                        ? `@user-${profileData.header.userId}`
                        : "@user"}
                    </p>
                  </div>
                  {!isOwnProfile && (
                    <div className="flex gap-2">
                      <Button
                        variant={isFollowing ? "outline" : "default"}
                        onClick={handleFollowToggle}
                        disabled={followLoading}
                        className={
                          !isFollowing
                            ? "bg-gradient-to-r from-brand to-brand-dark text-white hover:from-brand-dark hover:to-brand"
                            : "border-white/30 text-white"
                        }
                      >
                        {followLoading ? "..." : isFollowing ? "Following" : "Follow"}
                      </Button>
                      <Button variant="outline" className="border-white/30 text-white">
                        Message
                      </Button>
                    </div>
                  )}
                </div>

                {error && (
                  <div className="rounded-xl border border-destructive/40 bg-destructive/20 px-4 py-3 text-sm text-destructive-foreground">
                    {error}
                  </div>
                )}

                <p className="text-sm leading-relaxed text-slate-200">
                  Welcome to my profile! 🚀
                </p>

                <div className="grid grid-cols-3 gap-3 text-center">
                  <div className="rounded-2xl border border-white/10 bg-white/5 px-3 py-3">
                    <div className="text-xl font-semibold text-white">
                      {profileData?.header.followersCount ?? 0}
                    </div>
                    <div className="text-xs uppercase tracking-wide text-slate-300">Followers</div>
                  </div>
                  <div className="rounded-2xl border border-white/10 bg-white/5 px-3 py-3">
                    <div className="text-xl font-semibold text-white">
                      {profileData?.header.followingsCount ?? 0}
                    </div>
                    <div className="text-xs uppercase tracking-wide text-slate-300">Following</div>
                  </div>
                  <div className="rounded-2xl border border-white/10 bg-white/5 px-3 py-3">
                    <div className="text-xl font-semibold text-white">
                      {profileData?.postsCount ?? 0}
                    </div>
                    <div className="text-xs uppercase tracking-wide text-slate-300">Posts</div>
                  </div>
                </div>
              </div>
            </div>
          </CardContent>
        </Card>

        <div className="mt-6 rounded-3xl border border-white/10 bg-white/5 backdrop-blur p-6 space-y-4 shadow-2xl">
          <div className="flex items-center justify-between">
            <div>
              <h2 className="text-xl font-semibold text-white">Posts</h2>
              <p className="text-sm text-slate-300">
                {profileData?.postsCount ?? 0} {profileData?.postsCount === 1 ? "post" : "posts"}
              </p>
            </div>
          </div>
          <Separator className="bg-white/10" />

          {loading ? (
            <div className="text-slate-300">Loading posts...</div>
          ) : profileData?.posts && profileData.posts.length > 0 ? (
            <div className="space-y-4">
              {profileData.posts.map((post) => {
                const displayFirst = post.user.firstName || profileData.header.firstName;
                const displayLast = post.user.lastName || profileData.header.lastName;
                const displayUserName = post.user.userName || profileData.header.userName;
                const displayName = `${displayFirst} ${displayLast}`.trim();

                return (
                  <Card key={post.id} className="border-white/10 bg-white/5 backdrop-blur">
                    <CardContent className="p-4 space-y-3">
                      <div className="flex items-start gap-3">
                        <Avatar className="w-10 h-10">
                          <AvatarImage
                            src={`https://api.dicebear.com/8.x/initials/svg?seed=${encodeURIComponent(displayName || "User")}`}
                            alt={displayName || "User"}
                          />
                          <AvatarFallback className="bg-brand/20 text-brand text-sm">
                            {initials(displayFirst, displayLast)}
                          </AvatarFallback>
                        </Avatar>
                        <div className="flex-1">
                          <div className="flex flex-wrap items-center gap-2">
                            <span className="font-semibold text-white text-sm">
                              {displayName || "Unknown"}
                            </span>
                            {displayUserName && <span className="text-xs text-slate-400">@{displayUserName}</span>}
                            <span className="text-xs text-slate-400">
                              {formatDistanceToNow(new Date(post.createdAt), { addSuffix: true })}
                            </span>
                          </div>
                          <p className="text-sm text-slate-200 mt-2">{post.content}</p>

                          {(post.postImages?.length || post.postBrutals?.length) ? (
                            <div className="mt-3 grid grid-cols-1 gap-3 rounded-2xl overflow-hidden border border-white/10">
                              {[...(post.postImages ?? []), ...(post.postBrutals ?? [])].map((media, idx) => {
                                const src =
                                  typeof media === "string"
                                    ? media
                                    : (media as any)?.file ?? (media as any)?.File ?? "";
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

                          <div className="flex items-center gap-4 mt-3 text-slate-400">
                            <button
                              className={`flex items-center gap-1 transition-colors ${likeState[post.id]?.liked ? "text-red-400" : "hover:text-red-400"}`}
                              onClick={() => handleLike(post.id)}
                              disabled={likeState[post.id]?.liked}
                            >
                              <Heart className={`w-4 h-4 ${likeState[post.id]?.liked ? "fill-current" : ""}`} />
                              <span className="text-xs">{likeState[post.id]?.count ?? post.likeCount ?? 0}</span>
                            </button>
                            <button
                              className="flex items-center gap-1 hover:text-blue-400 transition-colors"
                              onClick={() => toggleComments(post.id)}
                            >
                              <MessageCircle className="w-4 h-4" />
                              <span className="text-xs">{post.commentCount ?? 0}</span>
                              {openComments[post.id] ? <ChevronUp className="w-3 h-3" /> : <ChevronDown className="w-3 h-3" />}
                            </button>
                          </div>

                          {/* Yorum önizlemesi */}
                          {!openComments[post.id] && (comments[post.id]?.length ?? 0) > 0 && (
                            <div className="space-y-1 rounded-2xl border border-white/10 bg-white/5 p-3 mt-3 text-sm text-slate-200">
                              {(() => {
                                const firstComment = comments[post.id][0];
                                const commenter = firstComment.user ?? {};
                                const userId =
                                  commenter.userId ??
                                  commenter.UserId ??
                                  commenter.id ??
                                  firstComment.userId ??
                                  firstComment.UserId;
                                const cached = userId ? userCache[userId] : null;
                                const first =
                                  commenter.firstName ??
                                  commenter.FirstName ??
                                  cached?.firstName ??
                                  "";
                                const last =
                                  commenter.lastName ??
                                  commenter.LastName ??
                                  cached?.lastName ??
                                  "";
                                const userName =
                                  commenter.userName ??
                                  commenter.UserName ??
                                  firstComment.userName ??
                                  cached?.userName;
                                const displayName = userName
                                  ? `@${userName}`
                                  : `${first} ${last}`.trim() || `User #${userId ?? "-"}`;

                                return (
                                  <>
                                    <div className="flex items-center justify-between text-xs text-slate-400">
                                      <span>{displayName}</span>
                                      {firstComment.createdAt && (
                                        <span>
                                          {formatDistanceToNow(new Date(firstComment.createdAt), { addSuffix: true })}
                                        </span>
                                      )}
                                    </div>
                                    <p>{firstComment.body ?? firstComment.content ?? ""}</p>
                                    <button
                                      className="mt-1 text-xs text-brand hover:text-brand-dark"
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
                            <div className="space-y-3 rounded-2xl border border-white/10 bg-white/5 p-3 mt-3">
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
                                  const displayName = userName
                                    ? `@${userName}`
                                    : `${first} ${last}`.trim() || `User #${userId ?? "-"}`;
                                  const profileHref = userName ? `/${userName}` : null;
                                  return (
                                    <div key={c.id} className="text-sm text-slate-100">
                                      <div className="flex items-center gap-2 text-xs text-slate-400">
                                        {profileHref ? (
                                          <a href={profileHref} className="hover:text-white">
                                            {displayName}
                                          </a>
                                        ) : (
                                          <span>{displayName}</span>
                                        )}
                                        <span>
                                          {c.createdAt
                                            ? new Date(c.createdAt).toLocaleString()
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

                          <div className="flex items-center gap-2 mt-2">
                            <input
                              type="text"
                              placeholder="Add a comment"
                              className="flex-1 rounded-full bg-white/5 border border-white/10 px-3 py-2 text-sm text-white placeholder:text-slate-400"
                              value={commentInputs[post.id] ?? ""}
                              onChange={(e) =>
                                setCommentInputs((prev) => ({ ...prev, [post.id]: e.target.value }))
                              }
                            />
                            <button
                              className="p-2 rounded-full bg-brand text-white hover:bg-brand-dark transition-colors"
                              onClick={() => handleComment(post.id)}
                            >
                              <Send className="w-4 h-4" />
                            </button>
                          </div>
                        </div>
                      </div>
                    </CardContent>
                  </Card>
                );
              })}
            </div>
          ) : (
            <div className="text-slate-400 text-sm text-center py-8">
              No posts yet. Start sharing your thoughts!
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
