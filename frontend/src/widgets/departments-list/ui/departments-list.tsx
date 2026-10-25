"use client";

import { useQuery } from "@tanstack/react-query";
import { departmentsApi, DepartmentsTable } from "@/entities/departments";
import { PagePagination } from "@/features/pagination";
import { QueryState } from "@/shared/components/query-state";

const PAGE_SIZE = 20;

type DepartmentsListProps = {
  page: number;
};

export function DepartmentsList({ page }: DepartmentsListProps) {
  const { data: pageResult, isLoading, error } = useQuery({
    queryFn: ({ signal }) =>
      departmentsApi.getDepartmentsListPage({ page, pageSize: PAGE_SIZE }, signal),
    queryKey: ["DepartmentItems", { page }],
  });

  const departments = pageResult?.value ?? [];
  const pagesCount = Math.ceil(
    (pageResult?.totalCount ?? 0) / (pageResult?.pageSize ?? 1),
  );

  return (
    <div className="flex flex-col p-5">
      <div className="overflow-hidden rounded-xl border">
        {departments.length > 0 && (
          <DepartmentsTable departments={departments} />
        )}
        <QueryState
          isLoading={isLoading}
          error={error}
          isEmpty={departments.length === 0}
        />
      </div>
      <PagePagination page={page} pagesCount={pagesCount} />
    </div>
  );
}
