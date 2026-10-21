"use client";

import React, { useState } from "react";
import { locationsApi } from "@/entities/location/api";
import { useSearchParams } from "next/navigation";
import { LocationListItem } from "@/entities/location/types";
import {
  Pagination,
  PaginationContent,
  PaginationItem,
  PaginationLink,
  PaginationNext,
  PaginationPrevious,
} from "@/shared/components/ui/pagination";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/shared/components/ui/table";
import { Address } from "@/shared/types/dto/address.type";
import { Spinner } from "@/shared/components/ui/spinner";

const PAGE_SIZE = 15;

export default function locations() {
  const searchParams = useSearchParams();
  const [locationItems, setLocationItems] = React.useState<LocationListItem[]>(
    [],
  );
  const [paginationItems, setPaginationItems] = useState<number[]>([]);
  const [isLoading, setIsLoading] = React.useState(true);
  const [error, setError] = React.useState<string | null>(null);
  const page = Number(searchParams.get("page")) || 1;

  React.useEffect((): any => {
    const controller = new AbortController();

    setPaginationItems([]);

    locationsApi
      .getLocationsPage({ page: page, pageSize: PAGE_SIZE }, controller.signal)
      .then((page) => {
        setLocationItems(page!.value);

        const pageCount = Math.ceil(page!.totalCount / PAGE_SIZE);
        const pagesArray: number[] = [];

        for (let pNum: number = 1; pNum <= pageCount; pNum++) {
          pagesArray.push(pNum);
        }

        setPaginationItems(pagesArray);
      })
      .catch((error) => {
        setError(error?.message || "Произошла ошибка при загрузке данных");
      })
      .finally(() => {
        setIsLoading(false);
      });

    return () => controller.abort();
  }, [page]);

  const addressToString = (address: Address) =>
    address.postalCode +
    ", " +
    address.country +
    ", " +
    address.region +
    ", г. " +
    address.city +
    ", ул. " +
    address.street +
    ", д. " +
    address.house +
    ", кв. " +
    (address.apartment ?? "");

  return (
    <div className="flex flex-col p-5">
      <div className="border rounded-xl overflow-hidden">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Id</TableHead>
              <TableHead>Название</TableHead>
              <TableHead>Адрес</TableHead>
              <TableHead>Количество подразделений</TableHead>
              <TableHead>Дата создания</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {locationItems.map((item) => (
              <TableRow key={item.id}>
                <TableCell>{item.id}</TableCell>
                <TableCell>{item.name}</TableCell>
                <TableCell>{addressToString(item.address)}</TableCell>
                <TableCell>{item.departmentCount}</TableCell>
                <TableCell>
                  {new Date(item.createdAt).toLocaleString()}
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
        {(isLoading || locationItems.length === 0) && (
          <div className="flex flex-col items-center justify-center h-50">
            <div className="flex flex-row justify-center items-center text-2xl font-semibold">
              {isLoading ? (
                <Spinner className="size-10" />
              ) : error != null ? (
                <h1>{error}</h1>
              ) : (
                locationItems.length === 0 && <>Список пуст</>
              )}
            </div>
          </div>
        )}
      </div>
      <Pagination className="mt-5">
        <PaginationContent>
          {page > 1 && page < paginationItems.length && (
            <PaginationItem>
              <PaginationPrevious href={`?page=${page - 1}`} text="Назад" />
            </PaginationItem>
          )}
          {paginationItems.map((num) => (
            <PaginationItem key={num}>
              <PaginationLink href={`?page=${num}`} isActive={num === page}>
                {num}
              </PaginationLink>
            </PaginationItem>
          ))}
          {page < paginationItems.length && (
            <PaginationItem>
              <PaginationNext href={`?page=${page + 1}`} text="Вперед" />
            </PaginationItem>
          )}
        </PaginationContent>
      </Pagination>
    </div>
  );
}
