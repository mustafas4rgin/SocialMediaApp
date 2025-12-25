"use client";

import { useState } from "react";
import { Image as ImageIcon, Loader2, Sparkles } from "lucide-react";
import { postApi, postImageApi, postBrutalApi } from "@/lib/queries";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";

async function uploadToCloudinary(file: File): Promise<string> {
  const cloudName = process.env.NEXT_PUBLIC_CLOUDINARY_CLOUD_NAME;
  const preset = process.env.NEXT_PUBLIC_CLOUDINARY_UPLOAD_PRESET;

  if (!cloudName || !preset) {
    throw new Error("Görsel yüklemek için Cloudinary bilgileri (cloud name + unsigned preset) .env'de tanımlı olmalı.");
  }

  const formData = new FormData();
  formData.append("file", file);
  formData.append("upload_preset", preset);

  const uploadUrl = `https://api.cloudinary.com/v1_1/${cloudName}/upload`;

  // Debug log: hangi URL ve preset ile gidiyoruz
  if (typeof window !== "undefined") {
    console.info("Uploading to Cloudinary", { uploadUrl, preset });
  }

  const res = await fetch(uploadUrl, {
    method: "POST",
    body: formData,
  });

  if (!res.ok) {
    const data = await res.json().catch(() => ({}));
    throw new Error(
      data?.error?.message ??
        `Cloudinary hata: ${res.status} ${res.statusText}. Cloud name/preset kontrol et.`
    );
  }

  const data = await res.json();
  if (!data?.secure_url) {
    throw new Error("Cloudinary yükleme yanıtında secure_url bulunamadı.");
  }
  return data.secure_url as string;
}

async function uploadToCloudinaryVideo(file: File): Promise<string> {
  const cloudName = process.env.NEXT_PUBLIC_CLOUDINARY_CLOUD_NAME;
  const preset = process.env.NEXT_PUBLIC_CLOUDINARY_UPLOAD_PRESET;
  if (!cloudName || !preset) {
    throw new Error("Video yüklemek için Cloudinary bilgileri (cloud name + unsigned preset) .env'de tanımlı olmalı.");
  }

  const formData = new FormData();
  formData.append("file", file);
  formData.append("upload_preset", preset);

  const uploadUrl = `https://api.cloudinary.com/v1_1/${cloudName}/video/upload`;

  const res = await fetch(uploadUrl, {
    method: "POST",
    body: formData,
  });

  if (!res.ok) {
    const data = await res.json().catch(() => ({}));
    throw new Error(
      data?.error?.message ??
        `Cloudinary video hata: ${res.status} ${res.statusText}. Cloud name/preset/policy kontrol et.`
    );
  }

  const data = await res.json();
  if (!data?.secure_url) {
    throw new Error("Cloudinary video yükleme yanıtında secure_url bulunamadı.");
  }
  return data.secure_url as string;
}

function getVideoDuration(file: File): Promise<number> {
  return new Promise((resolve, reject) => {
    const video = document.createElement("video");
    video.preload = "metadata";
    video.onloadedmetadata = () => {
      window.URL.revokeObjectURL(video.src);
      resolve(video.duration);
    };
    video.onerror = () => reject(new Error("Video okunamadı"));
    video.src = URL.createObjectURL(file);
  });
}

export function CreatePost({ onCreated }: { onCreated?: () => void }) {
  const [body, setBody] = useState("");
  const [status] = useState<number>(1); // 1 = Approved, feed'de direkt görünsün
  const [files, setFiles] = useState<File[]>([]);
  const [videos, setVideos] = useState<File[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const userStr =
  typeof window !== "undefined" ? localStorage.getItem("user") : null;

let currentUserId: number | null = null;

if (userStr && userStr !== "undefined") {
  try {
    currentUserId = Number(JSON.parse(userStr)?.id ?? null);
  } catch {
    // localStorage'da bozuk veri varsa temizle
    localStorage.removeItem("user");
    currentUserId = null;
  }
}



  const submit = async () => {
    if (!body.trim()) return;
    if (!currentUserId) {
      setError("Kullanıcı bulunamadı. Lütfen tekrar giriş yap.");
      return;
    }

    setLoading(true);
    setError(null);
    try {
      // 1) Post oluştur
      const res = await postApi.addPost({
        body: body.trim(),
        userId: Number(currentUserId),
        status,
      });

      const postPayload = res?.data ?? res;
      if (postPayload?.success === false) {
        throw new Error(postPayload?.message ?? "Post eklenemedi.");
      }
      const postId =
        postPayload?.data?.id ??
        postPayload?.data?.Id ??
        postPayload?.id ??
        postPayload?.Id;

      if (!postId) throw new Error("Post ID alınamadı (response içeriğini kontrol edin).");

      // 2) Resim ekle (varsa)
      for (const f of files) {
        const imageUrl = await uploadToCloudinary(f);

        await postImageApi.addImage({
          file: imageUrl, // backend validator uzunluk sınırı URL için de geçerli; Cloudinary URL kısa
          postId,
        });
      }

      // 3) Video ekle (varsa) -> PostBrutal
      for (const v of videos) {
        const duration = await getVideoDuration(v);
        if (duration > 60) {
          throw new Error("Video süresi 60 saniyeyi aşamaz.");
        }

        const videoUrl = await uploadToCloudinaryVideo(v);

        await postBrutalApi.addVideo({
          file: videoUrl,
          postId,
        });
      }

      // 3) UI temizle + feed yenile
      setBody("");
      setFiles([]);
      setVideos([]);
      onCreated?.();
    } catch (e: any) {
      console.error(e);
      const msg = e?.response?.data?.message ?? e?.message ?? "Post atılamadı";
      setError(msg);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="relative overflow-hidden rounded-3xl border border-white/10 bg-white/5 p-6 shadow-2xl backdrop-blur">
      <div className="absolute inset-0 opacity-60 bg-[radial-gradient(circle_at_20%_20%,rgba(168,85,247,0.12),transparent_25%),radial-gradient(circle_at_80%_0%,rgba(34,211,238,0.1),transparent_25%)] pointer-events-none" />
      <div className="relative flex items-start gap-3">
        <div className="mt-1 flex h-10 w-10 items-center justify-center rounded-full bg-brand/20 text-brand">
          <Sparkles className="h-5 w-5" />
        </div>
        <div className="flex-1 space-y-4">
          {error && (
            <div className="rounded-xl border border-destructive/30 bg-destructive/10 px-3 py-2 text-sm text-destructive">
              {error}
            </div>
          )}
          <Textarea
            placeholder="Bugün aklında ne var?"
            value={body}
            onChange={(e) => setBody(e.target.value)}
            className="min-h-[120px] resize-none border-white/10 bg-white/10 text-slate-50 placeholder:text-slate-300"
          />

          {files.length > 0 && (
            <div className="flex flex-wrap gap-2">
              {files.map((f) => (
                <span key={f.name} className="rounded-full border border-white/10 bg-white/10 px-3 py-1 text-xs text-slate-100">
                  {f.name}
                </span>
              ))}
            </div>
          )}
          {videos.length > 0 && (
            <div className="flex flex-wrap gap-2">
              {videos.map((f) => (
                <span key={f.name} className="rounded-full border border-white/10 bg-white/10 px-3 py-1 text-xs text-amber-100">
                  🎬 {f.name}
                </span>
              ))}
            </div>
          )}

          <div className="flex flex-wrap items-center justify-between gap-3">
            <label className="inline-flex cursor-pointer items-center gap-2 rounded-full border border-white/15 bg-white/10 px-4 py-2 text-sm font-medium text-slate-100 transition hover:border-white/30 hover:bg-white/15">
              <ImageIcon className="h-4 w-4" />
              Görsel ekle
              <input
                type="file"
                accept="image/*"
                multiple
                className="hidden"
                onChange={(e) => setFiles(e.target.files ? Array.from(e.target.files) : [])}
              />
            </label>
            <label className="inline-flex cursor-pointer items-center gap-2 rounded-full border border-white/15 bg-white/10 px-4 py-2 text-sm font-medium text-slate-100 transition hover:border-white/30 hover:bg-white/15">
              <ImageIcon className="h-4 w-4" />
              Video ekle (≤ 60 sn)
              <input
                type="file"
                accept="video/*"
                multiple
                className="hidden"
                onChange={(e) => setVideos(e.target.files ? Array.from(e.target.files) : [])}
              />
            </label>

            <Button
              onClick={submit}
              disabled={loading}
              className="bg-gradient-to-r from-brand to-brand-dark text-white shadow-lg shadow-brand/30 hover:from-brand-dark hover:to-brand"
            >
              {loading ? (
                <span className="flex items-center gap-2">
                  <Loader2 className="h-4 w-4 animate-spin" />
                  Paylaşılıyor...
                </span>
              ) : (
                "Paylaş"
              )}
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
}
