"use client";

import { useQuery } from "@tanstack/react-query";
import { locationsApi, LocationsTable } from "@/entities/location";
import { PagePagination } from "@/features/pagination";
import { QueryState } from "@/shared/components/query-state";

const PAGE_SIZE = 20;

type LocationsListProps = {
  page: number;
};

export function LocationsList({ page }: LocationsListProps) {
  const { data: pageResult, isLoading, error } = useQuery({
    queryFn: ({ signal }) =>
      locationsApi.getLocationsListPage({ page, pageSize: PAGE_SIZE }, signal),
    queryKey: ["LocationItems", { page }],
  });

  const locations = pageResult?.value ?? [];
  const pagesCount = Math.ceil(
    (pageResult?.totalCount ?? 0) / (pageResult?.pageSize ?? 1),
  );

  return (
    <div className="flex flex-col p-5">
      <div className="overflow-hidden rounded-xl border">
        {locations.length > 0 && <LocationsTable locations={locations} />}
        <QueryState
          isLoading={isLoading}
          error={error}
          isEmpty={locations.length === 0}
        />
      </div>
      <PagePagination page={page} pagesCount={pagesCount} />
    </div>
  );
}
