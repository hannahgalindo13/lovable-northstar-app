import { Button } from "@/components/ui/button";

type Column<T> = {
  key: string;
  label: string;
  render: (row: T) => React.ReactNode;
};

type DataTableProps<T> = {
  columns: Column<T>[];
  data: T[];
  rowKey: (row: T) => string | number;
  onEdit?: (row: T) => void;
  onDelete?: (row: T) => void;
};

export const DataTable = <T,>({ columns, data, rowKey, onEdit, onDelete }: DataTableProps<T>) => {
  return (
    <div className="rounded-2xl bg-card shadow-sm overflow-hidden">
      <table className="w-full text-sm">
        <thead className="bg-muted/40">
          <tr>
            {columns.map((column) => (
              <th key={column.key} className="text-left px-4 py-3 font-medium text-muted-foreground">
                {column.label}
              </th>
            ))}
            {(onEdit || onDelete) && <th className="text-right px-4 py-3 font-medium text-muted-foreground">Actions</th>}
          </tr>
        </thead>
        <tbody>
          {data.map((row) => (
            <tr key={rowKey(row)} className="border-t border-border/40">
              {columns.map((column) => (
                <td key={column.key} className="px-4 py-3 align-top">
                  {column.render(row)}
                </td>
              ))}
              {(onEdit || onDelete) && (
                <td className="px-4 py-3">
                  <div className="flex justify-end gap-2">
                    {onEdit && (
                      <Button variant="outline" size="sm" onClick={() => onEdit(row)}>
                        Edit
                      </Button>
                    )}
                    {onDelete && (
                      <Button variant="destructive" size="sm" onClick={() => onDelete(row)}>
                        Delete
                      </Button>
                    )}
                  </div>
                </td>
              )}
            </tr>
          ))}
        </tbody>
      </table>
      {data.length === 0 && <div className="p-6 text-sm text-muted-foreground">No records found.</div>}
    </div>
  );
};
