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
  const [mainFileIndex, setMainFileIndex] = useState<number>(0);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

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
    let successMsg: string | null = null;
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
      successMsg = postPayload?.message ?? "Post paylaşıldı.";
      const postId =
        postPayload?.data?.id ??
        postPayload?.data?.Id ??
        postPayload?.id ??
        postPayload?.Id;

      if (!postId) throw new Error("Post ID alınamadı (response içeriğini kontrol edin).");

      // 2) Resim ekle (varsa)
      const uploadedImages: { id: number; file: string }[] = [];
      for (const f of files) {
        const imageUrl = await uploadToCloudinary(f);

        const resp = await postImageApi.addImage({
          file: imageUrl, // backend validator uzunluk sınırı URL için de geçerli; Cloudinary URL kısa
          postId,
        });
        const saved =
          resp?.data ??
          resp?.Data ??
          resp;
        const savedId =
          saved?.id ??
          saved?.Id ??
          saved?.data?.id ??
          saved?.data?.Id;
        uploadedImages.push({ id: savedId ?? 0, file: imageUrl });
      }

      // 2b) Main image ataması
      if (uploadedImages.length > 0) {
        const targetIdx = Math.min(mainFileIndex, uploadedImages.length - 1);
        const main = uploadedImages[targetIdx] ?? uploadedImages[0];
        if (main?.id) {
          await postApi.updatePost(postId, {
            body: body.trim(),
            userId: Number(currentUserId),
            status,
            mainPostImageId: main.id,
          });
        }
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
      setMainFileIndex(0);
      setVideos([]);
      onCreated?.();
    } catch (e: any) {
      console.error(e);
      const msg = e?.response?.data?.message ?? e?.message ?? "Post atılamadı";
      setError(msg);
    } finally {
      setLoading(false);
      if (successMsg) {
        setSuccess(successMsg);
        setTimeout(() => setSuccess(null), 3000);
      }
    }
  };

  return (
    <div className="post-card p-6">
      <div className="flex items-start gap-3">
        <div className="mt-1 flex h-10 w-10 items-center justify-center rounded-full bg-gradient-to-br from-brand/20 to-brand-dark/20 text-brand shadow-sm">
          <Sparkles className="h-5 w-5" />
        </div>
        <div className="flex-1 space-y-4">
          {error && (
            <div className="rounded-xl border border-destructive/30 bg-destructive/10 px-3 py-2 text-sm text-destructive">
              {error}
            </div>
          )}
          {success && (
            <div className="rounded-xl border border-emerald-500/30 bg-emerald-500/10 px-3 py-2 text-sm text-emerald-200">
              {success}
            </div>
          )}
          <Textarea
            placeholder="Bugün aklında ne var?"
            value={body}
            onChange={(e) => setBody(e.target.value)}
            className="min-h-[120px] resize-none border-border/50 bg-background focus:border-primary/50 transition-colors"
          />

          {files.length > 0 && (
            <div className="flex flex-wrap gap-2">
              {files.map((f, idx) => (
                <button
                  key={`${f.name}-${idx}`}
                  type="button"
                  onClick={() => setMainFileIndex(idx)}
                  className={`relative rounded-lg border px-3 py-1 text-xs transition ${
                    idx === mainFileIndex
                      ? "border-primary bg-primary/10 text-primary"
                      : "border-border/50 bg-muted text-foreground hover:border-primary/40"
                  }`}
                >
                  📷 {f.name}
                  {idx === mainFileIndex && (
                    <span className="ml-1 text-[10px] font-semibold">(Main)</span>
                  )}
                </button>
              ))}
            </div>
          )}
          {videos.length > 0 && (
            <div className="flex flex-wrap gap-2">
              {videos.map((f) => (
                <span key={f.name} className="rounded-full border border-border/50 bg-muted px-3 py-1 text-xs text-foreground">
                  🎬 {f.name}
                </span>
              ))}
            </div>
          )}

          <div className="flex flex-wrap items-center justify-between gap-3 pt-2 border-t border-border/50">
            <div className="flex gap-2">
              <label className="inline-flex cursor-pointer items-center gap-2 rounded-lg border border-border/50 bg-muted/50 px-4 py-2 text-sm font-medium text-foreground transition hover:bg-muted hover:border-primary/30">
                <ImageIcon className="h-4 w-4" />
                Görsel
                <input
                  type="file"
                  accept="image/*"
                  multiple
                  className="hidden"
                  onChange={(e) => {
                    const selected = e.target.files ? Array.from(e.target.files) : [];
                    if (!selected.length) return;
                    setFiles((prev) => {
                      const next = [...prev, ...selected];
                      if (next.length > 0 && mainFileIndex >= next.length) {
                        setMainFileIndex(0);
                      }
                      return next;
                    });
                  }}
                />
              </label>
              <label className="inline-flex cursor-pointer items-center gap-2 rounded-lg border border-border/50 bg-muted/50 px-4 py-2 text-sm font-medium text-foreground transition hover:bg-muted hover:border-primary/30">
                <ImageIcon className="h-4 w-4" />
                Video (≤ 60s)
                <input
                  type="file"
                  accept="video/*"
                  multiple
                  className="hidden"
                  onChange={(e) => {
                    const selected = e.target.files ? Array.from(e.target.files) : [];
                    if (!selected.length) return;
                    setVideos((prev) => [...prev, ...selected]);
                  }}
                />
              </label>
            </div>

            <Button
              onClick={submit}
              disabled={loading || !body.trim()}
              className="bg-gradient-to-r from-brand to-brand-dark text-white shadow-sm hover:shadow-md hover:from-brand-dark hover:to-brand transition-all"
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
