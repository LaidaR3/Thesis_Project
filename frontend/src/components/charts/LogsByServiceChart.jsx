import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  Tooltip,
  ResponsiveContainer,
} from "recharts";

export default function LogsByServiceChart({ logs }) {
  const data = Object.values(
    logs.reduce((acc, log) => {
      const key = log.serviceName || "Unknown";
      acc[key] = acc[key] || { name: key, count: 0 };
      acc[key].count += 1;
      return acc;
    }, {})
  );

  return (
    <ResponsiveContainer width="100%" height={250}>
      <BarChart data={data}>
        <XAxis
          dataKey="name"
          tick={{ fontSize: 12 }}
          interval={0}
        />

        <YAxis allowDecimals={false} />
        <Tooltip />
        <Bar
          dataKey="count"
          fill="#60a5fa"
          radius={[6, 6, 0, 0]}
        />

      </BarChart>
    </ResponsiveContainer>
  );
}
