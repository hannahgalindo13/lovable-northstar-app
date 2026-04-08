import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import { Toaster as Sonner } from "@/components/ui/sonner";
import { Toaster } from "@/components/ui/toaster";
import { TooltipProvider } from "@/components/ui/tooltip";
import { ThemeProvider } from "@/lib/theme";
import { CookieBanner } from "@/components/CookieBanner";
import { ProtectedRoute } from "@/components/ProtectedRoute";
import { AuthProvider } from "@/lib/auth";
import Index from "./pages/Index.tsx";
import NotFound from "./pages/NotFound.tsx";
import ImpactDashboard from "./pages/ImpactDashboard.tsx";
import Login from "./pages/Login.tsx";
import Register from "./pages/Register.tsx";
import AdminDashboard from "./pages/AdminDashboard.tsx";
import DonorsPage from "./pages/DonorsPage.tsx";
import CaseloadPage from "./pages/CaseloadPage.tsx";
import RecordingsPage from "./pages/RecordingsPage.tsx";
import VisitationsPage from "./pages/VisitationsPage.tsx";
import ReportsPage from "./pages/ReportsPage.tsx";
import PrivacyPolicy from "./pages/PrivacyPolicy.tsx";
import { AdminLayout } from "./components/AdminLayout.tsx";
import DonorDashboard from "./pages/DonorDashboard.tsx";
import Donate from "./pages/Donate.tsx";

const queryClient = new QueryClient();

const App = () => (
  <ThemeProvider>
    <QueryClientProvider client={queryClient}>
      <AuthProvider>
        <TooltipProvider>
          <Toaster />
          <Sonner />
          <BrowserRouter>
            <Routes>
              <Route path="/" element={<Index />} />
              <Route path="/impact" element={<ImpactDashboard />} />
              <Route path="/donate" element={<Donate />} />
              <Route
                path="/donors"
                element={
                  <ProtectedRoute role="Donor">
                    <DonorDashboard />
                  </ProtectedRoute>
                }
              />
              <Route path="/donor" element={<Navigate to="/donors" replace />} />
              <Route path="/my-donations" element={<Navigate to="/donors" replace />} />
              <Route path="/login" element={<Login />} />
              <Route path="/register" element={<Register />} />
              <Route path="/privacy" element={<PrivacyPolicy />} />
              <Route path="/dashboard" element={<Navigate to="/admin/dashboard" replace />} />
              <Route
                path="/admin"
                element={
                  <ProtectedRoute role="Admin">
                    <AdminLayout />
                  </ProtectedRoute>
                }
              >
                <Route index element={<Navigate to="/admin/dashboard" replace />} />
                <Route path="dashboard" element={<AdminDashboard />} />
                <Route path="donors" element={<DonorsPage />} />
                <Route path="cases" element={<CaseloadPage />} />
                <Route path="process-records" element={<RecordingsPage />} />
                <Route path="visits" element={<VisitationsPage />} />
                <Route path="reports" element={<ReportsPage />} />
              </Route>
              <Route path="*" element={<NotFound />} />
            </Routes>
          </BrowserRouter>
          <CookieBanner />
        </TooltipProvider>
      </AuthProvider>
    </QueryClientProvider>
  </ThemeProvider>
);

export default App;
