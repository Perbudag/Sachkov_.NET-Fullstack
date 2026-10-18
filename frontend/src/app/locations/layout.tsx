import { Metadata } from "next";

export const metadata: Metadata = {
  title: "Локации",
};

export default function RootLayout({ children }: LayoutProps<"/">) {
  return <>{children}</>;
}
