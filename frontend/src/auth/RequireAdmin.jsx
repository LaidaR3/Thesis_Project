import { Navigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import api from "../api/httpClient";
import { useEffect } from "react";

export default function RequireAdmin({ children }) {
  const { user, loading } = useAuth();

  const isAdmin =
    Array.isArray(user?.roles)
      ? user.roles.includes("Admin")
      : user?.roles === "Admin";

  useEffect(() => {
    if (!loading && user && !isAdmin) {
      api.post("/audit", {
        targetEndpoint: "/dashboard",
        action: "Access Denied - Insufficient Permissions"
      }).catch(() => {});
    }
  }, [loading, user, isAdmin]);

  if (loading) return null;

  if (!isAdmin) {
    return <Navigate to="/unauthorized" replace />;
  }

  return children;
}
