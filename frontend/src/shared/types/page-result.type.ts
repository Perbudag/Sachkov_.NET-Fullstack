export type PageResult<T> = {
  value: T;
  totalCount: number;
  pageNumber: number;
  pageSize: number;
};
