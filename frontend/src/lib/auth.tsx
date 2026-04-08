import { createContext, useContext, useEffect, useMemo, useState } from "react";
import { getCurrentUser, login as apiLogin, logout as apiLogout, type AuthMeResponse } from "@/services/api";

type AuthContextValue = {
  user: AuthMeResponse | null;
  loading: boolean;
  refresh: () => Promise<void>;
  login: (email: string, password: string) => Promise<AuthMeResponse>;
  logout: () => Promise<void>;
  hasRole: (role: string) => boolean;
};

const AuthContext = createContext<AuthContextValue | null>(null);

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
  const [user, setUser] = useState<AuthMeResponse | null>(null);
  const [loading, setLoading] = useState(true);

  const refresh = async () => {
    const me = await getCurrentUser();
    setUser(me.isAuthenticated ? me : null);
  };

  useEffect(() => {
    refresh().finally(() => setLoading(false));
  }, []);

  const value = useMemo<AuthContextValue>(
    () => ({
      user,
      loading,
      refresh,
      login: async (email: string, password: string) => {
        await apiLogin(email, password, true);
        await refresh();
        return (await getCurrentUser()) as AuthMeResponse;
      },
      logout: async () => {
        await apiLogout();
        setUser(null);
      },
      hasRole: (role: string) => !!user?.roles?.includes(role),
    }),
    [loading, user],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within AuthProvider");
  }
  return context;
};
