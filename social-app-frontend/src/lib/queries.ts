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
  NotificationItem,
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
  const { data } = await api.get("/User/GetAll", {
    params: { pageSize, pageNumber: 1 },
  });

  const items: any[] = data?.User ?? data?.user ?? data?.data ?? data ?? [];
  const normalizedQuery = query.trim().toLowerCase();

  const isValidUsername = (v: any) =>
    typeof v === "string" && v.trim() !== "" && !/^\d+$/.test(v);

  const mapped = items.map((u) => ({
    id: u.id ?? u.Id,
    firstName: u.firstName ?? u.FirstName ?? "",
    lastName: u.lastName ?? u.LastName ?? "",
    userName: isValidUsername(u.userName)
      ? u.userName
      : isValidUsername(u.UserName)
      ? u.UserName
      : isValidUsername(u.username)
      ? u.username
      : "", // ❗ numeric olanları SIFIRLA
  }));

  // username boş olanları ID ile resolve et
  const enriched = await Promise.all(
    mapped.map(async (u) => {
      if (u.userName || !u.id) return u;

      try {
        const detail = await userApi.getUser(String(u.id));
        const resolved =
          detail.userName ??
          (detail as any)?.UserName ??
          detail.username;

        if (isValidUsername(resolved)) {
          return {
            ...u,
            userName: resolved,
            firstName: detail.firstName ?? u.firstName,
            lastName: detail.lastName ?? u.lastName,
          };
        }
      } catch {}

      return u;
    })
  );

  const filtered = enriched.filter((u) => {
    if (!u.userName) return false;
    const full = `${u.firstName} ${u.lastName}`.toLowerCase();
    return (
      u.userName.toLowerCase().includes(normalizedQuery) ||
      full.includes(normalizedQuery)
    );
  });

  return filtered.slice(0, 8);
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
      userName: u.userName ?? u.UserName ?? u.username ?? "",
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

  updateProfile: async (payload: {
    userName?: string;
    firstName?: string;
    lastName?: string;
  }) => {
    const body: any = {};
    if (payload.userName) body.userName = payload.userName;
    if (payload.firstName) body.firstName = payload.firstName;
    if (payload.lastName) body.lastName = payload.lastName;
    const { data } = await api.post("/Profile/update", body);
    return data;
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
      } catch (err: any) {
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

export const notificationApi = {
  getNotifications: async (): Promise<NotificationItem[]> => {
    try {
      const { data } = await api.get("/Notification/notifications");
      const payload =
        data?.data ??
        data?.Data ??
        data?.notifications ??
        data?.Notifications ??
        data;

      const rawList: any[] = Array.isArray(payload?.data)
        ? payload.data
        : Array.isArray(payload)
        ? payload
        : [];

      const list = rawList
        .map((n: any) => ({
          id: n.id ?? n.Id ?? 0,
          message: n.message ?? n.Message ?? "",
          isSeen: n.isSeen ?? n.IsSeen ?? false,
          createdAt:
            n.createdAt ??
            n.CreatedAt ??
            n.timestamp ??
            n.Timestamp ??
            n.date ??
            n.Date ??
            undefined,
        }))
        .filter((n: NotificationItem) => {
          if (!n.createdAt) return true;
          const dt = new Date(n.createdAt);
          if (Number.isNaN(dt.getTime())) return true;
          const cutoff = new Date();
          cutoff.setDate(cutoff.getDate() - 30);
          return dt >= cutoff;
        });

      return list;
    } catch (error: any) {
      const msg =
        error?.response?.data?.message ??
        error?.response?.data?.Message ??
        error?.message;
      if (msg && msg.toLowerCase().includes("no unread")) {
        return [];
      }
      // 400/404 için boş listeye düş
      if (error?.response?.status === 400 || error?.response?.status === 404) {
        return [];
      }
      throw error;
    }
  },
  markAsSeen: async (notificationId: number): Promise<void> => {
    await api.post(`/Notification/notifications/${notificationId}/mark-as-seen`);
  },
};
