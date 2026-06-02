import { FileText, Download, X, Loader2, Paperclip, ChevronLeft, ChevronRight } from 'lucide-react'
import { useEffect, useState } from 'react'
import { fetchAttachmentBlobUrl, downloadAttachment } from '../../services/attachmentService'

const IMAGE_TYPES = ['image/png', 'image/jpeg', 'image/jpg']

function formatSize(bytes) {
  if (bytes == null) return ''
  if (bytes < 1024) return `${bytes} B`
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`
  return `${(bytes / (1024 * 1024)).toFixed(1)} MB`
}

function formatDate(value) {
  if (!value) return ''
  try {
    const d = new Date(value)
    if (Number.isNaN(d.getTime())) return ''
    return d.toLocaleString()
  } catch {
    return ''
  }
}

function buildMeta(att) {
  const parts = []
  if (att.uploadedByName) parts.push(att.uploadedByName)
  const when = formatDate(att.uploadedAt)
  if (when) parts.push(when)
  return parts.join(' · ')
}

// PB-56: thumbnail mora ići kroz autentifikovani axios fetch (zbog Bearer tokena),
// pa privremeno učitavamo blob URL i koristimo ga kao <img src>.
function AuthenticatedImage({ attachmentId, fileName, onClick }) {
  const [blobUrl, setBlobUrl] = useState(null)
  const [error, setError] = useState(false)

  useEffect(() => {
    let cancelled = false
    let urlForCleanup = null
    fetchAttachmentBlobUrl(attachmentId)
      .then(url => {
        if (cancelled) { URL.revokeObjectURL(url); return }
        urlForCleanup = url
        setBlobUrl(url)
      })
      .catch(() => { if (!cancelled) setError(true) })
    return () => {
      cancelled = true
      if (urlForCleanup) URL.revokeObjectURL(urlForCleanup)
    }
  }, [attachmentId])

  if (error) {
    return (
      <div className="w-full h-full flex items-center justify-center text-[10px] text-red-500 bg-red-50">
        Greška
      </div>
    )
  }
  if (!blobUrl) {
    return (
      <div className="w-full h-full flex items-center justify-center bg-gray-100">
        <Loader2 size={16} className="animate-spin text-gray-400" />
      </div>
    )
  }
  return (
    <img
      src={blobUrl}
      alt={fileName}
      className="w-full h-full object-cover cursor-pointer"
      onClick={onClick}
      data-testid="attachment-thumbnail"
    />
  )
}

function LightboxImage({ attachmentId, fileName }) {
  const [blobUrl, setBlobUrl] = useState(null)

  useEffect(() => {
    let cancelled = false
    let urlForCleanup = null
    fetchAttachmentBlobUrl(attachmentId)
      .then(url => {
        if (cancelled) { URL.revokeObjectURL(url); return }
        urlForCleanup = url
        setBlobUrl(url)
      })
      .catch(() => {})
    return () => {
      cancelled = true
      if (urlForCleanup) URL.revokeObjectURL(urlForCleanup)
    }
  }, [attachmentId])

  if (!blobUrl) {
    return (
      <div className="flex items-center justify-center w-64 h-64">
        <Loader2 size={32} className="animate-spin text-white/70" />
      </div>
    )
  }
  return (
    <img
      src={blobUrl}
      alt={fileName}
      className="max-w-full max-h-[75vh] rounded-lg shadow-2xl object-contain"
    />
  )
}

function PdfViewer({ attachmentId, fileName, onDownload }) {
  const [blobUrl, setBlobUrl] = useState(null)
  const [error, setError] = useState(false)

  useEffect(() => {
    let cancelled = false
    let urlForCleanup = null
    fetchAttachmentBlobUrl(attachmentId)
      .then(url => {
        if (cancelled) { URL.revokeObjectURL(url); return }
        urlForCleanup = url
        setBlobUrl(url)
      })
      .catch(() => { if (!cancelled) setError(true) })
    return () => {
      cancelled = true
      if (urlForCleanup) URL.revokeObjectURL(urlForCleanup)
    }
  }, [attachmentId])

  if (!blobUrl && !error) {
    return (
      <div className="flex items-center justify-center w-64 h-64">
        <Loader2 size={32} className="animate-spin text-white/70" />
      </div>
    )
  }
  if (error) {
    return (
      <div className="flex flex-col items-center gap-4 text-center">
        <FileText size={48} className="text-gray-400" />
        <p className="text-white font-medium">{fileName}</p>
        <p className="text-gray-400 text-sm">PDF ne može biti prikazan</p>
        <button
          type="button"
          onClick={onDownload}
          className="inline-flex items-center gap-2 px-4 py-2 bg-white text-gray-900 rounded-lg font-medium text-sm hover:bg-gray-100 transition-colors"
        >
          <Download size={16} /> Preuzmi
        </button>
      </div>
    )
  }
  return (
    <iframe
      src={blobUrl}
      title={fileName}
      className="w-[min(860px,90vw)] h-[72vh] rounded-lg bg-white"
      onError={() => setError(true)}
    />
  )
}

function NonViewable({ att, onDownload }) {
  return (
    <div className="flex flex-col items-center gap-4 text-center px-6">
      <FileText size={52} className="text-gray-400" />
      <p className="text-white font-medium text-base">{att.fileName}</p>
      {att.size && <p className="text-gray-400 text-sm">{formatSize(att.size)}</p>}
      <p className="text-gray-500 text-sm">Ovaj tip fajla ne može biti prikazan</p>
      <button
        type="button"
        onClick={onDownload}
        className="inline-flex items-center gap-2 px-5 py-2.5 bg-white text-gray-900 rounded-lg font-medium text-sm hover:bg-gray-100 transition-colors"
      >
        <Download size={16} /> Preuzmi fajl
      </button>
    </div>
  )
}

function LightboxContent({ att }) {
  const isImage = IMAGE_TYPES.includes(att.contentType?.toLowerCase())
  const isPdf = att.contentType?.toLowerCase() === 'application/pdf'

  const handleDownload = async () => {
    try { await downloadAttachment(att.attachmentId, att.fileName) } catch { /* tihi fail */ }
  }

  if (isImage) return <LightboxImage attachmentId={att.attachmentId} fileName={att.fileName} />
  if (isPdf) return <PdfViewer attachmentId={att.attachmentId} fileName={att.fileName} onDownload={handleDownload} />
  return <NonViewable att={att} onDownload={handleDownload} />
}

export function AttachmentLightbox({ attachments, index, onClose, onChange }) {
  const att = attachments[index]
  const hasPrev = index > 0
  const hasNext = index < attachments.length - 1

  useEffect(() => {
    const handleKey = (e) => {
      if (e.key === 'ArrowLeft' && hasPrev) onChange(index - 1)
      if (e.key === 'ArrowRight' && hasNext) onChange(index + 1)
      if (e.key === 'Escape') onClose()
    }
    window.addEventListener('keydown', handleKey)
    return () => window.removeEventListener('keydown', handleKey)
  }, [index, hasPrev, hasNext, onChange, onClose])

  return (
    <div
      className="fixed inset-0 bg-black/85 z-50 flex flex-col items-center justify-center backdrop-blur-sm p-4"
      onClick={onClose}
      data-testid="attachment-lightbox"
    >
      <button
        type="button"
        className="absolute top-4 right-4 text-white hover:text-gray-300 bg-black/40 p-2 rounded-full transition-colors"
        onClick={onClose}
        aria-label="Zatvori"
      >
        <X size={24} />
      </button>

      {attachments.length > 1 && (
        <div className="absolute top-4 left-1/2 -translate-x-1/2 text-white/60 text-sm select-none">
          {index + 1} / {attachments.length}
        </div>
      )}

      {hasPrev && (
        <button
          type="button"
          className="absolute left-3 top-1/2 -translate-y-1/2 text-white hover:text-gray-200 bg-black/40 hover:bg-black/60 p-2.5 rounded-full transition-colors"
          onClick={(e) => { e.stopPropagation(); onChange(index - 1) }}
          aria-label="Prethodni prilog"
        >
          <ChevronLeft size={24} />
        </button>
      )}

      <div className="flex flex-col items-center max-w-full" onClick={e => e.stopPropagation()}>
        <LightboxContent att={att} />
        <p className="mt-3 text-center text-white text-sm opacity-80 font-medium">
          {att.fileName}
          {att.size ? <> · <span className="opacity-60">{formatSize(att.size)}</span></> : null}
          {buildMeta(att) && (
            <><br /><span className="opacity-50 text-xs">{buildMeta(att)}</span></>
          )}
        </p>
      </div>

      {hasNext && (
        <button
          type="button"
          className="absolute right-3 top-1/2 -translate-y-1/2 text-white hover:text-gray-200 bg-black/40 hover:bg-black/60 p-2.5 rounded-full transition-colors"
          onClick={(e) => { e.stopPropagation(); onChange(index + 1) }}
          aria-label="Sljedeći prilog"
        >
          <ChevronRight size={24} />
        </button>
      )}
    </div>
  )
}

export function FilesDrivePanel({ attachments, onOpenAttachment }) {
  if (!attachments || attachments.length === 0) return null

  const images = attachments.filter(a => IMAGE_TYPES.includes(a.contentType?.toLowerCase()))
  const docs = attachments.filter(a => !IMAGE_TYPES.includes(a.contentType?.toLowerCase()))

  return (
    <div className="px-5 py-4">
      <p className="text-[10px] font-semibold uppercase tracking-wider text-gray-400 mb-3 flex items-center gap-1">
        <Paperclip size={10} />
        Svi prilozi ({attachments.length})
      </p>

      {images.length > 0 && (
        <div className={docs.length > 0 ? 'mb-3' : ''}>
          <div className="grid grid-cols-3 gap-1.5">
            {images.map(a => (
              <div key={a.attachmentId} className="flex flex-col gap-0.5">
                <button
                  type="button"
                  onClick={() => onOpenAttachment(a.attachmentId)}
                  className="aspect-square rounded-md border border-gray-200 overflow-hidden hover:opacity-80 transition-opacity focus:outline-none focus:ring-2 focus:ring-navy-500"
                  aria-label={`Otvori sliku ${a.fileName}`}
                >
                  <AuthenticatedImage attachmentId={a.attachmentId} fileName={a.fileName} />
                </button>
                <span className="text-[10px] text-gray-500 truncate leading-tight" title={a.fileName}>{a.fileName}</span>
                {a.sourceName && (
                  <span className="text-[9px] text-gray-400 truncate">{a.sourceName}</span>
                )}
              </div>
            ))}
          </div>
        </div>
      )}

      {docs.length > 0 && (
        <div className="flex flex-col gap-1">
          {docs.map(a => (
            <button
              key={a.attachmentId}
              type="button"
              onClick={() => onOpenAttachment(a.attachmentId)}
              className="flex items-center gap-2 px-2.5 py-2 bg-gray-50 hover:bg-gray-100 border border-gray-200 rounded-lg text-left transition-colors group w-full"
              title={a.fileName}
            >
              <FileText size={14} className="text-gray-400 flex-shrink-0" />
              <div className="flex-1 min-w-0">
                <p className="text-xs font-medium text-gray-700 truncate">{a.fileName}</p>
                <p className="text-[10px] text-gray-400 truncate">
                  {formatSize(a.size)}{a.sourceName ? ` · ${a.sourceName}` : ''}
                </p>
              </div>
              <ChevronRight size={12} className="text-gray-300 flex-shrink-0 opacity-0 group-hover:opacity-100 transition-opacity" />
            </button>
          ))}
        </div>
      )}
    </div>
  )
}

export default function AttachmentList({ attachments, onOpenAttachment }) {
  // US-81: ako nema priloga, ne prikazujemo praznu sekciju.
  if (!attachments || attachments.length === 0) return null

  const images = attachments.filter(a => IMAGE_TYPES.includes(a.contentType?.toLowerCase()))
  const docs = attachments.filter(a => !IMAGE_TYPES.includes(a.contentType?.toLowerCase()))

  return (
    <div className="mt-3 space-y-2" data-testid="attachment-list">
      {images.length > 0 && (
        <div className="flex flex-wrap gap-2">
          {images.map(a => (
            <div key={a.attachmentId} className="flex flex-col items-start gap-1">
              <button
                type="button"
                onClick={() => onOpenAttachment(a.attachmentId)}
                className="w-20 h-20 rounded-lg border border-gray-200 overflow-hidden hover:opacity-80 transition-opacity focus:outline-none focus:ring-2 focus:ring-indigo-500"
                aria-label={`Otvori sliku ${a.fileName}`}
              >
                <AuthenticatedImage attachmentId={a.attachmentId} fileName={a.fileName} />
              </button>
              <div className="text-[10px] text-gray-500 max-w-[80px] truncate" title={a.fileName}>
                {a.fileName}
              </div>
              {buildMeta(a) && (
                <div
                  className="text-[10px] text-gray-400 max-w-[140px] truncate"
                  title={buildMeta(a)}
                  data-testid="attachment-meta"
                >
                  {buildMeta(a)}
                </div>
              )}
            </div>
          ))}
        </div>
      )}

      {docs.length > 0 && (
        <div className="flex flex-col gap-1.5">
          {docs.map(a => (
            <button
              key={a.attachmentId}
              type="button"
              onClick={() => onOpenAttachment(a.attachmentId)}
              className="inline-flex items-start gap-2 px-3 py-2 bg-gray-50 hover:bg-gray-100 border border-gray-200 rounded-lg text-sm text-gray-700 transition-colors w-fit max-w-full"
              data-testid="attachment-doc"
              title={a.fileName}
            >
              <FileText size={14} className="text-gray-400 flex-shrink-0 mt-0.5" />
              <div className="flex flex-col min-w-0 items-start">
                <span className="truncate max-w-[260px] md:max-w-[360px]" title={a.fileName}>
                  {a.fileName}
                </span>
                {buildMeta(a) && (
                  <span className="text-xs text-gray-400 mt-0.5 truncate" data-testid="attachment-meta">
                    {buildMeta(a)}
                  </span>
                )}
              </div>
              <span className="text-xs text-gray-400 ml-2 flex-shrink-0 self-start">{formatSize(a.size)}</span>
            </button>
          ))}
        </div>
      )}
    </div>
  )
}
