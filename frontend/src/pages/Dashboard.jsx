import { useEffect, useState } from "react";
import api from "../api/httpClient";
import { useAuth } from "../auth/AuthContext";
import { useNavigate } from "react-router-dom";
import "./Dashbaord.css";
import Sidebar from "../components/Sidebar";

export default function Dashboard() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const [logs, setLogs] = useState([]);
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState("");

  useEffect(() => {
    if (!user) return;

    api.get("/audit").then((res) => {
      setLogs(res.data);
    });
  }, [user]);



  const filteredLogs = logs.filter(
    (log) =>
      log.endpoint?.toLowerCase().includes(search.toLowerCase()) &&
      (!statusFilter || log.result === statusFilter)
  );

  
  const renderRoles = () => {
    if (!user?.roles) return "";
    if (Array.isArray(user.roles)) return user.roles.join(", ");
    return user.roles; 
  };

  return (
    <div style={{ display: "flex", minHeight: "100vh" }}>

      <Sidebar />
    <div className="dashboard" style={{ flex: 1 }}>
      
      <div className="dashboard-header">
        <div>
          <h1>Audit Logs</h1>
          <span className="user-email">
            {user?.email} {renderRoles() && `(${renderRoles()})`}
          </span>
        </div>

      
      </div>

      <div className="dashboard-toolbar">
        <input
          className="search-input"
          placeholder="Search endpoint..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />

        <select
          className="status-filter"
          value={statusFilter}
          onChange={(e) => setStatusFilter(e.target.value)}
        >
          <option value="">All statuses</option>
          <option value="LOGIN_SUCCESS">LOGIN_SUCCESS</option>
          <option value="LOGIN_FAILED_INVALID_CREDENTIALS">
            LOGIN_FAILED_INVALID_CREDENTIALS
          </option>
          <option value="LOGIN_FAILED_SYSTEM_ERROR">
            LOGIN_FAILED_SYSTEM_ERROR
          </option>
          <option value="ADMIN_CREATED">ADMIN_CREATED</option>
        </select>
      </div>

      {/* TABLE */}
      <div className="table-wrapper">
        <table className="audit-table">
          <thead>
            <tr>
              <th>Endpoint</th>
              <th>Actor</th>
              <th>Service</th>
              <th>Date</th>
              <th>Status</th>
            </tr>
          </thead>

          <tbody>
            {filteredLogs.map((log) => (
              <tr
                key={log.id}
                className={`row-${log.result?.toLowerCase()}`}
              >
                <td>{log.endpoint}</td>
                <td>{log.email ?? log.userId ?? "Service"}</td>
                <td>{log.serviceName}</td>
                <td>{new Date(log.timestamp).toLocaleString()}</td>
                <td>{log.result}</td>
              </tr>
            ))}

            {filteredLogs.length === 0 && (
              <tr>
                <td colSpan="5" style={{ textAlign: "center", padding: "20px" }}>
                  No logs found
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
    </div>
  );
}
