export default function MetricCard({ title, value, color }) {
  return (
    <div className="metric-card">
      <span className="metric-title">{title}</span>
      <div className="metric-value" style={{ color }}>
        {value.toLocaleString()}
      </div>
    </div>
  );
}
