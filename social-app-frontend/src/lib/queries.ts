import { api } from './api';
import type {
  Post,
  User,
  Comment,
  AuthTokens,
  LoginCredentials,
  RegisterData,
  CurrentUser,
  FeedPostDto,
} from '@/types';
// Auth APIs
export const authApi = {
  login: async (credentials: LoginCredentials): Promise<AuthTokens> => {
    const { data } = await api.post('/Auth/login', credentials);
    return {
      accessToken: data.accessToken ?? data.AccessToken,
      accessTokenExpiresAt: data.accessTokenExpiresAt ?? data.AccessTokenExpiresAt,
      refreshToken: data.refreshToken ?? data.RefreshToken,
      refreshTokenExpiresAt: data.refreshTokenExpiresAt ?? data.RefreshTokenExpiresAt,
    };
  },

  register: async (userData: RegisterData): Promise<{ message: string }> => {
    const { data } = await api.post('/Auth/register', userData);
    return data;
  },

  logout: async (): Promise<void> => {
    await api.post('/Auth/logout');
  },

  getCurrentUser: async (): Promise<CurrentUser> => {
    const { data } = await api.get('/Auth/me');
    return {
      userId: data.userId ?? data.UserId,
      firstName: data.firstName ?? data.FirstName,
      role: data.role ?? data.Role,
    };
  },
};

// User APIs
export const userApi = {
  getUser: async (userId: string): Promise<User> => {
    const { data } = await api.get(`/User/${userId}/getbyid`);
    const payload = data?.user ?? data?.User ?? data;
    return payload;
  },

  updateUser: async (userId: string, userData: Partial<User>): Promise<User> => {
    const { data } = await api.put(`/User/${userId}/update`, userData);
    return data;
  },

  getRecommendedUsers: async (pageSize = 5, pageNumber = 1) => {
    const { data } = await api.get('/User/recommended', {
      params: { pageSize, pageNumber },
    });
    const items: any[] = Array.isArray(data) ? data : data.data ?? data.Data ?? data?.user ?? [];
    return items.map((u) => ({
      id: u.id ?? u.Id,
      firstName: u.firstName ?? u.FirstName ?? "",
      lastName: u.lastName ?? u.LastName ?? "",
      isFollowedByMe: u.isFollowedByMe ?? u.IsFollowedByMe ?? false,
    }));
  },
};

// Profile APIs
export const profileApi = {
  getProfile: async (userId: string, pageSize = 10, pageNumber = 1) => {
    const { data } = await api.get('/Profile', {
      params: { pageSize, pageNumber },
    });
    const result = data?.data ?? data;
    
    return {
      header: {
        userId: result.headerDTO?.userId ?? result.headerDTO?.UserId ?? result.HeaderDTO?.UserId,
        firstName: result.headerDTO?.firstName ?? result.headerDTO?.FirstName ?? result.HeaderDTO?.FirstName ?? "",
        lastName: result.headerDTO?.lastName ?? result.headerDTO?.LastName ?? result.HeaderDTO?.LastName ?? "",
        userName: result.headerDTO?.userName ?? result.headerDTO?.UserName ?? result.HeaderDTO?.UserName,
        followersCount: result.headerDTO?.followersCount ?? result.headerDTO?.FollowersCount ?? result.HeaderDTO?.FollowersCount ?? 0,
        followingsCount: result.headerDTO?.followingsCount ?? result.headerDTO?.FollowingsCount ?? result.HeaderDTO?.FollowingsCount ?? 0,
      },
      postsCount: result.postsCount ?? result.PostsCount ?? 0,
      posts: (result.posts ?? result.Posts ?? []).map((p: any) => ({
        id: p.id ?? p.Id,
        content: p.content ?? p.Content ?? p.body ?? p.Body ?? "",
        createdAt: p.createdAt ?? p.CreatedAt ?? new Date().toISOString(),
        user: {
          id: p.user?.id ?? p.user?.Id ?? p.User?.Id ?? 0,
          firstName: p.user?.firstName ?? p.user?.FirstName ?? p.User?.FirstName ?? "",
          lastName: p.user?.lastName ?? p.user?.LastName ?? p.User?.LastName ?? "",
          userName: p.user?.userName ?? p.user?.UserName ?? p.User?.UserName,
        },
        likeCount: p.likeCount ?? p.LikeCount ?? 0,
        commentCount: p.commentCount ?? p.CommentCount ?? 0,
        postImages: p.postImages ?? p.PostImages ?? [],
        isLikedByMe: p.isLikedByMe ?? p.IsLikedByMe ?? false,
      })),
    };
  },
};

// Follow APIs
export const followApi = {
  followUser: async (followerId: number, followingId: number): Promise<void> => {
    await api.post('/Follow/Add', {
      followerId,
      followingId,
    });
  },

  getFollowers: async (userId: number, pageSize = 50, pageNumber = 1) => {
    const { data } = await api.get(`/Follow/followers/${userId}`, {
      params: { pageSize, pageNumber },
    });
    const items = data?.followers ?? data?.Followers ?? data?.data ?? data ?? [];
    return items;
  },

  findFollowId: async (followerId: number, followingId: number): Promise<number | null> => {
    const followers = await followApi.getFollowers(followingId, 50, 1);
    const match = followers.find(
      (f: any) =>
        (f.followerId ?? f.FollowerId) === followerId &&
        (f.followingId ?? f.FollowingId) === followingId
    );
    return match ? match.id ?? match.Id ?? null : null;
  },

  unfollowUser: async (followerId: number, followingId: number): Promise<void> => {
    const followId = await followApi.findFollowId(followerId, followingId);
    if (!followId) throw new Error("Takip kaydı bulunamadı.");
    await api.delete(`/Follow/${followId}/delete`);
  },

  checkFollowStatus: async (followerId: number, followingId: number): Promise<boolean> => {
    const followId = await followApi.findFollowId(followerId, followingId);
    return Boolean(followId);
  },
};

// Post APIs
export const postApi = {
  getFeed: async (pageSize = 10, pageNumber = 1): Promise<FeedPostDto[]> => {
    const { data } = await api.get("/Post/feed", {
      params: { pageSize, pageNumber },
    });
    const items: any[] = Array.isArray(data) ? data : data.items ?? data.data ?? [];
    return items.map((p) => ({
      id: p.id ?? p.Id,
      content: p.content ?? p.Content ?? p.body ?? p.Body ?? "",
      createdAt: p.createdAt ?? p.CreatedAt ?? new Date().toISOString(),
      user: {
        id: p.user?.id ?? p.user?.Id ?? 0,
        firstName: p.user?.firstName ?? p.user?.FirstName ?? "",
        lastName: p.user?.lastName ?? p.user?.LastName ?? "",
      },
      likeCount: p.likeCount ?? p.LikeCount ?? 0,
      commentCount: p.commentCount ?? p.CommentCount ?? 0,
      postImages: p.postImages ?? p.PostImages ?? [],
      postBrutals: p.postBrutals ?? p.PostBrutals ?? [],
      isLikedByMe: p.isLikedByMe ?? p.IsLikedByMe ?? false,
    }));
  },

  getPost: async (postId: string): Promise<Post> => {
    const { data } = await api.get(`/Post/${postId}/getbyid`);
    const payload = data?.post ?? data?.Post ?? data;
    return payload;
  },

  addPost: async (payload: { body: string; userId: number; status: number }) => {
    const { data } = await api.post("/Post/add", payload);
    return data; // { success, message, statusCode, data: { id, ... } }
  },
};

export const postImageApi = {
  addImage: async (payload: { file: string; postId: number }) => {
    const { data } = await api.post("/PostImage/add", payload);
    return data;
  },
};

// Comment APIs
export const commentApi = {
  getComments: async (postId: string): Promise<Comment[]> => {
    const { data } = await api.get(`/post/${postId}/comments`);
    return data;
  },

  createComment: async (postId: string, content: string): Promise<Comment> => {
    const { data } = await api.post(`/post/${postId}/comment`, { content });
    return data;
  },

  deleteComment: async (commentId: string): Promise<void> => {
    await api.delete(`/comment/${commentId}`);
  },

  likeComment: async (commentId: string): Promise<void> => {
    await api.post(`/comment/${commentId}/like`);
  },

  unlikeComment: async (commentId: string): Promise<void> => {
    await api.delete(`/comment/${commentId}/like`);
  },
};
