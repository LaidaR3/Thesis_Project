import { useEffect, useState } from "react";
import api from "../api/httpClient";
import { useAuth } from "../auth/AuthContext";
import { useNavigate } from "react-router-dom";
import "./Dashbaord.css";

export default function Dashboard() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const [logs, setLogs] = useState([]);
  const [search, setSearch] = useState("");

  useEffect(() => {
    if (!user) return;

    api.get("/audit").then((res) => {
      setLogs(res.data);
    });
  }, [user]);

  const handleLogout = () => {
    logout();               
    navigate("/");          
  };

  const filteredLogs = logs.filter((log) =>
    log.endpoint?.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <div className="dashboard">
   
      <div className="dashboard-header">
        <h1>Audit Logs</h1>

        <div className="user-actions">
          {/* <span className="user-email">{user?.email}</span> */}
          <button className="logout-btn" onClick={handleLogout}>
            Logout
          </button>
        </div>
      </div>

      <div className="dashboard-toolbar">
        <input
          className="search-input"
          placeholder="Search endpoint..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
      </div>

      <div className="table-wrapper">
        <table className="audit-table">
          <thead>
            <tr>
              <th>Endpoint</th>
              <th>Service</th>
              <th>Date</th>
              <th>Status</th>
            </tr>
          </thead>

          <tbody>
            {filteredLogs.map((log) => (
              <tr key={log.id}>
                <td>{log.endpoint}</td>
                <td>{log.serviceName}</td>
                <td>{new Date(log.timestamp).toLocaleString()}</td>
                <td>{log.result}</td>
              </tr>
            ))}

            {filteredLogs.length === 0 && (
              <tr>
                <td colSpan="4" style={{ textAlign: "center", padding: "20px" }}>
                  No logs found
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
