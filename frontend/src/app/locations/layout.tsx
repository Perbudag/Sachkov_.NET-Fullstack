import { Metadata } from "next";
import { routes } from "../../shared/routers";

export const metadata: Metadata = {
  title: routes.directoryService.locations.title,
};

type route = typeof routes.directoryService.locations.url;

export default function RootLayout({ children }: LayoutProps<route>) {
  return <>{children}</>;
}
