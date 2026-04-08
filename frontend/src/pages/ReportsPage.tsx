import { useState, useEffect } from "react";
import { BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer, LineChart, Line, CartesianGrid } from "recharts";
import { getDashboardStats, getImpactData, getSafehouses, type DashboardStats, type ImpactData, type Safehouse } from "@/services/api";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";

const ReportsPage = () => {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [stats, setStats] = useState<DashboardStats | null>(null);
  const [impact, setImpact] = useState<ImpactData | null>(null);
  const [safehouses, setSafehouses] = useState<Safehouse[]>([]);

  useEffect(() => {
    const load = async () => {
      try {
        setLoading(true);
        const [dashboardData, impactData, safehouseData] = await Promise.all([
          getDashboardStats(),
          getImpactData(),
          getSafehouses(),
        ]);
        setStats(dashboardData);
        setImpact(impactData);
        setSafehouses(safehouseData);
      } catch (err) {
        console.error(err);
        setError("Failed to load report analytics.");
      } finally {
        setLoading(false);
      }
    };
    load();
  }, []);

  const safehouseComparison = safehouses.map((safehouse) => ({
    name: safehouse.name,
    residents: Math.floor((stats?.totalResidents ?? 0) / Math.max(safehouses.length, 1)),
  }));

  const reintegrationSuccessRate = impact?.programOutcomes.length
    ? Math.round(impact.programOutcomes.reduce((sum, item) => sum + item.rate, 0) / impact.programOutcomes.length)
    : 0;

  return (
    <div className="space-y-8">
      <div>
        <h1 className="font-display text-2xl lg:text-3xl font-bold text-foreground">Reports & Analytics</h1>
        <p className="font-body text-sm text-muted-foreground mt-1">Annual accomplishment style view of services, counts, and outcomes.</p>
      </div>

      {loading && <p className="text-sm text-muted-foreground">Loading report data...</p>}
      {error && <p className="text-sm text-destructive">{error}</p>}

      {!loading && !error && stats && impact && (
        <>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div className="rounded-2xl bg-card p-5 shadow-sm"><p className="text-xs text-muted-foreground">Residents Served</p><p className="font-display text-2xl font-bold">{stats.totalResidents}</p></div>
            <div className="rounded-2xl bg-card p-5 shadow-sm"><p className="text-xs text-muted-foreground">Total Donations</p><p className="font-display text-2xl font-bold">${Math.round(stats.totalDonations).toLocaleString()}</p></div>
            <div className="rounded-2xl bg-card p-5 shadow-sm"><p className="text-xs text-muted-foreground">Reintegration Success</p><p className="font-display text-2xl font-bold">{reintegrationSuccessRate}%</p></div>
          </div>

          <div className="grid grid-cols-1 xl:grid-cols-2 gap-6">
            <div className="rounded-2xl bg-card p-5 shadow-sm">
              <p className="font-medium mb-3">Donation Trends</p>
              <ResponsiveContainer width="100%" height={250}>
                <LineChart data={impact.monthlyDonations}>
                  <CartesianGrid strokeDasharray="3 3" stroke="hsl(var(--border))" />
                  <XAxis dataKey="month" />
                  <YAxis />
                  <Tooltip formatter={(value: number) => `$${value.toLocaleString()}`} />
                  <Line type="monotone" dataKey="amount" stroke="hsl(10,55%,65%)" strokeWidth={2} />
                </LineChart>
              </ResponsiveContainer>
            </div>

            <div className="rounded-2xl bg-card p-5 shadow-sm">
              <p className="font-medium mb-3">Resident Outcomes</p>
              <ResponsiveContainer width="100%" height={250}>
                <BarChart data={impact.programOutcomes}>
                  <XAxis dataKey="program" />
                  <YAxis />
                  <Tooltip />
                  <Bar dataKey="rate" fill="hsl(150,18%,56%)" />
                </BarChart>
              </ResponsiveContainer>
            </div>
          </div>

          <div className="rounded-2xl bg-card p-5 shadow-sm">
            <p className="font-medium mb-3">Safehouse Comparisons</p>
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Safehouse</TableHead>
                  <TableHead>Estimated Residents</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {safehouseComparison.map((entry) => (
                  <TableRow key={entry.name}>
                    <TableCell>{entry.name}</TableCell>
                    <TableCell>{entry.residents}</TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        </>
      )}
    </div>
  );
};

export default ReportsPage;
