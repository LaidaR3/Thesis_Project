import { Navigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";

export default function RequireAdmin({ children }) {
  const { user, loading } = useAuth();

  if (loading) return null;

  const isAdmin =
    Array.isArray(user?.roles)
      ? user.roles.includes("Admin")
      : user?.roles === "Admin";

  if (!isAdmin) {
    return <Navigate to="/unauthorized" replace />;
  }

  return children;
}
