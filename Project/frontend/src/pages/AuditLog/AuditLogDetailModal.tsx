import { useState, useEffect } from 'react'
import Modal from '../../components/common/Modal'
import { getAuditLogDetail } from './auditLog.service'
import { AuditLogDetail } from './auditLog.types'
import { getActionTypeLabel } from './auditLog.utils'
import { formatDateTime } from '../../utils/formatDate'

interface Props {
  logId: number
  isOpen: boolean
  onClose: () => void
}

export default function AuditLogDetailModal({ logId, isOpen, onClose }: Props) {
  const [log, setLog] = useState<AuditLogDetail | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    if (!isOpen) return

    async function loadDetail() {
      setLoading(true)
      setError(null)
      try {
        const data = await getAuditLogDetail(logId)
        setLog(data)
      } catch (err: any) {
        setError(err?.response?.data?.message || 'Greška pri učitavanju detaljа')
      } finally {
        setLoading(false)
      }
    }

    loadDetail()
  }, [logId, isOpen])

  if (!isOpen) return null

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={`Detalji akcije — ${log ? getActionTypeLabel(log.actionType) : ''}`}
      size="lg"
    >
      {loading ? (
        <div className="flex justify-center py-8">
          <div className="w-8 h-8 border-4 border-slate-200 border-t-navy-700 rounded-full animate-spin" />
        </div>
      ) : error ? (
        <div className="text-center text-red-600 py-8">{error}</div>
      ) : log ? (
        <div className="space-y-6">
          {/* Grid */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4 pb-6 border-b border-slate-200">
            <div>
              <p className="text-xs font-semibold text-slate-500 uppercase mb-1">
                Datum i vrijeme
              </p>
              <p className="text-sm text-slate-900">{formatDateTime(log.timestamp)}</p>
            </div>

            <div>
              <p className="text-xs font-semibold text-slate-500 uppercase mb-1">
                Korisnik
              </p>
              <p className="text-sm text-slate-900">
                {log.userFullName ? (
                  <>
                    <span className="font-medium">{log.userFullName}</span>
                    <br />
                    <span className="text-xs text-slate-500">{log.userEmail}</span>
                  </>
                ) : (
                  'Sistem'
                )}
              </p>
            </div>

            {log.userRole && (
              <div>
                <p className="text-xs font-semibold text-slate-500 uppercase mb-1">
                  Uloga
                </p>
                <p className="text-sm text-slate-900">{log.userRole}</p>
              </div>
            )}

            <div>
              <p className="text-xs font-semibold text-slate-500 uppercase mb-1">
                Entitet
              </p>
              <p className="text-sm text-slate-900">
                <span>{log.entityType}</span>
                {log.entityId && <span className="ml-1">#{log.entityId}</span>}
              </p>
            </div>

            {log.ipAddress && (
              <div>
                <p className="text-xs font-semibold text-slate-500 uppercase mb-1">
                  IP adresa
                </p>
                <p className="text-sm text-slate-900">{log.ipAddress}</p>
              </div>
            )}
          </div>

          {/* Description */}
          <div>
            <p className="text-xs font-semibold text-slate-500 uppercase mb-2">
              Opis akcije
            </p>
            <p className="text-sm text-slate-900 whitespace-pre-wrap">{log.description}</p>
          </div>

          {/* Old/New Values */}
          {(log.oldValue || log.newValue) && (
            <div>
              <p className="text-xs font-semibold text-slate-500 uppercase mb-3">
                Promijenjene vrijednosti
              </p>

              <div className="overflow-x-auto">
                <table className="w-full text-sm">
                  <thead>
                    <tr className="border-b border-slate-200 bg-slate-50">
                      <th className="px-3 py-2 text-left text-xs font-semibold text-slate-700">
                        Polje
                      </th>
                      <th className="px-3 py-2 text-left text-xs font-semibold text-slate-700">
                        Stara vrijednost
                      </th>
                      <th className="px-3 py-2 text-left text-xs font-semibold text-slate-700">
                        Nova vrijednost
                      </th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-slate-200">
                    {/* Collect all keys from both old and new values */}
                    {Array.from(
                      new Set([
                        ...(log.oldValue ? Object.keys(log.oldValue) : []),
                        ...(log.newValue ? Object.keys(log.newValue) : []),
                      ])
                    ).map((key) => {
                      const oldVal = log.oldValue ? log.oldValue[key] : undefined
                      const newVal = log.newValue ? log.newValue[key] : undefined
                      const changed = JSON.stringify(oldVal) !== JSON.stringify(newVal)

                      return (
                        <tr
                          key={key}
                          className={changed ? 'bg-yellow-50' : ''}
                        >
                          <td className="px-3 py-2 font-medium text-slate-900">{key}</td>
                          <td className="px-3 py-2 text-slate-600 max-w-xs overflow-hidden text-ellipsis">
                            {oldVal !== undefined ? JSON.stringify(oldVal) : '—'}
                          </td>
                          <td className="px-3 py-2 text-slate-600 max-w-xs overflow-hidden text-ellipsis">
                            {newVal !== undefined ? JSON.stringify(newVal) : '—'}
                          </td>
                        </tr>
                      )
                    })}
                  </tbody>
                </table>
              </div>
            </div>
          )}

          {/* Close Button */}
          <div className="flex justify-end pt-4 border-t border-slate-200">
            <button
              onClick={onClose}
              className="px-4 py-2 rounded-lg bg-slate-200 text-slate-900 font-medium hover:bg-slate-300 transition"
            >
              Zatvori
            </button>
          </div>
        </div>
      ) : null}
    </Modal>
  )
}
