"use client";

import React from "react";
import { Button } from "@/components/ui/button";
import { useRouter } from "next/navigation";
import { routes } from "./routers";

export default function Home() {
  const router = useRouter();

  return (
    <div className="flex flex-col flex-1 items-center justify-center font-sans min-h-screen">
      <main className="flex flex-1 w-full min-w-md max-w-1/2 flex-col gap-8 py-8 px-8 bg-card sm:items-center shadow-sm">
        <div className="flex flex-col w-64 gap-2">
          {Object.values(routes.directoryService).map((route) => (
            <Button key={route.url} onClick={() => router.push(route.url)}>
              {route.title}
            </Button>
          ))}
        </div>
      </main>
    </div>
  );
}
