import { Navigate } from "react-router-dom";
import { useAuth } from "@/lib/auth";

export const ProtectedRoute = ({
  role,
  children,
}: {
  role: "Admin" | "Donor";
  children: React.ReactNode;
}) => {
  const { loading, user, hasRole } = useAuth();

  if (loading) {
    return <div className="p-8 text-sm text-muted-foreground">Checking session...</div>;
  }

  if (!user?.isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (!hasRole(role)) {
    return <Navigate to="/" replace />;
  }

  return <>{children}</>;
};
