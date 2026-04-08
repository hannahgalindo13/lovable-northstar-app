import { useState, useEffect } from "react";
import { Input } from "@/components/ui/input";
import { Search } from "lucide-react";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Button } from "@/components/ui/button";
import { FormModal } from "@/components/FormModal";
import { createHomeVisitation, getHomeVisitations, getResidents, type HomeVisitation, type Resident } from "@/services/api";

const VisitationsPage = () => {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [visits, setVisits] = useState<HomeVisitation[]>([]);
  const [residents, setResidents] = useState<Resident[]>([]);
  const [search, setSearch] = useState("");
  const [modalOpen, setModalOpen] = useState(false);
  const [form, setForm] = useState({
    residentId: 0,
    visitDate: new Date().toISOString().slice(0, 10),
    socialWorker: "",
    visitType: "Routine Follow-Up",
    observations: "",
    safetyConcernsNoted: 0,
    followUpNotes: "",
  });

  const load = async () => {
    try {
      setLoading(true);
      const [visitData, residentData] = await Promise.all([getHomeVisitations(), getResidents()]);
      setVisits(visitData);
      setResidents(residentData);
      if (residentData.length > 0 && form.residentId === 0) {
        setForm((prev) => ({ ...prev, residentId: residentData[0].residentId }));
      }
    } catch (err) {
      console.error(err);
      setError("Failed to load home visits.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  const residentCaseMap = Object.fromEntries(residents.map((resident) => [resident.residentId, resident.caseControlNo]));

  const filtered = visits.filter((visit) =>
    `${visit.socialWorker} ${visit.visitType} ${residentCaseMap[visit.residentId] ?? visit.residentId}`
      .toLowerCase()
      .includes(search.toLowerCase()),
  );

  const upcoming = filtered.filter((visit) => new Date(visit.visitDate) >= new Date());
  const past = filtered.filter((visit) => new Date(visit.visitDate) < new Date());

  const handleCreateVisit = async () => {
    if (!form.residentId || !form.socialWorker) return;
    try {
      await createHomeVisitation({
        ...form,
      });
      setModalOpen(false);
      await load();
    } catch (err) {
      console.error(err);
      setError("Failed to create home visit.");
    }
  };

  return (
    <div className="space-y-8">
      <div>
        <h1 className="font-display text-2xl lg:text-3xl font-bold text-foreground">Home Visits & Conferences</h1>
        <p className="font-body text-sm text-muted-foreground mt-1">Visit logs with upcoming and past conference-style schedule views.</p>
      </div>

      <Button onClick={() => setModalOpen(true)} className="w-fit">Add Visit</Button>

      <div className="relative max-w-xs">
        <Search className="absolute left-3.5 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
        <Input placeholder="Search visits..." value={search} onChange={(event) => setSearch(event.target.value)} className="pl-10 h-10 rounded-xl" />
      </div>

      {loading && <p className="text-sm text-muted-foreground">Loading visit logs...</p>}
      {error && <p className="text-sm text-destructive">{error}</p>}

      {!loading && !error && (
        <div className="grid grid-cols-1 xl:grid-cols-2 gap-6">
          <div className="rounded-2xl bg-card overflow-hidden shadow-sm">
            <div className="p-4 border-b"><p className="font-medium">Visit Logs</p></div>
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Resident</TableHead>
                  <TableHead>Date</TableHead>
                  <TableHead>Worker</TableHead>
                  <TableHead>Safety</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {filtered.map((visit) => (
                  <TableRow key={visit.visitationId}>
                    <TableCell>{residentCaseMap[visit.residentId] ?? visit.residentId}</TableCell>
                    <TableCell>{visit.visitDate}</TableCell>
                    <TableCell>{visit.socialWorker}</TableCell>
                    <TableCell>{visit.safetyConcernsNoted ? "Concern noted" : "No concern"}</TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>

          <div className="rounded-2xl bg-card p-4 shadow-sm">
            <p className="font-medium mb-3">Conference Schedule</p>
            <div className="space-y-4">
              <div>
                <p className="text-xs uppercase tracking-wide text-muted-foreground mb-2">Upcoming</p>
                {upcoming.slice(0, 6).map((visit) => (
                  <p key={visit.visitationId} className="text-sm py-1">
                    {visit.visitDate} · {residentCaseMap[visit.residentId] ?? visit.residentId} · {visit.visitType}
                  </p>
                ))}
              </div>
              <div>
                <p className="text-xs uppercase tracking-wide text-muted-foreground mb-2">Past</p>
                {past.slice(0, 6).map((visit) => (
                  <p key={visit.visitationId} className="text-sm py-1">
                    {visit.visitDate} · {residentCaseMap[visit.residentId] ?? visit.residentId} · {visit.followUpNotes ?? "No follow-up notes"}
                  </p>
                ))}
              </div>
            </div>
          </div>
        </div>
      )}

      <FormModal
        open={modalOpen}
        title="Add Home Visit"
        onClose={() => setModalOpen(false)}
        onSubmit={handleCreateVisit}
        submitLabel="Create"
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
        <Input type="date" value={form.visitDate} onChange={(event) => setForm((prev) => ({ ...prev, visitDate: event.target.value }))} />
        <Input
          placeholder="Social worker"
          value={form.socialWorker}
          onChange={(event) => setForm((prev) => ({ ...prev, socialWorker: event.target.value }))}
        />
        <Input
          placeholder="Visit type"
          value={form.visitType}
          onChange={(event) => setForm((prev) => ({ ...prev, visitType: event.target.value }))}
        />
        <Input
          placeholder="Observations"
          value={form.observations}
          onChange={(event) => setForm((prev) => ({ ...prev, observations: event.target.value }))}
        />
        <Input
          placeholder="Follow-up notes"
          value={form.followUpNotes}
          onChange={(event) => setForm((prev) => ({ ...prev, followUpNotes: event.target.value }))}
        />
        <select
          className="h-10 rounded-xl border bg-background px-3 text-sm w-full"
          value={form.safetyConcernsNoted}
          onChange={(event) => setForm((prev) => ({ ...prev, safetyConcernsNoted: Number(event.target.value) }))}
        >
          <option value={0}>No safety concern</option>
          <option value={1}>Safety concern noted</option>
        </select>
      </FormModal>
    </div>
  );
};

export default VisitationsPage;
