"use client";

import { useEffect, useState, useRef, useMemo, useCallback } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { profileApi, userApi, authApi, notificationApi } from "@/lib/queries";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { ScrollArea } from "@/components/ui/scroll-area";
import {
  Bell,
  CheckCheck,
  Home,
  Loader2,
  MessageSquare,
  RefreshCcw,
  Search,
  User,
  Settings,
  LogOut,
} from "lucide-react";
import { ThemeToggle } from "@/components/theme-toggle";
import type { NotificationItem } from "@/types";

function getStoredUserId(): number | null {
  if (typeof window === "undefined") return null;
  const raw = localStorage.getItem("user");
  if (!raw || raw === "undefined") return null;
  try {
    const parsed = JSON.parse(raw);
    const id = parsed?.id ?? parsed?.userId;
    return typeof id === "number" ? id : null;
  } catch {
    return null;
  }
}

export function Header() {
  const router = useRouter();
  const [profilePath, setProfilePath] = useState<string | null>(null);
  const [userFullName, setUserFullName] = useState<string>("John Doe");
  const [userHandle, setUserHandle] = useState<string>("@johndoe");
  const [searchTerm, setSearchTerm] = useState("");
  const [searching, setSearching] = useState(false);
  const [results, setResults] = useState<
    { id: number; firstName: string; lastName: string; userName: string }[]
  >([]);
  const [openResults, setOpenResults] = useState(false);
  const searchRef = useRef<HTMLDivElement>(null);
  const [notifications, setNotifications] = useState<NotificationItem[]>([]);
  const [notificationsOpen, setNotificationsOpen] = useState(false);
  const [loadingNotifications, setLoadingNotifications] = useState(false);
  const [notificationError, setNotificationError] = useState<string | null>(null);
  const [markingAll, setMarkingAll] = useState(false);

  useEffect(() => {
    const id = getStoredUserId();

    const resolvePath = async () => {
      try {
        const profile = await profileApi.getProfile();
        const userName = profile.header.userName;
        const userId = profile.header.userId;
        const fullName = `${profile.header.firstName ?? ""} ${profile.header.lastName ?? ""}`.trim();
        if (fullName) setUserFullName(fullName);
        if (userName) setUserHandle(`@${userName}`);
        if (userName) setProfilePath(`/${userName}`);
        else if (userId) setProfilePath(`/${userId}`);
        else if (id) setProfilePath(`/${id}`);
      } catch {
        if (id) setProfilePath(`/${id}`);
      }
    };

    resolvePath();
  }, []);

  // Debounced search
  useEffect(() => {
    const handler = setTimeout(async () => {
      const term = searchTerm.trim();
      if (term.length < 2) {
        setResults([]);
        return;
      }
      setSearching(true);
      try {
        const users = await userApi.searchUsers(term);
        setResults(users);
        setOpenResults(true);
      } catch (err) {
        console.error("Search failed:", err);
      } finally {
        setSearching(false);
      }
    }, 300);
    return () => clearTimeout(handler);
  }, [searchTerm]);

  // Close dropdown on click outside
  useEffect(() => {
    const onClickOutside = (e: MouseEvent) => {
      if (searchRef.current && !searchRef.current.contains(e.target as Node)) {
        setOpenResults(false);
      }
    };
    document.addEventListener("click", onClickOutside);
    return () => document.removeEventListener("click", onClickOutside);
  }, []);

  const parseDate = useCallback((value?: string) => {
    if (!value) return new Date();
    const d = new Date(value);
    return Number.isNaN(d.getTime()) ? new Date() : d;
  }, []);

  const startOfDay = useCallback((d: Date) => new Date(d.getFullYear(), d.getMonth(), d.getDate()), []);

  const formatGroupLabel = useCallback(
    (value?: string) => {
      const date = parseDate(value);
      const today = startOfDay(new Date());
      const dayDiff = Math.floor((today.getTime() - startOfDay(date).getTime()) / (1000 * 60 * 60 * 24));

      if (dayDiff < 7) return "Bu hafta";
      if (dayDiff < 14) return "Geçen hafta";
      if (dayDiff < 21) return "2 hafta önce";
      if (dayDiff < 28) return "3 hafta önce";
      return date.toLocaleDateString("tr-TR", { month: "long", year: "numeric" });
    },
    [parseDate, startOfDay]
  );

  const formatRelativeTime = useCallback(
    (value?: string) => {
      const date = parseDate(value);
      const diffMs = Date.now() - date.getTime();
      const minutes = Math.max(0, Math.floor(diffMs / 60000));
      if (minutes < 1) return "Şimdi";
      if (minutes < 60) return `${minutes} dk önce`;
      const hours = Math.floor(minutes / 60);
      if (hours < 24) return `${hours} sa önce`;
      const days = Math.floor(hours / 24);
      if (days < 7) return `${days} g önce`;
      if (days < 30) return `${Math.floor(days / 7)} hf önce`;
      return date.toLocaleDateString("tr-TR", { day: "numeric", month: "short" });
    },
    [parseDate]
  );

  const fetchNotifications = useCallback(async () => {
    setLoadingNotifications(true);
    try {
      const data = await notificationApi.getNotifications();
      setNotifications(
        data.map((n) => ({
          ...n,
          createdAt: n.createdAt ?? new Date().toISOString(),
        }))
      );
      setNotificationError(null);
    } catch (err) {
      setNotificationError("Bildirimler alınamadı.");
    } finally {
      setLoadingNotifications(false);
    }
  }, []);

  useEffect(() => {
    fetchNotifications();
  }, [fetchNotifications]);

  const handleNotificationsOpenChange = (open: boolean) => {
    setNotificationsOpen(open);
    if (open && !loadingNotifications) {
      fetchNotifications();
    }
  };

  const handleMarkAsSeen = async (notificationId: number) => {
    setNotifications((prev) =>
      prev.map((n) => (n.id === notificationId ? { ...n, isSeen: true } : n))
    );
    try {
      await notificationApi.markAsSeen(notificationId);
    } catch {
      setNotificationError("Bildirim okundu olarak işaretlenemedi.");
    }
  };

  const handleMarkAllAsSeen = async () => {
    const toMark = notifications.filter((n) => !n.isSeen).map((n) => n.id);
    if (!toMark.length) return;
    setMarkingAll(true);
    setNotifications((prev) => prev.map((n) => ({ ...n, isSeen: true })));
    try {
      await Promise.allSettled(toMark.map((id) => notificationApi.markAsSeen(id)));
    } finally {
      setMarkingAll(false);
    }
  };

  const groupedNotifications = useMemo(() => {
    const sorted = [...notifications].sort(
      (a, b) => parseDate(b.createdAt).getTime() - parseDate(a.createdAt).getTime()
    );
    return sorted.reduce<Record<string, NotificationItem[]>>((acc, item) => {
      const key = formatGroupLabel(item.createdAt);
      acc[key] = acc[key] ? [...acc[key], item] : [item];
      return acc;
    }, {});
  }, [notifications, formatGroupLabel, parseDate]);

  const unreadCount = notifications.filter((n) => !n.isSeen).length;

  const handleLogout = async () => {
    try {
      await authApi.logout();
    } catch {
      // ignore errors; devam et
    } finally {
      if (typeof window !== "undefined") {
        localStorage.removeItem("accessToken");
        localStorage.removeItem("refreshToken");
        localStorage.removeItem("user");
      }
      router.push("/login");
    }
  };

  return (
    <header className="sticky top-0 z-50 w-full border-b border-border/40 bg-background/80 backdrop-blur-xl supports-[backdrop-filter]:bg-background/60">
      <div className="container max-w-7xl mx-auto px-4">
        <div className="flex h-16 items-center justify-between gap-4">
          {/* Logo */}
          <Link href="/feed" className="flex items-center gap-2 group">
            <div className="w-10 h-10 bg-gradient-to-br from-brand to-brand-dark rounded-xl flex items-center justify-center shadow-lg shadow-brand/20 transition-all group-hover:shadow-brand/30 group-hover:scale-105">
              <svg className="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 8h2a2 2 0 012 2v6a2 2 0 01-2 2h-2v4l-4-4H9a1.994 1.994 0 01-1.414-.586m0 0L11 14h4a2 2 0 002-2V6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2v4l.586-.586z" />
              </svg>
            </div>
            <span className="text-xl font-bold gradient-text hidden sm:inline">SocialApp</span>
          </Link>

          {/* Search */}
          <div className="flex-1 max-w-md mx-4 hidden md:block" ref={searchRef}>
            <div className="relative">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
              <Input
                type="search"
                placeholder="Search users..."
                className="pl-10 bg-muted/50 border-border/50 focus:border-primary/50 transition-colors"
                value={searchTerm}
                onFocus={() => results.length && setOpenResults(true)}
                onChange={(e) => setSearchTerm(e.target.value)}
                onKeyDown={(e) => {
                  if (e.key === "Enter") {
                    e.preventDefault();
                    const q = searchTerm.trim();
                    if (q.length >= 1) {
                      router.push(`/search?query=${encodeURIComponent(q)}`);
                      setOpenResults(false);
                    }
                  }
                }}
              />
              {openResults && (
                <div className="absolute left-0 right-0 mt-2 rounded-xl border border-border bg-popover shadow-xl overflow-hidden">
                  {searching ? (
                    <div className="px-4 py-3 text-sm text-muted-foreground">Aranıyor...</div>
                  ) : results.length === 0 ? (
                    <div className="px-4 py-3 text-sm text-muted-foreground">Sonuç bulunamadı</div>
                  ) : (
                    <ul className="max-h-72 overflow-y-auto divide-y divide-border/60">
                      {results.map((u) => {
                        const name = `${u.firstName} ${u.lastName}`.trim() || u.userName;
                        const initials = `${u.firstName?.[0] ?? ""}${u.lastName?.[0] ?? ""}`.toUpperCase() || u.userName?.[0]?.toUpperCase();
                        const href = u.userName ? `/${u.userName}` : `/${u.id}`;
                        return (
                          <li key={u.id}>
                            <Link
                              href={href}
                              className="flex items-center gap-3 px-4 py-3 hover:bg-muted/60 transition-colors"
                              onClick={() => setOpenResults(false)}
                            >
                              <Avatar className="w-9 h-9">
                                <AvatarImage src="" alt={name} />
                                <AvatarFallback className="bg-brand/15 text-brand">{initials}</AvatarFallback>
                              </Avatar>
                              <div className="flex flex-col">
                                <span className="text-sm font-medium text-foreground">{name || "User"}</span>
                                {u.userName && <span className="text-xs text-muted-foreground">@{u.userName}</span>}
                              </div>
                            </Link>
                          </li>
                        );
                      })}
                    </ul>
                  )}
                </div>
              )}
            </div>
          </div>

          {/* Navigation */}
          <nav className="flex items-center gap-1">
            <Link href="/feed">
              <Button variant="ghost" size="icon" className="relative hover:bg-primary/10 transition-colors">
                <Home className="w-5 h-5" />
              </Button>
            </Link>
            <Button variant="ghost" size="icon" className="relative hover:bg-primary/10 transition-colors">
              <MessageSquare className="w-5 h-5" />
            </Button>
            <DropdownMenu open={notificationsOpen} onOpenChange={handleNotificationsOpenChange}>
              <DropdownMenuTrigger asChild>
                <Button variant="ghost" size="icon" className="relative hover:bg-primary/10 transition-colors">
                  <Bell className="w-5 h-5" />
                  {unreadCount > 0 && (
                    <span className="absolute top-1.5 right-1.5 min-w-[18px] h-[18px] px-1 rounded-full bg-destructive text-[10px] font-semibold text-white flex items-center justify-center shadow-sm">
                      {unreadCount > 9 ? "9+" : unreadCount}
                    </span>
                  )}
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align="end" className="w-[360px] p-0">
                <div className="flex items-center justify-between px-4 py-3 border-b border-border/70">
                  <div className="flex flex-col">
                    <span className="text-sm font-semibold">Notifications</span>
                    <span className="text-xs text-muted-foreground">Son bildirimler burada</span>
                  </div>
                  <div className="flex items-center gap-2">
                    <Button
                      variant="ghost"
                      size="icon"
                      className="h-8 w-8"
                      onClick={fetchNotifications}
                      disabled={loadingNotifications}
                    >
                      {loadingNotifications ? (
                        <Loader2 className="w-4 h-4 animate-spin" />
                      ) : (
                        <RefreshCcw className="w-4 h-4" />
                      )}
                    </Button>
                    <Button
                      variant="ghost"
                      size="sm"
                      disabled={unreadCount === 0 || markingAll}
                      onClick={handleMarkAllAsSeen}
                      className="text-xs"
                    >
                      {markingAll ? (
                        <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                      ) : (
                        <CheckCheck className="w-4 h-4 mr-2" />
                      )}
                      Hepsini okundu yap
                    </Button>
                  </div>
                </div>
                <ScrollArea className="max-h-[420px]">
                  <div className="py-2">
                    {loadingNotifications ? (
                      <div className="px-4 py-6 space-y-3">
                        {[1, 2, 3].map((n) => (
                          <div key={n} className="flex items-center gap-3 animate-pulse">
                            <span className="w-2 h-2 rounded-full bg-muted-foreground/50" />
                            <div className="flex-1 space-y-2">
                              <div className="h-3 w-3/4 rounded bg-muted" />
                              <div className="h-2 w-1/3 rounded bg-muted" />
                            </div>
                          </div>
                        ))}
                      </div>
                    ) : notificationError ? (
                      <div className="px-4 py-6 text-sm text-destructive">
                        {notificationError}
                      </div>
                    ) : notifications.length === 0 ? (
                      <div className="px-4 py-6 text-sm text-muted-foreground">
                        Şimdilik bildirim yok.
                      </div>
                    ) : (
                      Object.entries(groupedNotifications).map(([group, items]) => (
                        <div key={group} className="px-3 py-2">
                          <div className="flex items-center gap-2 text-[11px] uppercase tracking-wide text-muted-foreground">
                            <span className="h-px flex-1 bg-border" />
                            <span className="font-semibold">{group}</span>
                            <span className="h-px flex-1 bg-border" />
                          </div>
                          <div className="mt-2 space-y-1">
                            {items.map((item) => (
                              <button
                                key={`${group}-${item.id}`}
                                className={`w-full text-left px-3 py-2 rounded-lg transition-colors border border-transparent hover:border-border/70 ${
                                  item.isSeen
                                    ? "bg-muted/30 text-muted-foreground"
                                    : "bg-background text-foreground"
                                }`}
                                onClick={(e) => {
                                  e.preventDefault();
                                  if (!item.isSeen) handleMarkAsSeen(item.id);
                                }}
                              >
                                <div className="flex items-start gap-3">
                                  <span
                                    className={`mt-1 h-2 w-2 rounded-full ${
                                      item.isSeen ? "bg-border" : "bg-primary"
                                    }`}
                                  />
                                  <div className="flex-1">
                                    <p className="text-sm leading-snug">{item.message || "Yeni bildirim"}</p>
                                    <span className="text-[11px] text-muted-foreground">
                                      {formatRelativeTime(item.createdAt)}
                                    </span>
                                  </div>
                                  {!item.isSeen && (
                                    <span className="text-[11px] font-semibold text-primary/90 bg-primary/10 rounded-full px-2 py-0.5">
                                      Yeni
                                    </span>
                                  )}
                                </div>
                              </button>
                            ))}
                          </div>
                        </div>
                      ))
                    )}
                  </div>
                </ScrollArea>
              </DropdownMenuContent>
            </DropdownMenu>
            
            <ThemeToggle />

            {/* User Menu */}
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button variant="ghost" size="icon" className="rounded-full hover:bg-transparent">
                  <Avatar className="w-9 h-9 border-2 border-border hover:border-primary transition-all cursor-pointer ring-offset-background">
                    <AvatarImage src="https://i.pravatar.cc/150?u=currentuser" alt="User" />
                    <AvatarFallback className="bg-gradient-to-br from-brand to-brand-dark text-white">JD</AvatarFallback>
                  </Avatar>
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align="end" className="w-56">
                <DropdownMenuLabel>
                  <div className="flex flex-col space-y-1">
                    <p className="text-sm font-medium">{userFullName || "User"}</p>
                    <p className="text-xs text-muted-foreground">{userHandle || ""}</p>
                  </div>
                </DropdownMenuLabel>
                <DropdownMenuSeparator />
                {profilePath ? (
                  <DropdownMenuItem asChild>
                    <Link href={profilePath}>
                      <User className="w-4 h-4 mr-2" />
                      Profile
                    </Link>
                  </DropdownMenuItem>
                ) : (
                  <DropdownMenuItem disabled>
                    <User className="w-4 h-4 mr-2" />
                    Profile
                  </DropdownMenuItem>
                )}
                <DropdownMenuItem>
                  <Settings className="w-4 h-4 mr-2" />
                  Settings
                </DropdownMenuItem>
                <DropdownMenuSeparator />
                <DropdownMenuItem className="text-destructive" onSelect={(e) => { e.preventDefault(); handleLogout(); }}>
                  <LogOut className="w-4 h-4 mr-2" />
                  Logout
                </DropdownMenuItem>
              </DropdownMenuContent>
            </DropdownMenu>
          </nav>
        </div>
      </div>
    </header>
  );
}
