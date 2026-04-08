import { useState, useEffect } from "react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Search } from "lucide-react";
import { DataTable } from "@/components/DataTable";
import { FormModal } from "@/components/FormModal";
import { ConfirmDialog } from "@/components/ConfirmDialog";
import {
  createProcessRecording,
  deleteProcessRecording,
  getProcessRecordings,
  getResidents,
  updateProcessRecording,
  type ProcessRecording,
  type Resident,
} from "@/services/api";

const RecordingsPage = () => {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [recordings, setRecordings] = useState<ProcessRecording[]>([]);
  const [residents, setResidents] = useState<Resident[]>([]);
  const [search, setSearch] = useState("");
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<ProcessRecording | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<ProcessRecording | null>(null);
  const [form, setForm] = useState({
    residentId: 0,
    sessionDate: new Date().toISOString().slice(0, 10),
    socialWorker: "",
    sessionType: "Individual",
    emotionalStateObserved: "Calm",
    sessionNarrative: "",
    interventionsApplied: "",
    followUpActions: "",
  });

  useEffect(() => {
    const load = async () => {
      try {
        setLoading(true);
        const [recordingData, residentData] = await Promise.all([getProcessRecordings(), getResidents()]);
        setRecordings(recordingData);
        setResidents(residentData);
        if (residentData.length > 0 && form.residentId === 0) {
          setForm((prev) => ({ ...prev, residentId: residentData[0].residentId }));
        }
      } catch (err) {
        console.error(err);
        setError("Failed to load process recordings.");
      } finally {
        setLoading(false);
      }
    };
    load();
  }, []);

  const openCreate = () => {
    setEditing(null);
    setForm((prev) => ({
      ...prev,
      residentId: residents[0]?.residentId ?? prev.residentId,
      sessionDate: new Date().toISOString().slice(0, 10),
      socialWorker: "",
      sessionNarrative: "",
      interventionsApplied: "",
      followUpActions: "",
    }));
    setModalOpen(true);
  };

  const openEdit = (recording: ProcessRecording) => {
    setEditing(recording);
    setForm({
      residentId: recording.residentId,
      sessionDate: recording.sessionDate,
      socialWorker: recording.socialWorker,
      sessionType: recording.sessionType,
      emotionalStateObserved: recording.emotionalStateObserved,
      sessionNarrative: recording.sessionNarrative ?? "",
      interventionsApplied: recording.interventionsApplied ?? "",
      followUpActions: recording.followUpActions ?? "",
    });
    setModalOpen(true);
  };

  const handleSaveRecord = async () => {
    if (!form.residentId || !form.socialWorker) return;
    try {
      if (editing) {
        await updateProcessRecording(editing.recordingId, {
          ...editing,
          ...form,
        });
      } else {
        await createProcessRecording({
          ...form,
          sessionNarrative: form.sessionNarrative || "Session summary not provided.",
          interventionsApplied: form.interventionsApplied || "Supportive counseling",
          followUpActions: form.followUpActions || "Schedule next session",
        });
      }
      const refreshed = await getProcessRecordings();
      setRecordings(refreshed);
      setModalOpen(false);
    } catch (err) {
      console.error(err);
      setError("Failed to create process record.");
    }
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    try {
      await deleteProcessRecording(deleteTarget.recordingId);
      setDeleteTarget(null);
      setRecordings(await getProcessRecordings());
    } catch (err) {
      console.error(err);
      setError("Failed to delete process record.");
    }
  };

  const filtered = recordings
    .filter(
      (recording) =>
        recording.socialWorker.toLowerCase().includes(search.toLowerCase()) ||
        String(recording.residentId).includes(search),
    )
    .sort((a, b) => new Date(b.sessionDate).getTime() - new Date(a.sessionDate).getTime());

  const residentCaseMap = Object.fromEntries(
    residents.map((resident) => [resident.residentId, resident.caseControlNo]),
  );

  return (
    <div className="space-y-8">
      <div>
        <h1 className="font-display text-2xl lg:text-3xl font-bold text-foreground">Process Recording</h1>
        <p className="font-body text-sm text-muted-foreground mt-1">Chronological session logs with quick entry form.</p>
      </div>
      <Button onClick={openCreate} className="w-fit">Add Record</Button>

      <div className="relative max-w-xs">
        <Search className="absolute left-3.5 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
        <Input placeholder="Search worker or resident ID" value={search} onChange={(event) => setSearch(event.target.value)} className="pl-10 h-10 rounded-xl" />
      </div>

      {loading && <p className="text-sm text-muted-foreground">Loading records...</p>}
      {error && <p className="text-sm text-destructive">{error}</p>}

      {!loading && !error && (
        <DataTable
          columns={[
            { key: "resident", label: "Resident", render: (recording) => residentCaseMap[recording.residentId] ?? recording.residentId },
            { key: "date", label: "Date", render: (recording) => recording.sessionDate },
            { key: "worker", label: "Social Worker", render: (recording) => recording.socialWorker },
            { key: "type", label: "Session Type", render: (recording) => recording.sessionType },
            { key: "emotion", label: "Emotion", render: (recording) => recording.emotionalStateObserved },
            { key: "notes", label: "Notes", render: (recording) => recording.sessionNarrative ?? "No narrative" },
          ]}
          data={filtered}
          rowKey={(recording) => recording.recordingId}
          onEdit={openEdit}
          onDelete={(recording) => setDeleteTarget(recording)}
        />
      )}

      <FormModal
        open={modalOpen}
        title={editing ? "Edit Process Record" : "Add Process Record"}
        onClose={() => setModalOpen(false)}
        onSubmit={handleSaveRecord}
        submitLabel={editing ? "Update" : "Create"}
      >
        <select
          className="h-10 rounded-xl border bg-background px-3 text-sm w-full"
          value={form.residentId}
          onChange={(event) => setForm((prev) => ({ ...prev, residentId: Number(event.target.value) }))}
        >
          {residents.map((resident) => (
            <option key={resident.residentId} value={resident.residentId}>
              {resident.caseControlNo}
            </option>
          ))}
        </select>
        <Input
          type="date"
          value={form.sessionDate}
          onChange={(event) => setForm((prev) => ({ ...prev, sessionDate: event.target.value }))}
        />
        <Input
          placeholder="Social worker"
          value={form.socialWorker}
          onChange={(event) => setForm((prev) => ({ ...prev, socialWorker: event.target.value }))}
        />
        <Input
          placeholder="Session type"
          value={form.sessionType}
          onChange={(event) => setForm((prev) => ({ ...prev, sessionType: event.target.value }))}
        />
        <Input
          placeholder="Emotional state"
          value={form.emotionalStateObserved}
          onChange={(event) => setForm((prev) => ({ ...prev, emotionalStateObserved: event.target.value }))}
        />
        <Input
          placeholder="Notes"
          value={form.sessionNarrative}
          onChange={(event) => setForm((prev) => ({ ...prev, sessionNarrative: event.target.value }))}
        />
        <Input
          placeholder="Interventions"
          value={form.interventionsApplied}
          onChange={(event) => setForm((prev) => ({ ...prev, interventionsApplied: event.target.value }))}
        />
        <Input
          placeholder="Follow-up"
          value={form.followUpActions}
          onChange={(event) => setForm((prev) => ({ ...prev, followUpActions: event.target.value }))}
        />
      </FormModal>

      <ConfirmDialog
        open={Boolean(deleteTarget)}
        onCancel={() => setDeleteTarget(null)}
        onConfirm={handleDelete}
        description={deleteTarget ? `Delete record #${deleteTarget.recordingId}?` : "Delete record?"}
      />
    </div>
  );
};

export default RecordingsPage;
