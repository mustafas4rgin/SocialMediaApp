"use client";

import { useEffect, useMemo, useState } from "react";
import { useSearchParams } from "next/navigation";
import Link from "next/link";
import { formatDistanceToNow } from "date-fns";
import { postApi, postImageApi, postBrutalApi, userApi, profileApi } from "@/lib/queries";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Badge } from "@/components/ui/badge";
import { Search } from "lucide-react";

export default function SearchPage() {
  const params = useSearchParams();
  const initial = params.get("query") ?? "";
  const [query, setQuery] = useState(initial);
  const [loading, setLoading] = useState(false);
  const [users, setUsers] = useState<any[]>([]);
  const [posts, setPosts] = useState<any[]>([]);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    setQuery(initial);
  }, [initial]);

  const normalized = useMemo(() => query.trim().toLowerCase(), [query]);

  const runSearch = async () => {
    const term = query.trim();
    if (!term) return;
    setLoading(true);
    setError(null);
    try {
      const userResults = await userApi.searchUsers(term);

      // Posts search (front filter)
      const feed = await postApi.getFeed(200, 1);
      const filteredPosts = feed.filter((p) =>
        (p.content ?? "").toLowerCase().includes(term.toLowerCase()) ||
        (p.user?.userName ?? "").toLowerCase().includes(term.toLowerCase())
      );
      const withMedia = await Promise.all(
        filteredPosts.map(async (p) => {
          const imgs = !p.postImages?.length ? await postImageApi.getImages(p.id) : p.postImages;
          const vids = !p.postBrutals?.length ? await postBrutalApi.getVideos(p.id) : p.postBrutals;
          return { ...p, postImages: imgs, postBrutals: vids };
        })
      );

      // Username exact match üste
      const exactUsers = userResults.filter((u) => u.userName.toLowerCase() === term.toLowerCase());
      const otherUsers = userResults.filter((u) => u.userName.toLowerCase() !== term.toLowerCase());

      // Hiç user bulamazsak username ile profil endpoint'i dene
      let combined = [...exactUsers, ...otherUsers];
      if (combined.length === 0 && term.length >= 2) {
        try {
          const prof = await profileApi.getProfile(term);
          const h = prof.header;
          if (h?.userName) {
            combined = [
              {
                id: h.userId,
                firstName: h.firstName ?? "",
                lastName: h.lastName ?? "",
                userName: h.userName,
              },
            ];
          }
        } catch {
          // ignore
        }
      }

      setUsers(combined);
      setPosts(withMedia);
    } catch (e: any) {
      setError(e?.message ?? "Arama başarısız oldu.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (normalized) {
      runSearch();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [normalized]);

  const renderPostMedia = (post: any) => {
    const media = [...(post.postImages ?? []), ...(post.postBrutals ?? [])];
    if (!media.length) return null;
    return (
      <div className="mt-3 grid grid-cols-1 gap-3 rounded-2xl overflow-hidden border border-white/10">
        {media.map((m: any, idx: number) => {
          const src = typeof m === "string" ? m : m.file ?? m.File ?? "";
          const isVideo =
            src.endsWith(".mp4") ||
            src.includes("/video/upload") ||
            src.includes(".webm") ||
            src.includes(".mov");
          return isVideo ? (
            <video key={idx} controls className="w-full max-h-[320px] rounded-xl bg-black/30" src={src} />
          ) : (
            <img key={idx} src={src} alt="media" className="w-full max-h-[320px] object-cover rounded-xl" />
          );
        })}
      </div>
    );
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-950 to-slate-900 text-slate-50">
      <div className="container max-w-5xl mx-auto px-4 py-10 space-y-6">
        <div className="flex items-center gap-3">
          <div className="relative flex-1">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
            <Input
              placeholder="Search users or posts..."
              className="pl-10 bg-white/5 border-white/10"
              value={query}
              onChange={(e) => setQuery(e.target.value)}
              onKeyDown={(e) => {
                if (e.key === "Enter") runSearch();
              }}
            />
          </div>
          <Button onClick={runSearch} disabled={loading}>
            Search
          </Button>
        </div>

        {error && (
          <div className="rounded-xl border border-destructive/30 bg-destructive/10 px-4 py-3 text-sm text-destructive">
            {error}
          </div>
        )}

        {/* Users */}
        <div className="space-y-3">
          <div className="flex items-center justify-between">
            <h2 className="text-lg font-semibold text-white">Users</h2>
            {loading && <span className="text-xs text-slate-400">Loading...</span>}
          </div>
          {users.length === 0 ? (
            <div className="text-sm text-slate-400">No users found.</div>
          ) : (
            <div className="space-y-2">
              {users.map((u) => {
                const name = `${u.firstName} ${u.lastName}`.trim() || u.userName;
                const initials = `${u.firstName?.[0] ?? ""}${u.lastName?.[0] ?? ""}`.toUpperCase() || u.userName?.[0]?.toUpperCase();
                const href = u.userName ? `/${u.userName}` : `/${u.id}`;
                return (
                  <Link
                    key={`${u.id}-${u.userName}`}
                    href={href}
                    className="flex items-center gap-3 rounded-xl border border-white/10 bg-white/5 px-4 py-3 hover:bg-white/10 transition"
                  >
                    <Avatar className="w-10 h-10">
                      <AvatarFallback className="bg-brand/20 text-brand">{initials}</AvatarFallback>
                    </Avatar>
                    <div className="flex flex-col">
                      <span className="text-sm font-semibold text-white">{name || "User"}</span>
                      {u.userName && <span className="text-xs text-slate-400">@{u.userName}</span>}
                    </div>
                  </Link>
                );
              })}
            </div>
          )}
        </div>

        {/* Posts */}
        <div className="space-y-3 pt-4">
          <div className="flex items-center justify-between">
            <h2 className="text-lg font-semibold text-white">Posts</h2>
            {loading && <span className="text-xs text-slate-400">Loading...</span>}
          </div>
          {posts.length === 0 ? (
            <div className="text-sm text-slate-400">No posts found.</div>
          ) : (
            <div className="space-y-4">
              {posts.map((post) => {
                const displayName = `${post.user?.firstName ?? ""} ${post.user?.lastName ?? ""}`.trim();
                const initials = `${post.user?.firstName?.[0] ?? ""}${post.user?.lastName?.[0] ?? ""}`.toUpperCase();
                const href = post.user?.userName ? `/${post.user.userName}` : `/${post.user?.id ?? post.user?.userId ?? ""}`;
                return (
                  <Card key={post.id} className="border-white/10 bg-white/5 backdrop-blur">
                    <CardContent className="p-4 space-y-3">
                      <div className="flex items-start gap-3">
                        <Avatar className="w-10 h-10">
                          <AvatarFallback className="bg-brand/20 text-brand">{initials || "U"}</AvatarFallback>
                        </Avatar>
                        <div className="flex-1">
                          <div className="flex items-center gap-2">
                            <Link href={href} className="text-sm font-semibold text-white hover:underline">
                              {displayName || "User"}
                            </Link>
                            {post.user?.userName && (
                              <span className="text-xs text-slate-400">@{post.user.userName}</span>
                            )}
                            <Badge variant="secondary" className="bg-white/10 text-white border border-white/10">
                              Match
                            </Badge>
                          </div>
                          <div className="text-xs text-slate-400">
                            {post.createdAt && formatDistanceToNow(new Date(post.createdAt), { addSuffix: true })}
                          </div>
                        </div>
                      </div>
                      <p className="text-sm text-slate-100 leading-relaxed">{post.content}</p>
                      {renderPostMedia(post)}
                    </CardContent>
                  </Card>
                );
              })}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
