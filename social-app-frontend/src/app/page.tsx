"use client";

import { useEffect } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { Button } from "@/components/ui/button";

export default function Home() {
  const router = useRouter();

  useEffect(() => {
    const token = typeof window !== "undefined" ? localStorage.getItem("accessToken") : null;
    if (token) {
      router.replace("/feed");
    }
  }, [router]);

  return (
    <div className="min-h-screen bg-gradient-to-br from-brand/5 via-background to-accent/20">
      <div className="absolute inset-0 bg-[url('data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNjAiIGhlaWdodD0iNjAiIHZpZXdCb3g9IjAgMCA2MCA2MCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48ZyBmaWxsPSJub25lIiBmaWxsLXJ1bGU9ImV2ZW5vZGQiPjxnIGZpbGw9IiMwMDcyQkIiIGZpbGwtb3BhY2l0eT0iMC4wMyI+PHBhdGggZD0iTTM2IDM0djItaDJ2LTJoLTJ6bTAgNHYyaDJ2LTJoLTJ6bS0yIDJ2Mmgydi0yaC0yem0wLTR2Mmgydi0yaC0yem0yLTJ2LTJoLTJ2Mmgyem0wLTR2LTJoLTJ2Mmgyem0yIDJ2LTJoLTJ2Mmgyem0wIDR2LTJoLTJ2Mmgyem0yLTJ2Mmgydi0yaC0yem0wLTR2Mmgydi0yaC0yem0tMi0ydjJoMnYtMmgtMnptMC00djJoMnYtMmgtMnptMiAydjJoMnYtMmgtMnoiLz48L2c+PC9nPjwvc3ZnPg==')] opacity-40"></div>
      
      <div className="relative z-10">
        <div className="container mx-auto px-4 py-20">
          <div className="max-w-4xl mx-auto text-center space-y-8">
            <div className="flex justify-center mb-8">
              <div className="w-24 h-24 bg-gradient-to-br from-brand to-brand-dark rounded-3xl flex items-center justify-center shadow-2xl">
                <svg className="w-14 h-14 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 8h2a2 2 0 012 2v6a2 2 0 01-2 2h-2v4l-4-4H9a1.994 1.994 0 01-1.414-.586m0 0L11 14h4a2 2 0 002-2V6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2v4l.586-.586z" />
                </svg>
              </div>
            </div>

            <h1 className="text-6xl md:text-7xl font-bold text-heading bg-gradient-to-r from-brand via-brand-dark to-secondary bg-clip-text text-transparent">
              Welcome to SocialApp
            </h1>

            <p className="text-xl md:text-2xl text-muted-foreground max-w-2xl mx-auto">
              Connect with friends, share your moments, and build meaningful relationships in a modern social platform.
            </p>

            <div className="flex flex-col sm:flex-row gap-4 justify-center items-center pt-8">
              <Link href="/register">
                <Button size="lg" className="h-14 px-8 text-lg bg-gradient-to-r from-brand to-brand-dark hover:from-brand-dark hover:to-brand shadow-lg">
                  Get Started
                </Button>
              </Link>
              <Link href="/login">
                <Button size="lg" variant="outline" className="h-14 px-8 text-lg border-2 border-brand text-brand hover:bg-brand hover:text-white">
                  Sign In
                </Button>
              </Link>
            </div>
          </div>

          <div className="grid md:grid-cols-3 gap-8 mt-24 max-w-5xl mx-auto">
            <div className="glass-effect rounded-2xl p-8 text-center space-y-4 hover:shadow-xl transition-shadow">
              <div className="w-16 h-16 bg-gradient-to-br from-brand to-brand-light rounded-2xl flex items-center justify-center mx-auto">
                <svg className="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z" />
                </svg>
              </div>
              <h3 className="text-xl font-semibold text-heading">Connect</h3>
              <p className="text-muted-foreground">
                Build your network and stay connected with friends and family
              </p>
            </div>

            <div className="glass-effect rounded-2xl p-8 text-center space-y-4 hover:shadow-xl transition-shadow">
              <div className="w-16 h-16 bg-gradient-to-br from-brand to-brand-light rounded-2xl flex items-center justify-center mx-auto">
                <svg className="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
                </svg>
              </div>
              <h3 className="text-xl font-semibold text-heading">Share</h3>
              <p className="text-muted-foreground">
                Share your moments, photos, and thoughts with your community
              </p>
            </div>

            <div className="glass-effect rounded-2xl p-8 text-center space-y-4 hover:shadow-xl transition-shadow">
              <div className="w-16 h-16 bg-gradient-to-br from-brand to-brand-light rounded-2xl flex items-center justify-center mx-auto">
                <svg className="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M14 10h4.764a2 2 0 011.789 2.894l-3.5 7A2 2 0 0115.263 21h-4.017c-.163 0-.326-.02-.485-.06L7 20m7-10V5a2 2 0 00-2-2h-.095c-.5 0-.905.405-.905.905 0 .714-.211 1.412-.608 2.006L7 11v9m7-10h-2M7 20H5a2 2 0 01-2-2v-6a2 2 0 012-2h2.5" />
                </svg>
              </div>
              <h3 className="text-xl font-semibold text-heading">Engage</h3>
              <p className="text-muted-foreground">
                Like, comment, and interact with content that matters to you
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
