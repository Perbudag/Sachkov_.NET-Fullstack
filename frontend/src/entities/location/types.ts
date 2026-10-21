import { Address } from "@/shared/types/dto/address.type";

export type LocationListItem = {
  id: string;
  name: string;
  address: Address;
  departmentCount: number;
  createdAt: Date;
};
