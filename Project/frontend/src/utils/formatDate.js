// Shared date formatting utilities — avoids relying on locale availability (bs-BA not universally supported)

function pad(n) {
  return String(n).padStart(2, '0')
}

// Returns "15.01.2024. 10:30"
export function formatDateTime(dateStr) {
  if (!dateStr) return '—'
  const d = new Date(dateStr)
  if (isNaN(d)) return '—'
  return `${pad(d.getDate())}.${pad(d.getMonth() + 1)}.${d.getFullYear()}. ${pad(d.getHours())}:${pad(d.getMinutes())}`
}

// Returns "15.01.2024."
export function formatDateOnly(dateStr) {
  if (!dateStr) return '—'
  const d = new Date(dateStr)
  if (isNaN(d)) return '—'
  return `${pad(d.getDate())}.${pad(d.getMonth() + 1)}.${d.getFullYear()}.`
}

// Returns "prije 5 min", "prije 2h", "prije 3d", "upravo"
export function timeAgo(dateStr) {
  if (!dateStr) return ''
  const diff = Date.now() - new Date(dateStr).getTime()
  const m = Math.floor(diff / 60000)
  if (m < 1) return 'upravo'
  if (m < 60) return `prije ${m} min`
  const h = Math.floor(m / 60)
  if (h < 24) return `prije ${h}h`
  const d = Math.floor(h / 24)
  return `prije ${d}d`
}