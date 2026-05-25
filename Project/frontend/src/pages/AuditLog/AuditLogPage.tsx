import { useState, useEffect } from 'react'
import { getAuditLogs, getAuditActionTypes, getAuditLogUsers } from './auditLog.service'
import { AuditLogListItem, AuditLogUserDto, AuditLogFilters as AuditLogFiltersState } from './auditLog.types'
import AuditLogFilters from './AuditLogFilters'
import AuditLogTable from './AuditLogTable'
import EmptyState from '../../components/common/EmptyState'

export default function AuditLogPage() {
  const [logs, setLogs] = useState<AuditLogListItem[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [filterError, setFilterError] = useState<string | null>(null)

  const [filters, setFilters] = useState<AuditLogFiltersState>({
    search: '',
    actionType: '',
    userId: '',
    dateFrom: '',
    dateTo: '',
    page: 1,
    pageSize: 20,
  })

  const [totalCount, setTotalCount] = useState(0)
  const [totalPages, setTotalPages] = useState(1)
  const [actionTypes, setActionTypes] = useState<string[]>([])
  const [users, setUsers] = useState<AuditLogUserDto[]>([])
  const [loadingFilters, setLoadingFilters] = useState(true)

  useEffect(() => {
    async function loadFilterOptions() {
      try {
        const [types, userList] = await Promise.all([
          getAuditActionTypes(),
          getAuditLogUsers(),
        ])
        setActionTypes(types)
        setUsers(userList)
      } finally {
        setLoadingFilters(false)
      }
    }

    loadFilterOptions()
  }, [])

  const loadLogs = async (currentFilters = filters) => {
    setLoading(true)
    setError(null)
    try {
      const result = await getAuditLogs(currentFilters.page, currentFilters.pageSize, {
        search: currentFilters.search || undefined,
        actionType: currentFilters.actionType || undefined,
        userId: currentFilters.userId || undefined,
        dateFrom: currentFilters.dateFrom || undefined,
        dateTo: currentFilters.dateTo || undefined,
      })
      setLogs(result.items)
      setTotalCount(result.totalCount)
      setTotalPages(result.totalPages)
    } catch (err: any) {
      setError(err?.response?.data?.message || 'Greška pri učitavanju audit logova')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadLogs()
  }, [filters.page, filters.pageSize, filters.search, filters.actionType, filters.userId, filters.dateFrom, filters.dateTo])

  const handleApplyFilters = (newFilters: Partial<AuditLogFiltersState>) => {
    if (newFilters.dateFrom && newFilters.dateTo && newFilters.dateFrom > newFilters.dateTo) {
      setFilterError('Datum "Do" mora biti nakon datuma "Od"')
      return
    }

    setFilterError(null)
    setFilters((prev) => ({
      ...prev,
      ...newFilters,
      page: 1,
    }))
  }

  const handleResetFilters = () => {
    setError(null)
    setFilterError(null)
    setFilters({
      search: '',
      actionType: '',
      userId: '',
      dateFrom: '',
      dateTo: '',
      page: 1,
      pageSize: 20,
    })
  }

  const handlePageChange = (newPage: number) => {
    setFilters((prev) => ({
      ...prev,
      page: newPage,
    }))
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-semibold text-navy-900">Audit log</h1>
        <p className="text-sm text-slate-500 mt-1">Pregled sve aktivnosti u sistemu</p>
      </div>

      {!loadingFilters && (
        <AuditLogFilters
          filters={filters}
          actionTypes={actionTypes}
          users={users}
          onApply={handleApplyFilters}
          onReset={handleResetFilters}
          filterError={filterError}
        />
      )}

      {error && (
        <div className="rounded-lg bg-red-50 border border-red-200 p-4 flex items-center justify-between gap-3">
          <p className="text-sm text-red-800">{error}</p>
          <button
            type="button"
            onClick={() => loadLogs()}
            className="rounded-lg border border-red-300 px-3 py-1.5 text-sm font-medium text-red-800 hover:bg-red-100"
          >
            Pokušaj ponovo
          </button>
        </div>
      )}

      {loading ? (
        <div className="flex justify-center py-12">
          <div className="w-8 h-8 border-4 border-navy-200 border-t-navy-700 rounded-full animate-spin" />
        </div>
      ) : logs.length === 0 ? (
        <EmptyState
          title="Nema zapisa"
          description="Nema zapisa koji odgovaraju zadanim kriterijima"
        />
      ) : (
        <>
          <AuditLogTable logs={logs} />

          <div className="flex flex-col sm:flex-row items-center justify-between gap-4">
            <p className="text-sm text-slate-600">
              Prikazano {(filters.page - 1) * filters.pageSize + 1}-
              {Math.min(filters.page * filters.pageSize, totalCount)} od {totalCount} zapisa
            </p>

            <div className="flex items-center gap-2">
              <button
                onClick={() => handlePageChange(filters.page - 1)}
                disabled={filters.page === 1}
                className="px-3 py-2 rounded-lg border border-slate-300 text-sm font-medium text-slate-700 hover:bg-slate-50 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                Prethodna
              </button>

              <div className="flex gap-1">
                {Array.from({ length: totalPages }, (_, i) => i + 1).map((page) => {
                  const showPage =
                    page <= 3 ||
                    page > totalPages - 3 ||
                    (page >= filters.page - 1 && page <= filters.page + 1)

                  if (!showPage) {
                    if (page === 4) {
                      return (
                        <span key="dots-start" className="px-2 py-2 text-slate-400">
                          ...
                        </span>
                      )
                    }
                    return null
                  }

                  return (
                    <button
                      key={page}
                      onClick={() => handlePageChange(page)}
                      className={`w-8 h-8 rounded-lg font-medium text-sm transition ${
                        filters.page === page
                          ? 'bg-navy-700 text-white'
                          : 'border border-slate-300 text-slate-700 hover:bg-slate-50'
                      }`}
                    >
                      {page}
                    </button>
                  )
                })}
              </div>

              <button
                onClick={() => handlePageChange(filters.page + 1)}
                disabled={filters.page === totalPages}
                className="px-3 py-2 rounded-lg border border-slate-300 text-sm font-medium text-slate-700 hover:bg-slate-50 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                Sljedeća
              </button>
            </div>
          </div>
        </>
      )}
    </div>
  )
}
