import { DepartmentsList } from "@/widgets/departments-list";

export default async function DepartmentsRoute({
  searchParams,
}: PageProps<"/departments">) {
  const { page: pageParam } = await searchParams;
  const page = Number(Array.isArray(pageParam) ? pageParam[0] : pageParam) || 1;

  return <DepartmentsList page={page} />;
}
