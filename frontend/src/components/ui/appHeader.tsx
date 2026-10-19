import { Separator } from "./separator";
import { SidebarTrigger } from "./sidebar";

export const AppHeader = () => {
  return (
    <header className="flex h-16 shrink-0 items-center gap-2 border-b px-4 sticky top-0 z-50 bg-background">
      <SidebarTrigger className="-ml-1" />
    </header>
  );
};
