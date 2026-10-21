import { apiClient } from "@/shared/api/axios-instance";
import { Envelope } from "@/shared/types/envelope.type";
import { PageResult } from "@/shared/types/page-result.type";
import { LocationListItem } from "./types";

export type GetAllLocationsPageRequest = {
  Search?: string;
  MinDepartmentCount?: number;
  SortBy?: string;
  sordDir?: string;
  page: number;
  pageSize: number;
};

export const locationsApi = {
  getLocationsPage: async (
    request: GetAllLocationsPageRequest,
    signal?: AbortSignal,
  ): Promise<PageResult<LocationListItem[]> | null> => {
    const response = await apiClient.get<
      Envelope<PageResult<LocationListItem[]>>
    >("/locations", { params: request, signal: signal });

    return response.data.result || null;
  },
};
