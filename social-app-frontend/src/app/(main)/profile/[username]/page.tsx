"use client";

import { useEffect, useMemo, useState } from "react";
import { useParams } from "next/navigation";
import { formatDistanceToNow } from "date-fns";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Separator } from "@/components/ui/separator";
import { profileApi, followApi } from "@/lib/queries";
import { Calendar, Link as LinkIcon, MapPin, Sparkles, Heart, MessageCircle } from "lucide-react";
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
  const targetUserId = Number(routeParam) || currentUserId;
  const isOwnProfile = currentUserId && profileData?.header.userId === currentUserId;

  useEffect(() => {
    const fetchProfile = async () => {
      setLoading(true);
      setError(null);
      try {
        const data = await profileApi.getProfile(String(targetUserId), 10, 1);
        setProfileData(data);

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

    if (targetUserId) {
      fetchProfile();
    } else {
      setError("Please log in to view profiles.");
      setLoading(false);
    }
  }, [currentUserId, targetUserId]);

  const handleFollowToggle = async () => {
    if (!profileData || !currentUserId || isOwnProfile) return;

    setFollowLoading(true);
    try {
      if (isFollowing) {
        await followApi.unfollowUser(currentUserId, profileData.header.userId);
        setIsFollowing(false);
        setProfileData((prev) =>
          prev
            ? {
                ...prev,
                header: {
                  ...prev.header,
                  followersCount: prev.header.followersCount - 1,
                },
              }
            : null
        );
      } else {
        await followApi.followUser(currentUserId, profileData.header.userId);
        setIsFollowing(true);
        setProfileData((prev) =>
          prev
            ? {
                ...prev,
                header: {
                  ...prev.header,
                  followersCount: prev.header.followersCount + 1,
                },
              }
            : null
        );
      }
    } catch (e: any) {
      console.error("Failed to toggle follow:", e);
    } finally {
      setFollowLoading(false);
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
                      {profileData?.header.userName ? `@${profileData.header.userName}` : profileData ? `@user-${profileData.header.userId}` : "@user"}
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

                          <div className="flex items-center gap-4 mt-3 text-slate-400">
                            <button className="flex items-center gap-1 hover:text-red-400 transition-colors">
                              <Heart className="w-4 h-4" />
                              <span className="text-xs">{post.likeCount}</span>
                            </button>
                            <button className="flex items-center gap-1 hover:text-blue-400 transition-colors">
                              <MessageCircle className="w-4 h-4" />
                              <span className="text-xs">{post.commentCount}</span>
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
