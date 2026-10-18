"use client";

import React from "react";
import { Button } from "@/components/ui/button";
import { useRouter } from "next/navigation";

export default function Home() {
  const router = useRouter();

  return (
    <div className="flex flex-col flex-1 items-center justify-center bg-zinc-50 font-sans min-h-screen">
      <main className="flex flex-1 w-full max-w-md flex-col gap-8 py-8 px-8 bg-white sm:items-center shadow-sm">
        <div className="flex flex-col w-64 gap-2">
          <Button onClick={() => router.push("/locations")}>Локации</Button>
          <Button onClick={() => router.push("/departments")}>
            Подразделения
          </Button>
          <Button onClick={() => router.push("/positions")}>Позиции</Button>
        </div>
      </main>
    </div>
  );
}
