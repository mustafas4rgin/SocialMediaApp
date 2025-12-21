export interface User {
  id: string;
  username: string;
  email: string;
  firstName?: string;
  lastName?: string;
  bio?: string;
  profilePicture?: string;
  coverPhoto?: string;
  createdAt: string;
  updatedAt: string;
  followersCount?: number;
  followingCount?: number;
  postsCount?: number;
}

export interface Post {
  id: string;
  content: string;
  imageUrl?: string;
  userId: string;
  user: User;
  createdAt: string;
  updatedAt: string;
  likesCount: number;
  commentsCount: number;
  isLiked?: boolean;
}
export interface FeedUserDto {
  id: number;
  firstName: string;
  lastName: string;
  userName?: string;
}

export interface FeedPostDto {
  id: number;
  content: string;
  createdAt: string;
  user: FeedUserDto;
  likeCount: number;
  commentCount: number;
  postImages: any[];
  postBrutals: any[];
  isLikedByMe: boolean;
}


export interface Comment {
  id: string;
  content: string;
  postId: string;
  userId: string;
  user: User;
  createdAt: string;
  updatedAt: string;
  likesCount: number;
  isLiked?: boolean;
}

export interface Like {
  id: string;
  userId: string;
  postId?: string;
  commentId?: string;
  createdAt: string;
}

export interface Follow {
  id: string;
  followerId: string;
  followingId: string;
  createdAt: string;
}

export interface AuthTokens {
  accessToken: string;
  accessTokenExpiresAt?: string;
  refreshToken?: string;
  refreshTokenExpiresAt?: string;
}

export interface LoginCredentials {
  email: string;
  password: string;
}

export interface RegisterData {
  email: string;
  userName: string;
  firstName: string;
  lastName: string;
  password: string;
  passwordMatch: string;
}

export interface CurrentUser {
  userId: number;
  firstName: string;
  role: string;
}

export interface ProfileHeader {
  userId: number;
  firstName: string;
  lastName: string;
  userName?: string;
  followersCount: number;
  followingsCount: number;
}

export interface ProfileData {
  header: ProfileHeader;
  postsCount: number;
  posts: FeedPostDto[];
}
