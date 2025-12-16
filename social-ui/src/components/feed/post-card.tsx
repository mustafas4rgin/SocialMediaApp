import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";

export function PostCard({
  name,
  username,
  time,
  text,
  img,
}: {
  name: string;
  username: string;
  time: string;
  text: string;
  img?: string;
}) {
  return (
    <Card className="rounded-2xl border bg-background/60 backdrop-blur supports-[backdrop-filter]:bg-background/40">
      <div className="p-4 sm:p-5">
        <div className="flex items-start justify-between gap-3">
          <div className="flex items-center gap-3 min-w-0">
            <Avatar className="h-10 w-10">
              <AvatarImage src={`https://i.pravatar.cc/120?u=${username}`} />
              <AvatarFallback>{name.slice(0, 2).toUpperCase()}</AvatarFallback>
            </Avatar>

            <div className="min-w-0">
              <div className="flex items-center gap-2">
                <div className="truncate text-sm font-semibold">{name}</div>
                <div className="truncate text-xs text-muted-foreground">
                  @{username} · {time}
                </div>
              </div>

              <div className="mt-2 text-sm leading-relaxed text-foreground/90">
                {text}
              </div>
            </div>
          </div>

          <Button variant="ghost" className="rounded-xl">
            •••
          </Button>
        </div>

        {img && (
          <div className="mt-4 overflow-hidden rounded-2xl border">
            {/* eslint-disable-next-line @next/next/no-img-element */}
            <img src={img} alt="post" className="h-64 w-full object-cover" />
          </div>
        )}

        <div className="mt-4 flex items-center justify-between text-sm text-muted-foreground">
          <button className="rounded-xl px-3 py-2 hover:bg-accent transition">
            Beğen
          </button>
          <button className="rounded-xl px-3 py-2 hover:bg-accent transition">
            Yorum
          </button>
          <button className="rounded-xl px-3 py-2 hover:bg-accent transition">
            Paylaş
          </button>
          <button className="rounded-xl px-3 py-2 hover:bg-accent transition">
            Kaydet
          </button>
        </div>
      </div>
    </Card>
  );
}
