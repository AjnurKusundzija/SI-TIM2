import { useState } from 'react'
import { AuditLogFilters, AuditLogUserDto } from './auditLog.types'
import { getActionTypeLabel } from './auditLog.utils'

interface Props {
  filters: AuditLogFilters
  actionTypes: string[]
  users: AuditLogUserDto[]
  onApply: (filters: Partial<AuditLogFilters>) => void
  onReset: () => void
  filterError?: string | null
}

export default function AuditLogFilters({
  filters,
  actionTypes,
  users,
  onApply,
  onReset,
  filterError,
}: Props) {
  const [localFilters, setLocalFilters] = useState(filters)

  const handleApply = () => {
    onApply(localFilters)
  }

  const handleReset = () => {
    setLocalFilters({
      search: '',
      actionType: '',
      userId: '',
      dateFrom: '',
      dateTo: '',
      page: 1,
      pageSize: 20,
    })
    onReset()
  }

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter') {
      handleApply()
    }
  }

  return (
    <div className="bg-white rounded-3xl p-6 shadow-sm border border-slate-200">
      <div className="space-y-4">
        <div>
          <label htmlFor="audit-search" className="block text-sm font-medium text-slate-700 mb-2">
            Pretraga po opisu
          </label>
          <input
            id="audit-search"
            type="text"
            placeholder="Pretraži po opisu akcije..."
            value={localFilters.search}
            onChange={(e) =>
              setLocalFilters((prev) => ({ ...prev, search: e.target.value }))
            }
            onKeyDown={handleKeyDown}
            className="w-full rounded-lg border border-slate-300 px-4 py-2 text-sm focus:border-navy-500 focus:outline-none focus:ring-2 focus:ring-navy-200"
          />
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          <div>
            <label htmlFor="audit-action-type" className="block text-sm font-medium text-slate-700 mb-2">
              Tip akcije
            </label>
            <select
              id="audit-action-type"
              value={localFilters.actionType}
              onChange={(e) =>
                setLocalFilters((prev) => ({ ...prev, actionType: e.target.value }))
              }
              className="w-full rounded-lg border border-slate-300 px-4 py-2 text-sm focus:border-navy-500 focus:outline-none focus:ring-2 focus:ring-navy-200 bg-white"
            >
              <option value="">Sve akcije</option>
              {actionTypes.map((type) => (
                <option key={type} value={type}>
                  {getActionTypeLabel(type)}
                </option>
              ))}
            </select>
          </div>

          <div>
            <label htmlFor="audit-user" className="block text-sm font-medium text-slate-700 mb-2">
              Korisnik
            </label>
            <select
              id="audit-user"
              value={localFilters.userId}
              onChange={(e) =>
                setLocalFilters((prev) => ({ ...prev, userId: e.target.value }))
              }
              className="w-full rounded-lg border border-slate-300 px-4 py-2 text-sm focus:border-navy-500 focus:outline-none focus:ring-2 focus:ring-navy-200 bg-white"
            >
              <option value="">Svi korisnici</option>
              {users.map((user) => (
                <option key={user.id} value={user.id}>
                  {user.fullName} ({user.email})
                </option>
              ))}
            </select>
          </div>

          <div>
            <label htmlFor="audit-date-from" className="block text-sm font-medium text-slate-700 mb-2">
              Od datuma
            </label>
            <input
              id="audit-date-from"
              type="date"
              value={localFilters.dateFrom}
              onChange={(e) =>
                setLocalFilters((prev) => ({ ...prev, dateFrom: e.target.value }))
              }
              className="w-full rounded-lg border border-slate-300 px-4 py-2 text-sm focus:border-navy-500 focus:outline-none focus:ring-2 focus:ring-navy-200"
            />
          </div>

          <div>
            <label htmlFor="audit-date-to" className="block text-sm font-medium text-slate-700 mb-2">
              Do datuma
            </label>
            <input
              id="audit-date-to"
              type="date"
              value={localFilters.dateTo}
              onChange={(e) =>
                setLocalFilters((prev) => ({ ...prev, dateTo: e.target.value }))
              }
              className="w-full rounded-lg border border-slate-300 px-4 py-2 text-sm focus:border-navy-500 focus:outline-none focus:ring-2 focus:ring-navy-200"
            />
          </div>
        </div>

        {filterError && (
          <div className="text-sm text-red-600">
            {filterError}
          </div>
        )}

        <div className="flex gap-3 pt-2">
          <button
            onClick={handleApply}
            className="inline-flex items-center gap-2 rounded-lg bg-navy-700 px-4 py-2 text-sm font-semibold text-white transition hover:bg-navy-800"
          >
            Primijeni
          </button>
          <button
            onClick={handleReset}
            className="inline-flex items-center gap-2 rounded-lg border border-slate-300 px-4 py-2 text-sm font-semibold text-slate-700 transition hover:bg-slate-50"
          >
            Resetuj filtere
          </button>
        </div>
      </div>
    </div>
  )
}
