"use client";

import { SidebarInset, SidebarProvider } from "@/shared/components/ui/sidebar";
import { AppSidebar } from "../sidebar/app-sidebar";
import { AppHeader } from "../header/app-header";
import { QueryClientProvider } from "@tanstack/react-query";
import { queryClient } from "@/shared/api/query-client";
import { Toaster } from "sonner";
import { ErrorBoundary } from "@/shared/components/error-boundary";

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
          <ErrorBoundary>{children}</ErrorBoundary>
          <Toaster position="top-center" duration={3000} richColors={true} />
        </SidebarInset>
      </SidebarProvider>
    </QueryClientProvider>
  );
};
