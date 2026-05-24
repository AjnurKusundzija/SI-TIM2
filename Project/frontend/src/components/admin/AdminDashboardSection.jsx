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

  const [reportType, setReportType] = useState("TICKET_COUNT");
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

  const handleGenerateReport = async () => {
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
        reportType,
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
  };

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

    if (reportType === "TICKET_COUNT") {
      return (
        <div className="py-2 space-y-3 text-sm">
          <p className="text-lg font-bold text-gray-900">
            Ukupno: {data.totalCount}
          </p>
          {data.bucketGranularityLabel && (
            <p className="text-xs text-gray-500">{data.bucketGranularityLabel}</p>
          )}
          {data.buckets?.length > 0 && (
            <table className="w-full text-sm mt-1">
              <thead>
                <tr className="text-left text-gray-500 border-b">
                  <th className="py-2">Period</th>
                  <th className="py-2">Tiketa</th>
                </tr>
              </thead>
              <tbody>
                {data.buckets.map((row) => (
                  <tr key={row.label} className="border-b border-gray-50">
                    <td className="py-2">{row.label}</td>
                    <td className="py-2">{row.ticketCount}</td>
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
        <table className="w-full text-sm mt-2">
          <thead>
            <tr className="text-left text-gray-500 border-b">
              <th className="py-2">Status</th>
              <th className="py-2">Broj</th>
              <th className="py-2">%</th>
            </tr>
          </thead>
          <tbody>
            {data.items.map((row) => (
              <tr
                key={row.status}
                className="border-b border-gray-50 hover:bg-gray-50 cursor-pointer"
                onClick={() => drillDown({ status: row.status })}
              >
                <td className="py-2">
                  {STATUS_LABELS[row.status] ?? row.status}
                </td>
                <td className="py-2">{row.count}</td>
                <td className="py-2">{row.percentage}%</td>
              </tr>
            ))}
          </tbody>
        </table>
      );
    }

    if (reportType === "PROBLEM_TYPE" && data.items) {
      return (
        <table className="w-full text-sm mt-2">
          <thead>
            <tr className="text-left text-gray-500 border-b">
              <th className="py-2">Tip</th>
              <th className="py-2">Broj</th>
            </tr>
          </thead>
          <tbody>
            {data.items.map((row) => (
              <tr
                key={row.name}
                className="border-b border-gray-50 hover:bg-gray-50 cursor-pointer"
                onClick={() => drillDown({ problemCategory: row.name })}
              >
                <td className="py-2">{PROBLEM_LABELS[row.name] ?? row.name}</td>
                <td className="py-2">{row.count}</td>
              </tr>
            ))}
          </tbody>
        </table>
      );
    }

    if (reportType === "TEAM_WORKLOAD" && data.items) {
      return (
        <div className="space-y-5 text-sm py-2">
          <div>
            <p className="text-xs font-semibold text-gray-500 uppercase tracking-wide mb-2">
              Ukupno po agentu / tehničaru
            </p>
            <table className="w-full text-sm">
              <thead>
                <tr className="text-left text-gray-500 border-b">
                  <th className="py-2">Agent / Tehničar</th>
                  <th className="py-2">Uloga</th>
                  <th className="py-2">Zatvoreno u periodu</th>
                </tr>
              </thead>
              <tbody>
                {data.items.map((row) => (
                  <tr key={row.userId} className="border-b border-gray-50">
                    <td className="py-2">{row.fullName}</td>
                    <td className="py-2">{row.role}</td>
                    <td className="py-2">{row.resolvedCount}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          {data.periodRows?.length > 0 && data.agentNames?.length > 0 && (
            <div className="overflow-x-auto">
              <p className="text-xs font-semibold text-gray-500 uppercase tracking-wide mb-2">
                {data.bucketGranularityLabel}
              </p>
              <table className="text-sm min-w-full">
                <thead>
                  <tr className="text-left text-gray-500 border-b">
                    <th className="py-2 pr-4 whitespace-nowrap">Period</th>
                    {data.agentNames.map((name) => (
                      <th key={name} className="py-2 pr-3 whitespace-nowrap">
                        {name.split(" ")[0]}
                      </th>
                    ))}
                  </tr>
                </thead>
                <tbody>
                  {data.periodRows.map((row) => (
                    <tr key={row.label} className="border-b border-gray-50">
                      <td className="py-2 pr-4 whitespace-nowrap">{row.label}</td>
                      {row.counts.map((count, i) => (
                        <td key={i} className="py-2 pr-3">{count}</td>
                      ))}
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      );
    }

    if (reportType === "USER_RATINGS") {
      return (
        <div className="py-2 space-y-3 text-sm">
          <p>
            Prosječna ocjena:{" "}
            <span className="font-semibold">
              {data.averageRating != null ? formatRating(data.averageRating) : "—"}
            </span>
          </p>
          <p>Ocijenjenih tiketa: {data.ratedTicketsCount ?? 0}</p>
          {data.distribution?.map((d) => (
            <p key={d.stars}>
              {d.stars} ★ — {d.count}
            </p>
          ))}
          {data.buckets?.length > 0 && (
            <>
              <p className="text-xs text-gray-500 pt-1">
                {data.bucketGranularityLabel}
              </p>
              <table className="w-full text-sm mt-1">
                <thead>
                  <tr className="text-left text-gray-500 border-b">
                    <th className="py-2">Period</th>
                    <th className="py-2">Prosj. ocjena</th>
                    <th className="py-2">Broj</th>
                  </tr>
                </thead>
                <tbody>
                  {data.buckets.map((row) => (
                    <tr key={row.label} className="border-b border-gray-50">
                      <td className="py-2">{row.label}</td>
                      <td className="py-2">
                        {row.avgRating != null ? formatRating(row.avgRating) : "—"}
                      </td>
                      <td className="py-2">{row.count}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </>
          )}
        </div>
      );
    }

    if (reportType === "FIRST_RESPONSE") {
      return (
        <div className="py-2 space-y-3 text-sm">
          {reportResult.message && (
            <p className="text-amber-700 bg-amber-50 border border-amber-100 rounded-lg px-3 py-2">
              {reportResult.message}
            </p>
          )}
          <p>
            Prosjek u periodu{period === "custom" ? ` (${customFrom} — ${customTo})` : ""}:{" "}
            <span className="font-semibold">
              {data.avgFirstResponseMinutes != null
                ? formatMinutes(data.avgFirstResponseMinutes)
                : "—"}
            </span>
          </p>
          <p>
            Tiketi s odgovorom: {data.ticketsWithResponseCount ?? 0} /{" "}
            {data.totalTicketsCount ?? 0}
          </p>
          <p className="text-xs text-gray-500">{data.bucketGranularityLabel}</p>
          {data.buckets?.length > 0 ? (
            <table className="w-full text-sm mt-2">
              <thead>
                <tr className="text-left text-gray-500 border-b">
                  <th className="py-2">Period</th>
                  <th className="py-2">Tiketi</th>
                  <th className="py-2">S odgovorom</th>
                  <th className="py-2">Prosj. vrijeme</th>
                </tr>
              </thead>
              <tbody>
                {data.buckets.map((row) => (
                  <tr key={row.label} className="border-b border-gray-50">
                    <td className="py-2">{row.label}</td>
                    <td className="py-2">{row.ticketCount}</td>
                    <td className="py-2">{row.ticketsWithResponseCount}</td>
                    <td className="py-2">
                      {row.avgFirstResponseMinutes != null
                        ? formatMinutes(row.avgFirstResponseMinutes)
                        : "—"}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          ) : (
            <p className="text-gray-400 italic">
              Nema podataka po pod-periodima.
            </p>
          )}
        </div>
      );
    }

    if (reportType === "AVG_RESOLUTION") {
      return (
        <div className="py-2 space-y-3 text-sm">
          {reportResult.message && (
            <p className="text-amber-700 bg-amber-50 border border-amber-100 rounded-lg px-3 py-2">
              {reportResult.message}
            </p>
          )}
          <p>
            Prosječno rješavanje:{" "}
            <span className="font-semibold">
              {data.avgResolutionHours != null
                ? formatHours(data.avgResolutionHours)
                : "—"}
            </span>
          </p>
          <p>
            Zatvorenih tiketa: {data.closedTicketsCount ?? 0} /{" "}
            {data.totalTicketsCount ?? 0}
          </p>
          <p className="text-xs text-gray-500">{data.bucketGranularityLabel}</p>
          {data.buckets?.length > 0 ? (
            <table className="w-full text-sm mt-2">
              <thead>
                <tr className="text-left text-gray-500 border-b">
                  <th className="py-2">Period</th>
                  <th className="py-2">Tiketa</th>
                  <th className="py-2">Zatvoreno</th>
                  <th className="py-2">Prosj. rješavanje</th>
                </tr>
              </thead>
              <tbody>
                {data.buckets.map((row) => (
                  <tr key={row.label} className="border-b border-gray-50">
                    <td className="py-2">{row.label}</td>
                    <td className="py-2">{row.ticketCount}</td>
                    <td className="py-2">{row.closedCount}</td>
                    <td className="py-2">
                      {row.avgResolutionHours != null
                        ? formatHours(row.avgResolutionHours)
                        : "—"}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          ) : (
            <p className="text-gray-400 italic">
              Nema podataka po pod-periodima.
            </p>
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
          {showMetrics && (
            <button
              type="button"
              onClick={() => {
                if (!validatePeriod()) return;
                setLoading(true);
                void loadDashboard();
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
        <div className="bg-white rounded-xl border border-gray-100 shadow-sm p-5">
          <div className="flex flex-wrap items-center justify-between gap-3 mb-4">
            <h3 className="text-sm font-semibold text-gray-500 uppercase tracking-wide flex items-center gap-2">
              <BarChart2 size={16} />
              Generisanje izvještaja
            </h3>
            <button
              type="button"
              disabled
              title="Export će biti dostupan u PB-46"
              className="inline-flex items-center gap-2 px-3 py-2 text-sm font-medium text-gray-400 bg-gray-100 rounded-lg cursor-not-allowed"
            >
              <Download size={16} />
              Export
            </button>
          </div>
          <p className="text-xs text-gray-400 mb-3">
            CSV export planiran (PB-46)
          </p>

          <div className="flex flex-wrap gap-2 items-center">
            <select
              value={reportType}
              onChange={(e) => setReportType(e.target.value)}
              className="px-3 py-2 border border-gray-300 rounded-lg text-sm bg-white"
            >
              {REPORT_TYPES.map((r) => (
                <option key={r.value} value={r.value}>
                  {r.label}
                </option>
              ))}
            </select>
            <button
              type="button"
              onClick={handleGenerateReport}
              disabled={reportLoading || !!periodError}
              className="px-4 py-2 bg-navy-600 text-white text-sm font-medium rounded-lg hover:bg-navy-700 disabled:opacity-50"
            >
              {reportLoading ? "Generisanje..." : "Generiši izvještaj"}
            </button>
          </div>

          {reportResult?.showLargePeriodWarning && (
            <p className="text-sm text-amber-700 bg-amber-50 border border-amber-100 rounded-lg px-3 py-2 mt-3">
              Upozorenje: za veliki vremenski opseg izvještaj po statusu može
              dati nepouzdanu interpretaciju postotaka.
            </p>
          )}

          <div className="mt-4 border-t border-gray-100 pt-4">
            {reportLoading ? (
              <div className="flex items-center text-gray-400 text-sm py-4">
                <Loader2 size={18} className="animate-spin mr-2" />
                Učitavanje...
              </div>
            ) : (
              renderReportTable()
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
