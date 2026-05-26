import { useEffect, useRef, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import PropTypes from 'prop-types'
import { Bell, Menu, X, Check, CheckCheck, Bot, Search } from 'lucide-react'
import { useAuth } from '../../context/AuthContext'
import { useNotifications } from '../../context/NotificationContext'
import { useUIStore } from '../../store/uiStore'
import { timeAgo } from '../../utils/formatDate'

export default function Header({ onMenuToggle, title }) {
  const navigate = useNavigate()
  const { user } = useAuth()
  const { notifications, unreadCount, markAsRead, markAllAsRead } = useNotifications()
  const { aiPanelOpen, toggleAiPanel } = useUIStore()
  const isAdmin = user?.role === 'ADMINISTRATOR'

  function ticketPath(ticketId) {
    return user?.role === 'CLIENT' ? `/mytickets/${ticketId}` : `/tickets/${ticketId}`
  }

  const [notifOpen, setNotifOpen] = useState(false)
  const [searchQuery, setSearchQuery] = useState('')
  const [searchOpen, setSearchOpen] = useState(false)
  const dropdownRef = useRef(null)
  const searchRef = useRef(null)
  const preview = notifications.slice(0, 5)
  const initials = `${user?.firstName?.[0] ?? ''}${user?.lastName?.[0] ?? ''}`.toUpperCase()

  useEffect(() => {
    function handleClickOutside(e) {
      if (dropdownRef.current && !dropdownRef.current.contains(e.target)) {
        setNotifOpen(false)
      }
      if (searchRef.current && !searchRef.current.contains(e.target)) {
        setSearchOpen(false)
      }
    }
    document.addEventListener('mousedown', handleClickOutside)
    return () => document.removeEventListener('mousedown', handleClickOutside)
  }, [])

  function handleSearch(e) {
    e.preventDefault()
    const q = searchQuery.trim()
    if (!q) return
    const base = user?.role === 'CLIENT' ? '/mytickets' : '/tickets'
    navigate(`${base}?search=${encodeURIComponent(q)}`)
    setSearchQuery('')
    setSearchOpen(false)
  }

  async function handleNotificationClick(n) {
    if (!n.isRead) await markAsRead(n.notificationId)
    setNotifOpen(false)
    if (n.ticketId) navigate(ticketPath(n.ticketId))
  }

  return (
    <header className="sticky top-0 z-30 bg-[#f4f6f9] border-b border-slate-200 h-16 flex items-center px-4 lg:px-6">
      <div className="flex items-center justify-between w-full gap-4">

        {/* Left: mobile menu + page title */}
        <div className="flex items-center gap-3 flex-shrink-0">
          <button
            onClick={onMenuToggle}
            className="lg:hidden p-2 text-gray-400 hover:text-gray-600 rounded-lg hover:bg-white transition-colors"
          >
            <Menu size={20} />
          </button>
          <h1 className="text-base font-semibold text-gray-900 leading-tight">{title}</h1>
        </div>

        {/* Centre: search bar (desktop) */}
        <form
          onSubmit={handleSearch}
          className="hidden md:flex flex-1 max-w-sm items-center gap-2 bg-white border border-slate-200 rounded-xl px-3 py-2 focus-within:border-navy-400 focus-within:ring-2 focus-within:ring-navy-100 transition-all"
        >
          <Search size={15} className="text-gray-400 flex-shrink-0" />
          <input
            type="text"
            placeholder="Pretraži tikete..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            className="flex-1 text-sm text-gray-700 placeholder-gray-400 bg-transparent outline-none"
          />
        </form>

        {/* Right: actions */}
        <div className="flex items-center gap-2 flex-shrink-0">

          {/* Search icon — mobile only */}
          <div className="md:hidden relative" ref={searchRef}>
            <button
              onClick={() => setSearchOpen(o => !o)}
              className="p-2 text-gray-400 hover:text-gray-600 rounded-lg hover:bg-white transition-colors"
            >
              <Search size={19} />
            </button>
            {searchOpen && (
              <form
                onSubmit={handleSearch}
                className="absolute right-0 top-10 w-64 bg-white border border-slate-200 rounded-xl shadow-lg px-3 py-2 flex items-center gap-2 z-50"
              >
                <Search size={14} className="text-gray-400 flex-shrink-0" />
                <input
                  autoFocus
                  type="text"
                  placeholder="Pretraži tikete..."
                  value={searchQuery}
                  onChange={(e) => setSearchQuery(e.target.value)}
                  className="flex-1 text-sm text-gray-700 placeholder-gray-400 bg-transparent outline-none"
                />
              </form>
            )}
          </div>

          {/* AI Uvidi — samo za admina */}
          {isAdmin && (
            <button
              onClick={toggleAiPanel}
              className={`inline-flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-xs font-semibold transition-all ${
                aiPanelOpen
                  ? 'bg-violet-100 text-violet-700 border border-violet-200'
                  : 'bg-violet-600 text-white hover:bg-violet-700'
              }`}
            >
              <Bot size={14} />
              AI Uvidi
            </button>
          )}

          {/* Bell */}
          <div className="relative" ref={dropdownRef}>
            <button
              onClick={() => setNotifOpen(o => !o)}
              className={`relative p-2 rounded-lg transition-colors ${
                notifOpen ? 'bg-white text-slate-700' : 'text-gray-400 hover:text-gray-600 hover:bg-white'
              }`}
            >
              <Bell size={19} />
              {unreadCount > 0 && (
                <span className="absolute top-1.5 right-1.5 w-2 h-2 bg-navy-800 rounded-full" />
              )}
            </button>

            {notifOpen && (
              <div className="absolute right-0 mt-2 w-80 bg-white rounded-2xl shadow-xl border border-slate-200 z-50 overflow-hidden">
                <div className="flex items-center justify-between px-4 py-3 border-b border-slate-100">
                  <div className="flex items-center gap-2">
                    <span className="text-sm font-semibold text-gray-900">Notifikacije</span>
                    {unreadCount > 0 && (
                      <span className="px-1.5 py-0.5 bg-navy-800 text-white text-[10px] font-bold rounded-full leading-none">
                        {unreadCount}
                      </span>
                    )}
                  </div>
                  <div className="flex items-center gap-1">
                    {unreadCount > 0 && (
                      <button
                        onClick={markAllAsRead}
                        className="p-1.5 text-gray-400 hover:text-navy-700 hover:bg-navy-50 rounded-lg transition-colors"
                        title="Označi sve kao pročitano"
                      >
                        <CheckCheck size={15} />
                      </button>
                    )}
                    <button
                      onClick={() => setNotifOpen(false)}
                      className="p-1.5 text-gray-400 hover:text-gray-600 hover:bg-slate-100 rounded-lg transition-colors"
                    >
                      <X size={15} />
                    </button>
                  </div>
                </div>

                {preview.length === 0 ? (
                  <div className="px-4 py-8 text-center">
                    <Bell size={24} className="mx-auto text-gray-200 mb-2" />
                    <p className="text-sm text-gray-400">Nema notifikacija</p>
                  </div>
                ) : (
                  <ul className="divide-y divide-slate-50">
                    {preview.map((n) => (
                      <li
                        key={n.notificationId}
                        onClick={() => handleNotificationClick(n)}
                        className={`px-4 py-3 cursor-pointer hover:bg-slate-50 transition-colors ${
                          !n.isRead ? 'bg-navy-50/60' : ''
                        }`}
                      >
                        <div className="flex items-start gap-3">
                          <div className={`mt-1 w-2 h-2 rounded-full flex-shrink-0 ${!n.isRead ? 'bg-navy-700' : 'bg-transparent'}`} />
                          <div className="min-w-0 flex-1">
                            <p className="text-sm font-medium text-gray-900 leading-tight">{n.title}</p>
                            <p className="text-xs text-gray-500 mt-0.5 line-clamp-2">{n.content}</p>
                            <p className="text-xs text-gray-400 mt-1">{timeAgo(n.sentDate)}</p>
                          </div>
                          {!n.isRead && (
                            <button
                              onClick={(e) => { e.stopPropagation(); markAsRead(n.notificationId) }}
                              className="p-1 text-gray-300 hover:text-navy-700 rounded transition-colors flex-shrink-0 mt-0.5"
                              title="Označi kao pročitano"
                            >
                              <Check size={13} />
                            </button>
                          )}
                        </div>
                      </li>
                    ))}
                  </ul>
                )}

                {notifications.length > 5 && (
                  <div className="px-4 py-2 border-t border-slate-100 text-center">
                    <p className="text-xs text-gray-400">+{notifications.length - 5} više notifikacija</p>
                  </div>
                )}

                <div className="px-4 py-2.5 border-t border-slate-200">
                  <button
                    onClick={() => { setNotifOpen(false); navigate('/notifications') }}
                    className="w-full text-xs font-medium text-navy-700 hover:text-navy-900 text-center"
                  >
                    Pogledaj sve notifikacije
                  </button>
                </div>
              </div>
            )}
          </div>

          {/* User avatar */}
          <button
            onClick={() => navigate('/profile')}
            className="w-8 h-8 rounded-full bg-navy-800 flex items-center justify-center text-xs font-semibold text-white hover:bg-navy-900 transition-colors flex-shrink-0"
            title={`${user?.firstName} ${user?.lastName}`}
          >
            {initials || '?'}
          </button>

        </div>
      </div>
    </header>
  )
}

Header.propTypes = {
  onMenuToggle: PropTypes.func.isRequired,
  title: PropTypes.string.isRequired,
}
