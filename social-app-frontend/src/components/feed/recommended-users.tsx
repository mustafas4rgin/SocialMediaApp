"use client";

import { useEffect, useState } from "react";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { userApi } from "@/lib/queries";

type RecommendedUser = {
  id: number;
  firstName: string;
  lastName: string;
  isFollowedByMe?: boolean;
};

export function RecommendedUsers() {
  const [users, setUsers] = useState<RecommendedUser[]>([]);
  const [following, setFollowing] = useState<Set<number>>(new Set());

  useEffect(() => {
    const fetchRecommended = async () => {
      try {
        const data: RecommendedUser[] = await userApi.getRecommendedUsers(5, 1);
        setUsers(data);
        const initialFollowing = data
          .filter((u) => u.isFollowedByMe)
          .map((u) => u.id);
        setFollowing(new Set(initialFollowing));
      } catch (err) {
        console.error("Recommended users error:", err);
      }
    };

    fetchRecommended();
  }, []);

  const toggleFollow = (userId: number) => {
    setFollowing((prev) => {
      const newSet = new Set(prev);
      if (newSet.has(userId)) {
        newSet.delete(userId);
      } else {
        newSet.add(userId);
      }
      return newSet;
    });
  };

  return (
    <Card className="sticky top-6 rounded-3xl border border-white/10 bg-white/5 shadow-2xl backdrop-blur">
      <CardHeader>
        <CardTitle className="text-lg">Suggested For You</CardTitle>
      </CardHeader>
      <CardContent className="space-y-4">
        {users.length === 0 && (
          <p className="text-sm text-muted-foreground">No suggestions right now.</p>
        )}
        {users.map((user) => (
          <div key={user.id} className="flex items-center gap-3">
            <Avatar className="w-12 h-12 border border-white/10 bg-white/10">
              <AvatarImage src={`https://i.pravatar.cc/150?u=${user.firstName}${user.lastName}`} alt={`${user.firstName} ${user.lastName}`} />
              <AvatarFallback className="bg-brand/15 text-brand">
                {user.firstName[0]}{user.lastName[0]}
              </AvatarFallback>
            </Avatar>
            <div className="flex-1 min-w-0">
              <h4 className="font-medium text-sm truncate">
                {user.firstName} {user.lastName}
              </h4>
            </div>
            <Button
              size="sm"
              variant={following.has(user.id) ? "outline" : "default"}
              onClick={() => toggleFollow(user.id)}
              className={following.has(user.id) ? '' : 'bg-brand hover:bg-brand-dark'}
            >
              {following.has(user.id) ? "Following" : "Follow"}
            </Button>
          </div>
        ))}
      </CardContent>
    </Card>
  );
}
