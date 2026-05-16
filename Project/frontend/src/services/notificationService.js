import * as signalR from '@microsoft/signalr'
import api from './api'

export async function getNotifications() {
  const response = await api.get('/notifications')
  return response.data
}

export async function markAsRead(notificationId) {
  await api.post(`/notifications/${notificationId}/read`)
}

export async function markAllAsRead() {
  await api.post('/notifications/read-all')
}

export function createNotificationConnection() {
  const token = sessionStorage.getItem('accessToken') ?? ''
  return new signalR.HubConnectionBuilder()
    .withUrl('/notificationhub', {
      accessTokenFactory: () => token,
    })
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Warning)
    .build()
}
