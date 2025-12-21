"use client";

import { useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { authApi } from "@/lib/queries";
import type { LoginCredentials } from "@/types";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";

export default function LoginPage() {
  const router = useRouter();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setIsLoading(true);

    try {
      const payload: LoginCredentials = { email, password };
      const tokens = await authApi.login(payload);

      localStorage.setItem("accessToken", tokens.accessToken);
      if (tokens.refreshToken) localStorage.setItem("refreshToken", tokens.refreshToken);

      const me = await authApi.getCurrentUser();
      localStorage.setItem(
        "user",
        JSON.stringify({
          id: me.userId,
          name: me.firstName,
          role: me.role,
        })
      );

      router.push("/feed");

    } catch (err: any) {
      const msg =
        err?.response?.data?.message ||
        err?.response?.data?.error ||
        "Login failed. Please check your credentials.";
      setError(msg);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-brand/5 via-background to-accent/20 p-4">
      {/* ... senin mevcut UI ... */}

      <Card className="w-full max-w-md glass-effect shadow-2xl relative z-10">
        <CardHeader className="space-y-1 text-center">
          <CardTitle className="text-3xl text-heading">Welcome Back</CardTitle>
          <CardDescription className="text-base">Sign in to continue to SocialApp</CardDescription>
        </CardHeader>

        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            {error && (
              <div className="rounded-md border border-destructive/30 bg-destructive/10 px-3 py-2 text-sm">
                {error}
              </div>
            )}

            <div className="space-y-2">
              <Label htmlFor="email">Email</Label>
              <Input id="email" type="email" value={email} onChange={(e) => setEmail(e.target.value)} required className="h-11" />
            </div>

            <div className="space-y-2">
              <div className="flex items-center justify-between">
                <Label htmlFor="password">Password</Label>
                <Link href="/forgot-password" className="text-sm text-brand hover:text-brand-dark transition-colors">
                  Forgot password?
                </Link>
              </div>
              <Input id="password" type="password" value={password} onChange={(e) => setPassword(e.target.value)} required className="h-11" />
            </div>

            <Button type="submit" className="w-full h-11 bg-gradient-to-r from-brand to-brand-dark hover:from-brand-dark hover:to-brand text-base font-medium" disabled={isLoading}>
              {isLoading ? "Signing in..." : "Sign In"}
            </Button>
          </form>

          {/* ... kalan UI ... */}
        </CardContent>

        <CardFooter className="flex flex-col space-y-4">
          <div className="text-sm text-center text-muted-foreground">
            Don&apos;t have an account?{" "}
            <Link href="/register" className="text-brand hover:text-brand-dark font-medium transition-colors">
              Sign up
            </Link>
          </div>
        </CardFooter>
      </Card>
    </div>
  );
}
