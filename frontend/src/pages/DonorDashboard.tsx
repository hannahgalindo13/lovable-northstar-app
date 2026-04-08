import { useEffect, useMemo, useState } from "react";
import { PublicLayout } from "@/components/PublicLayout";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { createDonation, getDonations, getSupporters, type Donation, type Supporter } from "@/services/api";

const DonorDashboard = () => {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [supporters, setSupporters] = useState<Supporter[]>([]);
  const [donations, setDonations] = useState<Donation[]>([]);
  const [supporterId, setSupporterId] = useState<number>(0);
  const [amount, setAmount] = useState("1000");
  const [note, setNote] = useState("");
  const [type, setType] = useState("Monetary");

  const load = async () => {
    try {
      setLoading(true);
      const [supporterData, donationData] = await Promise.all([getSupporters(), getDonations()]);
      setSupporters(supporterData);
      setDonations(donationData);
      if (supporterData.length > 0 && supporterId === 0) {
        setSupporterId(supporterData[0].supporterId);
      }
    } catch (err) {
      console.error(err);
      setError("Failed to load donor dashboard data.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  const history = useMemo(
    () =>
      donations
        .filter((donation) => donation.supporterId === supporterId)
        .sort((a, b) => new Date(b.donationDate).getTime() - new Date(a.donationDate).getTime()),
    [donations, supporterId],
  );

  const handleSubmit = async () => {
    if (!supporterId) return;
    try {
      setError(null);
      await createDonation({
        supporterId,
        donationType: type,
        donationDate: new Date().toISOString().slice(0, 10),
        channelSource: "Direct",
        amount: Number(amount),
        estimatedValue: Number(amount),
        impactUnit: type === "Monetary" ? "pesos" : "hours",
        isRecurring: 0,
        notes: note,
      });
      setNote("");
      await load();
    } catch (err) {
      console.error(err);
      setError("Failed to submit donation.");
    }
  };

  return (
    <PublicLayout>
      <div className="max-w-5xl mx-auto px-6 py-24 space-y-8">
        <h1 className="font-display text-3xl font-bold">Donor Dashboard</h1>

        {loading && <p className="text-sm text-muted-foreground">Loading...</p>}
        {error && <p className="text-sm text-destructive">{error}</p>}

        {!loading && (
          <>
            <div className="rounded-2xl bg-card p-6 shadow-sm space-y-3">
              <h2 className="font-display text-xl font-semibold">Make a Donation</h2>
              <div className="grid grid-cols-1 md:grid-cols-4 gap-3">
                <select className="h-10 rounded-xl border bg-background px-3 text-sm" value={supporterId} onChange={(event) => setSupporterId(Number(event.target.value))}>
                  {supporters.map((supporter) => (
                    <option key={supporter.supporterId} value={supporter.supporterId}>
                      {supporter.displayName}
                    </option>
                  ))}
                </select>
                <select className="h-10 rounded-xl border bg-background px-3 text-sm" value={type} onChange={(event) => setType(event.target.value)}>
                  <option value="Monetary">Monetary</option>
                  <option value="Time">Time</option>
                  <option value="Skills">Skills</option>
                  <option value="InKind">In-kind</option>
                </select>
                <Input value={amount} onChange={(event) => setAmount(event.target.value)} placeholder="Amount/value" />
                <Input value={note} onChange={(event) => setNote(event.target.value)} placeholder="Note" />
              </div>
              <Button onClick={handleSubmit}>Submit Donation</Button>
            </div>

            <div className="rounded-2xl bg-card p-6 shadow-sm">
              <h2 className="font-display text-xl font-semibold mb-4">Your Contribution History</h2>
              <div className="space-y-2">
                {history.map((donation) => (
                  <div key={donation.donationId} className="flex flex-wrap gap-3 justify-between border-b border-border/40 pb-2 text-sm">
                    <span>{donation.donationDate}</span>
                    <span>{donation.donationType}</span>
                    <span>${Math.round(Number(donation.amount ?? donation.estimatedValue ?? 0)).toLocaleString()}</span>
                    <span className="text-muted-foreground">{donation.notes ?? "No note"}</span>
                  </div>
                ))}
                {history.length === 0 && <p className="text-sm text-muted-foreground">No donations yet.</p>}
              </div>
            </div>
          </>
        )}
      </div>
    </PublicLayout>
  );
};

export default DonorDashboard;
