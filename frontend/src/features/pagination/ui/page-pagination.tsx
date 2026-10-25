import React from "react";
import {
  Pagination,
  PaginationContent,
  PaginationEllipsis,
  PaginationItem,
  PaginationLink,
  PaginationNext,
  PaginationPrevious,
} from "@/shared/components/ui/pagination";

const MAX_PAGINATION_ITEMS = 10;
const PAGINATION_BOUNDARIES = 2;
const PAGINATION_RADIUS = Math.floor(
  (MAX_PAGINATION_ITEMS - PAGINATION_BOUNDARIES * 2 - 2) / 2,
);

type PagePaginationProps = {
  page: number;
  pagesCount: number;
};

export function PagePagination({ page, pagesCount }: PagePaginationProps) {
  if (pagesCount <= 0) {
    return null;
  }

  const renderItem = (number: number) => (
    <PaginationItem key={number}>
      <PaginationLink href={`?page=${number}`} isActive={number === page}>
        {number}
      </PaginationLink>
    </PaginationItem>
  );

  let pageItems: React.ReactNode[];

  if (pagesCount <= MAX_PAGINATION_ITEMS) {
    pageItems = Array.from({ length: pagesCount }, (_, index) =>
      renderItem(index + 1),
    );
  } else {
    pageItems = [];

    Array.from({ length: PAGINATION_BOUNDARIES }, (_, index) => index + 1).forEach(
      (number) => pageItems.push(renderItem(number)),
    );

    let startRange = Math.max(
      page - PAGINATION_RADIUS,
      PAGINATION_BOUNDARIES + 1,
    );
    let endRange = Math.min(
      page + PAGINATION_RADIUS,
      pagesCount - PAGINATION_BOUNDARIES,
    );

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

    if (startRange > PAGINATION_BOUNDARIES + 1) {
      pageItems.push(<PaginationEllipsis key="left-ellipsis" />);
    }

    const centerLength = endRange - startRange + 1;
    if (centerLength > 0) {
      Array.from({ length: centerLength }, (_, index) => startRange + index).forEach(
        (number) => pageItems.push(renderItem(number)),
      );
    }

    if (endRange < pagesCount - PAGINATION_BOUNDARIES) {
      pageItems.push(<PaginationEllipsis key="right-ellipsis" />);
    }

    Array.from(
      { length: PAGINATION_BOUNDARIES },
      (_, index) => pagesCount - PAGINATION_BOUNDARIES + index + 1,
    ).forEach((number) => pageItems.push(renderItem(number)));
  }

  return (
    <Pagination className="mt-5">
      <PaginationContent>
        <PaginationItem>
          <PaginationPrevious
            className={
              page <= 1 ? "pointer-events-none opacity-50" : undefined
            }
            href={`?page=${page - 1}`}
            text="Назад"
          />
        </PaginationItem>
        {pageItems}
        <PaginationItem>
          <PaginationNext
            className={
              page >= pagesCount ? "pointer-events-none opacity-50" : undefined
            }
            href={`?page=${page + 1}`}
            text="Вперед"
          />
        </PaginationItem>
      </PaginationContent>
    </Pagination>
  );
}
