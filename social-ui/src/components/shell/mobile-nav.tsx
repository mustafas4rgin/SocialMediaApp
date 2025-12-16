import { Card } from "@/components/ui/card";

export function MobileNav() {
  return (
    <div className="lg:hidden fixed bottom-3 left-0 right-0 z-50 px-4">
      <Card className="mx-auto max-w-md rounded-2xl border bg-background/80 backdrop-blur supports-[backdrop-filter]:bg-background/60">
        <div className="flex items-center justify-between px-6 py-3 text-sm">
          <button className="rounded-xl px-3 py-2 hover:bg-accent transition">🏠</button>
          <button className="rounded-xl px-3 py-2 hover:bg-accent transition">🔎</button>
          <button className="rounded-xl px-3 py-2 hover:bg-accent transition">➕</button>
          <button className="rounded-xl px-3 py-2 hover:bg-accent transition">💬</button>
          <button className="rounded-xl px-3 py-2 hover:bg-accent transition">👤</button>
        </div>
      </Card>
    </div>
  );
}
