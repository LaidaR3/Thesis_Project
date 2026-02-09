import { createContext, useContext, useEffect, useState } from "react";
import { jwtDecode } from "jwt-decode";
import api from "../api/httpClient";

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [token, setToken] = useState(localStorage.getItem("token"));
  const [loading, setLoading] = useState(true);

useEffect(() => {
  if (!token) {
    setUser(null);
    setLoading(false);
    return;
  }

  try {
    const decoded = jwtDecode(token);

    const rawRoles =
      decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

    const roles = Array.isArray(rawRoles) ? rawRoles : [rawRoles];

    setUser({
      id: decoded[
        "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
      ],
      email: decoded[
        "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
      ],
      roles,
      token,
    });
  } catch {
    localStorage.removeItem("token");
    setUser(null);
  }

  setLoading(false);
}, [token]);


  const login = (newToken) => {
    localStorage.setItem("token", newToken);
    setToken(newToken);
  };

  const logout = async () => {
    try {
      await api.post("/auth/logout");
    } catch (err) {
      console.error("Logout audit failed", err);
    } finally {
      localStorage.removeItem("token");
      setToken(null);
      setUser(null);
    }
  };

  return (
    <AuthContext.Provider value={{ user, login, logout, loading }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  return useContext(AuthContext);
}
