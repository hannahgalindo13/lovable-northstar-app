import { useEffect, useMemo, useState } from "react";
import { PublicLayout } from "@/components/PublicLayout";
import { Link, useNavigate } from "react-router-dom";
import { ApiError, getMyDonations, type DonorDonationHistoryItem } from "@/services/api";
import { Button } from "@/components/ui/button";

const DonorDashboard = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [donations, setDonations] = useState<DonorDonationHistoryItem[]>([]);

  const load = async () => {
    try {
      setLoading(true);
      setError(null);
      const donationData = await getMyDonations();
      setDonations(donationData);
    } catch (err) {
      if (err instanceof ApiError && (err.status === 401 || err.status === 403)) {
        navigate("/login", { replace: true });
        return;
      }
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
    () => [...donations].sort((a, b) => new Date(b.donationDate).getTime() - new Date(a.donationDate).getTime()),
    [donations],
  );
  const totalDonated = useMemo(
    () =>
      history.reduce((sum, donation) => {
        const value = Number(donation.amount ?? donation.estimatedValue ?? 0);
        return sum + (Number.isFinite(value) ? value : 0);
      }, 0),
    [history],
  );

  return (
    <PublicLayout>
      <div className="max-w-5xl mx-auto px-6 py-24 space-y-8">
        <h1 className="font-display text-3xl font-bold">Donor Dashboard</h1>

        {loading && <p className="text-sm text-muted-foreground">Loading...</p>}
        {error && <p className="text-sm text-destructive">{error}</p>}

        {!loading && (
          <>
            <div className="grid gap-4 md:grid-cols-2">
              <div className="rounded-2xl bg-card p-6 shadow-sm border border-border/50">
                <p className="text-sm text-muted-foreground">Total donated</p>
                <p className="font-display text-3xl font-bold mt-2">${totalDonated.toLocaleString(undefined, { maximumFractionDigits: 2 })}</p>
              </div>
              <div className="rounded-2xl bg-card p-6 shadow-sm border border-border/50 flex items-center justify-between">
                <div>
                  <p className="text-sm text-muted-foreground">Ready to help again?</p>
                  <p className="font-medium mt-1">Make another contribution today.</p>
                </div>
                <Button asChild>
                  <Link to="/donate">Donate Again</Link>
                </Button>
              </div>
            </div>

            <div className="rounded-2xl bg-card p-6 shadow-sm border border-border/50">
              <h2 className="font-display text-xl font-semibold mb-4">Your Contribution History</h2>
              <div className="space-y-2">
                {history.map((donation) => (
                  <div key={donation.donationId} className="flex flex-wrap gap-3 justify-between border-b border-border/40 pb-2 text-sm">
                    <span>{new Date(donation.donationDate).toLocaleDateString()}</span>
                    <span className="text-muted-foreground">{donation.donationType}</span>
                    <span>${Number(donation.amount ?? donation.estimatedValue ?? 0).toLocaleString(undefined, { maximumFractionDigits: 2 })}</span>
                    <span className="text-muted-foreground">{donation.impactUnit ?? "No impact unit"}</span>
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
