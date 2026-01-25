import {
  ResponsiveContainer,
  BarChart,
  Bar,
  XAxis,
  YAxis,
  Tooltip,
  CartesianGrid,
} from "recharts";

export default function LogsOverTimeChart({ logs = [] }) {
  const grouped = {};

  logs.forEach(log => {
    if (!log.timestamp) return;
    const date = new Date(log.timestamp).toISOString().slice(0, 10);
    grouped[date] = (grouped[date] || 0) + 1;
  });

  const data = Object.entries(grouped)
    .sort(([a], [b]) => new Date(a) - new Date(b))
    .map(([date, value]) => ({ date, value }));

  return (
    <div style={{ width: "100%", height: 280 }}>
      <ResponsiveContainer>
        <BarChart data={data}>
          <CartesianGrid strokeDasharray="3 3" vertical={false} />

          <XAxis
            dataKey="date"
            tickFormatter={(d) =>
              new Date(d).toLocaleDateString("en-US", {
                month: "short",
                day: "numeric",
              })
            }
          />

          <YAxis allowDecimals={false} />

          <Tooltip />

          <Bar
            dataKey="value"
            fill="#0e2491"
            radius={[8, 8, 0, 0]}
            animationDuration={600}
          />
        </BarChart>
      </ResponsiveContainer>
    </div>
  );
}
