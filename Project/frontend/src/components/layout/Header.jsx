import { useEffect, useRef, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import PropTypes from 'prop-types'
import { Bell, Menu, X } from 'lucide-react'
import { useNotifications } from '../../context/NotificationContext'

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

export default function Header({ onMenuToggle, title }) {
  const navigate = useNavigate()
  const { notifications, unreadCount, markAsRead, markAllAsRead } = useNotifications()
  const [open, setOpen] = useState(false)
  const dropdownRef = useRef(null)

  const preview = notifications.slice(0, 5)

  useEffect(() => {
    function handleClickOutside(e) {
      if (dropdownRef.current && !dropdownRef.current.contains(e.target)) {
        setOpen(false)
      }
    }
    document.addEventListener('mousedown', handleClickOutside)
    return () => document.removeEventListener('mousedown', handleClickOutside)
  }, [])

  async function handleNotificationClick(n) {
    if (!n.isRead) await markAsRead(n.notificationId)
    setOpen(false)
    // TODO: navigiraj na odgovarajući tiket (potrebno dodati ticketId u Notification entitet i DTO)
  }

  return (
    <header className="sticky top-0 z-30 bg-white border-b border-gray-200 px-4 lg:px-6 py-3">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <button
            onClick={onMenuToggle}
            className="lg:hidden p-2 text-gray-500 hover:text-gray-700 rounded-lg hover:bg-gray-100"
          >
            <Menu size={20} />
          </button>
          <h1 className="text-lg font-semibold text-gray-900">{title}</h1>
        </div>

        <div className="flex items-center gap-3" ref={dropdownRef}>
          {/* Bell button */}
          <div className="relative">
            <button
              onClick={() => setOpen((o) => !o)}
              className="relative p-2 text-gray-500 hover:text-gray-700 rounded-lg hover:bg-gray-100"
            >
              <Bell size={20} />
              {unreadCount > 0 && (
                <span className="absolute top-1 right-1 w-4 h-4 bg-red-500 text-white text-[10px] font-bold rounded-full flex items-center justify-center leading-none">
                  {unreadCount > 9 ? '9+' : unreadCount}
                </span>
              )}
            </button>

            {/* Dropdown */}
            {open && (
              <div className="absolute right-0 mt-1 w-80 bg-white rounded-xl shadow-lg border border-gray-100 z-50 overflow-hidden">
                {/* Header */}
                <div className="flex items-center justify-between px-4 py-3 border-b border-gray-100">
                  <span className="text-sm font-semibold text-gray-900">Notifikacije</span>
                  <div className="flex items-center gap-2">
                    {unreadCount > 0 && (
                      <button
                        onClick={markAllAsRead}
                        className="text-xs text-navy-600 hover:underline"
                      >
                        Označi sve kao pročitano
                      </button>
                    )}
                    <button onClick={() => setOpen(false)} className="text-gray-400 hover:text-gray-600">
                      <X size={14} />
                    </button>
                  </div>
                </div>

                {/* List */}
                {preview.length === 0 ? (
                  <div className="px-4 py-6 text-center text-sm text-gray-400">
                    Nema notifikacija.
                  </div>
                ) : (
                  <ul>
                    {preview.map((n) => (
                      <li
                        key={n.notificationId}
                        onClick={() => handleNotificationClick(n)}
                        className={`px-4 py-3 border-b border-gray-50 last:border-0 cursor-pointer hover:bg-gray-50 transition-colors ${
                          !n.isRead ? 'bg-blue-50/60' : ''
                        }`}
                      >
                        <div className="flex items-start gap-2">
                          {!n.isRead && (
                            <span className="mt-1.5 w-2 h-2 rounded-full bg-blue-500 flex-shrink-0" />
                          )}
                          <div className={!n.isRead ? '' : 'ml-4'}>
                            <p className="text-sm font-medium text-gray-900">{n.title}</p>
                            <p className="text-xs text-gray-500 mt-0.5">{n.content}</p>
                            <p className="text-xs text-gray-400 mt-1">{timeAgo(n.sentDate)}</p>
                          </div>
                        </div>
                      </li>
                    ))}
                  </ul>
                )}

                {/* Footer */}
                <div className="px-4 py-2 border-t border-gray-100 text-center">
                  <button
                    onClick={() => { setOpen(false); navigate('/notifications') }}
                    className="text-xs text-navy-600 hover:underline"
                  >
                    Pogledaj sve notifikacije
                  </button>
                </div>
              </div>
            )}
          </div>
        </div>
      </div>
    </header>
  )
}

Header.propTypes = {
  onMenuToggle: PropTypes.func.isRequired,
  title: PropTypes.string.isRequired,
}
