export const actionTypeLabels: Record<string, string> = {
  USER_LOGIN: 'Prijava korisnika',
  USER_LOGOUT: 'Odjava korisnika',
  USER_LOGIN_FAILED: 'Neuspješna prijava',
  USER_CREATED: 'Kreiranje korisnika',
  USER_UPDATED: 'Izmjena korisnika',
  USER_DEACTIVATED: 'Deaktivacija korisnika',
  USER_REACTIVATED: 'Reaktivacija korisnika',
  TICKET_CREATED: 'Kreiranje tiketa',
  TICKET_CLOSED: 'Zatvaranje tiketa',
  TICKET_CLOSURE_REQUESTED: 'Zahtjev za zatvaranje',
  TICKET_STATUS_CHANGED: 'Promjena statusa',
  TICKET_FORWARDED: 'Prosljeđivanje tiketa',
  TICKET_PRIORITY_CHANGED: 'Promjena prioriteta',
  PACKAGE_CREATED: 'Kreiranje paketa',
  PACKAGE_UPDATED: 'Izmjena paketa',
  PACKAGE_DEACTIVATED: 'Deaktivacija paketa',
  SUBSCRIPTION_ASSIGNED: 'Dodjela pretplate',
  SUBSCRIPTION_DEACTIVATED: 'Deaktivacija pretplate',
  REASSIGNMENT_REQUESTED: 'Zahtjev za premještanje',
  REASSIGNMENT_APPROVED: 'Zahtjev odobren',
  REASSIGNMENT_REJECTED: 'Zahtjev odbijen',
  REASSIGNMENT_COMPLETED: 'Premještanje završeno',
  AGENT_REASSIGNED: 'Premještanje agenta',
}

export function getActionTypeLabel(actionType: string): string {
  return actionTypeLabels[actionType] || actionType
}

export function getActionTypeCategory(actionType: string): 'auth' | 'user' | 'ticket' | 'package' {
  if (actionType.includes('LOGIN') || actionType.includes('LOGOUT')) return 'auth'
  if (actionType.startsWith('USER_')) return 'user'
  if (actionType.startsWith('TICKET_')) return 'ticket'
  return 'package'
}

export function getActionTypeBadgeColor(actionType: string): string {
  const category = getActionTypeCategory(actionType)
  switch (category) {
    case 'auth':
      return 'bg-slate-100 text-slate-800'
    case 'user':
      return 'bg-blue-100 text-blue-800'
    case 'ticket':
      return 'bg-green-100 text-green-800'
    case 'package':
      return 'bg-purple-100 text-purple-800'
    default:
      return 'bg-gray-100 text-gray-800'
  }
}
