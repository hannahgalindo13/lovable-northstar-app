import { useState, useEffect } from "react";
import { Input } from "@/components/ui/input";
import { Search } from "lucide-react";
import { DataTable } from "@/components/DataTable";
import { FormModal } from "@/components/FormModal";
import { ConfirmDialog } from "@/components/ConfirmDialog";
import {
  createResident,
  deleteResident,
  getResidents,
  getSafehouses,
  updateResident,
  type Resident,
  type ResidentFormInput,
  type Safehouse,
} from "@/services/api";
import { Button } from "@/components/ui/button";

const CaseloadPage = () => {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [residents, setResidents] = useState<Resident[]>([]);
  const [safehouses, setSafehouses] = useState<Safehouse[]>([]);
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState("All");
  const [safehouseFilter, setSafehouseFilter] = useState("All");
  const [categoryFilter, setCategoryFilter] = useState("All");
  const [selectedResident, setSelectedResident] = useState<Resident | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<Resident | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<Resident | null>(null);
  const [form, setForm] = useState<ResidentFormInput>({
    name: "",
    caseCategory: "Neglected",
    status: "Active",
    safehouseId: 0,
    socialWorker: "",
  });

  const load = async () => {
    try {
      setLoading(true);
      const [residentData, safehouseData] = await Promise.all([getResidents(), getSafehouses()]);
      setResidents(residentData);
      setSafehouses(safehouseData);
      if (safehouseData.length > 0 && form.safehouseId === 0) {
        setForm((prev) => ({ ...prev, safehouseId: safehouseData[0].safehouseId }));
      }
    } catch (err) {
      console.error(err);
      setError("Failed to load resident data.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  const safehouseNameById = Object.fromEntries(safehouses.map((safehouse) => [safehouse.safehouseId, safehouse.name]));
  const categories = Array.from(new Set(residents.map((resident) => resident.caseCategory)));

  const filtered = residents.filter((resident) => {
    const text = `${resident.caseControlNo} ${resident.assignedSocialWorker ?? ""}`.toLowerCase();
    const matchesSearch = text.includes(search.toLowerCase());
    const matchesStatus = statusFilter === "All" || resident.caseStatus === statusFilter;
    const matchesSafehouse = safehouseFilter === "All" || String(resident.safehouseId) === safehouseFilter;
    const matchesCategory = categoryFilter === "All" || resident.caseCategory === categoryFilter;
    return matchesSearch && matchesStatus && matchesSafehouse && matchesCategory;
  });

  const openCreate = () => {
    setEditing(null);
    setForm({
      name: "",
      caseCategory: "Neglected",
      status: "Active",
      safehouseId: safehouses[0]?.safehouseId ?? 0,
      socialWorker: "",
    });
    setModalOpen(true);
  };

  const openEdit = (resident: Resident) => {
    setEditing(resident);
    setForm({
      name: resident.caseControlNo,
      caseCategory: resident.caseCategory,
      status: resident.caseStatus,
      safehouseId: resident.safehouseId,
      socialWorker: resident.assignedSocialWorker ?? "",
    });
    setModalOpen(true);
  };

  const saveCase = async () => {
    try {
      if (editing) {
        await updateResident(editing, form);
      } else {
        await createResident(form);
      }
      setModalOpen(false);
      await load();
    } catch (err) {
      console.error(err);
      setError("Failed to save case.");
    }
  };

  const removeCase = async () => {
    if (!deleteTarget) return;
    try {
      await deleteResident(deleteTarget.residentId);
      setDeleteTarget(null);
      await load();
    } catch (err) {
      console.error(err);
      setError("Failed to delete case.");
    }
  };

  return (
    <div className="space-y-8">
      <div>
        <h1 className="font-display text-2xl lg:text-3xl font-bold text-foreground">Caseload Inventory</h1>
        <p className="font-body text-sm text-muted-foreground mt-1">Search, filter, and inspect resident case records.</p>
      </div>

      <Button onClick={openCreate} className="w-fit">Add Case</Button>

      <div className="grid grid-cols-1 lg:grid-cols-4 gap-3">
        <div className="relative">
          <Search className="absolute left-3.5 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
          <Input placeholder="Search case or worker" value={search} onChange={(event) => setSearch(event.target.value)} className="pl-10 h-10 rounded-xl" />
        </div>
        <select className="h-10 rounded-xl border bg-background px-3 text-sm" value={statusFilter} onChange={(event) => setStatusFilter(event.target.value)}>
          <option value="All">All statuses</option>
          <option value="Active">Active</option>
          <option value="Closed">Closed</option>
          <option value="Transferred">Transferred</option>
        </select>
        <select className="h-10 rounded-xl border bg-background px-3 text-sm" value={safehouseFilter} onChange={(event) => setSafehouseFilter(event.target.value)}>
          <option value="All">All safehouses</option>
          {safehouses.map((safehouse) => (
            <option key={safehouse.safehouseId} value={safehouse.safehouseId}>
              {safehouse.name}
            </option>
          ))}
        </select>
        <select className="h-10 rounded-xl border bg-background px-3 text-sm" value={categoryFilter} onChange={(event) => setCategoryFilter(event.target.value)}>
          <option value="All">All categories</option>
          {categories.map((category) => (
            <option key={category} value={category}>
              {category}
            </option>
          ))}
        </select>
      </div>

      {loading && <p className="text-sm text-muted-foreground">Loading cases...</p>}
      {error && <p className="text-sm text-destructive">{error}</p>}

      {!loading && !error && (
        <div className="grid grid-cols-1 xl:grid-cols-3 gap-6">
          <div className="xl:col-span-2">
            <DataTable
              columns={[
                { key: "case", label: "Case", render: (resident) => resident.caseControlNo },
                { key: "category", label: "Category", render: (resident) => resident.caseCategory },
                { key: "status", label: "Status", render: (resident) => resident.caseStatus },
                {
                  key: "safehouse",
                  label: "Safehouse",
                  render: (resident) => safehouseNameById[resident.safehouseId] ?? `Safehouse #${resident.safehouseId}`,
                },
                { key: "worker", label: "Social Worker", render: (resident) => resident.assignedSocialWorker ?? "Unassigned" },
              ]}
              data={filtered}
              rowKey={(resident) => resident.residentId}
              onEdit={(resident) => {
                setSelectedResident(resident);
                openEdit(resident);
              }}
              onDelete={(resident) => setDeleteTarget(resident)}
            />
          </div>

          <div className="rounded-2xl bg-card p-5 shadow-sm">
            <h2 className="font-display text-lg font-semibold mb-3">Case Details</h2>
            {!selectedResident && <p className="text-sm text-muted-foreground">Select a resident to view demographic and case details.</p>}
            {selectedResident && (
              <div className="space-y-2 text-sm">
                <p><span className="text-muted-foreground">Case:</span> {selectedResident.caseControlNo}</p>
                <p><span className="text-muted-foreground">Status:</span> {selectedResident.caseStatus}</p>
                <p><span className="text-muted-foreground">Category:</span> {selectedResident.caseCategory}</p>
                <p><span className="text-muted-foreground">Safehouse:</span> {safehouseNameById[selectedResident.safehouseId] ?? selectedResident.safehouseId}</p>
                <p><span className="text-muted-foreground">Assigned Worker:</span> {selectedResident.assignedSocialWorker ?? "Unassigned"}</p>
                <p><span className="text-muted-foreground">Present Age:</span> {selectedResident.presentAge ?? "N/A"}</p>
                <p><span className="text-muted-foreground">PWD:</span> {selectedResident.isPwd ? "Yes" : "No"}</p>
                <p><span className="text-muted-foreground">Special Needs:</span> {selectedResident.hasSpecialNeeds ? "Yes" : "No"}</p>
                <p><span className="text-muted-foreground">Family Background:</span> Solo Parent {selectedResident.familySoloParent ? "Yes" : "No"}, Indigenous {selectedResident.familyIndigenous ? "Yes" : "No"}</p>
                <p><span className="text-muted-foreground">Admission:</span> {selectedResident.dateOfAdmission}</p>
                <p><span className="text-muted-foreground">Referral:</span> {selectedResident.referralSource ?? "N/A"}</p>
                <p><span className="text-muted-foreground">Reintegration:</span> {selectedResident.reintegrationStatus ?? "Not set"}</p>
              </div>
            )}
          </div>
        </div>
      )}

      <FormModal
        open={modalOpen}
        title={editing ? "Edit Case" : "Add Case"}
        onClose={() => setModalOpen(false)}
        onSubmit={saveCase}
        submitLabel={editing ? "Update" : "Create"}
      >
        <Input
          placeholder="Case reference"
          value={form.name}
          onChange={(event) => setForm((prev) => ({ ...prev, name: event.target.value }))}
        />
        <Input
          placeholder="Social worker"
          value={form.socialWorker}
          onChange={(event) => setForm((prev) => ({ ...prev, socialWorker: event.target.value }))}
        />
        <select
          className="h-10 rounded-xl border bg-background px-3 text-sm w-full"
          value={form.caseCategory}
          onChange={(event) => setForm((prev) => ({ ...prev, caseCategory: event.target.value }))}
        >
          <option value="Neglected">Neglected</option>
          <option value="Abandoned">Abandoned</option>
          <option value="Foundling">Foundling</option>
          <option value="Surrendered">Surrendered</option>
        </select>
        <select
          className="h-10 rounded-xl border bg-background px-3 text-sm w-full"
          value={form.status}
          onChange={(event) => setForm((prev) => ({ ...prev, status: event.target.value }))}
        >
          <option value="Active">Active</option>
          <option value="Closed">Closed</option>
          <option value="Transferred">Transferred</option>
        </select>
        <select
          className="h-10 rounded-xl border bg-background px-3 text-sm w-full"
          value={form.safehouseId}
          onChange={(event) => setForm((prev) => ({ ...prev, safehouseId: Number(event.target.value) }))}
        >
          {safehouses.map((safehouse) => (
            <option key={safehouse.safehouseId} value={safehouse.safehouseId}>
              {safehouse.name}
            </option>
          ))}
        </select>
      </FormModal>

      <ConfirmDialog
        open={Boolean(deleteTarget)}
        onCancel={() => setDeleteTarget(null)}
        onConfirm={removeCase}
        description={deleteTarget ? `Delete case ${deleteTarget.caseControlNo}?` : "Delete case?"}
      />
    </div>
  );
};

export default CaseloadPage;
