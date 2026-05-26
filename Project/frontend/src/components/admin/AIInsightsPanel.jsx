import { useState, useEffect, useCallback } from 'react'
import PropTypes from 'prop-types'
import { AlertTriangle, Bot, ChevronRight, Lightbulb, Loader2, RefreshCw, X } from 'lucide-react'
import { getAdminInsights } from '../../services/aiService'

const CATEGORY_LABELS = {
  INTERNET: 'Internet',
  TV: 'TV',
  MOBILE_NETWORK: 'Mobilna mreža',
  BILLING: 'Računi',
  TECHNICAL_SUPPORT: 'Tehnička podrška',
}

export default function AIInsightsPanel({ onClose, dashboard, onDrillDown }) {
  const [insights, setInsights] = useState(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState(null)

  useEffect(() => {
    setInsights(null)
    setError(null)
  }, [dashboard])

  useEffect(() => {
    function handleKey(e) {
      if (e.key === 'Escape') onClose()
    }
    document.addEventListener('keydown', handleKey)
    return () => document.removeEventListener('keydown', handleKey)
  }, [onClose])

  const fetchInsights = useCallback(async () => {
    setLoading(true)
    setError(null)
    try {
      const data = await getAdminInsights(dashboard)
      setInsights(data)
    } catch (err) {
      setError(
        err.response?.data?.message ?? 'AI servis trenutno nije dostupan. Pokušajte ponovo.'
      )
    } finally {
      setLoading(false)
    }
  }, [dashboard])

  return (
    <div className="flex flex-col h-full">
      {/* Header */}
      <div className="flex items-center justify-between px-4 py-3 border-b border-violet-100 bg-gradient-to-r from-violet-50/80 to-white flex-shrink-0">
        <div className="flex items-center gap-2">
          <div className="w-7 h-7 rounded-lg bg-violet-100 flex items-center justify-center">
            <Bot size={14} className="text-violet-600" />
          </div>
          <div>
            <p className="text-sm font-semibold text-gray-900">AI Uvidi</p>
            <p className="text-xs text-gray-400">Gemini AI analiza</p>
          </div>
        </div>
        <div className="flex items-center gap-1.5">
          <button
            type="button"
            onClick={fetchInsights}
            disabled={loading}
            className="inline-flex items-center gap-1.5 px-2.5 py-1.5 text-xs font-medium text-violet-600 border border-violet-200 hover:bg-violet-50 disabled:opacity-50 disabled:cursor-not-allowed rounded-lg transition-colors"
          >
            {loading ? <Loader2 size={12} className="animate-spin" /> : <RefreshCw size={12} />}
            {insights ? 'Osvježi' : 'Generiši'}
          </button>
          <button
            type="button"
            onClick={onClose}
            className="p-1.5 text-gray-400 hover:text-gray-600 rounded-lg hover:bg-gray-100"
          >
            <X size={16} />
          </button>
        </div>
      </div>

      {/* Scrollable content */}
      <div className="flex-1 overflow-y-auto px-4 py-4 space-y-3">
        {!insights && !loading && !error && (
          <div className="flex flex-col items-center justify-center py-10 gap-3 text-center">
            <div className="w-12 h-12 rounded-2xl bg-violet-50 flex items-center justify-center">
              <Bot size={24} className="text-violet-300" />
            </div>
            <p className="text-sm text-gray-400 max-w-[220px]">
              Klikni "Generiši" da dobiješ AI analizu trenutnih metrika.
            </p>
          </div>
        )}

        {loading && (
          <div className="flex items-center justify-center gap-2 py-10 text-gray-400">
            <Loader2 size={18} className="animate-spin text-violet-500" />
            <span className="text-sm">AI analizira metrike…</span>
          </div>
        )}

        {error && !loading && (
          <div className="flex items-start gap-2 text-sm text-red-600 bg-red-50 border border-red-100 rounded-lg px-3 py-2.5">
            <AlertTriangle size={14} className="mt-0.5 shrink-0" />
            <span>{error}</span>
          </div>
        )}

        {insights && !loading && (
          <>
            {insights.narrative && (
              <div className="text-sm text-gray-700 leading-relaxed bg-violet-50/50 rounded-xl px-3 py-3 border border-violet-100">
                {insights.narrative}
              </div>
            )}

            {insights.anomalies?.length > 0 && (
              <div>
                <p className="text-xs font-semibold text-gray-500 uppercase tracking-wide mb-2 flex items-center gap-1.5">
                  <AlertTriangle size={11} className="text-amber-500" />
                  Anomalije
                </p>
                <ul className="space-y-2">
                  {insights.anomalies.map((a, i) => (
                    <li key={i} className="flex items-start gap-2 bg-amber-50 border border-amber-100 rounded-lg px-3 py-2.5">
                      <AlertTriangle size={13} className="text-amber-500 mt-0.5 shrink-0" />
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
                <AlertTriangle size={11} className="text-gray-300" />
                Nema detektovanih anomalija.
              </p>
            )}

            {insights.recommendations?.length > 0 && (
              <div>
                <p className="text-xs font-semibold text-gray-500 uppercase tracking-wide mb-2 flex items-center gap-1.5">
                  <Lightbulb size={11} className="text-violet-500" />
                  Preporuke
                </p>
                <ul className="space-y-2">
                  {insights.recommendations.map((r, i) => (
                    <li key={i} className="flex items-start gap-2 bg-gray-50 border border-gray-100 rounded-lg px-3 py-2.5">
                      <Lightbulb size={13} className="text-violet-500 mt-0.5 shrink-0" />
                      <div className="flex-1 min-w-0">
                        <p className="text-sm font-medium text-gray-800">{r.title}</p>
                        <p className="text-xs text-gray-500 mt-0.5">{r.description}</p>
                        {r.teamFilter && onDrillDown && (
                          <button
                            type="button"
                            onClick={() => { onDrillDown({ type: r.teamFilter }); onClose() }}
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

AIInsightsPanel.propTypes = {
  onClose: PropTypes.func.isRequired,
  dashboard: PropTypes.object.isRequired,
  onDrillDown: PropTypes.func,
}
