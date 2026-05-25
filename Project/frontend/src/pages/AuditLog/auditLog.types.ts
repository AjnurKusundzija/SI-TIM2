export interface AuditLogListItem {
  id: number
  timestamp: string
  userId: number | null
  userFullName: string | null
  userEmail: string | null
  userRole: string | null
  actionType: string
  entityType: string
  entityId: string | null
  description: string
  hasDetails: boolean
}

export interface AuditLogDetail extends AuditLogListItem {
  oldValue: Record<string, unknown> | null
  newValue: Record<string, unknown> | null
  ipAddress: string | null
}

export interface AuditLogResponse {
  items: AuditLogListItem[]
  totalCount: number
  page: number
  pageSize: number
  totalPages: number
}

export interface AuditLogFilters {
  search: string
  actionType: string
  userId: string
  dateFrom: string
  dateTo: string
  page: number
  pageSize: number
}

export interface AuditLogUserDto {
  id: number
  fullName: string
  email: string
}
