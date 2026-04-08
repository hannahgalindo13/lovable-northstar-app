import { useState, useEffect } from "react";
import { Users, Home, Heart, Activity } from "lucide-react";
import { AreaChart, Area, XAxis, YAxis, Tooltip, ResponsiveContainer } from "recharts";
import { getDashboardStats, getImpactData, getDonations, type DashboardStats, type ImpactData, type Donation } from "@/services/api";

const AdminDashboard = () => {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [stats, setStats] = useState<DashboardStats | null>(null);
  const [impactData, setImpactData] = useState<ImpactData | null>(null);
  const [recentDonations, setRecentDonations] = useState<Donation[]>([]);

  useEffect(() => {
    const load = async () => {
      try {
        setLoading(true);
        const [dashboardData, impact, donations] = await Promise.all([
          getDashboardStats(),
          getImpactData(),
          getDonations(),
        ]);

        setStats(dashboardData);
        setImpactData(impact);
        setRecentDonations(
          [...donations]
            .sort((a, b) => new Date(b.donationDate).getTime() - new Date(a.donationDate).getTime())
            .slice(0, 6),
        );
      } catch (err) {
        console.error(err);
        setError("Failed to load dashboard data.");
      } finally {
        setLoading(false);
      }
    };

    load();
  }, []);

  return (
    <div className="space-y-8">
      <div>
        <h1 className="font-display text-2xl lg:text-3xl font-bold text-foreground">Admin Dashboard</h1>
        <p className="font-body text-sm text-muted-foreground mt-1">Real-time operational overview from backend data.</p>
      </div>

      {loading && <div className="text-sm text-muted-foreground">Loading dashboard data...</div>}
      {error && <div className="text-sm text-destructive">{error}</div>}

      {!loading && !error && stats && impactData && (
        <>
          <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-4 gap-4">
            {[
              { label: "Total Residents", value: stats.totalResidents.toLocaleString(), icon: Users },
              { label: "Active Safehouses", value: stats.safehouseCount.toLocaleString(), icon: Home },
              { label: "Total Donations", value: `$${Math.round(stats.totalDonations).toLocaleString()}`, icon: Heart },
              { label: "Active Residents", value: stats.activeResidents.toLocaleString(), icon: Activity },
            ].map((item) => (
              <div key={item.label} className="rounded-2xl bg-card p-5 shadow-sm">
                <div className="flex items-center gap-2 text-muted-foreground mb-2">
                  <item.icon className="w-4 h-4" />
                  <span className="text-xs uppercase tracking-wide">{item.label}</span>
                </div>
                <p className="font-display text-2xl font-bold text-foreground">{item.value}</p>
              </div>
            ))}
          </div>

          <div className="grid grid-cols-1 xl:grid-cols-2 gap-6">
            <div className="rounded-2xl bg-card p-5 shadow-sm">
              <p className="font-body text-sm font-medium mb-4">Donation Trends</p>
              <ResponsiveContainer width="100%" height={240}>
                <AreaChart data={impactData.monthlyDonations}>
                  <XAxis dataKey="month" axisLine={false} tickLine={false} />
                  <YAxis axisLine={false} tickLine={false} />
                  <Tooltip formatter={(v: number) => `$${v.toLocaleString()}`} />
                  <Area type="monotone" dataKey="amount" stroke="hsl(10,55%,65%)" fill="hsl(10 55% 65% / 0.2)" />
                </AreaChart>
              </ResponsiveContainer>
            </div>

            <div className="rounded-2xl bg-card p-5 shadow-sm">
              <p className="font-body text-sm font-medium mb-4">Recent Activity</p>
              <div className="space-y-3">
                {recentDonations.map((donation) => (
                  <div key={donation.donationId} className="flex items-center justify-between text-sm border-b border-border/40 pb-2">
                    <span className="text-foreground">{donation.donationType}</span>
                    <span className="text-muted-foreground">{donation.donationDate}</span>
                    <span className="font-medium text-foreground">${Math.round(donation.amount ?? donation.estimatedValue ?? 0).toLocaleString()}</span>
                  </div>
                ))}
              </div>
            </div>
          </div>
        </>
      )}
    </div>
  );
};

export default AdminDashboard;
