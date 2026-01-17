import { Routes, Route } from "react-router-dom";
import Login from "./pages/Login";
import Register from "./pages/Register";
import Dashboard from "./pages/Dashboard";
import RequireAuth from "./auth/RequireAuth";
import RequireAdmin from "./auth/RequireAdmin";
import Unauthorized from "./pages/Unauthorized";
import Profile from "./pages/Profile";
import CreateAdmin from "./pages/CreateAdmin";
import Users from "./pages/Users";

function App() {
  return (
    <Routes>
      <Route path="/" element={<Login />} />
      <Route path="/register" element={<Register />} />
      <Route
        path="/dashboard"
        element={
          <RequireAuth>
            <RequireAdmin>
              <Dashboard />
            </RequireAdmin>
          </RequireAuth>
        }
      />
      <Route path="/unauthorized" element={<Unauthorized />} />
      <Route
        path="/profile"
        element={
          <RequireAuth>
            <Profile />
          </RequireAuth>
        }
      />
      <Route
        path="/create-admin"
        element={
          <RequireAuth>
            <CreateAdmin />
          </RequireAuth>
        }
      />
      <Route
        path="/users"
        element={
          <RequireAuth>
            <Users />
          </RequireAuth>
        }
      />


    </Routes>

  );
}

export default App;
