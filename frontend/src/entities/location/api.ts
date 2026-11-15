import { apiClient } from "@/shared/api/axios-instance";
import { PageResult } from "@/shared/types/page-result.type";
import { LocationListItem } from "./types";
import type { Address } from "@/shared/types/dto/address.type";

export type GetLocationsListPageRequest = {
  Search?: string;
  MinDepartmentCount?: number;
  SortBy?: string;
  sordDir?: string;
  page: number;
  pageSize: number;
};

export type CreateLocationRequest = {
  name: string;
  address: Address;
};

export const locationsApi = {
  getListPage: async (
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

  create: async (request: CreateLocationRequest) => {
    const response = await apiClient.post<string>("/locations", request);

    return response.data;
  },
};
