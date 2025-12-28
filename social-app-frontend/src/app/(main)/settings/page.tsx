"use client";

import { useEffect, useMemo, useRef, useState } from "react";
import Link from "next/link";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Switch } from "@/components/ui/switch";
import { Separator } from "@/components/ui/separator";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Check, Mail, Shield, Bell, Globe, Upload, ArrowLeft, Loader2 } from "lucide-react";
import { profileApi, userImageApi } from "@/lib/queries";

export default function SettingsPage() {
  const [profile, setProfile] = useState({
    firstName: "",
    lastName: "",
    userName: "",
    bio: "",
    location: "",
    website: "",
  });
  const [userId, setUserId] = useState<number | null>(null);
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [avatarPreview, setAvatarPreview] = useState<string | null>(null);
  const [avatarFile, setAvatarFile] = useState<File | null>(null);
  const fileInputRef = useRef<HTMLInputElement | null>(null);

  const [prefs, setPrefs] = useState({
    notifyComments: true,
    notifyLikes: true,
    notifyNewFollowers: true,
    publicProfile: true,
    searchable: true,
  });

  const isComplete = useMemo(() => {
    return avatarFile !== null || Object.values(profile).some((v) => v.trim());
  }, [profile, avatarFile]);

  useEffect(() => {
    const load = async () => {
      if (typeof window !== "undefined") {
        const raw = localStorage.getItem("user");
        if (raw && raw !== "undefined") {
          try {
            const parsed = JSON.parse(raw);
            const id = parsed?.id ?? parsed?.userId;
            if (id) setUserId(id);
          } catch {
            // ignore
          }
        }
      }
      try {
        const data = await profileApi.getProfile();
        const header = data.header;
        setUserId(header.userId ?? null);
        setProfile((prev) => ({
          ...prev,
          firstName: header.firstName ?? "",
          lastName: header.lastName ?? "",
          userName: header.userName ?? "",
        }));
      } catch (e: any) {
        // silently ignore; keep empty form
        console.error("Profile fetch failed", e);
      }
    };
    load();
  }, []);

  const readPreview = (file: File) =>
    new Promise<string>((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = () => {
        if (typeof reader.result === "string") resolve(reader.result);
        else reject(new Error("File read failed"));
      };
      reader.onerror = () => reject(new Error("File read failed"));
      reader.readAsDataURL(file);
    });

  const uploadAvatarToCloudinary = async (file: File): Promise<string> => {
    const cloudName = process.env.NEXT_PUBLIC_CLOUDINARY_CLOUD_NAME;
    const preset = process.env.NEXT_PUBLIC_CLOUDINARY_UPLOAD_PRESET;
    if (!cloudName || !preset) {
      throw new Error("Cloudinary config eksik (NEXT_PUBLIC_CLOUDINARY_CLOUD_NAME / NEXT_PUBLIC_CLOUDINARY_UPLOAD_PRESET)");
    }
    const formData = new FormData();
    formData.append("file", file);
    formData.append("upload_preset", preset);
    const uploadUrl = `https://api.cloudinary.com/v1_1/${cloudName}/upload`;
    const res = await fetch(uploadUrl, {
      method: "POST",
      body: formData,
    });
    if (!res.ok) {
      const data = await res.json().catch(() => ({}));
      throw new Error(
        data?.error?.message ??
          `Cloudinary hata: ${res.status} ${res.statusText}`
      );
    }
    const data = await res.json();
    if (!data?.secure_url) {
      throw new Error("Cloudinary yanıtında secure_url bulunamadı.");
    }
    return data.secure_url;
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-950 to-slate-900 text-slate-50">
      <div className="absolute inset-0 pointer-events-none bg-[radial-gradient(circle_at_20%_20%,rgba(59,130,246,0.12),transparent_25%),radial-gradient(circle_at_80%_0%,rgba(236,72,153,0.14),transparent_25%),radial-gradient(circle_at_50%_80%,rgba(168,85,247,0.1),transparent_22%)]" />

      <div className="relative container max-w-5xl mx-auto px-4 py-10 space-y-6">
        <div className="flex items-center justify-between">
          <div>
            <p className="text-sm text-slate-400 flex items-center gap-2">
              <ArrowLeft className="w-4 h-4" />
              <Link href="/feed" className="hover:text-white transition-colors">Back to feed</Link>
            </p>
            <h1 className="text-3xl font-semibold text-white mt-2">Settings</h1>
            <p className="text-slate-300 text-sm">Control your profile, privacy, and notifications.</p>
          </div>
        </div>

        <form
          className="space-y-6"
          onSubmit={(e) => {
            e.preventDefault();
            if (!isComplete) return;
            const payload: any = {};
            if (profile.userName.trim()) payload.userName = profile.userName.trim();
            if (profile.firstName.trim()) payload.firstName = profile.firstName.trim();
            if (profile.lastName.trim()) payload.lastName = profile.lastName.trim();

            const submit = async () => {
              setLoading(true);
              setMessage(null);
              setError(null);
              try {
                const jobs: Promise<any>[] = [];
                if (Object.keys(payload).length > 0) {
                  jobs.push(profileApi.updateProfile(payload));
                }
                if (avatarFile && userId) {
                  const url = await uploadAvatarToCloudinary(avatarFile);
                  jobs.push(userImageApi.upload(userId, url));
                }
                if (jobs.length === 0) return;
                await Promise.all(jobs);
                setMessage("Profile updated.");
              } catch (err: any) {
                const msg =
                  err?.response?.data?.message ??
                  err?.response?.data?.Message ??
                  err?.message ??
                  "Update failed.";
                setError(msg);
              } finally {
                setLoading(false);
              }
            };
            submit();
          }}
        >
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
            {/* Left column */}
            <div className="lg:col-span-2 space-y-6">
              <Card className="border-white/10 bg-white/5 backdrop-blur shadow-2xl">
                <CardHeader className="flex flex-col gap-2">
                  <CardTitle className="text-white">Profile</CardTitle>
                  <CardDescription className="text-slate-300">Update how others see you.</CardDescription>
                </CardHeader>
                <CardContent className="space-y-4">
                  {message && (
                    <div className="rounded-lg border border-emerald-500/40 bg-emerald-500/10 px-3 py-2 text-sm text-emerald-100">
                      {message}
                    </div>
                  )}
                  {error && (
                    <div className="rounded-lg border border-destructive/40 bg-destructive/15 px-3 py-2 text-sm text-destructive-foreground">
                      {error}
                    </div>
                  )}
                  <div className="flex items-center gap-4">
                    <Avatar className="w-16 h-16 border-2 border-white/20">
                      <AvatarImage
                        src={
                          avatarPreview ||
                          `https://api.dicebear.com/8.x/initials/svg?seed=${encodeURIComponent(profile.firstName || "User")}`
                        }
                        alt="Avatar"
                      />
                      <AvatarFallback className="bg-brand/20 text-brand text-xl">U</AvatarFallback>
                    </Avatar>
                    <div className="flex items-center gap-3">
                      <input
                        type="file"
                        accept="image/*"
                        className="hidden"
                        ref={fileInputRef}
                        onChange={async (e) => {
                          const file = e.target.files?.[0];
                          if (!file) return;
                          try {
                            const dataUrl = await readPreview(file);
                            setAvatarPreview(dataUrl);
                            setAvatarFile(file);
                            setError(null);
                          } catch (err: any) {
                            setError("Image could not be processed.");
                          }
                        }}
                      />
                      <Button
                        type="button"
                        variant="outline"
                        className="flex items-center gap-2 border-white/20 text-white"
                        onClick={() => fileInputRef.current?.click()}
                      >
                        <Upload className="w-4 h-4" />
                        Upload new photo
                      </Button>
                      {avatarPreview && (
                        <Button
                          type="button"
                          variant="ghost"
                          className="text-xs text-muted-foreground hover:text-white"
                          onClick={() => {
                            setAvatarFile(null);
                            setAvatarPreview(null);
                          }}
                        >
                          Remove
                        </Button>
                      )}
                    </div>
                  </div>
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div className="space-y-2">
                      <label className="text-sm text-slate-200">First name</label>
                    <Input
                      value={profile.firstName}
                      onChange={(e) => setProfile((p) => ({ ...p, firstName: e.target.value }))}
                      placeholder="Ada"
                      className="bg-white/10 border-white/10 text-white"
                    />
                    </div>
                    <div className="space-y-2">
                      <label className="text-sm text-slate-200">Last name</label>
                    <Input
                      value={profile.lastName}
                      onChange={(e) => setProfile((p) => ({ ...p, lastName: e.target.value }))}
                      placeholder="Lovelace"
                      className="bg-white/10 border-white/10 text-white"
                    />
                    </div>
                    <div className="space-y-2">
                      <label className="text-sm text-slate-200">Username</label>
                    <Input
                      value={profile.userName}
                      onChange={(e) => setProfile((p) => ({ ...p, userName: e.target.value }))}
                      placeholder="adalovelace"
                      className="bg-white/10 border-white/10 text-white"
                    />
                    </div>
                    <div className="space-y-2">
                      <label className="text-sm text-slate-200">Location</label>
                    <Input
                      value={profile.location}
                      onChange={(e) => setProfile((p) => ({ ...p, location: e.target.value }))}
                      placeholder="London, UK"
                      className="bg-white/10 border-white/10 text-white"
                    />
                    </div>
                  </div>
                  <div className="space-y-2">
                    <label className="text-sm text-slate-200">Bio</label>
                    <Textarea
                      value={profile.bio}
                      onChange={(e) => setProfile((p) => ({ ...p, bio: e.target.value }))}
                      placeholder="Tell the community about yourself..."
                      className="bg-white/10 border-white/10 text-white min-h-[120px]"
                    />
                  </div>
                  <div className="space-y-2">
                    <label className="text-sm text-slate-200">Website</label>
                    <Input
                      value={profile.website}
                      onChange={(e) => setProfile((p) => ({ ...p, website: e.target.value }))}
                      placeholder="https://"
                      className="bg-white/10 border-white/10 text-white"
                    />
                  </div>
                </CardContent>
              </Card>

              <div className="flex justify-end">
                <Button
                  type="submit"
                  disabled={!isComplete}
                  className={`min-w-[160px] h-11 bg-gradient-to-r from-brand to-brand-dark hover:from-brand-dark hover:to-brand shadow-lg transition ${
                    !isComplete ? "opacity-60 cursor-not-allowed" : ""
                  }`}
                >
                  {loading ? <Loader2 className="w-4 h-4 animate-spin" /> : "Submit"}
                </Button>
              </div>

              <Card className="border-white/10 bg-white/5 backdrop-blur shadow-2xl">
                <CardHeader>
                  <CardTitle className="text-white flex items-center gap-2">
                    <Bell className="w-4 h-4" />
                    Notifications
                  </CardTitle>
                  <CardDescription className="text-slate-300">Choose what we should notify you about.</CardDescription>
                </CardHeader>
                <CardContent className="space-y-4">
                  {[
                    { key: "notifyComments", label: "Comments on your posts" },
                    { key: "notifyLikes", label: "Likes on your posts" },
                    { key: "notifyNewFollowers", label: "New followers" },
                  ].map((item) => (
                    <div key={item.key} className="flex items-center justify-between rounded-xl border border-white/10 bg-white/5 px-3 py-2">
                      <div>
                        <p className="text-sm text-white">{item.label}</p>
                        <p className="text-xs text-slate-400">Receive push and in-app alerts.</p>
                      </div>
                      <Switch
                        checked={(prefs as any)[item.key]}
                        onCheckedChange={(val) => setPrefs((p) => ({ ...p, [item.key]: val }))}
                      />
                    </div>
                  ))}
                </CardContent>
              </Card>
            </div>

            {/* Right column */}
            <div className="space-y-6">
              <Card className="border-white/10 bg-white/5 backdrop-blur">
                <CardHeader>
                  <CardTitle className="text-white flex items-center gap-2">
                    <Shield className="w-4 h-4" />
                    Privacy
                  </CardTitle>
                  <CardDescription className="text-slate-300">Control how others find and contact you.</CardDescription>
                </CardHeader>
                <CardContent className="space-y-3">
                  <div className="flex items-center justify-between rounded-xl border border-white/10 bg-white/5 px-3 py-2">
                    <div>
                      <p className="text-sm text-white">Public profile</p>
                      <p className="text-xs text-slate-400">Allow everyone to view your posts.</p>
                    </div>
                    <Switch
                      checked={prefs.publicProfile}
                      onCheckedChange={(val) => setPrefs((p) => ({ ...p, publicProfile: val }))}
                    />
                  </div>
                  <div className="flex items-center justify-between rounded-xl border border-white/10 bg-white/5 px-3 py-2">
                    <div>
                      <p className="text-sm text-white">Appear in search</p>
                      <p className="text-xs text-slate-400">Let others find you by name or handle.</p>
                    </div>
                    <Switch
                      checked={prefs.searchable}
                      onCheckedChange={(val) => setPrefs((p) => ({ ...p, searchable: val }))}
                    />
                  </div>
                </CardContent>
              </Card>

              <Card className="border-white/10 bg-white/5 backdrop-blur">
                <CardHeader>
                  <CardTitle className="text-white flex items-center gap-2">
                    <Mail className="w-4 h-4" />
                    Contact
                  </CardTitle>
                  <CardDescription className="text-slate-300">How we keep in touch.</CardDescription>
                </CardHeader>
                <CardContent className="space-y-3">
                  <div className="space-y-2">
                    <label className="text-sm text-slate-200">Email</label>
                  <Input
                    placeholder="you@example.com"
                    className="bg-white/10 border-white/10 text-white"
                  />
                  </div>
                  <Button variant="outline" className="w-full flex items-center gap-2 border-white/20 text-white">
                    <Check className="w-4 h-4" />
                    Verify email
                  </Button>
                </CardContent>
              </Card>

              <Card className="border-white/10 bg-white/5 backdrop-blur">
                <CardHeader>
                  <CardTitle className="text-white flex items-center gap-2">
                    <Globe className="w-4 h-4" />
                    Danger Zone
                  </CardTitle>
                  <CardDescription className="text-slate-300">Export data or deactivate your account.</CardDescription>
                </CardHeader>
                <CardContent className="space-y-3">
                  <Button variant="outline" className="w-full border-amber-500/40 text-amber-400 hover:bg-amber-500/10">
                    Export my data
                  </Button>
                  <Separator className="bg-white/10" />
                  <Button variant="destructive" className="w-full">
                    Deactivate account
                  </Button>
                </CardContent>
              </Card>
            </div>
          </div>

        </form>
      </div>
    </div>
  );
}
