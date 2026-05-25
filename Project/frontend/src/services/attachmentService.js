import api from './api'

// PB-56 / US-81: download priloga uvijek ide kroz axios da bi se proslijedio Bearer token,
// jer plain <a href> / <img src> ne šalju Authorization header pa API vraća 401.

export async function fetchAttachmentBlobUrl(attachmentId) {
  const response = await api.get(`/attachments/${attachmentId}`, { responseType: 'blob' })
  return URL.createObjectURL(response.data)
}

export async function downloadAttachment(attachmentId, fileName) {
  const blobUrl = await fetchAttachmentBlobUrl(attachmentId)
  const link = document.createElement('a')
  link.href = blobUrl
  link.download = fileName || 'prilog'
  document.body.appendChild(link)
  link.click()
  document.body.removeChild(link)
  // Drop the blob from memory after the browser has had a moment to start the download
  setTimeout(() => URL.revokeObjectURL(blobUrl), 1000)
}
