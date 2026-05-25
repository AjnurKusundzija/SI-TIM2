import { useState } from 'react'
import { Eye } from 'lucide-react'
import { AuditLogListItem } from './auditLog.types'
import Badge from '../../components/common/Badge'
import AuditLogDetailModal from './AuditLogDetailModal'
import { getActionTypeLabel, getActionTypeBadgeColor } from './auditLog.utils'
import { formatDateTime } from '../../utils/formatDate'

interface Props {
  logs: AuditLogListItem[]
}

export default function AuditLogTable({ logs }: Props) {
  const [selectedLogId, setSelectedLogId] = useState<number | null>(null)

  const truncate = (text: string, maxLength: number = 80) => {
    if (text.length <= maxLength) return text
    return text.substring(0, maxLength) + '...'
  }

  const getRoleBadgeColor = (role: string): string => {
    switch (role) {
      case 'ADMINISTRATOR':
        return 'bg-red-100 text-red-800'
      case 'AGENT':
        return 'bg-blue-100 text-blue-800'
      case 'TECHNICIAN':
        return 'bg-yellow-100 text-yellow-800'
      case 'CLIENT':
        return 'bg-green-100 text-green-800'
      default:
        return 'bg-gray-100 text-gray-800'
    }
  }

  return (
    <>
      {/* Desktop View */}
      <div className="block rounded-3xl bg-white shadow-sm border border-slate-200 overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead>
              <tr className="border-b border-slate-200 bg-slate-50">
                <th className="px-6 py-3 text-left text-xs font-semibold text-slate-700">
                  Datum i vrijeme
                </th>
                <th className="px-6 py-3 text-left text-xs font-semibold text-slate-700">
                  Korisnik
                </th>
                <th className="px-6 py-3 text-left text-xs font-semibold text-slate-700">
                  Uloga
                </th>
                <th className="px-6 py-3 text-left text-xs font-semibold text-slate-700">
                  Tip akcije
                </th>
                <th className="px-6 py-3 text-left text-xs font-semibold text-slate-700">
                  Entitet
                </th>
                <th className="px-6 py-3 text-left text-xs font-semibold text-slate-700">
                  Opis
                </th>
                <th className="px-6 py-3 text-center text-xs font-semibold text-slate-700">
                  Akcija
                </th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-200">
              {logs.map((log) => (
                <tr key={log.id} className="hover:bg-slate-50 transition">
                  <td className="px-6 py-4 text-sm text-slate-900">
                    {formatDateTime(log.timestamp)}
                  </td>
                  <td className="px-6 py-4 text-sm text-slate-900">
                    {log.userId ? (
                      <div>
                        <div className="font-medium">{log.userFullName}</div>
                        <div className="text-xs text-slate-500">{log.userEmail}</div>
                      </div>
                    ) : (
                      <span className="text-slate-500">Sistem</span>
                    )}
                  </td>
                  <td className="px-6 py-4 text-sm">
                    {log.userRole && (
                      <span
                        className={`inline-flex px-2.5 py-1 rounded-full text-xs font-medium ${getRoleBadgeColor(
                          log.userRole
                        )}`}
                      >
                        {log.userRole}
                      </span>
                    )}
                  </td>
                  <td className="px-6 py-4 text-sm">
                    <span
                      className={`inline-flex px-2.5 py-1 rounded-full text-xs font-medium ${getActionTypeBadgeColor(
                        log.actionType
                      )}`}
                    >
                      {getActionTypeLabel(log.actionType)}
                    </span>
                  </td>
                  <td className="px-6 py-4 text-sm text-slate-900">
                    <span>{log.entityType}{log.entityId ? ` #${log.entityId}` : ''}</span>
                  </td>
                  <td className="px-6 py-4 text-sm text-slate-600 max-w-xs">
                    <div title={log.description}>{truncate(log.description)}</div>
                  </td>
                  <td className="px-6 py-4 text-center">
                    {log.hasDetails && (
                      <button
                        onClick={() => setSelectedLogId(log.id)}
                        className="inline-flex items-center gap-1 rounded-lg px-2 py-1 text-navy-600 hover:bg-navy-50 transition"
                        title="Pregledaj detalje"
                      >
                        <Eye size={18} />
                        <span>Pregledaj</span>
                      </button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {/* Detail Modal */}
      {selectedLogId && (
        <AuditLogDetailModal
          logId={selectedLogId}
          isOpen={selectedLogId !== null}
          onClose={() => setSelectedLogId(null)}
        />
      )}
    </>
  )
}
