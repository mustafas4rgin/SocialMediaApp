"use client";

import { useEffect, useState, useRef } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { profileApi, userApi, authApi } from "@/lib/queries";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Bell, Home, MessageSquare, Search, User, Settings, LogOut } from "lucide-react";

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
    <header className="sticky top-0 z-50 w-full border-b border-border bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
      <div className="container max-w-7xl mx-auto px-4">
        <div className="flex h-16 items-center justify-between">
          {/* Logo */}
          <Link href="/feed" className="flex items-center gap-2">
            <div className="w-10 h-10 bg-gradient-to-br from-brand to-brand-dark rounded-xl flex items-center justify-center">
              <svg className="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 8h2a2 2 0 012 2v6a2 2 0 01-2 2h-2v4l-4-4H9a1.994 1.994 0 01-1.414-.586m0 0L11 14h4a2 2 0 002-2V6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2v4l.586-.586z" />
              </svg>
            </div>
            <span className="text-xl font-bold text-heading hidden sm:inline">SocialApp</span>
          </Link>

          {/* Search */}
          <div className="flex-1 max-w-md mx-4 hidden md:block" ref={searchRef}>
            <div className="relative">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
              <Input
                type="search"
                placeholder="Search..."
                className="pl-10 bg-muted/50"
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
          <nav className="flex items-center gap-2">
            <Link href="/feed">
              <Button variant="ghost" size="icon" className="relative">
                <Home className="w-5 h-5" />
              </Button>
            </Link>
            <Button variant="ghost" size="icon" className="relative">
              <MessageSquare className="w-5 h-5" />
            </Button>
            <Button variant="ghost" size="icon" className="relative">
              <Bell className="w-5 h-5" />
              <span className="absolute top-1 right-1 w-2 h-2 bg-destructive rounded-full"></span>
            </Button>

            {/* User Menu */}
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button variant="ghost" size="icon" className="rounded-full">
                  <Avatar className="w-9 h-9 border border-slate-200 bg-slate-100 dark:border-white/10 dark:bg-white/5">
                    <AvatarImage src="https://i.pravatar.cc/150?u=currentuser" alt="User" />
                    <AvatarFallback className="bg-brand/15 text-brand">JD</AvatarFallback>
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
