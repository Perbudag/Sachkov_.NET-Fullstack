"use client";

import React from "react";
import { routes } from "../routers";

export default function locations() {
  return (
    <div className="flex justify-center">
      <h1 className="text-4xl font-bold tracking-tight">
        {routes.directoryService.locations.title}
      </h1>
    </div>
  );
}
