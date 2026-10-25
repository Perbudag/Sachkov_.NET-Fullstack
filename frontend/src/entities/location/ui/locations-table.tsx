import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/shared/components/ui/table";
import { LocationListItem } from "../types";
import { addressToString } from "../lib/address-to-string";

type LocationsTableProps = {
  locations: LocationListItem[];
};

export function LocationsTable({ locations }: LocationsTableProps) {
  return (
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
        {locations.map((location) => (
          <TableRow key={location.id}>
            <TableCell>{location.id}</TableCell>
            <TableCell>{location.name}</TableCell>
            <TableCell className="max-w-[420px]">
              <div className="truncate">{addressToString(location.address)}</div>
            </TableCell>
            <TableCell>{location.departmentCount}</TableCell>
            <TableCell>
              {new Date(location.createdAt).toLocaleString()}
            </TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  );
}
