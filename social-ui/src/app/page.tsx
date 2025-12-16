import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Separator } from "@/components/ui/separator";

/** =========================
 *  API helper (simple & clear)
 *  ========================= */
const API_BASE =
  process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5084";

type UserDto = {
  id: number;
  firstName: string;
  lastName: string;
};

type UsersResponse = {
  message: string;
  user: UserDto[];
};

async function getUsers(): Promise<UserDto[]> {
  // ✅ Endpoint: GET {API_BASE}/User/GetAll  (sen "hallettik" dediğin için bunu baz aldım)
  // Eğer sende /users veya /api/User/GetAll gibi bir şeyse burayı değiştir.
  const res = await fetch(`${API_BASE}/api/User/GetAll`, { cache: "no-store" });

  if (!res.ok) {
    throw new Error(`Users fetch failed: ${res.status}`);
  }

  const data = (await res.json()) as UsersResponse;
  return data.user ?? [];
}

/** =========================
 *  UI pieces
 *  ========================= */
function Logo() {
  return (
    <div className="flex items-center gap-3">
      <div className="h-10 w-10 rounded-2xl bg-gradient-to-br from-zinc-900 to-zinc-500 dark:from-zinc-100 dark:to-zinc-400" />
      <div className="leading-tight">
        <div className="text-base font-semibold">Socia</div>
        <div className="text-xs text-muted-foreground">social network</div>
      </div>
    </div>
  );
}

function LeftNav() {
  const items = [
    "Ana Sayfa",
    "Keşfet",
    "Bildirimler",
    "Mesajlar",
    "Kaydedilenler",
    "Profil",
    "Ayarlar",
  ];

  return (
    <aside className="hidden lg:block lg:col-span-3 xl:col-span-2">
      <Card className="sticky top-6 rounded-2xl border bg-background/60 backdrop-blur supports-[backdrop-filter]:bg-background/40">
        <div className="p-5">
          <Logo />
          <Separator className="my-5" />

          <nav className="grid gap-1">
            {items.map((t) => (
              <button
                key={t}
                className="rounded-xl px-3 py-2 text-left text-sm font-medium text-foreground/80 hover:bg-accent hover:text-foreground transition"
              >
                {t}
              </button>
            ))}
          </nav>

          <Separator className="my-5" />

          <div className="flex items-center gap-3">
            <Avatar className="h-10 w-10">
              <AvatarImage src="https://i.pravatar.cc/120?img=12" />
              <AvatarFallback>ME</AvatarFallback>
            </Avatar>
            <div className="min-w-0">
              <div className="truncate text-sm font-semibold">Senin Adın</div>
              <div className="truncate text-xs text-muted-foreground">@username</div>
            </div>
          </div>

          <Button className="mt-4 w-full rounded-xl">Yeni Post</Button>
        </div>
      </Card>
    </aside>
  );
}

function PostComposer() {
  return (
    <Card className="rounded-2xl border bg-background/60 backdrop-blur supports-[backdrop-filter]:bg-background/40">
      <div className="p-4 sm:p-5">
        <div className="flex items-start gap-3">
          <Avatar className="h-10 w-10">
            <AvatarImage src="https://i.pravatar.cc/120?img=32" />
            <AvatarFallback>U</AvatarFallback>
          </Avatar>

          <div className="flex-1">
            <Input placeholder="Bugün ne düşünüyorsun?" className="rounded-xl" />
            <div className="mt-3 flex flex-wrap items-center justify-between gap-2">
              <div className="flex gap-2">
                <Badge variant="secondary" className="rounded-xl">
                  Fotoğraf
                </Badge>
                <Badge variant="secondary" className="rounded-xl">
                  Anket
                </Badge>
                <Badge variant="secondary" className="rounded-xl">
                  Konum
                </Badge>
              </div>
              <Button className="rounded-xl">Paylaş</Button>
            </div>
          </div>
        </div>
      </div>
    </Card>
  );
}

function PostCard({
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

/** =========================
 *  Right panel (Users from API)
 *  ========================= */
async function RightPanel() {
  let users: UserDto[] = [];
  let hasError = false;

  try {
    users = await getUsers();
  } catch {
    hasError = true;
  }

  const trends = [
    { tag: "#dotnet", posts: "12.4K" },
    { tag: "#react", posts: "9.1K" },
    { tag: "#startup", posts: "7.8K" },
    { tag: "#tasarım", posts: "6.2K" },
    { tag: "#yapayzeka", posts: "18.9K" },
  ];

  return (
    <aside className="hidden xl:block xl:col-span-4">
      <div className="sticky top-6 space-y-4">
        <Card className="rounded-2xl border bg-background/60 backdrop-blur supports-[backdrop-filter]:bg-background/40">
          <div className="p-5">
            <div className="text-sm font-semibold">Ara</div>
            <Input
              className="mt-3 rounded-xl"
              placeholder="Kişi, etiket, içerik..."
            />
          </div>
        </Card>

        <Card className="rounded-2xl border bg-background/60 backdrop-blur supports-[backdrop-filter]:bg-background/40">
          <div className="p-5">
            <div className="text-sm font-semibold">Trendler</div>
            <div className="mt-3 grid gap-2">
              {trends.map((t) => (
                <button
                  key={t.tag}
                  className="flex items-center justify-between rounded-xl px-3 py-2 hover:bg-accent transition"
                >
                  <span className="text-sm font-medium">{t.tag}</span>
                  <span className="text-xs text-muted-foreground">
                    {t.posts}
                  </span>
                </button>
              ))}
            </div>
          </div>
        </Card>

        <Card className="rounded-2xl border bg-background/60 backdrop-blur supports-[backdrop-filter]:bg-background/40">
          <div className="p-5">
            <div className="flex items-center justify-between">
              <div className="text-sm font-semibold">Önerilenler</div>
              <Badge variant="secondary" className="rounded-xl">
                {users.length}
              </Badge>
            </div>

            {hasError ? (
              <div className="mt-3 text-sm text-muted-foreground">
                Kullanıcılar alınamadı. (Endpoint/CORS kontrol)
              </div>
            ) : users.length === 0 ? (
              <div className="mt-3 text-sm text-muted-foreground">
                Şu an önerilecek kullanıcı yok.
              </div>
            ) : (
              <div className="mt-3 grid gap-3">
                {users.map((u) => {
                  const fullName = `${u.firstName} ${u.lastName}`;
                  const username = `${u.firstName}.${u.lastName}`
                    .toLowerCase()
                    .replaceAll(" ", "");

                  return (
                    <div
                      key={u.id}
                      className="flex items-center justify-between gap-3"
                    >
                      <div className="flex items-center gap-3 min-w-0">
                        <Avatar className="h-9 w-9">
                          <AvatarImage
                            src={`https://i.pravatar.cc/120?u=${u.id}`}
                          />
                          <AvatarFallback>
                            {u.firstName?.[0]}
                            {u.lastName?.[0]}
                          </AvatarFallback>
                        </Avatar>

                        <div className="min-w-0">
                          <div className="truncate text-sm font-semibold">
                            {fullName}
                          </div>
                          <div className="truncate text-xs text-muted-foreground">
                            @{username}
                          </div>
                        </div>
                      </div>

                      <Button variant="secondary" className="rounded-xl">
                        Takip Et
                      </Button>
                    </div>
                  );
                })}
              </div>
            )}
          </div>
        </Card>
      </div>
    </aside>
  );
}

/** =========================
 *  Mobile bottom nav
 *  ========================= */
function MobileNav() {
  return (
    <div className="lg:hidden fixed bottom-3 left-0 right-0 z-50 px-4">
      <Card className="mx-auto max-w-md rounded-2xl border bg-background/80 backdrop-blur supports-[backdrop-filter]:bg-background/60">
        <div className="flex items-center justify-between px-6 py-3 text-sm">
          <button className="rounded-xl px-3 py-2 hover:bg-accent transition">
            🏠
          </button>
          <button className="rounded-xl px-3 py-2 hover:bg-accent transition">
            🔎
          </button>
          <button className="rounded-xl px-3 py-2 hover:bg-accent transition">
            ➕
          </button>
          <button className="rounded-xl px-3 py-2 hover:bg-accent transition">
            💬
          </button>
          <button className="rounded-xl px-3 py-2 hover:bg-accent transition">
            👤
          </button>
        </div>
      </Card>
    </div>
  );
}

/** =========================
 *  Page
 *  ========================= */
export default function HomePage() {
  return (
    <main className="min-h-screen bg-gradient-to-b from-background to-muted/40 pb-24 lg:pb-6">
      <div className="mx-auto max-w-6xl px-4 py-6">
        <div className="grid grid-cols-12 gap-4">
          <LeftNav />

          <section className="col-span-12 lg:col-span-9 xl:col-span-6 space-y-4">
            <div className="flex items-center justify-between">
              <div>
                <div className="text-xl font-semibold tracking-tight">
                  Ana Sayfa
                </div>
                <div className="text-sm text-muted-foreground">
                  Senin için öne çıkanlar
                </div>
              </div>
              <Button variant="secondary" className="rounded-xl">
                Filtrele
              </Button>
            </div>

            <PostComposer />

            <PostCard
              name="Ece K."
              username="ece"
              time="2s"
              text="Backend hazır, şimdi premium bir frontendle işi bitiriyoruz. Minimal ama gerçekten şık."
              img="https://images.unsplash.com/photo-1521737604893-d14cc237f11d?auto=format&fit=crop&w=1200&q=80"
            />

            <PostCard
              name="Kerem"
              username="keremdev"
              time="5s"
              text="UI’da spacing + tipografi = profesyonel görünümün yarısı."
            />

            <PostCard
              name="Sinem"
              username="sinemui"
              time="1g"
              text="Dark mode ve kart tasarımı projeyi direkt bir seviye yukarı taşır."
            />
          </section>

          {/* ✅ Async Server Component */}
          <RightPanel />
        </div>
      </div>

      <MobileNav />
    </main>
  );
}
