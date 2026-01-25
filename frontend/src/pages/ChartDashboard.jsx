import Sidebar from "../components/Sidebar";
import MetricCard from "../components/MetricCard";
import DashboardCard from "../components/DashboardCard";

import LogsByResultChart from "../components/charts/LogsByResultChart";
import LogsOverTimeChart from "../components/charts/LogsOverTimeChart";

import { useEffect, useState } from "react";
import api from "../api/httpClient";

import "./Chart.css";

/* ✅ NORMALIZATION — TOP LEVEL */
const normalizeResult = (result = "") => {
  const r = result.toLowerCase().trim();

  if (r.includes("login successfully")) return "Login Success";
  if (r.includes("login failed")) return "Login Failed";
  if (r.includes("admin created")) return "Admin Created";
  if (r.includes("register")) return "User Registered";

  return "Other";
};

export default function ChartDashboard() {
  const [logs, setLogs] = useState([]);

  useEffect(() => {
    const fetchLogs = () => {
      api.get("/audit").then(res => {
        setLogs(res.data || []);
      });
    };

    fetchLogs();
    const interval = setInterval(fetchLogs, 5000);

    return () => clearInterval(interval);
  }, []);

  /* ✅ KPIs — USE NORMALIZED VALUES */
  const total = logs.length;

  const success = logs.filter(
    l => normalizeResult(l.result) === "Login Success"
  ).length;

  const failed = logs.filter(
    l => normalizeResult(l.result) === "Login Failed"
  ).length;

  const admin = logs.filter(
    l => normalizeResult(l.result) === "Admin Created"
  ).length;

  const services = new Set(logs.map(l => l.serviceName)).size;

  return (
    <div style={{ display: "flex" }}>
      <Sidebar />

      <div className="analytics-page">
        {/* HEADER */}
        <div className="analytics-header">
          <h1>Analytics</h1>
          <select>
            <option>Last 7 days</option>
            <option>Last 30 days</option>
          </select>
        </div>

        {/* METRICS */}
        <div className="metrics-grid">
          <MetricCard title="Total Logs" value={total} />
          <MetricCard title="Login Success" value={success} color="#16a34a" />
          <MetricCard title="Login Failed" value={failed} color="#dc2626" />
          <MetricCard title="Admin Actions" value={admin} />
          <MetricCard title="Services" value={services} />
        </div>

        {/* CHARTS */}
        <div className="charts-grid">
          <DashboardCard title="Logs by Result">
            <LogsByResultChart logs={logs} />
          </DashboardCard>

          <DashboardCard title="Logs Over Time">
            <LogsOverTimeChart logs={logs} />
          </DashboardCard>
        </div>
      </div>
    </div>
  );
}
