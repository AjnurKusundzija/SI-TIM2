import { createContext, useContext, useEffect, useMemo, useRef, useState, useCallback } from 'react'
import PropTypes from 'prop-types'
import { useAuth } from './AuthContext'
import {
  getNotifications,
  markAsRead as apiMarkAsRead,
  markAllAsRead as apiMarkAllAsRead,
  createNotificationConnection,
} from '../services/notificationService'

const NotificationContext = createContext(null)

export function NotificationProvider({ children }) {
  const { user } = useAuth()
  const [notifications, setNotifications] = useState([])
  const connectionRef = useRef(null)

  // Exposed as `reload` so consumers can manually refresh
  const load = useCallback(() => {
    if (!user) return
    getNotifications()
      .then((data) => setNotifications(data))
      .catch(() => {})
  }, [user])

  useEffect(() => {
    if (!user) return

    // setState happens inside .then() callback — not synchronously in the effect body
    getNotifications()
      .then((data) => setNotifications(data))
      .catch(() => {})

    const connection = createNotificationConnection()
    connectionRef.current = connection

    connection.on('ReceiveNotification', (notification) => {
      setNotifications((prev) => [notification, ...prev])
    })

    connection
      .start()
      .then(() => connection.invoke('JoinUserGroup', String(user.userId)))
      .catch(() => {})

    return () => {
      connection.stop()
    }
  }, [user])

  const markAsRead = useCallback(async (notificationId) => {
    await apiMarkAsRead(notificationId)
    setNotifications((prev) =>
      prev.map((n) => (n.notificationId === notificationId ? { ...n, isRead: true } : n))
    )
  }, [])

  const markAllAsRead = useCallback(async () => {
    await apiMarkAllAsRead()
    setNotifications((prev) => prev.map((n) => ({ ...n, isRead: true })))
  }, [])

  const value = useMemo(() => {
    const activeNotifications = user ? notifications : []
    return {
      notifications: activeNotifications,
      unreadCount: activeNotifications.filter((n) => !n.isRead).length,
      markAsRead,
      markAllAsRead,
      reload: load,
    }
  }, [user, notifications, markAsRead, markAllAsRead, load])

  return (
    <NotificationContext.Provider value={value}>
      {children}
    </NotificationContext.Provider>
  )
}

NotificationProvider.propTypes = {
  children: PropTypes.node.isRequired,
}

// eslint-disable-next-line react-refresh/only-export-components
export function useNotifications() {
  return useContext(NotificationContext)
}
