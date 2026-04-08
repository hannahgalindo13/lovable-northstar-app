import { Button } from "@/components/ui/button";

type FormModalProps = {
  open: boolean;
  title: string;
  onClose: () => void;
  onSubmit: () => void;
  children: React.ReactNode;
  submitLabel?: string;
};

export const FormModal = ({
  open,
  title,
  onClose,
  onSubmit,
  children,
  submitLabel = "Save",
}: FormModalProps) => {
  if (!open) return null;

  return (
    <div className="fixed inset-0 z-50 bg-black/40 flex items-center justify-center p-4">
      <div className="w-full max-w-lg rounded-2xl bg-card p-5 shadow-lg space-y-4">
        <h3 className="font-display text-xl font-semibold">{title}</h3>
        <div className="space-y-3">{children}</div>
        <div className="flex justify-end gap-2">
          <Button variant="outline" onClick={onClose}>
            Cancel
          </Button>
          <Button onClick={onSubmit}>{submitLabel}</Button>
        </div>
      </div>
    </div>
  );
};
