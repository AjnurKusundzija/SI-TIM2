import { createContext, useContext, useEffect, useRef, useState, useCallback } from 'react'
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

  const unreadCount = notifications.filter((n) => !n.isRead).length

  const load = useCallback(async () => {
    if (!user) return
    try {
      const data = await getNotifications()
      setNotifications(data)
    } catch {
      // ignore — user may not be authenticated yet
    }
  }, [user])

  useEffect(() => {
    if (!user) {
      setNotifications([])
      return
    }

    load()

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
  }, [user, load])

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

  return (
    <NotificationContext.Provider value={{ notifications, unreadCount, markAsRead, markAllAsRead, reload: load }}>
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
