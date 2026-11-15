"use client";

import { useQuery } from "@tanstack/react-query";
import { locationsApi, LocationsTable } from "@/entities/location";
import { PagePagination } from "@/features/pagination";
import { QueryState } from "@/shared/components/query-state";
import { Button } from "@/shared/components/ui/button";
import { CreateLocationDialog } from "@/features/locations";
import React from "react";

const PAGE_SIZE = 18;

type LocationsListProps = {
  page: number;
};

export function LocationsList({ page }: LocationsListProps) {
  const {
    data: pageResult,
    isLoading,
    error,
  } = useQuery({
    queryFn: ({ signal }) =>
      locationsApi.getListPage({ page, pageSize: PAGE_SIZE }, signal),
    queryKey: ["LocationItems", { page }],
  });

  const locations = pageResult?.value ?? [];
  const pagesCount = Math.ceil(
    (pageResult?.totalCount ?? 0) / (pageResult?.pageSize ?? 1),
  );

  const [isOpen, setIsOpen] = React.useState(false);

  return (
    <div className="flex flex-col p-5">
      <div className="flex flex-row justify-end mb-3">
        <Button size="sm" onClick={() => setIsOpen(true)}>
          Создать
        </Button>
      </div>

      <div className="overflow-hidden rounded-xl border">
        {locations.length > 0 && <LocationsTable locations={locations} />}
        <QueryState
          isLoading={isLoading}
          error={error}
          isEmpty={locations.length === 0}
        />
      </div>

      <PagePagination page={page} pagesCount={pagesCount} />

      <CreateLocationDialog
        open={isOpen}
        onOpenChange={(open) => setIsOpen(open)}
      />
    </div>
  );
}
