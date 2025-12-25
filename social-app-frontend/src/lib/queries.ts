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
    if (payload) {
      payload.userName = payload.userName ?? payload.UserName ?? payload.username;
    }
    return payload;
  },

  searchUsers: async (query: string, pageSize = 200) => {
    // Backend'te spesifik bir arama ucu yok; geniş bir liste çekip (pageSize) front-end filtre yapıyoruz.
    const { data } = await api.get("/User/GetAll", {
      params: { pageSize, pageNumber: 1 },
    });
    const items: any[] = data?.User ?? data?.user ?? data?.data ?? data ?? [];
    const normalizedQuery = query.trim().toLowerCase();

    const mapped = items.map((u) => ({
      id: u.id ?? u.Id,
      firstName: u.firstName ?? u.FirstName ?? "",
      lastName: u.lastName ?? u.LastName ?? "",
      userName: u.userName ?? u.UserName ?? u.username ?? "",
    }));

    // Eksik username'leri detay çağrısıyla tamamla (liste max 8)
    const enriched = await Promise.all(
      mapped.map(async (u) => {
        if (!u.userName && u.id) {
          try {
            const detail = await userApi.getUser(String(u.id));
            return {
              ...u,
              userName: detail.userName ?? detail.UserName ?? detail.username ?? "",
              firstName: detail.firstName ?? detail.FirstName ?? u.firstName,
              lastName: detail.lastName ?? detail.LastName ?? u.lastName,
            };
          } catch {
            return u;
          }
        }
        return u;
      })
    );

    let filtered = enriched
      .filter((u) => {
        const full = `${u.firstName} ${u.lastName}`.toLowerCase();
        return (
          u.userName.toLowerCase().includes(normalizedQuery) ||
          full.includes(normalizedQuery)
        );
      });

    // Eğer hâlâ sonuç yoksa, doğrudan profile endpoint'iyle username yakalamayı dene
    if (filtered.length === 0 && normalizedQuery.length >= 2) {
      try {
        const prof = await profileApi.getProfile(query);
        const h = prof.header;
        if (h?.userName) {
          filtered = [
            {
              id: h.userId,
              firstName: h.firstName ?? "",
              lastName: h.lastName ?? "",
              userName: h.userName,
            },
          ];
        }
      } catch {
        // yok say
      }
    }

    return filtered.slice(0, 8); // küçük bir sonuç listesi döndür
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
  getProfile: async (identifier?: string, pageSize = 10, pageNumber = 1) => {
    const url = identifier ? `/Profile/${identifier}` : "/Profile";
    const { data } = await api.get(url, {
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
        userName: p.user?.userName ?? p.user?.UserName ?? p.user?.username,
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
    if (data?.success === false) {
      throw new Error(data?.message ?? "Post image eklenemedi.");
    }
    return data;
  },
  getImages: async (postId: number) => {
    try {
      const { data } = await api.get(`/PostImage/post-images/${postId}`);
      const list =
        data?.data?.data ??
        data?.data ??
        data?.Data?.data ??
        data?.Data?.Data ??
        data?.Data ??
        data;
      if (typeof window !== "undefined") {
        console.info("Post images fetch", { postId, count: (list ?? []).length });
      }
      return (list ?? []).map((i: any) => i.file ?? i.File ?? "");
    } catch (e) {
      return [];
    }
  },
};

export const postBrutalApi = {
  addVideo: async (payload: { file: string; postId: number }) => {
    const { data } = await api.post("/PostBrutal/add", payload);
    if (data?.success === false) {
      throw new Error(data?.message ?? "Video eklenemedi.");
    }
    return data;
  },
  getVideos: async (postId: number) => {
    try {
      const { data } = await api.get(`/PostBrutal/post-brutals/${postId}`);
      const list =
        data?.data?.data ??
        data?.data ??
        data?.Data?.data ??
        data?.Data?.Data ??
        data?.Data ??
        data;
      return (list ?? []).map((i: any) => i.file ?? i.File ?? "");
    } catch {
      return [];
    }
  },
};

// Comment APIs
export const commentApi = {
  getComments: async (postId: string): Promise<Comment[]> => {
    try {
      const { data } = await api.get(`/Comment/post-comments/${postId}`, {
        params: { pageSize: 50, pageNumber: 1 },
      });
      return data?.comments ?? data?.Comments ?? data?.data ?? data ?? [];
    } catch (err: any) {
      // Backend 404 döndürüyorsa "yorum yok" anlamında boş listeyle dönelim
      if (err?.response?.status === 404) return [];
      throw err;
    }
  },

  createComment: async (postId: number, userId: number, body: string): Promise<Comment> => {
    // Bazı ortamlarda rota isimleri farklı olabildiği için küçük bir fallback zinciri ekliyoruz.
    const payload = { postId, userId, body };
    const tryEndpoints = ["/Comment/Add", "/Comment/add", "/Comment"];

    let lastError: any = null;
    for (const endpoint of tryEndpoints) {
      try {
        const { data } = await api.post(endpoint, payload);
        return data?.data ?? data;
      } catch (err) {
        lastError = err;
        // 404 ise bir sonraki endpoint'i dene
        if (err?.response?.status !== 404) break;
      }
    }
    throw lastError ?? new Error("Yorum eklenemedi.");
  },

  deleteComment: async (commentId: string): Promise<void> => {
    await api.delete(`/Comment/${commentId}/delete`);
  },
};

export const likeApi = {
  likePost: async (postId: number, userId: number) => {
    const { data } = await api.post("/Like/Add", { postId, userId });
    return data;
  },
  unlikePost: async (likeId: number) => {
    await api.delete(`/Like/${likeId}/delete`);
  },
};
