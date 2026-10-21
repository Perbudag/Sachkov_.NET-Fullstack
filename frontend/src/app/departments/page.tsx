"use client";

import React from "react";
import { routes } from "../../shared/routers";

export default function departments() {
  return (
    <div className="flex justify-center">
      <h1 className="text-4xl font-bold tracking-tight">
        {routes.directoryService.departments.title}
      </h1>
    </div>
  );
}
