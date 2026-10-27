import { apiClient } from "@/shared/api/axios-instance";
import { Envelope } from "@/shared/types/envelope.type";
import { PageResult } from "@/shared/types/page-result.type";
import { LocationListItem } from "./types";

export type GetLocationsListPageRequest = {
  Search?: string;
  MinDepartmentCount?: number;
  SortBy?: string;
  sordDir?: string;
  page: number;
  pageSize: number;
};

export const locationsApi = {
  getLocationsListPage: async (
    request: GetLocationsListPageRequest,
    signal?: AbortSignal,
  ) => {
    const response = await apiClient.get<PageResult<LocationListItem[]>>(
      "/locations",
      {
        params: request,
        signal: signal,
      },
    );

    return response.data;
  },
};
