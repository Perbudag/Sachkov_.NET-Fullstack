import { Address } from "@/shared/types/dto/address.type";

export function addressToString(address: Address) {
  return (
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
    (address.apartment ?? "")
  );
}
