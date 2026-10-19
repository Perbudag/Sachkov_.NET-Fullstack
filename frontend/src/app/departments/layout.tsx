import { Metadata } from "next";
import { routes } from "../routers";

export const metadata: Metadata = {
  title: routes.directoryService.departments.title,
};

type route = typeof routes.directoryService.departments.url;

export default function RootLayout({ children }: LayoutProps<route>) {
  return <>{children}</>;
}
