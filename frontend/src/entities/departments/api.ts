import { apiClient } from "@/shared/api/axios-instance";
import { PageResult } from "@/shared/types/page-result.type";
import { DepartmentListItem } from "./types";

export type GetDepartmentsListPageRequest = {
  search?: string;
  sortBy?: string;
  sortDir?: string;
  page: number;
  pageSize: number;
};

export const departmentsApi = {
  getDepartmentsListPage: async (
    request: GetDepartmentsListPageRequest,
    signal?: AbortSignal,
  ) => {
    const response = await apiClient.get<PageResult<DepartmentListItem[]>>(
      "/departments",
      {
        params: request,
        signal: signal,
      },
    );

    return response.data;
  },
};
