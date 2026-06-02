import { useCallback, useEffect, useState } from 'react'
import { AlertCircle, ChevronDown, HelpCircle, Pencil, Plus, RefreshCcw, Trash2, X } from 'lucide-react'
import {
  createFaq,
  deleteFaq,
  getAllFaqs,
  getFaqs,
  updateFaq,
} from '../services/faqService'
import { useAuth } from '../context/AuthContext'
import EmptyState from '../components/common/EmptyState'
import ConfirmDialog from '../components/common/ConfirmDialog'
import Modal from '../components/common/Modal'

const EMPTY_FORM = { question: '', answer: '', category: '', sortOrder: 0 }

export default function Faq() {
  const { user } = useAuth()
  const isAdmin = user?.role === 'ADMINISTRATOR'

  const [faqs, setFaqs] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)
  const [openFaqId, setOpenFaqId] = useState(null)
  const [notification, setNotification] = useState(null)

  // Admin form state
  const [formOpen, setFormOpen] = useState(false)
  const [editingFaq, setEditingFaq] = useState(null)
  const [formValues, setFormValues] = useState(EMPTY_FORM)
  const [formError, setFormError] = useState(null)
  const [submitting, setSubmitting] = useState(false)

  // Delete confirmation
  const [deletingFaq, setDeletingFaq] = useState(null)

  const loadFaqs = useCallback(async () => {
    setLoading(true)
    setError(null)

    try {
      const data = isAdmin ? await getAllFaqs() : await getFaqs()
      setFaqs(data)
    } catch (err) {
      console.error(err)
      setError('Nije moguće učitati često postavljana pitanja.')
    } finally {
      setLoading(false)
    }
  }, [isAdmin])

  useEffect(() => {
    let isMounted = true

    const fetcher = isAdmin ? getAllFaqs : getFaqs

    fetcher()
      .then((data) => {
        if (isMounted) {
          setFaqs(data)
        }
      })
      .catch((err) => {
        console.error(err)
        if (isMounted) {
          setError('Nije moguće učitati često postavljana pitanja.')
        }
      })
      .finally(() => {
        if (isMounted) {
          setLoading(false)
        }
      })

    return () => {
      isMounted = false
    }
  }, [isAdmin])

  const toggleFaq = (faqId) => {
    setOpenFaqId((current) => (current === faqId ? null : faqId))
  }

  const showNotification = (type, message) => {
    setNotification({ type, message })
    setTimeout(() => setNotification(null), 3000)
  }

  const openCreateForm = () => {
    setEditingFaq(null)
    setFormValues(EMPTY_FORM)
    setFormError(null)
    setFormOpen(true)
  }

  const openEditForm = (faq) => {
    setEditingFaq(faq)
    setFormValues({
      question: faq.question ?? '',
      answer: faq.answer ?? '',
      category: faq.category ?? '',
      sortOrder: faq.sortOrder ?? 0,
    })
    setFormError(null)
    setFormOpen(true)
  }

  const closeForm = () => {
    setFormOpen(false)
    setEditingFaq(null)
    setFormValues(EMPTY_FORM)
    setFormError(null)
  }

  const handleSubmit = async (event) => {
    event.preventDefault()
    setFormError(null)

    if (!formValues.question.trim()) {
      setFormError('Pitanje ne smije biti prazno.')
      return
    }
    if (!formValues.answer.trim()) {
      setFormError('Odgovor ne smije biti prazan.')
      return
    }

    setSubmitting(true)
    try {
      const payload = {
        question: formValues.question.trim(),
        answer: formValues.answer.trim(),
        category: formValues.category.trim() || null,
        sortOrder: Number(formValues.sortOrder) || 0,
      }

      if (editingFaq) {
        await updateFaq(editingFaq.faqId, payload)
        showNotification('success', 'FAQ stavka je uspješno ažurirana.')
      } else {
        await createFaq(payload)
        showNotification('success', 'Nova FAQ stavka je uspješno dodana.')
      }

      await loadFaqs()
      closeForm()
    } catch (err) {
      console.error(err)
      setFormError(err.response?.data?.poruka || 'Greška pri čuvanju FAQ stavke.')
    } finally {
      setSubmitting(false)
    }
  }

  const handleConfirmDelete = async () => {
    if (!deletingFaq) return
    const targetId = deletingFaq.faqId
    setDeletingFaq(null)
    try {
      await deleteFaq(targetId)
      showNotification('success', 'FAQ stavka je obrisana.')
      await loadFaqs()
    } catch (err) {
      console.error(err)
      showNotification('error', err.response?.data?.poruka || 'Greška pri brisanju FAQ stavke.')
    }
  }

  return (
    <div className="space-y-5">
      <section className="bg-gradient-to-r from-navy-800 to-navy-700 rounded-xl px-6 py-5 text-white">
        <div className="flex items-start gap-4">
          <div className="w-11 h-11 rounded-xl bg-white/10 flex items-center justify-center shrink-0">
            <HelpCircle size={22} className="text-navy-100" />
          </div>
          <div className="flex-1">
            <h2 className="text-xl font-bold">Često postavljana pitanja</h2>
            <p className="text-sm text-navy-200 mt-1">
              Brzi odgovori za najčešće probleme sa internetom, TV uslugom, mobilnom mrežom i tiketima.
            </p>
          </div>
          {isAdmin && (
            <button
              type="button"
              onClick={openCreateForm}
              className="inline-flex items-center gap-2 px-4 py-2 bg-white text-navy-800 text-sm font-semibold rounded-lg hover:bg-navy-50 transition-colors"
            >
              <Plus size={16} />
              Dodaj pitanje
            </button>
          )}
        </div>
      </section>

      {notification && (
        <div
          role="status"
          className={`flex items-center justify-between gap-3 px-4 py-3 rounded-lg border text-sm font-medium ${
            notification.type === 'success'
              ? 'bg-green-50 text-green-700 border-green-100'
              : 'bg-red-50 text-red-700 border-red-100'
          }`}
        >
          <span>{notification.message}</span>
          <button
            type="button"
            onClick={() => setNotification(null)}
            className="text-current opacity-70 hover:opacity-100"
            aria-label="Zatvori obavještenje"
          >
            <X size={16} />
          </button>
        </div>
      )}

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
            description={
              isAdmin
                ? 'Još uvijek nije dodano nijedno pitanje. Kliknite „Dodaj pitanje" da kreirate prvo.'
                : 'Trenutno nema dostupnih odgovora. Provjerite kasnije ili otvorite novi tiket za podršku.'
            }
          />
        ) : (
          <div className="divide-y divide-gray-100">
            {faqs.map((faq) => {
              const isOpen = openFaqId === faq.faqId

              return (
                <article key={faq.faqId} className="px-5 py-4">
                  <div className="flex items-start gap-3">
                    <button
                      type="button"
                      onClick={() => toggleFaq(faq.faqId)}
                      aria-expanded={isOpen}
                      className="flex-1 flex items-start justify-between gap-4 text-left"
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

                    {isAdmin && (
                      <div className="flex items-center gap-1 flex-shrink-0">
                        <button
                          type="button"
                          onClick={() => openEditForm(faq)}
                          aria-label={`Uredi pitanje ${faq.question}`}
                          className="p-1.5 text-gray-500 hover:text-navy-700 hover:bg-navy-50 rounded-md transition-colors"
                        >
                          <Pencil size={16} />
                        </button>
                        <button
                          type="button"
                          onClick={() => setDeletingFaq(faq)}
                          aria-label={`Obriši pitanje ${faq.question}`}
                          className="p-1.5 text-gray-500 hover:text-red-600 hover:bg-red-50 rounded-md transition-colors"
                        >
                          <Trash2 size={16} />
                        </button>
                      </div>
                    )}
                  </div>

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

      {isAdmin && (
        <Modal
          isOpen={formOpen}
          onClose={closeForm}
          title={editingFaq ? 'Uredi FAQ stavku' : 'Dodaj novu FAQ stavku'}
          size="md"
        >
          <form onSubmit={handleSubmit} className="space-y-4">
            <div>
              <label htmlFor="faq-question" className="block text-xs font-semibold uppercase tracking-wider text-gray-500 mb-1">
                Pitanje
              </label>
              <input
                id="faq-question"
                type="text"
                value={formValues.question}
                onChange={(e) => setFormValues((prev) => ({ ...prev, question: e.target.value }))}
                className="w-full px-3 py-2 border border-gray-200 rounded-lg text-sm focus:ring-2 focus:ring-navy-500 focus:border-navy-500 outline-none"
                placeholder="Unesite pitanje..."
              />
            </div>

            <div>
              <label htmlFor="faq-answer" className="block text-xs font-semibold uppercase tracking-wider text-gray-500 mb-1">
                Odgovor
              </label>
              <textarea
                id="faq-answer"
                rows={5}
                value={formValues.answer}
                onChange={(e) => setFormValues((prev) => ({ ...prev, answer: e.target.value }))}
                className="w-full px-3 py-2 border border-gray-200 rounded-lg text-sm focus:ring-2 focus:ring-navy-500 focus:border-navy-500 outline-none resize-none"
                placeholder="Unesite odgovor..."
              />
            </div>

            <div className="grid grid-cols-2 gap-3">
              <div>
                <label htmlFor="faq-category" className="block text-xs font-semibold uppercase tracking-wider text-gray-500 mb-1">
                  Kategorija
                </label>
                <input
                  id="faq-category"
                  type="text"
                  value={formValues.category}
                  onChange={(e) => setFormValues((prev) => ({ ...prev, category: e.target.value }))}
                  className="w-full px-3 py-2 border border-gray-200 rounded-lg text-sm focus:ring-2 focus:ring-navy-500 focus:border-navy-500 outline-none"
                  placeholder="npr. Internet"
                />
              </div>
              <div>
                <label htmlFor="faq-sortorder" className="block text-xs font-semibold uppercase tracking-wider text-gray-500 mb-1">
                  Redoslijed
                </label>
                <input
                  id="faq-sortorder"
                  type="number"
                  value={formValues.sortOrder}
                  onChange={(e) => setFormValues((prev) => ({ ...prev, sortOrder: e.target.value }))}
                  className="w-full px-3 py-2 border border-gray-200 rounded-lg text-sm focus:ring-2 focus:ring-navy-500 focus:border-navy-500 outline-none"
                />
              </div>
            </div>

            {formError && (
              <div role="alert" className="flex items-center gap-2 text-sm text-red-600 bg-red-50 border border-red-100 rounded-lg px-3 py-2">
                <AlertCircle size={14} />
                {formError}
              </div>
            )}

            <div className="flex items-center justify-end gap-2 pt-2 border-t border-gray-100">
              <button
                type="button"
                onClick={closeForm}
                className="px-4 py-2 text-sm font-medium text-gray-700 bg-gray-100 hover:bg-gray-200 rounded-lg transition-colors"
              >
                Odustani
              </button>
              <button
                type="submit"
                disabled={submitting}
                className="px-4 py-2 text-sm font-semibold bg-navy-700 hover:bg-navy-800 text-white rounded-lg transition-colors disabled:opacity-50"
              >
                {submitting ? 'Čuvanje...' : editingFaq ? 'Sačuvaj izmjene' : 'Dodaj pitanje'}
              </button>
            </div>
          </form>
        </Modal>
      )}

      {isAdmin && (
        <ConfirmDialog
          isOpen={Boolean(deletingFaq)}
          onClose={() => setDeletingFaq(null)}
          onConfirm={handleConfirmDelete}
          title="Obriši FAQ stavku"
          message={deletingFaq ? `Jeste li sigurni da želite obrisati pitanje „${deletingFaq.question}"? Ova akcija se ne može poništiti.` : ''}
          confirmText="Obriši"
          cancelText="Odustani"
          variant="danger"
        />
      )}
    </div>
  )
}
