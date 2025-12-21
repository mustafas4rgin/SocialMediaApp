"use client";

import { useEffect, useState } from "react";
import { CreatePost } from "@/components/feed/create-post";
import { RecommendedUsers } from "@/components/feed/recommended-users";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Badge } from "@/components/ui/badge";
import { Heart, MessageCircle, Bookmark, MoreHorizontal } from "lucide-react";
import { postApi } from "@/lib/queries";

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
  const pageSize = 10;

  const avatarInitials = (user: FeedUserDto) =>
    `${user.firstName?.[0] ?? ""}${user.lastName?.[0] ?? ""}`.toUpperCase();

  const fetchFeed = async (page = 1, append = false) => {
    setIsLoading(true);
    try {
      const data = await postApi.getFeed(pageSize, page);

      // Bazı backendler {items: []} döndürebiliyor; ikisini de karşıla
      const items: FeedPostDto[] = Array.isArray(data) ? data : data.items ?? [];

      setPosts((prev) => (append ? [...prev, ...items] : items));
    } catch (err) {
      console.error("Feed error:", err);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchFeed(1, false);
  }, []);

  const handleLoadMore = async () => {
    const next = pageNumber + 1;
    setPageNumber(next);
    await fetchFeed(next, true);
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
                            <div className="font-semibold text-lg text-white">
                              {post.user.firstName} {post.user.lastName}
                            </div>
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

                    <div className="mt-4 flex items-center justify-between text-sm text-slate-200">
                      <div className="flex items-center gap-3">
                        <Button variant="ghost" size="sm" className="gap-2 text-slate-200 hover:text-white hover:bg-white/10">
                          <Heart className="w-5 h-5" />
                          <span>{post.likeCount}</span>
                        </Button>
                        <Button variant="ghost" size="sm" className="gap-2 text-slate-200 hover:text-white hover:bg-white/10">
                          <MessageCircle className="w-5 h-5" />
                          <span>{post.commentCount}</span>
                        </Button>
                      </div>
                      <div className="flex items-center gap-2">
                        <Button variant="ghost" size="sm" className="text-slate-200 hover:text-white hover:bg-white/10">
                          <Bookmark className="w-5 h-5" />
                        </Button>
                      </div>
                    </div>
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
