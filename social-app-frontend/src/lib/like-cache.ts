export type LikeKey = string;

const likeKey = (postId: number, userId: number) => `${postId}:${userId}`;

// Basit bellek içi cache; oturum süresince tutulur
const likeIdCache = new Map<LikeKey, number>();

export function setLikeId(postId: number, userId: number, likeId: number) {
  likeIdCache.set(likeKey(postId, userId), likeId);
}

export function getLikeId(postId: number, userId: number): number | null {
  const v = likeIdCache.get(likeKey(postId, userId));
  return typeof v === "number" ? v : null;
}

export function clearLikeId(postId: number, userId: number) {
  likeIdCache.delete(likeKey(postId, userId));
}
