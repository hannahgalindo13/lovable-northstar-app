import { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { DataTable } from "@/components/DataTable";
import { FormModal } from "@/components/FormModal";
import { ConfirmDialog } from "@/components/ConfirmDialog";
import {
  createSupporter,
  deleteSupporter,
  getDonations,
  getSupporters,
  updateSupporter,
  type Donation,
  type Supporter,
} from "@/services/api";

const DonorsPage = () => {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [supporters, setSupporters] = useState<Supporter[]>([]);
  const [donations, setDonations] = useState<Donation[]>([]);
  const [search, setSearch] = useState("");
  const [modalOpen, setModalOpen] = useState(false);
  const [deleteTarget, setDeleteTarget] = useState<Supporter | null>(null);
  const [editing, setEditing] = useState<Supporter | null>(null);
  const [form, setForm] = useState({ displayName: "", supporterType: "MonetaryDonor", status: "Active" });

  const load = async () => {
    try {
      setLoading(true);
      setError(null);
      const [supportersData, donationsData] = await Promise.all([getSupporters(), getDonations()]);
      setSupporters(supportersData);
      setDonations(donationsData);
    } catch (err) {
      console.error(err);
      setError("Failed to load donor data.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  const openCreate = () => {
    setEditing(null);
    setForm({ displayName: "", supporterType: "MonetaryDonor", status: "Active" });
    setModalOpen(true);
  };

  const openEdit = (supporter: Supporter) => {
    setEditing(supporter);
    setForm({
      displayName: supporter.displayName,
      supporterType: supporter.supporterType,
      status: supporter.status,
    });
    setModalOpen(true);
  };

  const handleSave = async () => {
    if (!form.displayName.trim()) return;
    try {
      if (editing) {
        await updateSupporter(editing.supporterId, {
          ...editing,
          displayName: form.displayName,
          supporterType: form.supporterType,
          status: form.status,
          relationshipType: "Local",
        });
      } else {
        await createSupporter({
          displayName: form.displayName,
          supporterType: form.supporterType,
          relationshipType: "Local",
          status: form.status,
          email: null,
        });
      }
      setModalOpen(false);
      await load();
    } catch (err) {
      console.error(err);
      setError("Could not save donor.");
    }
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    try {
      await deleteSupporter(deleteTarget.supporterId);
      setDeleteTarget(null);
      await load();
    } catch (err) {
      console.error(err);
      setError("Could not delete donor.");
    }
  };

  const filtered = supporters.filter((supporter) => {
    const matchSearch =
      supporter.displayName.toLowerCase().includes(search.toLowerCase()) ||
      (supporter.email ?? "").toLowerCase().includes(search.toLowerCase());
    return matchSearch;
  });

  const totalsByDonor = donations.reduce<Record<number, { monetary: number; time: number; skills: number; inKind: number }>>(
    (acc, donation) => {
      if (!acc[donation.supporterId]) {
        acc[donation.supporterId] = { monetary: 0, time: 0, skills: 0, inKind: 0 };
      }
      const value = Number(donation.amount ?? donation.estimatedValue ?? 0);
      if (donation.donationType === "Monetary") acc[donation.supporterId].monetary += value;
      else if (donation.donationType === "Time") acc[donation.supporterId].time += value;
      else if (donation.donationType === "Skills") acc[donation.supporterId].skills += value;
      else acc[donation.supporterId].inKind += value;
      return acc;
    },
    {},
  );

  return (
    <div className="space-y-8">
      <div>
        <h1 className="font-display text-2xl lg:text-3xl font-bold text-foreground">Donors & Contributions</h1>
        <p className="font-body text-sm text-muted-foreground mt-1">List supporters, add donors, and view contribution types.</p>
      </div>

      <div className="rounded-2xl bg-card p-4 flex flex-col sm:flex-row gap-3 sm:items-center sm:justify-between">
        <Input
          placeholder="Search donors..."
          value={search}
          onChange={(event) => setSearch(event.target.value)}
          className="max-w-sm"
        />
        <Button onClick={openCreate}>Add Donor</Button>
      </div>

      {loading && <p className="text-sm text-muted-foreground">Loading donors...</p>}
      {error && <p className="text-sm text-destructive">{error}</p>}

      {!loading && !error && (
        <DataTable
          columns={[
            {
              key: "name",
              label: "Name",
              render: (donor) => (
                <div>
                  <p className="font-medium">{donor.displayName}</p>
                  <p className="text-xs text-muted-foreground">{donor.email ?? "No email"}</p>
                </div>
              ),
            },
            { key: "type", label: "Type", render: (donor) => donor.supporterType },
            { key: "status", label: "Status", render: (donor) => donor.status },
            {
              key: "monetary",
              label: "Monetary",
              render: (donor) => `$${Math.round((totalsByDonor[donor.supporterId]?.monetary ?? 0)).toLocaleString()}`,
            },
            {
              key: "time",
              label: "Time",
              render: (donor) => Math.round(totalsByDonor[donor.supporterId]?.time ?? 0).toLocaleString(),
            },
            {
              key: "skills",
              label: "Skills",
              render: (donor) => Math.round(totalsByDonor[donor.supporterId]?.skills ?? 0).toLocaleString(),
            },
            {
              key: "inkind",
              label: "In-kind",
              render: (donor) => Math.round(totalsByDonor[donor.supporterId]?.inKind ?? 0).toLocaleString(),
            },
          ]}
          data={filtered}
          rowKey={(donor) => donor.supporterId}
          onEdit={openEdit}
          onDelete={(donor) => setDeleteTarget(donor)}
        />
      )}

      <FormModal
        open={modalOpen}
        title={editing ? "Edit Donor" : "Add Donor"}
        onClose={() => setModalOpen(false)}
        onSubmit={handleSave}
        submitLabel={editing ? "Update" : "Create"}
      >
        <Input
          placeholder="Name"
          value={form.displayName}
          onChange={(event) => setForm((prev) => ({ ...prev, displayName: event.target.value }))}
        />
        <Input
          placeholder="Type"
          value={form.supporterType}
          onChange={(event) => setForm((prev) => ({ ...prev, supporterType: event.target.value }))}
        />
        <select
          className="h-10 rounded-xl border bg-background px-3 text-sm w-full"
          value={form.status}
          onChange={(event) => setForm((prev) => ({ ...prev, status: event.target.value }))}
        >
          <option value="Active">Active</option>
          <option value="Inactive">Inactive</option>
        </select>
      </FormModal>

      <ConfirmDialog
        open={Boolean(deleteTarget)}
        onCancel={() => setDeleteTarget(null)}
        onConfirm={handleDelete}
        description={deleteTarget ? `Delete donor ${deleteTarget.displayName}?` : "Delete donor?"}
      />
    </div>
  );
};

export default DonorsPage;
