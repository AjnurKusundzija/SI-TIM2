import PropTypes from "prop-types";
import { useState, useEffect, useCallback, useMemo } from "react";
import { useNavigate } from "react-router-dom";
import { getAdminDashboard, generateReport } from "../../services/adminService";
import {
  Ticket,
  CheckCircle,
  AlertCircle,
  MessageSquare,
  Clock,
  Star,
  BarChart2,
  Loader2,
  Download,
} from "lucide-react";
import {
  ResponsiveContainer,
  PieChart,
  Pie,
  Cell,
  Tooltip,
  Legend,
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
} from "recharts";

const STATUS_COLORS = {
  OPEN: "#3b82f6",
  CLOSED: "#10b981",
  CLOSURE_REQUESTED: "#f59e0b",
};

const STATUS_LABELS = {
  OPEN: "Otvoreni",
  CLOSED: "Zatvoreni",
  CLOSURE_REQUESTED: "Čeka se",
};

const PROBLEM_LABELS = {
  INTERNET: "Internet",
  TV: "TV",
  MOBILE_NETWORK: "Mobilna mreža",
  BILLING: "Računi",
  TECHNICAL_SUPPORT: "Tehnička podrška",
};

const REPORT_TYPES = [
  { value: "TICKET_COUNT", label: "Broj tiketa" },
  { value: "TICKET_STATUS", label: "Status tiketa" },
  { value: "PROBLEM_TYPE", label: "Tip problema" },
  { value: "TEAM_WORKLOAD", label: "Opterećenje agenata/tehničara" },
  { value: "USER_RATINGS", label: "Ocjene korisnika" },
  { value: "FIRST_RESPONSE", label: "Prosj. prvi odgovor" },
  { value: "AVG_RESOLUTION", label: "Prosj. rješavanje tiketa" },
];

function formatDuration(totalMinutes) {
  if (totalMinutes == null) return null;
  const mins = Math.round(totalMinutes);
  const MINS_IN_YEAR = 525960;
  const MINS_IN_MONTH = 43830;
  const MINS_IN_DAY = 1440;
  const MINS_IN_HOUR = 60;

  const years = Math.floor(mins / MINS_IN_YEAR);
  const months = Math.floor((mins % MINS_IN_YEAR) / MINS_IN_MONTH);
  const days = Math.floor((mins % MINS_IN_MONTH) / MINS_IN_DAY);
  const hours = Math.floor((mins % MINS_IN_DAY) / MINS_IN_HOUR);
  const minutes = mins % MINS_IN_HOUR;

  const parts = [];
  if (years > 0) parts.push(`${years} god`);
  if (months > 0) parts.push(`${months} mj`);
  if (days > 0) parts.push(`${days} d`);
  if (hours > 0) parts.push(`${hours} h`);
  if (minutes > 0) parts.push(`${minutes} min`);

  return parts.length > 0 ? parts.join(" ") : "0 min";
}

function formatMinutes(minutes) {
  return formatDuration(minutes);
}

function formatHours(hours) {
  if (hours == null) return null;
  return formatDuration(hours * 60);
}

function formatRating(rating) {
  if (rating == null) return null;
  return `${rating.toFixed(1)} / 5`;
}

function toDateInputValue(date) {
  return date.toISOString().slice(0, 10);
}

function StatCard({
  icon,
  label,
  value,
  color,
  description,
  onClick,
  emptyMessage,
}) {
  const Icon = icon;
  const display = value ?? null;
  const isEmpty = display === null && emptyMessage;

  return (
    <button
      type="button"
      onClick={onClick}
      disabled={!onClick}
      className={`bg-white rounded-xl p-5 shadow-sm border border-gray-100 text-left w-full ${
        onClick
          ? "cursor-pointer hover:shadow-md transition-shadow"
          : "cursor-default"
      }`}
    >
      <div className="flex items-start justify-between">
        <div>
          <p className="text-sm text-gray-500 mb-1">{label}</p>
          {isEmpty ? (
            <p className="text-sm text-gray-400 italic">{emptyMessage}</p>
          ) : (
            <p className="text-2xl font-bold text-gray-900">{display}</p>
          )}
          {description && !isEmpty && (
            <p className="text-xs text-gray-400 mt-1">{description}</p>
          )}
        </div>
        <div
          className={`w-11 h-11 rounded-xl flex items-center justify-center ${color}`}
        >
          <Icon size={20} className="text-white" />
        </div>
      </div>
    </button>
  );
}

StatCard.propTypes = {
  icon: PropTypes.elementType.isRequired,
  label: PropTypes.string.isRequired,
  value: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
  color: PropTypes.string.isRequired,
  description: PropTypes.string,
  onClick: PropTypes.func,
  emptyMessage: PropTypes.string,
};

function SkeletonGrid() {
  return (
    <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 gap-4 animate-pulse">
      {Array.from({ length: 8 }).map((_, i) => (
        <div
          key={i}
          className="bg-white rounded-xl h-24 border border-gray-100"
        />
      ))}
    </div>
  );
}

const ChartTooltip = ({ active, payload }) => {
  if (!active || !payload?.length) return null;
  return (
    <div className="bg-white border border-gray-200 rounded-lg px-3 py-2 shadow-sm text-xs">
      <p className="font-semibold text-gray-800">{payload[0].payload.name}</p>
      <p className="text-gray-600">{payload[0].value}</p>
    </div>
  );
};

ChartTooltip.propTypes = {
  active: PropTypes.bool,
  payload: PropTypes.array,
};

export default function AdminDashboardSection({ mode = "metrics" }) {
  const showMetrics = mode === "metrics";
  const showReports = mode === "reports";
  const navigate = useNavigate();
  const [period, setPeriod] = useState("month");
  const [customFrom, setCustomFrom] = useState(() =>
    toDateInputValue(new Date(Date.now() - 30 * 86400000)),
  );
  const [customTo, setCustomTo] = useState(() => toDateInputValue(new Date()));
  const [periodError, setPeriodError] = useState(null);

  const [dashboard, setDashboard] = useState(null);
  const [loading, setLoading] = useState(showMetrics);
  const [error, setError] = useState(null);

  const [reportType, setReportType] = useState(null);
  const [reportResult, setReportResult] = useState(null);
  const [reportLoading, setReportLoading] = useState(false);

  const buildQuery = useCallback(() => {
    const q = { period };
    if (period === "custom") {
      q.from = new Date(`${customFrom}T00:00:00`).toISOString();
      q.to = new Date(`${customTo}T23:59:59`).toISOString();
    }
    return q;
  }, [period, customFrom, customTo]);

  const drillDown = useCallback(
    (extra = {}) => {
      const params = new URLSearchParams({ ...buildQuery(), ...extra });
      navigate(`/tickets?${params.toString()}`);
    },
    [navigate, buildQuery],
  );

  const loadDashboard = useCallback(async () => {
    try {
      const q = buildQuery();
      const data = await getAdminDashboard({
        period: q.period,
        from: q.from,
        to: q.to,
      });
      setDashboard(data);
      setError(null);
    } catch {
      setError("Greška pri učitavanju admin dashboarda.");
    } finally {
      setLoading(false);
    }
  }, [buildQuery]);

  const validatePeriod = useCallback(() => {
    if (period === "custom" && customFrom > customTo) {
      setPeriodError("Datum kraja mora biti nakon datuma početka.");
      return false;
    }

    setPeriodError(null);
    return true;
  }, [period, customFrom, customTo]);

  useEffect(() => {
    if (showMetrics) {
      const timeoutId = window.setTimeout(() => {
        void loadDashboard();
      }, 0);

      return () => window.clearTimeout(timeoutId);
    }
  }, [showMetrics, loadDashboard]);

  const statusChartData = useMemo(() => {
    if (!dashboard?.statusBreakdown?.length) return [];
    return dashboard.statusBreakdown.map((s) => ({
      name: STATUS_LABELS[s.status] ?? s.status,
      value: s.count,
      status: s.status,
      color: STATUS_COLORS[s.status] ?? "#94a3b8",
    }));
  }, [dashboard]);

  const problemChartData = useMemo(() => {
    if (!dashboard?.topProblemTypes?.length) return [];
    return dashboard.topProblemTypes.map((p) => ({
      name: PROBLEM_LABELS[p.name] ?? p.name,
      value: p.count,
      category: p.name,
    }));
  }, [dashboard]);

  const workloadChartData = useMemo(() => {
    if (!dashboard?.topAgentWorkload?.length) return [];
    return dashboard.topAgentWorkload.map((a) => ({
      name: a.fullName.split(" ")[0],
      value: a.resolvedCount,
      userId: a.userId,
    }));
  }, [dashboard]);

  const fetchReport = useCallback(async (type) => {
    if (period === "custom" && customFrom > customTo) {
      setPeriodError("Datum kraja mora biti nakon datuma početka.");
      return;
    }
    setPeriodError(null);
    setReportLoading(true);
    setReportResult(null);
    try {
      const q = buildQuery();
      const result = await generateReport({
        reportType: type,
        period: q.period,
        from: q.from,
        to: q.to,
      });
      setReportResult(result);
    } catch {
      setReportResult({
        hasData: false,
        message: "Greška pri generisanju izvještaja.",
      });
    } finally {
      setReportLoading(false);
    }
  }, [buildQuery, period, customFrom, customTo]);

  const handleSelectChip = useCallback((type) => {
    setReportType(type);
    void fetchReport(type);
  }, [fetchReport]);

  const renderReportTable = () => {
    if (!reportResult) return null;
    if (!reportResult.hasData) {
      return (
        <p className="text-sm text-gray-500 italic py-4">
          {reportResult.message ?? "Nema podataka."}
        </p>
      );
    }

    const data = reportResult.data;
    const th = "py-2.5 px-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wide";
    const td = "py-2.5 px-3 text-sm text-gray-700";
    const tr = "border-b border-gray-100 hover:bg-gray-50 transition-colors";

    if (reportType === "TICKET_COUNT") {
      return (
        <div className="space-y-4">
          <div className="bg-gray-50 rounded-lg px-4 py-3 inline-block">
            <p className="text-xs text-gray-500 mb-0.5">Ukupno tiketa</p>
            <p className="text-2xl font-bold text-gray-900">{data.totalCount}</p>
            {data.bucketGranularityLabel && (
              <p className="text-xs text-gray-400 mt-0.5">{data.bucketGranularityLabel}</p>
            )}
          </div>
          {data.buckets?.length > 0 && (
            <table className="w-full">
              <thead>
                <tr className="bg-gray-50 border-b border-gray-200">
                  <th className={th}>Period</th>
                  <th className={th}>Tiketa</th>
                </tr>
              </thead>
              <tbody>
                {data.buckets.map((row) => (
                  <tr key={row.label} className={tr}>
                    <td className={td}>{row.label}</td>
                    <td className={td}>{row.ticketCount}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>
      );
    }

    if (reportType === "TICKET_STATUS" && data.items) {
      return (
        <table className="w-full">
          <thead>
            <tr className="bg-gray-50 border-b border-gray-200">
              <th className={th}>Status</th>
              <th className={th}>Broj</th>
              <th className={th}>%</th>
            </tr>
          </thead>
          <tbody>
            {data.items.map((row) => (
              <tr
                key={row.status}
                className={`${tr} cursor-pointer`}
                onClick={() => drillDown({ status: row.status })}
              >
                <td className={td}>{STATUS_LABELS[row.status] ?? row.status}</td>
                <td className={td}>{row.count}</td>
                <td className={td}>{row.percentage}%</td>
              </tr>
            ))}
          </tbody>
        </table>
      );
    }

    if (reportType === "PROBLEM_TYPE" && data.items) {
      return (
        <table className="w-full">
          <thead>
            <tr className="bg-gray-50 border-b border-gray-200">
              <th className={th}>Tip problema</th>
              <th className={th}>Broj</th>
            </tr>
          </thead>
          <tbody>
            {data.items.map((row) => (
              <tr
                key={row.name}
                className={`${tr} cursor-pointer`}
                onClick={() => drillDown({ problemCategory: row.name })}
              >
                <td className={td}>{PROBLEM_LABELS[row.name] ?? row.name}</td>
                <td className={td}>{row.count}</td>
              </tr>
            ))}
          </tbody>
        </table>
      );
    }

    if (reportType === "TEAM_WORKLOAD" && data.items) {
      return (
        <div className="space-y-6">
          <div>
            <p className="text-xs font-semibold text-gray-500 uppercase tracking-wide mb-2">
              Ukupno po agentu / tehničaru
            </p>
            <table className="w-full">
              <thead>
                <tr className="bg-gray-50 border-b border-gray-200">
                  <th className={th}>Agent / Tehničar</th>
                  <th className={th}>Uloga</th>
                  <th className={th}>Zatvoreno u periodu</th>
                </tr>
              </thead>
              <tbody>
                {data.items.map((row) => (
                  <tr key={row.userId} className={tr}>
                    <td className={td}>{row.fullName}</td>
                    <td className={td}>{row.role}</td>
                    <td className={td}>{row.resolvedCount}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          {data.periodRows?.length > 0 && data.agentNames?.length > 0 && (
            <div>
              <p className="text-xs font-semibold text-gray-500 uppercase tracking-wide mb-2">
                {data.bucketGranularityLabel}
              </p>
              <div className="overflow-x-auto">
                <table className="min-w-full">
                  <thead>
                    <tr className="bg-gray-50 border-b border-gray-200">
                      <th className={`${th} whitespace-nowrap`}>Period</th>
                      {data.agentNames.map((name) => (
                        <th key={name} className={`${th} whitespace-nowrap`}>
                          {name.split(" ")[0]}
                        </th>
                      ))}
                    </tr>
                  </thead>
                  <tbody>
                    {data.periodRows.map((row) => (
                      <tr key={row.label} className={tr}>
                        <td className={`${td} whitespace-nowrap`}>{row.label}</td>
                        {row.counts.map((count, i) => (
                          <td key={i} className={td}>{count}</td>
                        ))}
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          )}
        </div>
      );
    }

    if (reportType === "USER_RATINGS") {
      return (
        <div className="space-y-4">
          <div className="grid grid-cols-2 sm:grid-cols-3 gap-3">
            <div className="bg-gray-50 rounded-lg px-4 py-3">
              <p className="text-xs text-gray-500 mb-0.5">Prosječna ocjena</p>
              <p className="text-2xl font-bold text-gray-900">
                {data.averageRating != null ? formatRating(data.averageRating) : "—"}
              </p>
            </div>
            <div className="bg-gray-50 rounded-lg px-4 py-3">
              <p className="text-xs text-gray-500 mb-0.5">Ocijenjenih tiketa</p>
              <p className="text-2xl font-bold text-gray-900">{data.ratedTicketsCount ?? 0}</p>
            </div>
            {data.distribution?.length > 0 && (
              <div className="bg-gray-50 rounded-lg px-4 py-3 col-span-2 sm:col-span-1">
                <p className="text-xs text-gray-500 mb-1">Distribucija</p>
                <div className="flex flex-wrap gap-x-3 gap-y-0.5">
                  {data.distribution.map((d) => (
                    <span key={d.stars} className="text-sm text-gray-700">
                      {d.stars}★ <span className="font-semibold">{d.count}</span>
                    </span>
                  ))}
                </div>
              </div>
            )}
          </div>
          {data.buckets?.length > 0 && (
            <div>
              {data.bucketGranularityLabel && (
                <p className="text-xs text-gray-500 mb-2">{data.bucketGranularityLabel}</p>
              )}
              <table className="w-full">
                <thead>
                  <tr className="bg-gray-50 border-b border-gray-200">
                    <th className={th}>Period</th>
                    <th className={th}>Prosj. ocjena</th>
                    <th className={th}>Broj</th>
                  </tr>
                </thead>
                <tbody>
                  {data.buckets.map((row) => (
                    <tr key={row.label} className={tr}>
                      <td className={td}>{row.label}</td>
                      <td className={td}>
                        {row.avgRating != null ? formatRating(row.avgRating) : "—"}
                      </td>
                      <td className={td}>{row.count}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      );
    }

    if (reportType === "FIRST_RESPONSE") {
      return (
        <div className="space-y-4">
          {reportResult.message && (
            <p className="text-sm text-amber-700 bg-amber-50 border border-amber-100 rounded-lg px-3 py-2">
              {reportResult.message}
            </p>
          )}
          <div className="grid grid-cols-2 gap-3">
            <div className="bg-gray-50 rounded-lg px-4 py-3">
              <p className="text-xs text-gray-500 mb-0.5">Prosj. 1. odgovor</p>
              <p className="text-2xl font-bold text-gray-900">
                {data.avgFirstResponseMinutes != null
                  ? formatMinutes(data.avgFirstResponseMinutes)
                  : "—"}
              </p>
            </div>
            <div className="bg-gray-50 rounded-lg px-4 py-3">
              <p className="text-xs text-gray-500 mb-0.5">S odgovorom / ukupno</p>
              <p className="text-2xl font-bold text-gray-900">
                {data.ticketsWithResponseCount ?? 0}
                <span className="text-base font-normal text-gray-400"> / {data.totalTicketsCount ?? 0}</span>
              </p>
            </div>
          </div>
          {data.buckets?.length > 0 ? (
            <div>
              {data.bucketGranularityLabel && (
                <p className="text-xs text-gray-500 mb-2">{data.bucketGranularityLabel}</p>
              )}
              <table className="w-full">
                <thead>
                  <tr className="bg-gray-50 border-b border-gray-200">
                    <th className={th}>Period</th>
                    <th className={th}>Tiketi</th>
                    <th className={th}>S odgovorom</th>
                    <th className={th}>Prosj. vrijeme</th>
                  </tr>
                </thead>
                <tbody>
                  {data.buckets.map((row) => (
                    <tr key={row.label} className={tr}>
                      <td className={td}>{row.label}</td>
                      <td className={td}>{row.ticketCount}</td>
                      <td className={td}>{row.ticketsWithResponseCount}</td>
                      <td className={td}>
                        {row.avgFirstResponseMinutes != null
                          ? formatMinutes(row.avgFirstResponseMinutes)
                          : "—"}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          ) : (
            <p className="text-sm text-gray-400 italic">Nema podataka po pod-periodima.</p>
          )}
        </div>
      );
    }

    if (reportType === "AVG_RESOLUTION") {
      return (
        <div className="space-y-4">
          {reportResult.message && (
            <p className="text-sm text-amber-700 bg-amber-50 border border-amber-100 rounded-lg px-3 py-2">
              {reportResult.message}
            </p>
          )}
          <div className="grid grid-cols-2 gap-3">
            <div className="bg-gray-50 rounded-lg px-4 py-3">
              <p className="text-xs text-gray-500 mb-0.5">Prosj. rješavanje</p>
              <p className="text-2xl font-bold text-gray-900">
                {data.avgResolutionHours != null
                  ? formatHours(data.avgResolutionHours)
                  : "—"}
              </p>
            </div>
            <div className="bg-gray-50 rounded-lg px-4 py-3">
              <p className="text-xs text-gray-500 mb-0.5">Zatvoreno / ukupno</p>
              <p className="text-2xl font-bold text-gray-900">
                {data.closedTicketsCount ?? 0}
                <span className="text-base font-normal text-gray-400"> / {data.totalTicketsCount ?? 0}</span>
              </p>
            </div>
          </div>
          {data.buckets?.length > 0 ? (
            <div>
              {data.bucketGranularityLabel && (
                <p className="text-xs text-gray-500 mb-2">{data.bucketGranularityLabel}</p>
              )}
              <table className="w-full">
                <thead>
                  <tr className="bg-gray-50 border-b border-gray-200">
                    <th className={th}>Period</th>
                    <th className={th}>Tiketa</th>
                    <th className={th}>Zatvoreno</th>
                    <th className={th}>Prosj. rješavanje</th>
                  </tr>
                </thead>
                <tbody>
                  {data.buckets.map((row) => (
                    <tr key={row.label} className={tr}>
                      <td className={td}>{row.label}</td>
                      <td className={td}>{row.ticketCount}</td>
                      <td className={td}>{row.closedCount}</td>
                      <td className={td}>
                        {row.avgResolutionHours != null
                          ? formatHours(row.avgResolutionHours)
                          : "—"}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          ) : (
            <p className="text-sm text-gray-400 italic">Nema podataka po pod-periodima.</p>
          )}
        </div>
      );
    }

    return null;
  };


  return (
    <div className="space-y-6">
      {/* US-72: globalni filter */}
      <div className="bg-white rounded-xl border border-gray-100 shadow-sm p-4">
        <h3 className="text-sm font-semibold text-gray-500 uppercase tracking-wide mb-3">
          Vremenski period
        </h3>
        <div className="flex flex-wrap gap-2 items-end">
          {[
            { value: "week", label: "Sedmica" },
            { value: "month", label: "Mjesec" },
            { value: "year", label: "Godina" },
            { value: "alltime", label: "Svi tiketi" },
            { value: "custom", label: "Prilagođeno" },
          ].map((opt) => (
            <button
              key={opt.value}
              type="button"
              onClick={() => setPeriod(opt.value)}
              className={`px-3 py-2 rounded-lg text-sm font-medium transition-colors ${
                period === opt.value
                  ? "bg-navy-700 text-white"
                  : "bg-gray-100 text-gray-600 hover:bg-gray-200"
              }`}
            >
              {opt.label}
            </button>
          ))}
          {period === "custom" && (
            <>
              <input
                type="date"
                value={customFrom}
                onChange={(e) => setCustomFrom(e.target.value)}
                className="px-3 py-2 border border-gray-300 rounded-lg text-sm"
              />
              <span className="text-gray-400 text-sm">—</span>
              <input
                type="date"
                value={customTo}
                onChange={(e) => setCustomTo(e.target.value)}
                className="px-3 py-2 border border-gray-300 rounded-lg text-sm"
              />
            </>
          )}
          {(showMetrics || (showReports && reportType)) && (
            <button
              type="button"
              onClick={() => {
                if (!validatePeriod()) return;
                if (showMetrics) {
                  setLoading(true);
                  void loadDashboard();
                }
                if (showReports && reportType) {
                  void fetchReport(reportType);
                }
              }}
              className="px-4 py-2 bg-navy-600 text-white text-sm font-medium rounded-lg hover:bg-navy-700"
            >
              Primijeni
            </button>
          )}
        </div>
        {periodError && (
          <p className="text-sm text-red-600 mt-2">{periodError}</p>
        )}
      </div>

      {showMetrics && error && (
        <div className="flex items-center gap-2 text-red-500 bg-red-50 border border-red-100 rounded-xl p-4">
          <AlertCircle size={18} />
          <span className="text-sm">{error}</span>
        </div>
      )}

      {showMetrics && loading && <SkeletonGrid />}

      {showMetrics && !loading && dashboard && (
        <>
          {/* US-71 / US-86: KPI kartice */}
          <div>
            <h3 className="text-sm font-semibold text-gray-500 uppercase tracking-wide mb-3">
              Ključne metrike
            </h3>
            <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 gap-4">
              <StatCard
                icon={Ticket}
                label="Kreirani tiketi"
                value={dashboard.totalTicketsInPeriod}
                color="bg-navy-600"
                onClick={() => drillDown()}
                emptyMessage="Nema tiketa u periodu"
              />
              <StatCard
                icon={MessageSquare}
                label="Prosj. 1. odgovor"
                value={formatMinutes(dashboard.avgFirstResponseMinutes)}
                color="bg-violet-500"
                emptyMessage="Nema odgovora u periodu"
              />
              <StatCard
                icon={Clock}
                label="Prosj. rješavanje"
                value={formatHours(dashboard.avgResolutionHours)}
                description={dashboard.closedInPeriodCount != null ? `${dashboard.closedInPeriodCount} zatvorenih tiketa` : undefined}
                color="bg-blue-500"
                emptyMessage="Nema zatvorenih tiketa"
              />
              <StatCard
                icon={Star}
                label="Prosj. ocjena"
                value={formatRating(dashboard.avgRating)}
                color="bg-yellow-500"
                emptyMessage="Nema ocjena u periodu"
              />
              <StatCard
                icon={CheckCircle}
                label="Otvoreni (trenutno)"
                value={dashboard.openTicketsCount}
                color="bg-emerald-500"
                onClick={() => drillDown({ status: "OPEN", snapshot: "true" })}
              />
              <StatCard
                icon={AlertCircle}
                label="Čeka zatvaranje"
                value={dashboard.closureRequestedCount}
                color="bg-amber-500"
                onClick={() =>
                  drillDown({ status: "CLOSURE_REQUESTED", snapshot: "true" })
                }
              />
              <StatCard
                icon={CheckCircle}
                label="Zatvoreni"
                value={dashboard.closedInPeriodCount ?? 0}
                color="bg-emerald-600"
                onClick={() => drillDown({ status: "CLOSED" })}
                emptyMessage="Nema zatvorenih u periodu"
              />
              <StatCard
                icon={Clock}
                label="Zastarjeli (7+ dana)"
                value={dashboard.staleTicketsCount}
                color="bg-red-500"
                onClick={() => drillDown({ stale: "true", snapshot: "true" })}
              />
            </div>
          </div>

          <div>
            <h3 className="text-sm font-semibold text-gray-500 uppercase tracking-wide mb-3">
              Aktivni korisnici
            </h3>
            <div className="grid grid-cols-2 sm:grid-cols-4 gap-3">
              {[
                {
                  label: "Klijenti",
                  value: dashboard.activeUsersByRole?.clients,
                },
                { label: "Agenti", value: dashboard.activeUsersByRole?.agents },
                {
                  label: "Tehničari",
                  value: dashboard.activeUsersByRole?.technicians,
                },
                {
                  label: "Admini",
                  value: dashboard.activeUsersByRole?.administrators,
                },
              ].map((item) => (
                <div
                  key={item.label}
                  className="bg-white rounded-lg px-4 py-3 border border-gray-100 shadow-sm"
                >
                  <p className="text-xs text-gray-400">{item.label}</p>
                  <p className="text-lg font-bold text-gray-900">
                    {item.value ?? 0}
                  </p>
                </div>
              ))}
            </div>
          </div>

          {/* US-82: grafovi */}
          <div>
            <h3 className="text-sm font-semibold text-gray-500 uppercase tracking-wide mb-3">
              Grafovi
            </h3>
            <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-4">
              <div className="bg-white rounded-xl p-5 shadow-sm border border-gray-100">
                <p className="text-sm font-semibold text-gray-700 mb-3">
                  Po statusu
                </p>
                {statusChartData.length > 0 ? (
                  <ResponsiveContainer width="100%" height={200}>
                    <PieChart>
                      <Pie
                        data={statusChartData}
                        cx="50%"
                        cy="50%"
                        innerRadius={45}
                        outerRadius={75}
                        dataKey="value"
                        onClick={(_, index) =>
                          drillDown({ status: statusChartData[index]?.status })
                        }
                        style={{ cursor: "pointer" }}
                      >
                        {statusChartData.map((entry) => (
                          <Cell key={entry.name} fill={entry.color} />
                        ))}
                      </Pie>
                      <Tooltip content={<ChartTooltip />} />
                      <Legend iconType="circle" iconSize={8} />
                    </PieChart>
                  </ResponsiveContainer>
                ) : (
                  <p className="text-sm text-gray-400 italic py-8 text-center">
                    Nema podataka za grafikon.
                  </p>
                )}
              </div>

              <div className="bg-white rounded-xl p-5 shadow-sm border border-gray-100">
                <p className="text-sm font-semibold text-gray-700 mb-3">
                  Top tipovi problema
                </p>
                {problemChartData.length > 0 ? (
                  <ResponsiveContainer width="100%" height={200}>
                    <BarChart data={problemChartData}>
                      <CartesianGrid
                        strokeDasharray="3 3"
                        stroke="#f0f0f0"
                        vertical={false}
                      />
                      <XAxis dataKey="name" tick={{ fontSize: 10 }} />
                      <YAxis tick={{ fontSize: 10 }} width={28} />
                      <Tooltip content={<ChartTooltip />} />
                      <Bar
                        dataKey="value"
                        fill="#3b82f6"
                        radius={[4, 4, 0, 0]}
                        onClick={(data) =>
                          drillDown({ problemCategory: data.category })
                        }
                        style={{ cursor: "pointer" }}
                      />
                    </BarChart>
                  </ResponsiveContainer>
                ) : (
                  <p className="text-sm text-gray-400 italic py-8 text-center">
                    Nema podataka za grafikon.
                  </p>
                )}
              </div>

              <div className="bg-white rounded-xl p-5 shadow-sm border border-gray-100">
                <p className="text-sm font-semibold text-gray-700 mb-3">
                  Opterećenje agenata
                </p>
                {workloadChartData.length > 0 ? (
                  <ResponsiveContainer width="100%" height={200}>
                    <BarChart data={workloadChartData}>
                      <CartesianGrid
                        strokeDasharray="3 3"
                        stroke="#f0f0f0"
                        vertical={false}
                      />
                      <XAxis dataKey="name" tick={{ fontSize: 10 }} />
                      <YAxis tick={{ fontSize: 10 }} width={28} />
                      <Tooltip content={<ChartTooltip />} />
                      <Bar
                        dataKey="value"
                        fill="#8b5cf6"
                        radius={[4, 4, 0, 0]}
                      />
                    </BarChart>
                  </ResponsiveContainer>
                ) : (
                  <p className="text-sm text-gray-400 italic py-8 text-center">
                    Nema podataka za grafikon.
                  </p>
                )}
              </div>
            </div>
          </div>
        </>
      )}

      {showReports && (
        <div className="bg-white rounded-xl border border-gray-100 shadow-sm overflow-hidden">
          <div className="px-5 py-4 border-b border-gray-100 flex items-center justify-between">
            <div className="flex items-center gap-2">
              <BarChart2 size={16} className="text-gray-400" />
              <h3 className="text-sm font-semibold text-gray-500 uppercase tracking-wide">
                Izvještaji
              </h3>
            </div>
            <button
              type="button"
              disabled
              title="Export će biti dostupan u budućoj verziji"
              className="inline-flex items-center gap-2 px-3 py-1.5 text-sm font-medium text-gray-400 bg-gray-100 rounded-lg cursor-not-allowed"
            >
              <Download size={15} />
              Export
            </button>
          </div>

          <div className="overflow-x-auto border-b border-gray-100">
            <div className="flex gap-1.5 px-4 py-3 min-w-max">
              {REPORT_TYPES.map((r) => (
                <button
                  key={r.value}
                  type="button"
                  onClick={() => handleSelectChip(r.value)}
                  className={`px-3.5 py-1.5 rounded-full text-sm font-medium whitespace-nowrap transition-colors ${
                    reportType === r.value
                      ? "bg-navy-700 text-white"
                      : "bg-gray-100 text-gray-600 hover:bg-gray-200"
                  }`}
                >
                  {r.label}
                </button>
              ))}
            </div>
          </div>

          <div className="p-5">
            {!reportType && (
              <p className="text-sm text-gray-400 italic py-2">
                Odaberi tip izvještaja gore.
              </p>
            )}
            {reportLoading && (
              <div className="flex items-center text-gray-400 text-sm py-4">
                <Loader2 size={18} className="animate-spin mr-2" />
                Učitavanje...
              </div>
            )}
            {!reportLoading && reportType && (
              <>
                {reportResult?.showLargePeriodWarning && (
                  <p className="text-sm text-amber-700 bg-amber-50 border border-amber-100 rounded-lg px-3 py-2 mb-4">
                    Upozorenje: za veliki vremenski opseg izvještaj po statusu može dati nepouzdanu interpretaciju postotaka.
                  </p>
                )}
                {renderReportTable()}
              </>
            )}
          </div>
        </div>
      )}
    </div>
  );
}

AdminDashboardSection.propTypes = {
  mode: PropTypes.oneOf(["metrics", "reports"]),
};
