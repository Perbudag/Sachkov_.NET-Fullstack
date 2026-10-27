import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/shared/components/ui/table";
import { DepartmentListItem } from "../types";

type DepartmentsTableProps = {
  departments: DepartmentListItem[];
};

export function DepartmentsTable({ departments }: DepartmentsTableProps) {
  return (
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead>Id</TableHead>
          <TableHead>Название</TableHead>
          <TableHead>Путь</TableHead>
          <TableHead>Дата создания</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {departments.map((department) => (
          <TableRow key={department.id}>
            <TableCell>{department.id}</TableCell>
            <TableCell>{department.name}</TableCell>
            <TableCell className="max-w-[420px]">
              <div className="truncate">{department.path}</div>
            </TableCell>
            <TableCell>
              {new Date(department.createdAt).toLocaleString()}
            </TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  );
}
