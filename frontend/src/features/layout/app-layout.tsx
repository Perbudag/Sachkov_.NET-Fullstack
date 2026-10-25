"use client";

import { SidebarInset, SidebarProvider } from "@/shared/components/ui/sidebar";
import { AppSidebar } from "../sidebar/app-sidebar";
import { AppHeader } from "../header/app-header";
import { QueryClientProvider } from "@tanstack/react-query";
import { queryClient } from "@/shared/api/query-client";

export const AppLayout = ({
  children,
  ...props
}: React.ComponentProps<typeof SidebarInset>) => {
  return (
    <QueryClientProvider client={queryClient}>
      <SidebarProvider defaultOpen={false}>
        <AppSidebar />
        <SidebarInset {...props}>
          <AppHeader />
          {children}
        </SidebarInset>
      </SidebarProvider>
    </QueryClientProvider>
  );
};
