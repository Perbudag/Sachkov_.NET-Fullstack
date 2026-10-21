"use client";

import React from "react";
import {
  Sidebar,
  SidebarContent,
  SidebarGroup,
  SidebarGroupContent,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuSub,
} from "@/shared/components/ui/sidebar";
import { usePathname, useRouter } from "next/navigation";
import { routes } from "@/shared/routers";

const navRoutes = [
  {
    title: "Directory Service",
    url: "/",
    pages: Object.values(routes.directoryService),
  },
];

export const AppSidebar = ({
  ...props
}: React.ComponentProps<typeof Sidebar>) => {
  const pathname = usePathname();
  const router = useRouter();

  const navItems = navRoutes.map((service) => ({
    ...service,
    isActive: service.url === pathname,
    pages: service.pages.map((page) => ({
      ...page,
      isActive: page.url === pathname,
    })),
  }));

  return (
    <Sidebar {...props}>
      <SidebarContent>
        <SidebarGroup>
          <SidebarGroupContent>
            {navItems.map((s) => (
              <SidebarMenu key={s.url}>
                <SidebarMenuButton
                  onClick={() => router.push(s.url)}
                  isActive={s.isActive}
                >
                  {s.title}
                </SidebarMenuButton>
                <SidebarMenuSub>
                  {s.pages.map((p) => (
                    <SidebarMenuButton
                      key={p.url}
                      onClick={() => router.push(p.url)}
                      isActive={p.isActive}
                    >
                      {p.title}
                    </SidebarMenuButton>
                  ))}
                </SidebarMenuSub>
              </SidebarMenu>
            ))}
          </SidebarGroupContent>
        </SidebarGroup>
      </SidebarContent>
    </Sidebar>
  );
};
