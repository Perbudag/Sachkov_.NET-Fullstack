"use client";

import React, { useState } from "react";
import { locationsApi } from "@/entities/location/api";
import { useSearchParams } from "next/navigation";
import { LocationListItem } from "@/entities/location/types";
import {
  Pagination,
  PaginationContent,
  PaginationEllipsis,
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
import { useQuery } from "@tanstack/react-query";
import axios, { AxiosError } from "axios";
import { Envelope } from "@/shared/types/envelope.type";
import { isEnvelopeError } from "@/shared/api/axios-instance";

const PAGE_SIZE = 20;
const MAX_PAGINATION_ITEMS = 10;
const PAGINATION_BOUNDARIES = 2;
const PAGINATION_RADIUS = Math.floor(
  (MAX_PAGINATION_ITEMS - PAGINATION_BOUNDARIES * 2 - 2) / 2,
);

export default function locations() {
  const searchParams = useSearchParams();
  const page = Number(searchParams.get("page")) || 1;
  const {
    data: pageResult,
    isLoading,
    error,
  } = useQuery({
    queryFn: ({ signal }) =>
      locationsApi.getLocationsListPage(
        { page: page, pageSize: PAGE_SIZE },
        signal,
      ),
    queryKey: ["LocationItems", { page }],
  });

  const locationItems = pageResult?.value ?? [];
  const pagesCount = Math.ceil(
    (pageResult?.totalCount ?? 0) / (pageResult?.pageSize ?? 1),
  );

  const paginationRender = () => {
    // Хелпер для рендеринга одной кнопки, чтобы не дублировать код
    const renderItem = (num: number) => (
      <PaginationItem key={num}>
        <PaginationLink href={`?page=${num}`} isActive={num === page}>
          {num}
        </PaginationLink>
      </PaginationItem>
    );

    // Простой случай: страниц мало, выводим их все подряд
    if (pagesCount <= MAX_PAGINATION_ITEMS) {
      return Array.from({ length: pagesCount }, (_, i) => renderItem(i + 1));
    }

    const items: React.ReactNode[] = [];

    // 1. Левая граница (от 1 до BOUNDARY_PAGES)
    const leftBoundary = Array.from(
      { length: PAGINATION_BOUNDARIES },
      (_, i) => i + 1,
    );
    leftBoundary.forEach((num) => items.push(renderItem(num)));

    // Вычисляем корректные рамки для центрального диапазона
    let startRange = Math.max(
      page - PAGINATION_RADIUS,
      PAGINATION_BOUNDARIES + 1,
    );
    let endRange = Math.min(
      page + PAGINATION_RADIUS,
      pagesCount - PAGINATION_BOUNDARIES,
    );

    // Корректируем длину центра, если текущая страница слишком близко к краям
    if (page - PAGINATION_BOUNDARIES <= PAGINATION_RADIUS) {
      endRange = Math.min(
        PAGINATION_BOUNDARIES + 1 + PAGINATION_RADIUS * 2,
        pagesCount - PAGINATION_BOUNDARIES,
      );
    } else if (pagesCount - PAGINATION_BOUNDARIES - page <= PAGINATION_RADIUS) {
      startRange = Math.max(
        pagesCount - PAGINATION_BOUNDARIES - PAGINATION_RADIUS * 2,
        PAGINATION_BOUNDARIES + 1,
      );
    }

    // 2. Левое многоточие
    if (startRange > PAGINATION_BOUNDARIES + 1) {
      items.push(<PaginationEllipsis key="left-ellipsis" />);
    }

    // 3. Центральный диапазон страниц вокруг текущей
    const centerLength = endRange - startRange + 1;
    if (centerLength > 0) {
      Array.from({ length: centerLength }, (_, i) => startRange + i).forEach(
        (num) => items.push(renderItem(num)),
      );
    }

    // 4. Правое многоточие
    if (endRange < pagesCount - PAGINATION_BOUNDARIES) {
      items.push(<PaginationEllipsis key="right-ellipsis" />);
    }

    // 5. Правая граница (последние страницы на основе BOUNDARY_PAGES)
    const rightBoundary = Array.from(
      { length: PAGINATION_BOUNDARIES },
      (_, i) => pagesCount - PAGINATION_BOUNDARIES + i + 1,
    );
    rightBoundary.forEach((num) => items.push(renderItem(num)));

    return items;
  };

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
        {locationItems.length > 0 && (
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
              {locationItems?.map((item) => (
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
        )}
        {(isLoading || locationItems?.length === 0) && (
          <div className="flex flex-col items-center justify-center h-50">
            <div className="flex flex-row justify-center items-center text-2xl font-semibold">
              {isLoading ? (
                <Spinner className="size-10" />
              ) : error ? (
                <h1>{error.message}</h1>
              ) : (
                locationItems?.length === 0 && <>Список пуст</>
              )}
            </div>
          </div>
        )}
      </div>
      {pagesCount > 0 && (
        <Pagination className="mt-5">
          <PaginationContent>
            <PaginationItem>
              <PaginationPrevious
                className={
                  page <= 1 && page < pagesCount
                    ? "pointer-events-none opacity-50"
                    : ""
                }
                href={`?page=${page - 1}`}
                text="Назад"
              />
            </PaginationItem>
            {paginationRender()}
            <PaginationItem>
              <PaginationNext
                className={
                  page > pagesCount - 1 ? "pointer-events-none opacity-50" : ""
                }
                href={`?page=${page + 1}`}
                text="Вперед"
              />
            </PaginationItem>
          </PaginationContent>
        </Pagination>
      )}
    </div>
  );
}
