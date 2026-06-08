import PropTypes from "prop-types";
import { useState, useEffect, useMemo } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { getAllTickets } from "../services/ticketService";
import { useAuth } from "../context/AuthContext";
import { Search, Ticket } from "lucide-react";
import { formatDateOnly } from "../utils/formatDate";
import EmptyState from "../components/common/EmptyState";
import Badge from "../components/common/Badge";
import SlaIndicator from "../components/common/SlaIndicator";
import TicketPreviewPanel from "../components/tickets/TicketPreviewPanel";

const STATUS_LABELS = {
  OPEN: "Otvoren",
  CLOSED: "Zatvoren",
  CLOSURE_REQUESTED: "Čeka se",
};
const PRIORITY_LABELS = { LOW: "Nizak", MEDIUM: "Srednji", HIGH: "Visok", CRITICAL: "Kritičan" };
const INTERNAL_PRIORITY_LABELS = {
  LOW: "Nizak",
  MEDIUM: "Srednji",
  HIGH: "Visok",
  CRITICAL: "Kritičan",
};
const TYPE_LABELS = {
  INTERNET: "Internet",
  TV: "TV",
  MOBILE_NETWORK: "Mobilna mreža",
  BILLING: "Računi/Naplata",
  TECHNICAL_SUPPORT: "Tehnička podrška",
};

function SkeletonTable({ hasInternalPriority }) {
  const colCount = hasInternalPriority ? 7 : 6;
  return (
    <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden animate-pulse">
      <div className="hidden md:block">
        <div className="flex gap-4 px-5 py-3 border-b border-gray-100 bg-gray-50">
          {Array.from({ length: colCount }).map((_, i) => (
            <div
              key={i}
              className="h-3 bg-gray-200 rounded"
              style={{ width: i === 0 ? 180 : 80 }}
            />
          ))}
        </div>
        {Array.from({ length: 5 }).map((_, i) => (
          <div
            key={i}
            className="flex items-center gap-4 px-5 py-3.5 border-b border-gray-50 last:border-0"
          >
            <div className="flex-1">
              <div className="h-3.5 bg-gray-200 rounded w-52 mb-1.5" />
              <div className="h-2.5 bg-gray-100 rounded w-16" />
            </div>
            <div className="h-5 bg-gray-200 rounded-full w-20" />
            <div className="h-5 bg-gray-200 rounded-full w-14" />
            {hasInternalPriority && (
              <div className="h-5 bg-gray-200 rounded-full w-16" />
            )}
            <div className="h-3.5 bg-gray-100 rounded w-20" />
            <div className="h-3 bg-gray-100 rounded w-20" />
          </div>
        ))}
      </div>
      <div className="md:hidden divide-y divide-gray-50">
        {Array.from({ length: 4 }).map((_, i) => (
          <div key={i} className="px-4 py-3">
            <div className="h-3.5 bg-gray-200 rounded w-48 mb-2" />
            <div className="flex gap-2">
              <div className="h-5 bg-gray-200 rounded-full w-16" />
              <div className="h-5 bg-gray-200 rounded-full w-14" />
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}

const STALE_DAYS = 7;

export default function Tickets({ assignedOnly = false }) {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const { user } = useAuth();
  const [now] = useState(() => Date.now());
  const isStaff =
    user?.role === "AGENT" ||
    user?.role === "ADMINISTRATOR" ||
    user?.role === "TECHNICIAN";

  const drillStatus = searchParams.get("status");
  const drillCategory = searchParams.get("problemCategory");
  const drillFrom = searchParams.get("from");
  const drillTo = searchParams.get("to");
  const drillSnapshot = searchParams.get("snapshot") === "true";
  const drillUnassigned = searchParams.get("unassigned") === "true";
  const drillStale = searchParams.get("stale") === "true";
  const drillSlaBreached = searchParams.get("slaBreached") === "true";
  const hasDrillDown = !!(
    drillStatus ||
    drillCategory ||
    drillFrom ||
    drillUnassigned ||
    drillStale ||
    drillSlaBreached
  );

  const [tickets, setTickets] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState(drillStatus ?? "ALL");
  const [typeFilter, setTypeFilter] = useState(drillCategory ?? "ALL");
  const [priorityFilter, setPriorityFilter] = useState("ALL");

  // Preview drawer — staff only; toggle on same row click
  const [selectedTicketId, setSelectedTicketId] = useState(null);

  const handleRowClick = (ticketId) => {
    if (isStaff) {
      setSelectedTicketId((prev) => (prev === ticketId ? null : ticketId));
    } else {
      navigate(`/tickets/${ticketId}`);
    }
  };

  // Close drawer on Escape
  useEffect(() => {
    const onKey = (e) => { if (e.key === "Escape") setSelectedTicketId(null); };
    document.addEventListener("keydown", onKey);
    return () => document.removeEventListener("keydown", onKey);
  }, []);

  useEffect(() => {
    getAllTickets(assignedOnly)
      .then((data) => {
        setError(null);
        setTickets(data);
      })
      .catch((err) => {
        console.error(err);
        setError("Greška pri učitavanju tiketa.");
      })
      .finally(() => setLoading(false));
  }, [assignedOnly]);

  const filtered = useMemo(() => {
    const fromMs =
      drillFrom && !drillSnapshot ? new Date(drillFrom).getTime() : null;
    const toMs = drillTo && !drillSnapshot ? new Date(drillTo).getTime() : null;
    const staleBefore = now - STALE_DAYS * 86400000;

    return tickets.filter((t) => {
      if (statusFilter !== "ALL" && t.status !== statusFilter) return false;
      if (typeFilter !== "ALL" && t.problemCategory !== typeFilter)
        return false;
      if (priorityFilter !== "ALL" && t.priority !== priorityFilter)
        return false;
      if (fromMs != null || toMs != null) {
        const created = new Date(t.createdDate).getTime();
        if (fromMs != null && created < fromMs) return false;
        if (toMs != null && created > toMs) return false;
      }
      if (drillUnassigned && (t.status !== "OPEN" || t.hasAssignment))
        return false;
      if (drillStale) {
        const created = new Date(t.createdDate).getTime();
        const isStaleStatus =
          t.status === "OPEN" || t.status === "CLOSURE_REQUESTED";
        if (!isStaleStatus || created > staleBefore) return false;
      }
      if (drillSlaBreached && !t.slaIsBreached) return false;
      if (search) {
        const s = search.toLowerCase();
        if (
          !t.title?.toLowerCase().includes(s) &&
          !t.subject?.toLowerCase().includes(s)
        )
          return false;
      }
      return true;
    });
  }, [
    tickets,
    search,
    statusFilter,
    typeFilter,
    priorityFilter,
    drillFrom,
    drillTo,
    drillSnapshot,
    drillUnassigned,
    drillStale,
    drillSlaBreached,
    now,
  ]);

  return (
    <div className="space-y-4">
      {hasDrillDown && (
        <div className="flex flex-wrap items-center justify-between gap-2 bg-navy-50 border border-navy-100 rounded-lg px-4 py-3 text-sm text-navy-800">
          <span>Prikaz filtriran iz admin dashboarda.</span>
          <button
            type="button"
            onClick={() => navigate("/tickets")}
            className="text-navy-600 font-medium hover:underline"
          >
            Ukloni filtere
          </button>
        </div>
      )}

      {/* Toolbar */}
      <div className="flex flex-col sm:flex-row gap-3 items-start sm:items-center justify-between">
        <div className="relative flex-1 w-full sm:max-w-xs">
          <Search
            size={16}
            className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400"
          />
          <input
            type="text"
            placeholder="Pretraži tikete..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            className="w-full pl-9 pr-3 py-2 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-navy-500 focus:border-navy-500 outline-none"
          />
        </div>
        <div className="flex flex-wrap gap-2">
          <select
            value={statusFilter}
            onChange={(e) => setStatusFilter(e.target.value)}
            className="px-3 py-2 border border-gray-300 rounded-lg text-sm bg-white outline-none focus:ring-2 focus:ring-navy-500"
          >
            <option value="ALL">Svi statusi</option>
            {Object.entries(STATUS_LABELS).map(([v, l]) => (
              <option key={v} value={v}>
                {l}
              </option>
            ))}
          </select>
          <select
            value={typeFilter}
            onChange={(e) => setTypeFilter(e.target.value)}
            className="px-3 py-2 border border-gray-300 rounded-lg text-sm bg-white outline-none focus:ring-2 focus:ring-navy-500"
          >
            <option value="ALL">Svi tipovi</option>
            {Object.entries(TYPE_LABELS).map(([v, l]) => (
              <option key={v} value={v}>
                {l}
              </option>
            ))}
          </select>
          <select
            value={priorityFilter}
            onChange={(e) => setPriorityFilter(e.target.value)}
            className="px-3 py-2 border border-gray-300 rounded-lg text-sm bg-white outline-none focus:ring-2 focus:ring-navy-500"
          >
            <option value="ALL">Svi prioriteti</option>
            {Object.entries(PRIORITY_LABELS).map(([v, l]) => (
              <option key={v} value={v}>
                {l}
              </option>
            ))}
          </select>
        </div>
      </div>

      {/* Flex row: table shrinks, panel appears on the right */}
      <div className="flex gap-4 items-start">

      {/* ── Table column ── */}
      <div className="flex-1 min-w-0">
      {loading ? (
        <SkeletonTable hasInternalPriority={isStaff} />
      ) : error ? (
        <div className="p-4 bg-red-50 border border-red-200 rounded-lg text-sm text-red-700">
          {error}
        </div>
      ) : filtered.length === 0 ? (
        <EmptyState
          icon={Ticket}
          title="Nema pronađenih tiketa"
          description="Nijedan tiket ne odgovara vašim filterima."
        />
      ) : (
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
          <div className="hidden md:block overflow-x-auto">
            <table className="w-full">
              <thead>
                <tr className="border-b border-gray-100 bg-gray-50">
                  <th className="text-left px-5 py-3 text-xs font-medium text-gray-500 uppercase">
                    Tiket
                  </th>
                  <th className="text-left px-5 py-3 text-xs font-medium text-gray-500 uppercase">
                    Status
                  </th>
                  <th className="text-left px-5 py-3 text-xs font-medium text-gray-500 uppercase">
                    Prioritet
                  </th>
                  {isStaff && (
                    <th className="text-left px-5 py-3 text-xs font-medium text-gray-500 uppercase">
                      Interni Prioritet
                    </th>
                  )}
                  {isStaff && (
                    <th className="text-left px-5 py-3 text-xs font-medium text-gray-500 uppercase">
                      SLA
                    </th>
                  )}
                  <th className="text-left px-5 py-3 text-xs font-medium text-gray-500 uppercase">
                    Tip
                  </th>
                  <th className="text-left px-5 py-3 text-xs font-medium text-gray-500 uppercase">
                    Kreirano
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-50">
                {filtered.map((t) => {
                  const isSelected = t.ticketId === selectedTicketId;
                  return (
                    <tr
                      key={t.ticketId}
                      onClick={() => handleRowClick(t.ticketId)}
                      className={`transition-colors cursor-pointer ${
                        isSelected
                          ? "bg-blue-50/60 border-l-2 border-l-navy-500"
                          : "hover:bg-gray-50 border-l-2 border-l-transparent"
                      }`}
                    >
                      <td className={`py-3 ${isSelected ? "pl-[14px] pr-5" : "px-5"}`}>
                        <p className="text-sm font-medium text-gray-900 truncate max-w-xs">
                          {t.title || t.subject}
                        </p>
                        <p className="text-xs text-gray-400">{t.ticketId}</p>
                      </td>
                      <td className="px-5 py-3">
                        <Badge value={t.status} />
                      </td>
                      <td className="px-5 py-3">
                        <Badge value={t.priority} />
                      </td>
                      {isStaff && (
                        <td className="px-5 py-3">
                          {t.internalPriority ? (
                            <Badge value={t.internalPriority} />
                          ) : (
                            <span className="text-xs text-gray-400 italic">—</span>
                          )}
                        </td>
                      )}
                      {isStaff && (
                        <td className="px-5 py-3">
                          <SlaIndicator
                            slaStatus={t.slaStatus}
                            slaRemainingMinutes={t.slaRemainingMinutes}
                            slaIsBreached={t.slaIsBreached}
                            compact
                          />
                        </td>
                      )}
                      <td className="px-5 py-3 text-sm text-gray-600">
                        {TYPE_LABELS[t.problemCategory] ?? t.problemCategory}
                      </td>
                      <td className="px-5 py-3 text-sm text-gray-500 whitespace-nowrap">
                        {t.createdDate ? formatDateOnly(t.createdDate) : "—"}
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>

          <div className="md:hidden divide-y divide-gray-50">
            {filtered.map((t) => {
              const isSelected = t.ticketId === selectedTicketId;
              return (
                <div
                  key={t.ticketId}
                  onClick={() => handleRowClick(t.ticketId)}
                  className={`px-4 py-3 cursor-pointer transition-colors ${
                    isSelected ? "bg-blue-50/60" : "hover:bg-gray-50"
                  }`}
                >
                  <p className="text-sm font-medium text-gray-900 truncate">
                    {t.title || t.subject}
                  </p>
                  <div className="flex flex-wrap items-center gap-2 mt-2">
                    <Badge value={t.status} />
                    <Badge value={t.priority} />
                    {isStaff && t.internalPriority && (
                      <Badge value={t.internalPriority} />
                    )}
                    {isStaff && t.slaStatus && (
                      <SlaIndicator
                        slaStatus={t.slaStatus}
                        slaRemainingMinutes={t.slaRemainingMinutes}
                        slaIsBreached={t.slaIsBreached}
                        compact
                      />
                    )}
                  </div>
                  <p className="text-xs text-gray-400 mt-1">
                    {t.createdDate ? formatDateOnly(t.createdDate) : "—"}
                  </p>
                </div>
              );
            })}
          </div>
        </div>
      )}
      </div>{/* end table column */}

      {/*
       * ── Preview panel ──
       * Desktop: sticky column on the right. The explicit height
       * (viewport minus header 53px and main padding 24px top + 24px bottom)
       * gives TicketPreviewPanel a defined h-full so its inner flex-1
       * overflow-y-auto keeps the header/footer pinned while only the
       * conversation scrolls.
       * Mobile: full-screen fixed overlay.
       */}
      {isStaff && selectedTicketId !== null && (
        <>
          {/* Desktop sticky panel */}
          <div
            className="hidden lg:flex flex-col flex-shrink-0 w-[440px] sticky self-start
                       bg-white border border-gray-200 rounded-xl overflow-hidden shadow-sm"
            style={{ top: '69px', height: 'calc(100vh - 85px)' }}
          >
            <TicketPreviewPanel
              ticketId={selectedTicketId}
              onClose={() => setSelectedTicketId(null)}
            />
          </div>

          {/* Mobile fixed overlay */}
          <div
            className="lg:hidden fixed inset-x-0 bottom-0 top-[53px] z-30
                       bg-white border-t border-gray-200 flex flex-col shadow-2xl"
          >
            <TicketPreviewPanel
              ticketId={selectedTicketId}
              onClose={() => setSelectedTicketId(null)}
            />
          </div>
          <div
            className="lg:hidden fixed inset-0 z-20 bg-black/20"
            onClick={() => setSelectedTicketId(null)}
          />
        </>
      )}

      </div>{/* end flex row */}
    </div>
  );
}

Tickets.propTypes = {
  assignedOnly: PropTypes.bool,
};

SkeletonTable.propTypes = {
  hasInternalPriority: PropTypes.bool,
};
