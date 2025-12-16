import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { Input } from "@/components/ui/input";

export function PostComposer() {
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
                <Badge variant="secondary" className="rounded-xl">Fotoğraf</Badge>
                <Badge variant="secondary" className="rounded-xl">Anket</Badge>
                <Badge variant="secondary" className="rounded-xl">Konum</Badge>
              </div>
              <Button className="rounded-xl">Paylaş</Button>
            </div>
          </div>
        </div>
      </div>
    </Card>
  );
}
