import { Metadata } from "next";
import { routes } from "../../shared/routers";

export const metadata: Metadata = {
  title: routes.directoryService.positions.title,
};

type route = typeof routes.directoryService.positions.url;

export default function RootLayout({ children }: LayoutProps<route>) {
  return <>{children}</>;
}
