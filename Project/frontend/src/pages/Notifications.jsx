import { useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import { useNotifications } from '../context/NotificationContext'
import { Bell, CheckCheck } from 'lucide-react'

function timeAgo(dateStr) {
  const diff = Date.now() - new Date(dateStr).getTime()
  const m = Math.floor(diff / 60000)
  if (m < 1) return 'upravo'
  if (m < 60) return `prije ${m} min`
  const h = Math.floor(m / 60)
  if (h < 24) return `prije ${h}h`
  const d = Math.floor(h / 24)
  return `prije ${d}d`
}

const typeLabel = {
  TICKET_ASSIGNED: 'Dodijeljen tiket',
  TICKET_FORWARDED: 'Proslijeđen tiket',
  TICKET_RESPONSE: 'Odgovor na tiket',
  STATUS_CHANGED: 'Promjena statusa',
  TICKET_CLOSED: 'Tiket zatvoren',
}

const typeColor = {
  TICKET_ASSIGNED: 'bg-blue-100 text-blue-700',
  TICKET_FORWARDED: 'bg-violet-100 text-violet-700',
  TICKET_RESPONSE: 'bg-emerald-100 text-emerald-700',
  STATUS_CHANGED: 'bg-amber-100 text-amber-700',
  TICKET_CLOSED: 'bg-gray-100 text-gray-600',
}

export default function Notifications() {
  const navigate = useNavigate()
  const { user } = useAuth()
  const { notifications, unreadCount, markAsRead, markAllAsRead } = useNotifications()

  function handleClick(n) {
    if (!n.isRead) markAsRead(n.notificationId)
    if (n.ticketId) {
      const path = user?.role === 'CLIENT' ? `/mytickets/${n.ticketId}` : `/tickets/${n.ticketId}`
      navigate(path)
    }
  }

  return (
    <div className="space-y-6">
      {/* Header row */}
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-xl font-bold text-gray-900">Notifikacije</h2>
          <p className="text-sm text-gray-500 mt-0.5">
            {unreadCount > 0 ? `${unreadCount} nepročitanih` : 'Sve notifikacije su pročitane'}
          </p>
        </div>
        {unreadCount > 0 && (
          <button
            onClick={markAllAsRead}
            className="flex items-center gap-2 px-4 py-2 bg-navy-600 text-white text-sm font-medium rounded-lg hover:bg-navy-700 transition-colors"
          >
            <CheckCheck size={16} />
            Označi sve kao pročitano
          </button>
        )}
      </div>

      {/* List */}
      {notifications.length === 0 ? (
        <div className="bg-white rounded-xl border border-gray-100 shadow-sm px-4 py-12 text-center">
          <Bell size={32} className="mx-auto text-gray-300 mb-3" />
          <p className="text-sm text-gray-400">Nemate notifikacija.</p>
        </div>
      ) : (
        <div className="bg-white rounded-xl border border-gray-100 shadow-sm overflow-hidden">
          {notifications.map((n) => (
            <div
              key={n.notificationId}
              onClick={() => handleClick(n)}
              className={`flex items-start gap-4 px-5 py-4 border-b border-gray-50 last:border-0 transition-colors cursor-pointer hover:bg-gray-50 ${
                !n.isRead ? 'bg-blue-50/50 hover:bg-blue-50' : ''
              }`}
            >
              {/* Unread dot */}
              <div className="pt-1.5 flex-shrink-0">
                {!n.isRead ? (
                  <span className="block w-2.5 h-2.5 rounded-full bg-blue-500" />
                ) : (
                  <span className="block w-2.5 h-2.5 rounded-full bg-transparent" />
                )}
              </div>

              {/* Content */}
              <div className="flex-1 min-w-0">
                <div className="flex items-center gap-2 flex-wrap">
                  <p className="text-sm font-semibold text-gray-900">{n.title}</p>
                  <span className={`text-[11px] font-medium px-2 py-0.5 rounded-full ${typeColor[n.notificationType] ?? 'bg-gray-100 text-gray-600'}`}>
                    {typeLabel[n.notificationType] ?? n.notificationType}
                  </span>
                </div>
                <p className="text-sm text-gray-600 mt-0.5">{n.content}</p>
                <p className="text-xs text-gray-400 mt-1">{timeAgo(n.sentDate)}</p>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}
