"use client";

import React from "react";
import { routes } from "../routers";

export default function positions() {
  return (
    <div className="flex justify-center">
      <h1 className="text-4xl font-bold tracking-tight">
        {routes.directoryService.positions.title}
      </h1>
    </div>
  );
}
