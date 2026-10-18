import { Metadata } from "next";

export const metadata: Metadata = {
  title: "Позиции",
};

export default function RootLayout({ children }: LayoutProps<"/">) {
  return <>{children}</>;
}
