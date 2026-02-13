import {
  PieChart,
  Pie,
  Cell,
  ResponsiveContainer,
  Tooltip,
} from "recharts";

const COLORS = ["#031A6B", "#0935d4ff", "#275ebdff", "#005bb6ff", "#128ef3ff", "#a5ceefff"];

export default function LogsByResultChart({ logs = [] }) {
  if (!logs.length) return <p className="empty">No data</p>;

  const grouped = {};
  logs.forEach(log => {
    const key = log.result || "Unknown";
    grouped[key] = (grouped[key] || 0) + 1;
  });

  const data = Object.entries(grouped)
    .map(([name, value]) => ({ name, value }))
    .sort((a, b) => b.value - a.value);

  const total = data.reduce((s, i) => s + i.value, 0);

  return (
    <div className="donut-wrapper">
      <ResponsiveContainer width="100%" height={260}>
        <PieChart>
          <Pie
            data={data}
            dataKey="value"
            innerRadius={80}
            outerRadius={110}
            paddingAngle={6}
            stroke="none"
          >
            {data.map((_, i) => (
              <Cell key={i} fill={COLORS[i % COLORS.length]} />
            ))}
          </Pie>

          {/* CENTER NUMBER */}
          <text
            x="50%"
            y="50%"
            textAnchor="middle"
            dominantBaseline="middle"
            style={{ fontSize: "26px", fontWeight: 600, fill: "#0f172a" }}
          >
            {total}
          </text>

          {/* CENTER LABEL */}
          <text
            x="50%"
            y="58%"
            textAnchor="middle"
            dominantBaseline="middle"
            style={{ fontSize: "12px", fill: "#64748b" }}
          >
            Total Logs
          </text>

          <Tooltip formatter={(v) => [`${v} logs`, "Count"]} />
        </PieChart>
      </ResponsiveContainer>

      {/* LEGEND */}
      <div className="donut-legend">
        {data.map((item, i) => (
          <div key={item.name} className="legend-item">
            <span
              className="dot"
              style={{ background: COLORS[i % COLORS.length] }}
            />
            {item.name}
            <b>{item.value}</b>
            <span className="percent">
              {((item.value / total) * 100).toFixed(1)}%
            </span>
          </div>
        ))}
      </div>
    </div>
  );
}
