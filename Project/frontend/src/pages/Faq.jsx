import { useCallback, useEffect, useState } from 'react'
import { AlertCircle, ChevronDown, HelpCircle, RefreshCcw } from 'lucide-react'
import { getFaqs } from '../services/faqService'
import EmptyState from '../components/common/EmptyState'

export default function Faq() {
  const [faqs, setFaqs] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)
  const [openFaqId, setOpenFaqId] = useState(null)

  const loadFaqs = useCallback(async () => {
    setLoading(true)
    setError(null)

    try {
      const data = await getFaqs()
      setFaqs(data)
    } catch (err) {
      console.error(err)
      setError('Nije moguće učitati često postavljana pitanja.')
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => {
    loadFaqs()
  }, [loadFaqs])

  const toggleFaq = (faqId) => {
    setOpenFaqId((current) => (current === faqId ? null : faqId))
  }

  return (
    <div className="space-y-5">
      <section className="bg-gradient-to-r from-navy-800 to-navy-700 rounded-xl px-6 py-5 text-white">
        <div className="flex items-start gap-4">
          <div className="w-11 h-11 rounded-xl bg-white/10 flex items-center justify-center shrink-0">
            <HelpCircle size={22} className="text-navy-100" />
          </div>
          <div>
            <h2 className="text-xl font-bold">Često postavljana pitanja</h2>
            <p className="text-sm text-navy-200 mt-1">
              Brzi odgovori za najčešće probleme sa internetom, TV uslugom, mobilnom mrežom i tiketima.
            </p>
          </div>
        </div>
      </section>

      <section className="bg-white rounded-xl border border-gray-100 shadow-sm overflow-hidden">
        {loading ? (
          <div className="flex justify-center py-16" role="status" aria-label="Učitavanje pitanja">
            <div className="w-8 h-8 border-2 border-navy-600 border-t-transparent rounded-full animate-spin" />
          </div>
        ) : error ? (
          <div className="p-6">
            <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 rounded-lg border border-red-200 bg-red-50 p-4">
              <div className="flex items-start gap-3">
                <AlertCircle size={20} className="text-red-600 mt-0.5 shrink-0" />
                <div>
                  <h3 className="text-sm font-semibold text-red-800">Došlo je do greške</h3>
                  <p className="text-sm text-red-700 mt-1">{error}</p>
                </div>
              </div>
              <button
                type="button"
                onClick={loadFaqs}
                className="inline-flex items-center justify-center gap-2 px-4 py-2 bg-red-600 hover:bg-red-700 text-white text-sm font-medium rounded-lg transition-colors"
              >
                <RefreshCcw size={16} />
                Pokušaj ponovo
              </button>
            </div>
          </div>
        ) : faqs.length === 0 ? (
          <EmptyState
            icon={HelpCircle}
            title="Nema FAQ pitanja"
            description="Trenutno nema dostupnih odgovora. Provjerite kasnije ili otvorite novi tiket za podršku."
          />
        ) : (
          <div className="divide-y divide-gray-100">
            {faqs.map((faq) => {
              const isOpen = openFaqId === faq.faqId

              return (
                <article key={faq.faqId} className="px-5 py-4">
                  <button
                    type="button"
                    onClick={() => toggleFaq(faq.faqId)}
                    aria-expanded={isOpen}
                    className="w-full flex items-start justify-between gap-4 text-left"
                  >
                    <span>
                      {faq.category && (
                        <span className="inline-flex items-center rounded-full bg-navy-50 px-2.5 py-0.5 text-xs font-medium text-navy-700 mb-2">
                          {faq.category}
                        </span>
                      )}
                      <span className="block text-sm font-semibold text-gray-900">
                        {faq.question}
                      </span>
                    </span>
                    <ChevronDown
                      size={20}
                      className={`text-gray-400 mt-1 shrink-0 transition-transform ${isOpen ? 'rotate-180' : ''}`}
                    />
                  </button>

                  {isOpen && (
                    <p className="text-sm text-gray-600 leading-6 mt-3 pr-8">
                      {faq.answer}
                    </p>
                  )}
                </article>
              )
            })}
          </div>
        )}
      </section>
    </div>
  )
}
