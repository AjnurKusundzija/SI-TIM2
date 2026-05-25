import api from '../../services/api'
import { AuditLogResponse, AuditLogDetail, AuditLogUserDto } from './auditLog.types'

export async function getAuditLogs(
  page: number = 1,
  pageSize: number = 20,
  filters?: {
    actionType?: string
    userId?: string | number
    search?: string
    dateFrom?: string
    dateTo?: string
  }
): Promise<AuditLogResponse> {
  const params: any = {
    page,
    pageSize,
  }

  if (filters?.actionType) params.actionType = filters.actionType
  if (filters?.userId) params.userId = filters.userId
  if (filters?.search) params.search = filters.search
  if (filters?.dateFrom) params.dateFrom = filters.dateFrom
  if (filters?.dateTo) params.dateTo = filters.dateTo

  const response = await api.get('/audit-logs', { params })
  return response.data
}

export async function getAuditLogDetail(id: number): Promise<AuditLogDetail> {
  const response = await api.get(`/audit-logs/${id}`)
  return response.data
}

export async function getAuditActionTypes(): Promise<string[]> {
  const response = await api.get('/audit-logs/action-types')
  return response.data
}

export async function getAuditLogUsers(): Promise<AuditLogUserDto[]> {
  const response = await api.get('/audit-logs/users')
  return response.data
}
