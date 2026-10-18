import { Metadata } from "next";

export const metadata: Metadata = {
  title: "Подразделения",
};

export default function RootLayout({ children }: LayoutProps<"/">) {
  return <>{children}</>;
}
