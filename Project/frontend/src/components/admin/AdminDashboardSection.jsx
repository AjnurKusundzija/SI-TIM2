import PropTypes from "prop-types";
import { useState, useEffect, useCallback, useMemo } from "react";
import { useNavigate } from "react-router-dom";
import { getAdminDashboard, generateReport } from "../../services/adminService";
import AIInsightsPanel from "./AIInsightsPanel";
import { useUIStore } from "../../store/uiStore";
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
  Users,
  TrendingUp,
  ArrowUpRight,
  ArrowDownRight,
  Minus,
  X,
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
  { value: "TEAM_WORKLOAD", label: "Opterećenje tima" },
  { value: "USER_RATINGS", label: "Ocjene korisnika" },
  { value: "FIRST_RESPONSE", label: "Prosj. prvi odgovor" },
  { value: "AVG_RESOLUTION", label: "Prosj. rješavanje" },
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

const CARD_ACCENT = {
  blue:    { bg: "bg-navy-50",    icon: "text-navy-700",    border: "border-navy-100" },
  violet:  { bg: "bg-violet-50",  icon: "text-violet-700",  border: "border-violet-100" },
  emerald: { bg: "bg-emerald-50", icon: "text-emerald-700", border: "border-emerald-100" },
  amber:   { bg: "bg-amber-50",   icon: "text-amber-700",   border: "border-amber-100" },
  red:     { bg: "bg-red-50",     icon: "text-red-600",     border: "border-red-100" },
  sky:     { bg: "bg-sky-50",     icon: "text-sky-700",     border: "border-sky-100" },
  indigo:  { bg: "bg-indigo-50",  icon: "text-indigo-700",  border: "border-indigo-100" },
  teal:    { bg: "bg-teal-50",    icon: "text-teal-700",    border: "border-teal-100" },
};

// trend: { value: number, label: string } — positive = green up, negative = red down, 0 = neutral
function StatCard({ icon, label, value, accent = "blue", description, onClick, emptyMessage, trend }) {
  const Icon = icon;
  const display = value ?? null;
  const isEmpty = display === null && emptyMessage;
  const colors = CARD_ACCENT[accent] ?? CARD_ACCENT.blue;

  const TrendIcon = trend == null ? null : trend.value > 0 ? ArrowUpRight : trend.value < 0 ? ArrowDownRight : Minus;
  const trendColor = trend == null ? "" : trend.value > 0 ? "text-emerald-600" : trend.value < 0 ? "text-red-500" : "text-gray-400";
  const trendBg   = trend == null ? "" : trend.value > 0 ? "bg-emerald-50" : trend.value < 0 ? "bg-red-50" : "bg-gray-100";

  return (
    <button
      type="button"
      onClick={onClick}
      disabled={!onClick}
      className={`bg-white rounded-2xl p-5 border border-slate-200 text-left w-full transition-all ${
        onClick ? "cursor-pointer hover:shadow-md hover:-translate-y-0.5 shadow-sm" : "cursor-default shadow-sm"
      }`}
    >
      {/* Row 1: icon + trend */}
      <div className="flex items-start justify-between mb-3">
        <div className={`w-9 h-9 rounded-xl flex items-center justify-center flex-shrink-0 ${colors.bg}`}>
          <Icon size={17} className={colors.icon} />
        </div>
        {trend != null && TrendIcon && (
          <div className={`flex items-center gap-1 px-2 py-0.5 rounded-full text-[11px] font-semibold ${trendBg} ${trendColor}`}>
            <TrendIcon size={11} />
            <span>{trend.label}</span>
          </div>
        )}
      </div>

      {/* Row 2: number */}
      {isEmpty ? (
        <p className="text-sm text-gray-300 italic">{emptyMessage}</p>
      ) : (
        <p className="text-2xl font-bold text-gray-900 leading-none">{display}</p>
      )}

      {/* Row 3: label + description */}
      <p className="text-xs font-medium text-gray-400 mt-2 leading-tight">{label}</p>
      {description && !isEmpty && (
        <p className="text-xs text-gray-400 mt-0.5">{description}</p>
      )}
    </button>
  );
}

StatCard.propTypes = {
  icon: PropTypes.elementType.isRequired,
  label: PropTypes.string.isRequired,
  value: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
  accent: PropTypes.string,
  description: PropTypes.string,
  onClick: PropTypes.func,
  emptyMessage: PropTypes.string,
  trend: PropTypes.shape({ value: PropTypes.number, label: PropTypes.string }),
};

function SkeletonGrid() {
  return (
    <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 gap-4 animate-pulse">
      {Array.from({ length: 8 }).map((_, i) => (
        <div key={i} className="bg-white rounded-2xl h-24 border border-gray-100 shadow-sm" />
      ))}
    </div>
  );
}

const ChartTooltip = ({ active, payload }) => {
  if (!active || !payload?.length) return null;
  return (
    <div className="bg-white border border-gray-100 rounded-xl px-3 py-2 shadow-lg text-xs">
      <p className="font-semibold text-gray-800">{payload[0].payload.name}</p>
      <p className="text-gray-500 mt-0.5">{payload[0].value}</p>
    </div>
  );
};

ChartTooltip.propTypes = {
  active: PropTypes.bool,
  payload: PropTypes.array,
};

const SectionTitle = ({ children }) => (
  <h3 className="text-sm font-semibold text-gray-500 mb-3">{children}</h3>
);
SectionTitle.propTypes = { children: PropTypes.node.isRequired };

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
  const [bannerDismissed, setBannerDismissed] = useState(false);
  const { aiPanelOpen, closeAiPanel, setAlert } = useUIStore();

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
      const data = await getAdminDashboard({ period: q.period, from: q.from, to: q.to });
      setDashboard(data);
      const stale = data.staleTicketsCount ?? 0;
      const pending = data.closureRequestedCount ?? 0;
      const alertUrl = stale > 0
        ? `/tickets?stale=true&snapshot=true&period=${buildQuery().period}`
        : `/tickets?status=CLOSURE_REQUESTED&snapshot=true&period=${buildQuery().period}`;
      setAlert(stale + pending, stale + pending > 0 ? alertUrl : '');
      setBannerDismissed(false);
      setError(null);
    } catch {
      setError("Greška pri učitavanju admin dashboarda.");
    } finally {
      setLoading(false);
    }
  }, [buildQuery, setAlert]);

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
      const timeoutId = window.setTimeout(() => { void loadDashboard(); }, 0);
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
      const result = await generateReport({ reportType: type, period: q.period, from: q.from, to: q.to });
      setReportResult(result);
    } catch {
      setReportResult({ hasData: false, message: "Greška pri generisanju izvještaja." });
    } finally {
      setReportLoading(false);
    }
  }, [buildQuery, period, customFrom, customTo]);

  const handleSelectChip = useCallback((type) => {
    setReportType(type);
    void fetchReport(type);
  }, [fetchReport]);

  const th = "py-3 px-4 text-left text-xs font-semibold text-gray-400 uppercase tracking-wide";
  const td = "py-3 px-4 text-sm text-gray-700";
  const tr = "border-b border-gray-50 hover:bg-gray-50/50 transition-colors";

  const renderReportTable = () => {
    if (!reportResult) return null;
    if (!reportResult.hasData) {
      return (
        <p className="text-sm text-gray-400 italic py-4">
          {reportResult.message ?? "Nema podataka."}
        </p>
      );
    }

    const data = reportResult.data;

    if (reportType === "TICKET_COUNT") {
      return (
        <div className="space-y-4">
          <div className="bg-navy-50 rounded-xl px-4 py-3 inline-block">
            <p className="text-xs text-navy-400 font-medium mb-0.5">Ukupno tiketa</p>
            <p className="text-2xl font-bold text-navy-800">{data.totalCount}</p>
            {data.bucketGranularityLabel && (
              <p className="text-xs text-blue-400 mt-0.5">{data.bucketGranularityLabel}</p>
            )}
          </div>
          {data.buckets?.length > 0 && (
            <table className="w-full">
              <thead>
                <tr className="border-b border-gray-100">
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
            <tr className="border-b border-gray-100">
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
            <tr className="border-b border-gray-100">
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
            <p className="text-xs font-semibold text-gray-400 uppercase tracking-wide mb-2">
              Ukupno po agentu / tehničaru
            </p>
            <table className="w-full">
              <thead>
                <tr className="border-b border-gray-100">
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
              <p className="text-xs font-semibold text-gray-400 uppercase tracking-wide mb-2">
                {data.bucketGranularityLabel}
              </p>
              <div className="overflow-x-auto">
                <table className="min-w-full">
                  <thead>
                    <tr className="border-b border-gray-100">
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
            <div className="bg-amber-50 rounded-xl px-4 py-3">
              <p className="text-xs text-amber-500 font-medium mb-0.5">Prosječna ocjena</p>
              <p className="text-2xl font-bold text-amber-700">
                {data.averageRating != null ? formatRating(data.averageRating) : "—"}
              </p>
            </div>
            <div className="bg-gray-50 rounded-xl px-4 py-3">
              <p className="text-xs text-gray-400 font-medium mb-0.5">Ocijenjenih tiketa</p>
              <p className="text-2xl font-bold text-gray-900">{data.ratedTicketsCount ?? 0}</p>
            </div>
            {data.distribution?.length > 0 && (
              <div className="bg-gray-50 rounded-xl px-4 py-3 col-span-2 sm:col-span-1">
                <p className="text-xs text-gray-400 font-medium mb-1">Distribucija</p>
                <div className="flex flex-wrap gap-x-3 gap-y-0.5">
                  {data.distribution.map((d) => (
                    <span key={d.stars} className="text-sm text-gray-600">
                      {d.stars}★ <span className="font-semibold text-gray-800">{d.count}</span>
                    </span>
                  ))}
                </div>
              </div>
            )}
          </div>
          {data.buckets?.length > 0 && (
            <div>
              {data.bucketGranularityLabel && (
                <p className="text-xs text-gray-400 mb-2">{data.bucketGranularityLabel}</p>
              )}
              <table className="w-full">
                <thead>
                  <tr className="border-b border-gray-100">
                    <th className={th}>Period</th>
                    <th className={th}>Prosj. ocjena</th>
                    <th className={th}>Broj</th>
                  </tr>
                </thead>
                <tbody>
                  {data.buckets.map((row) => (
                    <tr key={row.label} className={tr}>
                      <td className={td}>{row.label}</td>
                      <td className={td}>{row.avgRating != null ? formatRating(row.avgRating) : "—"}</td>
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
            <p className="text-sm text-amber-700 bg-amber-50 border border-amber-100 rounded-xl px-3 py-2">
              {reportResult.message}
            </p>
          )}
          <div className="grid grid-cols-2 gap-3">
            <div className="bg-blue-50 rounded-xl px-4 py-3">
              <p className="text-xs text-navy-400 font-medium mb-0.5">Prosj. 1. odgovor</p>
              <p className="text-2xl font-bold text-navy-800">
                {data.avgFirstResponseMinutes != null ? formatMinutes(data.avgFirstResponseMinutes) : "—"}
              </p>
            </div>
            <div className="bg-gray-50 rounded-xl px-4 py-3">
              <p className="text-xs text-gray-400 font-medium mb-0.5">S odgovorom / ukupno</p>
              <p className="text-2xl font-bold text-gray-900">
                {data.ticketsWithResponseCount ?? 0}
                <span className="text-base font-normal text-gray-400"> / {data.totalTicketsCount ?? 0}</span>
              </p>
            </div>
          </div>
          {data.buckets?.length > 0 ? (
            <div>
              {data.bucketGranularityLabel && (
                <p className="text-xs text-gray-400 mb-2">{data.bucketGranularityLabel}</p>
              )}
              <table className="w-full">
                <thead>
                  <tr className="border-b border-gray-100">
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
                        {row.avgFirstResponseMinutes != null ? formatMinutes(row.avgFirstResponseMinutes) : "—"}
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
            <p className="text-sm text-amber-700 bg-amber-50 border border-amber-100 rounded-xl px-3 py-2">
              {reportResult.message}
            </p>
          )}
          <div className="grid grid-cols-2 gap-3">
            <div className="bg-blue-50 rounded-xl px-4 py-3">
              <p className="text-xs text-navy-400 font-medium mb-0.5">Prosj. rješavanje</p>
              <p className="text-2xl font-bold text-navy-800">
                {data.avgResolutionHours != null ? formatHours(data.avgResolutionHours) : "—"}
              </p>
            </div>
            <div className="bg-gray-50 rounded-xl px-4 py-3">
              <p className="text-xs text-gray-400 font-medium mb-0.5">Zatvoreno / ukupno</p>
              <p className="text-2xl font-bold text-gray-900">
                {data.closedTicketsCount ?? 0}
                <span className="text-base font-normal text-gray-400"> / {data.totalTicketsCount ?? 0}</span>
              </p>
            </div>
          </div>
          {data.buckets?.length > 0 ? (
            <div>
              {data.bucketGranularityLabel && (
                <p className="text-xs text-gray-400 mb-2">{data.bucketGranularityLabel}</p>
              )}
              <table className="w-full">
                <thead>
                  <tr className="border-b border-gray-100">
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
                        {row.avgResolutionHours != null ? formatHours(row.avgResolutionHours) : "—"}
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

      {/* Period filter */}
      <div className="flex flex-wrap gap-2 items-center">
        <div className="flex gap-1 bg-white border border-gray-100 rounded-xl p-1 shadow-sm">
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
              className={`px-3 py-1.5 rounded-lg text-xs font-semibold transition-all ${
                period === opt.value
                  ? "bg-navy-800 text-white shadow-sm"
                  : "text-gray-500 hover:text-gray-700 hover:bg-slate-100"
              }`}
            >
              {opt.label}
            </button>
          ))}
        </div>

        {period === "custom" && (
          <div className="flex items-center gap-2 bg-white border border-gray-100 rounded-xl px-3 py-1.5 shadow-sm">
            <input
              type="date"
              value={customFrom}
              onChange={(e) => setCustomFrom(e.target.value)}
              className="text-xs text-gray-600 outline-none bg-transparent"
            />
            <span className="text-gray-300 text-xs">—</span>
            <input
              type="date"
              value={customTo}
              onChange={(e) => setCustomTo(e.target.value)}
              className="text-xs text-gray-600 outline-none bg-transparent"
            />
          </div>
        )}

        {(showMetrics || (showReports && reportType)) && (
          <button
            type="button"
            onClick={() => {
              if (!validatePeriod()) return;
              if (showMetrics) { setLoading(true); void loadDashboard(); }
              if (showReports && reportType) { void fetchReport(reportType); }
            }}
            className="px-4 py-1.5 bg-navy-800 text-white text-xs font-semibold rounded-xl hover:bg-navy-900 transition-colors shadow-sm"
          >
            Primijeni
          </button>
        )}
      </div>

      {periodError && (
        <p className="text-xs text-red-500 -mt-2">{periodError}</p>
      )}

      {showMetrics && error && (
        <div className="flex items-center gap-3 text-red-600 bg-red-50 border border-red-100 rounded-2xl p-4">
          <AlertCircle size={18} className="flex-shrink-0" />
          <span className="text-sm">{error}</span>
        </div>
      )}

      {showMetrics && loading && <SkeletonGrid />}

      {showMetrics && !loading && dashboard && (
        <div className="space-y-6">

          {/* Alert banner */}
          {!bannerDismissed && (dashboard.staleTicketsCount > 0 || dashboard.closureRequestedCount > 0) && (() => {
            const isStale = dashboard.staleTicketsCount > 0;
            const colors = isStale
              ? { wrap: "bg-red-50 border-red-100 hover:bg-red-100", icon: "text-red-500", title: "text-red-700", sub: "text-red-400", pill: "bg-red-100 text-red-700" }
              : { wrap: "bg-amber-50 border-amber-100 hover:bg-amber-100", icon: "text-amber-500", title: "text-amber-700", sub: "text-amber-400", pill: "bg-amber-100 text-amber-700" };
            return (
              <div className={`flex items-center gap-3 px-4 py-3 rounded-2xl border cursor-pointer transition-colors ${colors.wrap}`}
                onClick={() => drillDown(isStale ? { stale: "true", snapshot: "true" } : { status: "CLOSURE_REQUESTED", snapshot: "true" })}
              >
                <AlertCircle size={17} className={`${colors.icon} flex-shrink-0`} />
                <div className="flex-1 min-w-0">
                  <p className={`text-sm font-semibold ${colors.title}`}>
                    {isStale
                      ? `${dashboard.staleTicketsCount} zastarjelih tiketa zahtijeva pažnju`
                      : `${dashboard.closureRequestedCount} tiketa čeka odobrenje zatvaranja`
                    }
                  </p>
                  <p className={`text-xs mt-0.5 ${colors.sub}`}>
                    {isStale ? "Tiketi bez aktivnosti 7+ dana — klikni za pregled" : "Klijenti su zatražili zatvaranje — klikni za pregled"}
                  </p>
                </div>
                <span className={`text-[11px] font-semibold px-2.5 py-1 rounded-lg flex-shrink-0 ${colors.pill}`}>
                  Pregledaj →
                </span>
                <button
                  onClick={(e) => { e.stopPropagation(); setBannerDismissed(true); }}
                  className="p-1 text-gray-400 hover:text-gray-600 flex-shrink-0"
                >
                  <X size={14} />
                </button>
              </div>
            );
          })()}

          {/* KPI stat cards */}
          <div>
            <SectionTitle>Ključne metrike</SectionTitle>
            <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 gap-3">
              <StatCard
                icon={Ticket}
                label="Kreirani tiketi"
                value={dashboard.totalTicketsInPeriod}
                accent="blue"
                onClick={() => drillDown()}
                emptyMessage="Nema tiketa u periodu"
              />
              <StatCard
                icon={MessageSquare}
                label="Prosj. 1. odgovor"
                value={formatMinutes(dashboard.avgFirstResponseMinutes)}
                accent="violet"
                emptyMessage="Nema odgovora u periodu"
              />
              <StatCard
                icon={Clock}
                label="Prosj. rješavanje"
                value={formatHours(dashboard.avgResolutionHours)}
                description={dashboard.closedInPeriodCount != null ? `${dashboard.closedInPeriodCount} zatvorenih tiketa` : undefined}
                accent="sky"
                emptyMessage="Nema zatvorenih tiketa"
              />
              <StatCard
                icon={Star}
                label="Prosj. ocjena"
                value={formatRating(dashboard.avgRating)}
                accent="amber"
                emptyMessage="Nema ocjena u periodu"
              />
              <StatCard
                icon={CheckCircle}
                label="Otvoreni (trenutno)"
                value={dashboard.openTicketsCount}
                accent="emerald"
                onClick={() => drillDown({ status: "OPEN", snapshot: "true" })}
              />
              <StatCard
                icon={AlertCircle}
                label="Čeka zatvaranje"
                value={dashboard.closureRequestedCount}
                accent="amber"
                onClick={() => drillDown({ status: "CLOSURE_REQUESTED", snapshot: "true" })}
                trend={dashboard.closureRequestedCount > 0 ? { value: -1, label: "Čeka odobrenje" } : undefined}
              />
              <StatCard
                icon={TrendingUp}
                label="Zatvoreni"
                value={dashboard.closedInPeriodCount ?? 0}
                accent="teal"
                onClick={() => drillDown({ status: "CLOSED" })}
                emptyMessage="Nema zatvorenih u periodu"
                trend={
                  dashboard.closedInPeriodCount > 0 && dashboard.totalTicketsInPeriod > 0
                    ? { value: 1, label: `${Math.round((dashboard.closedInPeriodCount / dashboard.totalTicketsInPeriod) * 100)}% zatvoreno` }
                    : undefined
                }
              />
              <StatCard
                icon={Clock}
                label="Zastarjeli (7+ dana)"
                value={dashboard.staleTicketsCount}
                accent="red"
                onClick={() => drillDown({ stale: "true", snapshot: "true" })}
                trend={dashboard.staleTicketsCount > 0 ? { value: -1, label: "Zahtijeva pažnju" } : undefined}
              />
            </div>
          </div>

          {/* AI Insights panel — inline, ispod ključnih metrika */}
          {aiPanelOpen && (
            <div className="bg-white rounded-2xl border border-violet-200 shadow-sm overflow-hidden">
              <AIInsightsPanel
                dashboard={dashboard}
                onClose={closeAiPanel}
                onDrillDown={(extra) => drillDown(extra)}
              />
            </div>
          )}

          {/* Active users */}
          <div>
            <SectionTitle>Aktivni korisnici</SectionTitle>
            <div className="grid grid-cols-2 sm:grid-cols-4 gap-3">
              {[
                { label: "Klijenti",  value: dashboard.activeUsersByRole?.clients,        accent: "bg-navy-50 text-navy-700" },
                { label: "Agenti",    value: dashboard.activeUsersByRole?.agents,         accent: "bg-violet-50 text-violet-700" },
                { label: "Tehničari", value: dashboard.activeUsersByRole?.technicians,    accent: "bg-emerald-50 text-emerald-700" },
                { label: "Admini",    value: dashboard.activeUsersByRole?.administrators, accent: "bg-amber-50 text-amber-700" },
              ].map((item) => (
                <div
                  key={item.label}
                  className="bg-white rounded-2xl px-4 py-3.5 border border-slate-200 shadow-sm flex items-center gap-3"
                >
                  <div className={`w-9 h-9 rounded-xl flex items-center justify-center flex-shrink-0 ${item.accent.split(' ')[0]}`}>
                    <Users size={16} className={item.accent.split(' ')[1]} />
                  </div>
                  <div>
                    <p className="text-xs text-gray-400 font-medium">{item.label}</p>
                    <p className="text-lg font-bold text-gray-900 leading-none mt-0.5">{item.value ?? 0}</p>
                  </div>
                </div>
              ))}
            </div>
          </div>

          {/* Charts */}
          <div>
            <SectionTitle>Analiza tiketa</SectionTitle>
            <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-4">

              <div className="bg-white rounded-2xl p-5 shadow-sm border border-slate-200">
                <p className="text-sm font-semibold text-gray-700 mb-1">Po statusu</p>
                <p className="text-xs text-gray-400 mb-4">Raspodjela tiketa prema statusu</p>
                {statusChartData.length > 0 ? (
                  <ResponsiveContainer width="100%" height={200}>
                    <PieChart>
                      <Pie
                        data={statusChartData}
                        cx="50%"
                        cy="50%"
                        innerRadius={50}
                        outerRadius={78}
                        dataKey="value"
                        onClick={(_, index) => drillDown({ status: statusChartData[index]?.status })}
                        style={{ cursor: "pointer" }}
                        strokeWidth={0}
                      >
                        {statusChartData.map((entry) => (
                          <Cell key={entry.name} fill={entry.color} />
                        ))}
                      </Pie>
                      <Tooltip content={<ChartTooltip />} />
                      <Legend iconType="circle" iconSize={7} wrapperStyle={{ fontSize: "12px" }} />
                    </PieChart>
                  </ResponsiveContainer>
                ) : (
                  <p className="text-sm text-gray-300 italic py-8 text-center">Nema podataka.</p>
                )}
              </div>

              <div className="bg-white rounded-2xl p-5 shadow-sm border border-slate-200">
                <p className="text-sm font-semibold text-gray-700 mb-1">Top tipovi problema</p>
                <p className="text-xs text-gray-400 mb-4">Klikni na stupac za drill-down</p>
                {problemChartData.length > 0 ? (
                  <ResponsiveContainer width="100%" height={200}>
                    <BarChart data={problemChartData} barSize={22}>
                      <CartesianGrid strokeDasharray="3 3" stroke="#f1f5f9" vertical={false} />
                      <XAxis dataKey="name" tick={{ fontSize: 10, fill: "#94a3b8" }} axisLine={false} tickLine={false} />
                      <YAxis tick={{ fontSize: 10, fill: "#94a3b8" }} width={24} axisLine={false} tickLine={false} />
                      <Tooltip content={<ChartTooltip />} cursor={{ fill: "#f8fafc" }} />
                      <Bar
                        dataKey="value"
                        fill="#1f3d72"
                        radius={[6, 6, 0, 0]}
                        onClick={(data) => drillDown({ problemCategory: data.category })}
                        style={{ cursor: "pointer" }}
                      />
                    </BarChart>
                  </ResponsiveContainer>
                ) : (
                  <p className="text-sm text-gray-300 italic py-8 text-center">Nema podataka.</p>
                )}
              </div>

              <div className="bg-white rounded-2xl p-5 shadow-sm border border-slate-200">
                <p className="text-sm font-semibold text-gray-700 mb-1">Opterećenje agenata</p>
                <p className="text-xs text-gray-400 mb-4">Zatvoreni tiketi po agentu</p>
                {workloadChartData.length > 0 ? (
                  <ResponsiveContainer width="100%" height={200}>
                    <BarChart data={workloadChartData} barSize={22}>
                      <CartesianGrid strokeDasharray="3 3" stroke="#f1f5f9" vertical={false} />
                      <XAxis dataKey="name" tick={{ fontSize: 10, fill: "#94a3b8" }} axisLine={false} tickLine={false} />
                      <YAxis tick={{ fontSize: 10, fill: "#94a3b8" }} width={24} axisLine={false} tickLine={false} />
                      <Tooltip content={<ChartTooltip />} cursor={{ fill: "#f8fafc" }} />
                      <Bar dataKey="value" fill="#4c3b8c" radius={[6, 6, 0, 0]} />
                    </BarChart>
                  </ResponsiveContainer>
                ) : (
                  <p className="text-sm text-gray-300 italic py-8 text-center">Nema podataka.</p>
                )}
              </div>

            </div>
          </div>

          {/* Key highlights — derived from topProblemTypes */}
          {dashboard.topProblemTypes?.length > 0 && (
            <div>
              <SectionTitle>Ključni problemi</SectionTitle>
              <p className="text-xs text-gray-400 -mt-2 mb-3">Najčešći tipovi problema rangirani po broju tiketa</p>
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
                {dashboard.topProblemTypes.slice(0, 4).map((p, i) => {
                  const maxCount = dashboard.topProblemTypes[0]?.count ?? 1;
                  const ratio = p.count / maxCount;
                  const severity = ratio >= 0.7 ? { label: "Kritično", bg: "bg-red-50", text: "text-red-600", bar: "bg-red-400" }
                    : ratio >= 0.4 ? { label: "Visoko",   bg: "bg-amber-50", text: "text-amber-600", bar: "bg-amber-400" }
                    : { label: "Nisko",    bg: "bg-emerald-50", text: "text-emerald-600", bar: "bg-emerald-400" };
                  return (
                    <button
                      key={p.name}
                      type="button"
                      onClick={() => drillDown({ problemCategory: p.name })}
                      className="bg-white rounded-2xl px-4 py-4 border border-slate-200 shadow-sm text-left hover:shadow-md hover:-translate-y-0.5 transition-all"
                    >
                      <div className="flex items-start justify-between gap-3 mb-3">
                        <div>
                          <p className="text-sm font-semibold text-gray-900 leading-tight">
                            {PROBLEM_LABELS[p.name] ?? p.name}
                          </p>
                          <p className="text-xs text-gray-400 mt-0.5">{p.count} tiketa</p>
                        </div>
                        <span className={`text-[11px] font-semibold px-2 py-0.5 rounded-full flex-shrink-0 ${severity.bg} ${severity.text}`}>
                          {severity.label}
                        </span>
                      </div>
                      <div className="w-full bg-slate-100 rounded-full h-1.5">
                        <div
                          className={`h-1.5 rounded-full ${severity.bar} transition-all`}
                          style={{ width: `${Math.round(ratio * 100)}%` }}
                        />
                      </div>
                      <p className="text-[10px] text-gray-400 mt-1.5">
                        {i === 0 ? "Najčešći problem" : `#${i + 1} po učestalosti`}
                      </p>
                    </button>
                  );
                })}
              </div>
            </div>
          )}

        </div>
      )}

      {/* Reports section */}
      {showReports && (
        <div className="bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden">
          <div className="px-5 py-4 border-b border-gray-100 flex items-center justify-between">
            <div className="flex items-center gap-2.5">
              <div className="w-8 h-8 rounded-lg bg-navy-50 flex items-center justify-center">
                <BarChart2 size={15} className="text-navy-700" />
              </div>
              <div>
                <h3 className="text-sm font-semibold text-gray-900">Izvještaji</h3>
                <p className="text-xs text-gray-400">Odaberi tip i period za generisanje</p>
              </div>
            </div>
            <button
              type="button"
              disabled
              title="Export će biti dostupan u budućoj verziji"
              className="inline-flex items-center gap-2 px-3 py-1.5 text-xs font-medium text-gray-300 bg-gray-50 rounded-lg cursor-not-allowed border border-gray-100"
            >
              <Download size={13} />
              Export
            </button>
          </div>

          <div className="overflow-x-auto border-b border-gray-50">
            <div className="flex gap-2 px-5 py-3 min-w-max">
              {REPORT_TYPES.map((r) => (
                <button
                  key={r.value}
                  type="button"
                  onClick={() => handleSelectChip(r.value)}
                  className={`px-3.5 py-1.5 rounded-full text-xs font-semibold whitespace-nowrap transition-all ${
                    reportType === r.value
                      ? "bg-navy-800 text-white shadow-sm"
                      : "bg-slate-100 text-gray-500 hover:bg-slate-200 hover:text-gray-700"
                  }`}
                >
                  {r.label}
                </button>
              ))}
            </div>
          </div>

          <div className="p-5">
            {!reportType && (
              <div className="py-8 text-center">
                <BarChart2 size={28} className="mx-auto text-gray-200 mb-2" />
                <p className="text-sm text-gray-400">Odaberi tip izvještaja gore.</p>
              </div>
            )}
            {reportLoading && (
              <div className="flex items-center gap-2 text-gray-400 text-sm py-6">
                <Loader2 size={17} className="animate-spin" />
                Učitavanje...
              </div>
            )}
            {!reportLoading && reportType && (
              <>
                {reportResult?.showLargePeriodWarning && (
                  <p className="text-sm text-amber-700 bg-amber-50 border border-amber-100 rounded-xl px-3 py-2 mb-4">
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
