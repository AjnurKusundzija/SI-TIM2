import { useState, useRef, useEffect, useCallback } from 'react'
import PropTypes from 'prop-types'
import { AlertTriangle, Loader2, Send, Sparkles, X } from 'lucide-react'
import { adminCopilotQuery } from '../../services/aiService'
import AdminCopilotMessage from './AdminCopilotMessage'

// PB-70 / US-108 — MCP Admin Copilot chat panel (slobodan tekst, chat format, loading, greške).
const SUGGESTIONS = [
  'Koji tim je najopterećeniji?',
  'Prikaži tikete bez odgovora duže od 2 sata',
  'Koji problemi se ponavljaju, a nisu pokriveni FAQ-om?',
]

export default function AdminCopilotPanel({ onClose, context }) {
  const [messages, setMessages] = useState([]) // { role: 'user'|'assistant', text?, data? }
  const [input, setInput] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState(null)
  const scrollRef = useRef(null)

  useEffect(() => {
    if (scrollRef.current) scrollRef.current.scrollTop = scrollRef.current.scrollHeight
  }, [messages, loading])

  useEffect(() => {
    function handleKey(e) {
      if (e.key === 'Escape') onClose()
    }
    document.addEventListener('keydown', handleKey)
    return () => document.removeEventListener('keydown', handleKey)
  }, [onClose])

  const send = useCallback(
    async (question) => {
      const q = (question ?? '').trim()
      if (!q || loading) return

      setError(null)
      setInput('')
      setMessages((prev) => [...prev, { role: 'user', text: q }])
      setLoading(true)
      try {
        const data = await adminCopilotQuery(q, context ?? {})
        setMessages((prev) => [...prev, { role: 'assistant', data }])
      } catch (err) {
        setError(
          err.response?.data?.message ?? 'MCP Admin Copilot trenutno nije dostupan. Pokušajte ponovo.'
        )
      } finally {
        setLoading(false)
      }
    },
    [loading, context]
  )

  function handleSubmit(e) {
    e.preventDefault()
    void send(input)
  }

  return (
    <div className="flex flex-col h-full max-h-[640px]">
      {/* Header */}
      <div className="flex items-center justify-between px-4 py-3 border-b border-navy-100 bg-gradient-to-r from-navy-50/80 to-white flex-shrink-0">
        <div className="flex items-center gap-2">
          <div className="w-7 h-7 rounded-lg bg-navy-100 flex items-center justify-center">
            <Sparkles size={14} className="text-navy-700" />
          </div>
          <div>
            <p className="text-sm font-semibold text-gray-900">MCP Admin Copilot</p>
            <p className="text-xs text-gray-400">Pitanja o živim podacima preko MCP alata</p>
          </div>
        </div>
        <button
          type="button"
          onClick={onClose}
          className="p-1.5 text-gray-400 hover:text-gray-600 rounded-lg hover:bg-gray-100"
          aria-label="Zatvori"
        >
          <X size={16} />
        </button>
      </div>

      {/* Chat */}
      <div ref={scrollRef} className="flex-1 overflow-y-auto px-4 py-4 space-y-3 min-h-[200px]">
        {messages.length === 0 && !loading && (
          <div className="flex flex-col items-center justify-center py-8 gap-3 text-center">
            <div className="w-12 h-12 rounded-2xl bg-navy-50 flex items-center justify-center">
              <Sparkles size={24} className="text-navy-300" />
            </div>
            <p className="text-sm text-gray-400 max-w-[260px]">
              Postavi pitanje o tiketima, timovima ili FAQ pokrivenosti. Probaj jedan od prijedloga:
            </p>
            <div className="flex flex-col gap-2 w-full max-w-[320px]">
              {SUGGESTIONS.map((s) => (
                <button
                  key={s}
                  type="button"
                  onClick={() => void send(s)}
                  className="text-left text-xs text-navy-700 bg-navy-50 hover:bg-navy-100 border border-navy-100 rounded-lg px-3 py-2 transition-colors"
                >
                  {s}
                </button>
              ))}
            </div>
          </div>
        )}

        {messages.map((m, i) =>
          m.role === 'user' ? (
            <div key={i} className="flex justify-end">
              <div className="bg-navy-800 text-white text-sm rounded-2xl rounded-br-sm px-3.5 py-2 max-w-[85%]">
                {m.text}
              </div>
            </div>
          ) : (
            <AdminCopilotMessage key={i} data={m.data} />
          )
        )}

        {loading && (
          <div className="flex items-center gap-2 py-3 text-gray-400">
            <Loader2 size={16} className="animate-spin text-navy-500" />
            <span className="text-sm">Copilot analizira žive podatke…</span>
          </div>
        )}

        {error && !loading && (
          <div
            role="alert"
            className="flex items-start gap-2 text-sm text-red-600 bg-red-50 border border-red-100 rounded-lg px-3 py-2.5"
          >
            <AlertTriangle size={14} className="mt-0.5 shrink-0" />
            <span>{error}</span>
          </div>
        )}
      </div>

      {/* Input */}
      <form onSubmit={handleSubmit} className="flex items-center gap-2 px-4 py-3 border-t border-slate-100 flex-shrink-0">
        <input
          type="text"
          value={input}
          onChange={(e) => setInput(e.target.value)}
          placeholder="Postavi pitanje…"
          aria-label="Pitanje za MCP Admin Copilot"
          disabled={loading}
          className="flex-1 text-sm text-gray-700 placeholder-gray-400 bg-slate-50 border border-slate-200 rounded-xl px-3 py-2 outline-none focus:border-navy-400 focus:ring-2 focus:ring-navy-100 disabled:opacity-50"
        />
        <button
          type="submit"
          disabled={loading || !input.trim()}
          className="inline-flex items-center justify-center w-9 h-9 rounded-xl bg-navy-800 text-white hover:bg-navy-900 disabled:opacity-40 disabled:cursor-not-allowed transition-colors"
          aria-label="Pošalji"
        >
          {loading ? <Loader2 size={15} className="animate-spin" /> : <Send size={15} />}
        </button>
      </form>
    </div>
  )
}

AdminCopilotPanel.propTypes = {
  onClose: PropTypes.func.isRequired,
  context: PropTypes.object,
}
