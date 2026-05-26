import { useState } from 'react'
import PropTypes from 'prop-types'
import { AlertCircle, Bot, CheckCheck, Loader2, X } from 'lucide-react'
import { getAgentSuggestion } from '../../services/aiService'

// PB-57 / US-96, US-97
export default function AISuggestionModal({ ticket, comments, onUse, onClose }) {
  const [suggestion, setSuggestion] = useState(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState(null)

  async function fetchSuggestion() {
    setLoading(true)
    setError(null)
    try {
      const recentComments = (comments ?? [])
        .filter((c) => !c.isSystemMessage)
        .slice(-6)
        .map((c) => ({
          authorRole: c.authorRole ?? c.role ?? 'CLIENT',
          content: c.content ?? c.text ?? '',
        }))
      const data = await getAgentSuggestion({
        ticketId: ticket.ticketId ?? ticket.id,
        title: ticket.title,
        description: ticket.description,
        category: ticket.problemCategory ?? ticket.category ?? '',
        recentComments,
      })
      setSuggestion(data.suggestion)
    } catch (err) {
      setError(
        err.response?.data?.message ??
          'AI servis trenutno nije dostupan. Provjerite da li je API ključ konfigurisan.'
      )
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
      <div className="bg-white rounded-2xl shadow-xl w-full max-w-lg flex flex-col">
        {/* Header */}
        <div className="flex items-center justify-between px-5 py-4 border-b border-gray-100">
          <div className="flex items-center gap-2">
            <div className="w-8 h-8 rounded-lg bg-violet-100 flex items-center justify-center">
              <Bot size={16} className="text-violet-600" />
            </div>
            <div>
              <p className="text-sm font-semibold text-gray-900">AI Prijedlog odgovora</p>
              <p className="text-xs text-gray-400">Generisano uz pomoć AI — pregledaj prije slanja</p>
            </div>
          </div>
          <button
            type="button"
            onClick={onClose}
            className="p-1.5 rounded-lg hover:bg-gray-100 text-gray-400 hover:text-gray-600 transition-colors"
          >
            <X size={16} />
          </button>
        </div>

        {/* Body */}
        <div className="px-5 py-4 flex-1 min-h-[160px] flex flex-col gap-3">
          {!suggestion && !loading && !error && (
            <div className="flex flex-col items-center justify-center flex-1 gap-3 text-center py-4">
              <Bot size={32} className="text-violet-300" />
              <p className="text-sm text-gray-500">
                AI će analizirati tiket i predložiti odgovor na osnovu baze znanja.
              </p>
              <button
                type="button"
                onClick={fetchSuggestion}
                className="px-4 py-2 bg-violet-600 hover:bg-violet-700 text-white text-sm font-medium rounded-lg transition-colors"
              >
                Generiši prijedlog
              </button>
            </div>
          )}

          {loading && (
            <div className="flex flex-col items-center justify-center flex-1 gap-2 py-6 text-gray-400">
              <Loader2 size={24} className="animate-spin text-violet-500" />
              <p className="text-sm">AI analizira tiket…</p>
            </div>
          )}

          {error && (
            <div className="flex flex-col gap-3">
              <div className="flex items-start gap-2 text-sm text-red-600 bg-red-50 border border-red-100 rounded-lg px-3 py-2.5">
                <AlertCircle size={15} className="mt-0.5 shrink-0" />
                <span>{error}</span>
              </div>
              <button
                type="button"
                onClick={fetchSuggestion}
                className="self-start px-3 py-1.5 text-sm font-medium text-violet-600 hover:text-violet-700 border border-violet-200 hover:bg-violet-50 rounded-lg transition-colors"
              >
                Pokušaj ponovo
              </button>
            </div>
          )}

          {suggestion && !loading && (
            <div className="flex flex-col gap-3">
              <div className="bg-violet-50 border border-violet-100 rounded-xl px-4 py-3 text-sm text-gray-800 leading-relaxed whitespace-pre-wrap">
                {suggestion}
              </div>
              <button
                type="button"
                onClick={fetchSuggestion}
                className="self-start text-xs text-gray-400 hover:text-gray-600 underline underline-offset-2 transition-colors"
              >
                Generiši novi prijedlog
              </button>
            </div>
          )}
        </div>

        {/* Footer */}
        {suggestion && !loading && (
          <div className="px-5 py-3 border-t border-gray-100 flex items-center justify-end gap-2">
            <button
              type="button"
              onClick={onClose}
              className="px-3 py-1.5 text-sm text-gray-500 hover:text-gray-700 font-medium rounded-lg hover:bg-gray-100 transition-colors"
            >
              Odbaci
            </button>
            <button
              type="button"
              onClick={() => {
                onUse(suggestion)
                onClose()
              }}
              className="inline-flex items-center gap-2 px-4 py-1.5 bg-violet-600 hover:bg-violet-700 text-white text-sm font-medium rounded-lg transition-colors"
            >
              <CheckCheck size={15} />
              Koristi odgovor
            </button>
          </div>
        )}
      </div>
    </div>
  )
}

AISuggestionModal.propTypes = {
  ticket: PropTypes.object.isRequired,
  comments: PropTypes.array,
  onUse: PropTypes.func.isRequired,
  onClose: PropTypes.func.isRequired,
}
