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

type AnyObj = Record<string, any>;

function pickHandle(obj: AnyObj | null | undefined) {
  const raw = String(obj?.userName ?? obj?.username ?? "").trim();
  if (!raw) return "";
  // sadece rakamsa handle değildir -> "/14" bug'ını engeller
  if (/^\d+$/.test(raw)) return "";
  return raw;
}

export default function SearchPage() {
  const params = useSearchParams();
  const initial = params.get("query") ?? "";
  const [query, setQuery] = useState(initial);
  const [loading, setLoading] = useState(false);
  const [users, setUsers] = useState<any[]>([]);
  const [posts, setPosts] = useState<any[]>([]);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => setQuery(initial), [initial]);

  const normalized = useMemo(() => query.trim().toLowerCase(), [query]);

  const runSearch = async () => {
    const term = query.trim();
    if (!term) return;

    setLoading(true);
    setError(null);

    try {
      // ---- USERS ----
      const userResults = await userApi.searchUsers(term);

      // Username exact match üste (ama sayısal username'i de ele)
      const exactUsers = userResults.filter((u: AnyObj) => {
        const h = pickHandle(u);
        return h && h.toLowerCase() === term.toLowerCase();
      });

      const otherUsers = userResults.filter((u: AnyObj) => {
        const h = pickHandle(u);
        return h && h.toLowerCase() !== term.toLowerCase();
      });

      // combined: önce exact, sonra diğerleri
      let combined: AnyObj[] = [...exactUsers, ...otherUsers];

      // Hiç user bulamazsak username ile profil endpoint'i dene
      if (combined.length === 0 && term.length >= 2) {
        try {
          const prof = await profileApi.getProfile(term);
          const h = prof?.header;
          const handleFromProfile = pickHandle(h);

          if (handleFromProfile) {
            combined = [
              {
                id: h?.userId,
                firstName: h?.firstName ?? "",
                lastName: h?.lastName ?? "",
                userName: handleFromProfile,
              },
            ];
          }
        } catch {
          // ignore
        }
      }

      // Kullanıcı adını (handle) olmayan / sayısal gelen sonuçlar için handle'ı düzelt
      const enriched = await Promise.all(
        combined.map(async (u: AnyObj) => {
          // eğer u üzerinde geçerli handle varsa direkt dön
          const direct = pickHandle(u);
          if (direct) return { ...u, userName: direct };

          // id yoksa bir şey yapamayız
          if (!u?.id) return u;

          // 1) Profil endpointinden (id ile) dene (bazı backendler id kabul ediyor)
          try {
            const prof = await profileApi.getProfile(String(u.id));
            const h = prof?.header;
            const handleFromProfile = pickHandle(h);

            if (handleFromProfile) {
              return {
                ...u,
                userName: handleFromProfile,
                firstName: u.firstName || h?.firstName || "",
                lastName: u.lastName || h?.lastName || "",
              };
            }
          } catch {
            // ignore
          }

          // 2) user/getbyid ile gerçek username'i çek
          try {
            const detail = await userApi.getUser(String(u.id));
            const handleFromDetail =
              pickHandle(detail) || pickHandle((detail as AnyObj)?.user) || String((detail as AnyObj)?.UserName ?? "").trim();

            if (handleFromDetail) {
              return {
                ...u,
                userName: handleFromDetail,
                firstName:
                  u.firstName ||
                  (detail as AnyObj)?.firstName ||
                  (detail as AnyObj)?.FirstName ||
                  "",
                lastName:
                  u.lastName ||
                  (detail as AnyObj)?.lastName ||
                  (detail as AnyObj)?.LastName ||
                  "",
              };
            }
          } catch {
            // ignore
          }

          return u;
        })
      );

      // username'i olmayan (veya sayısal) kullanıcıları düşür — route username bekliyor
      const withHandle = enriched.filter((u: AnyObj) => pickHandle(u));

      // ---- POSTS ----
      const feed = await postApi.getFeed(200, 1);

      const filteredPosts = feed.filter((p: AnyObj) => {
        const contentOk = String(p.content ?? "").toLowerCase().includes(term.toLowerCase());
        const handleOk = String(p.user?.userName ?? p.user?.username ?? "").toLowerCase().includes(term.toLowerCase());
        return contentOk || handleOk;
      });

      const withMedia = await Promise.all(
        filteredPosts.map(async (p: AnyObj) => {
          const imgs = !p.postImages?.length ? await postImageApi.getImages(p.id) : p.postImages;
          const vids = !p.postBrutals?.length ? await postBrutalApi.getVideos(p.id) : p.postBrutals;
          return { ...p, postImages: imgs, postBrutals: vids };
        })
      );

      setUsers(withHandle);
      setPosts(withMedia);
    } catch (e: any) {
      setError(e?.message ?? "Arama başarısız oldu.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (normalized) runSearch();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [normalized]);

  const renderPostMedia = (post: AnyObj) => {
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
              placeholder="Search users or posts."
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
                 console.log("USER OBJ:", u); // ✅ burada u var
                const handle = pickHandle(u); // ✅ sayısal gelmez
                console.log("HANDLE:", handle, "ID:", u.id); // ✅ burada da var
                const name = `${u.firstName ?? ""} ${u.lastName ?? ""}`.trim() || handle || "User";
                const initials =
                  (`${u.firstName?.[0] ?? ""}${u.lastName?.[0] ?? ""}`.toUpperCase() ||
                    handle?.[0]?.toUpperCase() ||
                    "U");

                const href = `/${encodeURIComponent(handle)}`; // ✅ her zaman /username

                return (
                  <Link
                    key={`${u.id}-${handle}`}
                    href={href}
                    className="flex items-center gap-3 rounded-xl border border-white/10 bg-white/5 px-4 py-3 hover:bg-white/10 transition"
                  >
                    <Avatar className="w-10 h-10">
                      <AvatarFallback className="bg-brand/20 text-brand">{initials}</AvatarFallback>
                    </Avatar>
                    <div className="flex flex-col">
                      <span className="text-sm font-semibold text-white">{name}</span>
                      {handle && <span className="text-xs text-slate-400">@{handle}</span>}
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

                const postHandle = pickHandle(post.user);
                const postHref = postHandle
                  ? `/${encodeURIComponent(postHandle)}`
                  : "#"; // handle yoksa profil linki vermeyelim

                return (
                  <Card key={post.id} className="border-white/10 bg-white/5 backdrop-blur">
                    <CardContent className="p-4 space-y-3">
                      <div className="flex items-start gap-3">
                        <Avatar className="w-10 h-10">
                          <AvatarFallback className="bg-brand/20 text-brand">{initials || "U"}</AvatarFallback>
                        </Avatar>

                        <div className="flex-1">
                          <div className="flex items-center gap-2">
                            {postHandle ? (
                              <Link href={postHref} className="text-sm font-semibold text-white hover:underline">
                                {displayName || "User"}
                              </Link>
                            ) : (
                              <span className="text-sm font-semibold text-white">{displayName || "User"}</span>
                            )}

                            {postHandle && <span className="text-xs text-slate-400">@{postHandle}</span>}

                            <Badge variant="secondary" className="ml-auto">
                              {formatDistanceToNow(new Date(post.createdAt ?? post.created_at ?? Date.now()), {
                                addSuffix: true,
                              })}
                            </Badge>
                          </div>

                          {post.content && <p className="mt-2 text-sm text-slate-200 whitespace-pre-wrap">{post.content}</p>}
                          {renderPostMedia(post)}
                        </div>
                      </div>
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
