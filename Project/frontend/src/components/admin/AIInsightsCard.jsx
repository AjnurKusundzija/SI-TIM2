import { useState, useEffect } from 'react'
import PropTypes from 'prop-types'
import { AlertTriangle, Bot, ChevronRight, Lightbulb, Loader2, RefreshCw } from 'lucide-react'
import { getAdminInsights } from '../../services/aiService'

// PB-58 / US-98, US-99
export default function AIInsightsCard({ dashboard, onDrillDown }) {
  const [insights, setInsights] = useState(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState(null)

  useEffect(() => {
    function reset() {
      setInsights(null)
      setError(null)
    }
    reset()
  }, [dashboard])

  async function fetchInsights() {
    setLoading(true)
    setError(null)
    try {
      const data = await getAdminInsights(dashboard)
      setInsights(data)
    } catch (err) {
      setError(
        err.response?.data?.message ??
          'AI servis trenutno nije dostupan. Pokušajte ponovo.'
      )
    } finally {
      setLoading(false)
    }
  }

  const CATEGORY_LABELS = {
    INTERNET: 'Internet',
    TV: 'TV',
    MOBILE_NETWORK: 'Mobilna mreža',
    BILLING: 'Računi',
    TECHNICAL_SUPPORT: 'Tehnička podrška',
  }

  return (
    <div className="bg-white rounded-xl border border-violet-100 shadow-sm overflow-hidden">
      {/* Header */}
      <div className="px-5 py-4 border-b border-violet-50 flex items-center justify-between bg-gradient-to-r from-violet-50/60 to-white">
        <div className="flex items-center gap-2">
          <div className="w-8 h-8 rounded-lg bg-violet-100 flex items-center justify-center">
            <Bot size={16} className="text-violet-600" />
          </div>
          <div>
            <p className="text-sm font-semibold text-gray-900">AI Uvidi</p>
            <p className="text-xs text-gray-400">Analiza metrika uz pomoć Gemini AI</p>
          </div>
        </div>
        <button
          type="button"
          onClick={fetchInsights}
          disabled={loading}
          className="inline-flex items-center gap-1.5 px-3 py-1.5 text-sm font-medium text-violet-600 border border-violet-200 hover:bg-violet-50 disabled:opacity-50 disabled:cursor-not-allowed rounded-lg transition-colors"
        >
          {loading ? (
            <Loader2 size={14} className="animate-spin" />
          ) : (
            <RefreshCw size={14} />
          )}
          {insights ? 'Osvježi' : 'Generiši uvide'}
        </button>
      </div>

      {/* Body */}
      <div className="px-5 py-4 space-y-4">
        {/* Empty state */}
        {!insights && !loading && !error && (
          <div className="flex flex-col items-center justify-center py-6 gap-2 text-center">
            <Bot size={28} className="text-violet-200" />
            <p className="text-sm text-gray-400">
              Klikni "Generiši uvide" da dobiješ AI analizu trenutnih metrika.
            </p>
          </div>
        )}

        {/* Loading */}
        {loading && (
          <div className="flex items-center justify-center gap-2 py-6 text-gray-400">
            <Loader2 size={18} className="animate-spin text-violet-500" />
            <span className="text-sm">AI analizira metrike…</span>
          </div>
        )}

        {/* Error */}
        {error && !loading && (
          <div className="flex items-start gap-2 text-sm text-red-600 bg-red-50 border border-red-100 rounded-lg px-3 py-2.5">
            <AlertTriangle size={15} className="mt-0.5 shrink-0" />
            <span>{error}</span>
          </div>
        )}

        {/* Results */}
        {insights && !loading && (
          <>
            {/* Narrative */}
            {insights.narrative && (
              <div className="text-sm text-gray-700 leading-relaxed bg-violet-50/50 rounded-xl px-4 py-3 border border-violet-100">
                {insights.narrative}
              </div>
            )}

            {/* Anomalies */}
            {insights.anomalies?.length > 0 && (
              <div>
                <p className="text-xs font-semibold text-gray-500 uppercase tracking-wide mb-2 flex items-center gap-1.5">
                  <AlertTriangle size={12} className="text-amber-500" />
                  Anomalije
                </p>
                <ul className="space-y-2">
                  {insights.anomalies.map((a, i) => (
                    <li
                      key={i}
                      className="flex items-start gap-2.5 bg-amber-50 border border-amber-100 rounded-lg px-3 py-2.5"
                    >
                      <AlertTriangle size={14} className="text-amber-500 mt-0.5 shrink-0" />
                      <div>
                        <p className="text-sm font-medium text-gray-800">{a.title}</p>
                        <p className="text-xs text-gray-500 mt-0.5">{a.description}</p>
                      </div>
                    </li>
                  ))}
                </ul>
              </div>
            )}

            {insights.anomalies?.length === 0 && (
              <p className="text-xs text-gray-400 italic flex items-center gap-1.5">
                <AlertTriangle size={12} className="text-gray-300" />
                Nema detektovanih anomalija.
              </p>
            )}

            {/* Recommendations */}
            {insights.recommendations?.length > 0 && (
              <div>
                <p className="text-xs font-semibold text-gray-500 uppercase tracking-wide mb-2 flex items-center gap-1.5">
                  <Lightbulb size={12} className="text-violet-500" />
                  Preporuke
                </p>
                <ul className="space-y-2">
                  {insights.recommendations.map((r, i) => (
                    <li key={i} className="flex items-start gap-2.5 bg-gray-50 border border-gray-100 rounded-lg px-3 py-2.5">
                      <Lightbulb size={14} className="text-violet-500 mt-0.5 shrink-0" />
                      <div className="flex-1 min-w-0">
                        <p className="text-sm font-medium text-gray-800">{r.title}</p>
                        <p className="text-xs text-gray-500 mt-0.5">{r.description}</p>
                        {r.teamFilter && onDrillDown && (
                          <button
                            type="button"
                            onClick={() => onDrillDown({ type: r.teamFilter })}
                            className="mt-1.5 inline-flex items-center gap-1 text-xs text-violet-600 hover:text-violet-800 font-medium"
                          >
                            Pogledaj {CATEGORY_LABELS[r.teamFilter] ?? r.teamFilter} tikete
                            <ChevronRight size={11} />
                          </button>
                        )}
                      </div>
                    </li>
                  ))}
                </ul>
              </div>
            )}
          </>
        )}
      </div>
    </div>
  )
}

AIInsightsCard.propTypes = {
  dashboard: PropTypes.object.isRequired,
  onDrillDown: PropTypes.func,
}
