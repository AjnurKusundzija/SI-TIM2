import PropTypes from 'prop-types'
import { Link } from 'react-router-dom'
import {
  AlertTriangle,
  CheckCircle2,
  ChevronRight,
  HelpCircle,
  Lightbulb,
  Sparkles,
  Ticket as TicketIcon,
} from 'lucide-react'

// PB-70 / US-109, US-110, US-111 — prikaz strukturiranog odgovora Admin Copilota.
export default function AdminCopilotMessage({ data }) {
  if (!data) return null

  const {
    answer,
    metrics = [],
    recommendations = [],
    sources = [],
    usedTools = [],
    relatedTickets = [],
    faqCoverage = [],
    message,
  } = data

  return (
    <div className="bg-white border border-slate-200 rounded-xl px-4 py-3.5 space-y-3.5 shadow-sm">
      {/* Narativ */}
      {answer && (
        <div className="flex items-start gap-2">
          <div className="w-7 h-7 rounded-lg bg-navy-50 flex items-center justify-center flex-shrink-0">
            <Sparkles size={14} className="text-navy-700" />
          </div>
          <p className="text-sm text-gray-700 leading-relaxed whitespace-pre-wrap flex-1">{answer}</p>
        </div>
      )}

      {/* Napomena (preciziranje / parcijalni podaci) */}
      {message && (
        <div className="flex items-start gap-2 text-sm text-amber-700 bg-amber-50 border border-amber-100 rounded-lg px-3 py-2">
          <HelpCircle size={14} className="mt-0.5 shrink-0 text-amber-500" />
          <span>{message}</span>
        </div>
      )}

      {/* Ključne metrike */}
      {metrics.length > 0 && (
        <div>
          <p className="text-xs font-semibold text-gray-500 uppercase tracking-wide mb-2">Ključne metrike</p>
          <div className="grid grid-cols-2 sm:grid-cols-3 gap-2">
            {metrics.map((m, i) => (
              <div key={i} className="bg-slate-50 border border-slate-100 rounded-lg px-3 py-2">
                <p className="text-[11px] text-gray-400 font-medium leading-tight">{m.label}</p>
                <p className="text-sm font-bold text-gray-900 mt-0.5">{m.value}</p>
                {m.hint && <p className="text-[10px] text-gray-400 mt-0.5">{m.hint}</p>}
              </div>
            ))}
          </div>
        </div>
      )}

      {/* US-110 — relevantni tiketi */}
      {relatedTickets.length > 0 && (
        <div>
          <p className="text-xs font-semibold text-gray-500 uppercase tracking-wide mb-2">Relevantni tiketi</p>
          <ul className="space-y-1.5">
            {relatedTickets.map((t) => (
              <li key={t.ticketId}>
                <Link
                  to={`/tickets/${t.ticketId}`}
                  className="flex items-center gap-2 bg-slate-50 hover:bg-navy-50 border border-slate-100 rounded-lg px-3 py-2 transition-colors group"
                >
                  <TicketIcon size={13} className="text-navy-600 shrink-0" />
                  <span className="text-sm text-gray-700 truncate flex-1">
                    #{t.ticketId} {t.title}
                  </span>
                  {t.minutesWithoutResponse != null && (
                    <span className="text-[10px] font-semibold text-red-600 bg-red-50 px-1.5 py-0.5 rounded-full shrink-0">
                      {t.minutesWithoutResponse} min bez odg.
                    </span>
                  )}
                  <ChevronRight size={12} className="text-gray-300 group-hover:text-navy-500 shrink-0" />
                </Link>
              </li>
            ))}
          </ul>
        </div>
      )}

      {/* US-111 — FAQ pokrivenost ponavljanih problema */}
      {faqCoverage.length > 0 && (
        <div>
          <p className="text-xs font-semibold text-gray-500 uppercase tracking-wide mb-2">
            Ponavljani problemi i FAQ pokrivenost
          </p>
          <ul className="space-y-2">
            {faqCoverage.map((fc, i) => (
              <li key={i} className="bg-slate-50 border border-slate-100 rounded-lg px-3 py-2.5">
                <div className="flex items-center justify-between gap-2">
                  <span className="text-sm font-medium text-gray-800 truncate">
                    {fc.problem}
                    <span className="text-xs text-gray-400 font-normal"> · {fc.occurrenceCount} tiketa</span>
                  </span>
                  {fc.covered ? (
                    <span className="text-[10px] font-semibold text-emerald-700 bg-emerald-50 px-2 py-0.5 rounded-full shrink-0 inline-flex items-center gap-1">
                      <CheckCircle2 size={10} /> FAQ postoji
                    </span>
                  ) : (
                    <span className="text-[10px] font-semibold text-amber-700 bg-amber-50 px-2 py-0.5 rounded-full shrink-0 inline-flex items-center gap-1">
                      <AlertTriangle size={10} /> Nije pokriveno
                    </span>
                  )}
                </div>
                {fc.covered && fc.matchedFaqQuestion && (
                  <p className="text-xs text-gray-500 mt-1">Postojeći FAQ: „{fc.matchedFaqQuestion}”</p>
                )}
                {!fc.covered && fc.suggestedQuestion && (
                  <div className="mt-1.5 text-xs text-gray-600 bg-white border border-amber-100 rounded-md px-2.5 py-2">
                    <p className="font-medium text-gray-700">Prijedlog FAQ pitanja:</p>
                    <p className="mt-0.5">{fc.suggestedQuestion}</p>
                    {fc.suggestedAnswer && <p className="mt-1 text-gray-500">{fc.suggestedAnswer}</p>}
                  </div>
                )}
              </li>
            ))}
          </ul>
        </div>
      )}

      {/* Preporuke */}
      {recommendations.length > 0 && (
        <div>
          <p className="text-xs font-semibold text-gray-500 uppercase tracking-wide mb-2 flex items-center gap-1.5">
            <Lightbulb size={11} className="text-navy-500" />
            Preporuke
          </p>
          <ul className="space-y-2">
            {recommendations.map((r, i) => (
              <li key={i} className="flex items-start gap-2 bg-slate-50 border border-slate-100 rounded-lg px-3 py-2.5">
                <Lightbulb size={13} className="text-navy-500 mt-0.5 shrink-0" />
                <div className="flex-1 min-w-0">
                  <p className="text-sm font-medium text-gray-800">{r.title}</p>
                  <p className="text-xs text-gray-500 mt-0.5">{r.description}</p>
                  {r.teamFilter && (
                    <Link
                      to={`/tickets?teamId=${r.teamFilter}&status=OPEN&snapshot=true`}
                      className="mt-1.5 inline-flex items-center gap-1 text-xs text-navy-600 hover:text-navy-800 font-medium"
                    >
                      Pogledaj tikete tima
                      <ChevronRight size={11} />
                    </Link>
                  )}
                </div>
              </li>
            ))}
          </ul>
        </div>
      )}

      {/* US-109 — korišteni izvori / MCP alati */}
      {(sources.length > 0 || usedTools.length > 0) && (
        <div className="pt-1 border-t border-slate-100">
          <p className="text-[11px] text-gray-400">
            <span className="font-semibold">Izvori (MCP alati):</span>{' '}
            {(sources.length > 0 ? sources.map((s) => s.tool) : usedTools).join(', ')}
          </p>
        </div>
      )}
    </div>
  )
}

AdminCopilotMessage.propTypes = {
  data: PropTypes.object,
}
