import { FileText, Download, X, Loader2 } from 'lucide-react'
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
        if (cancelled) {
          URL.revokeObjectURL(url)
          return
        }
        urlForCleanup = url
        setBlobUrl(url)
      })
      .catch(() => {
        if (!cancelled) setError(true)
      })
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
        if (cancelled) {
          URL.revokeObjectURL(url)
          return
        }
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
      className="max-w-full max-h-[80vh] rounded-lg shadow-2xl object-contain"
    />
  )
}

export default function AttachmentList({ attachments }) {
  const [lightbox, setLightbox] = useState(null)
  const [downloadingId, setDownloadingId] = useState(null)

  // US-81: ako nema priloga, ne prikazujemo praznu sekciju.
  if (!attachments || attachments.length === 0) return null

  const images = attachments.filter(a => IMAGE_TYPES.includes(a.contentType?.toLowerCase()))
  const docs = attachments.filter(a => !IMAGE_TYPES.includes(a.contentType?.toLowerCase()))

  const handleDownload = async (att) => {
    setDownloadingId(att.attachmentId)
    try {
      await downloadAttachment(att.attachmentId, att.fileName)
    } catch (err) {
      // best-effort; tihi fail
      console.error('Download failed', err)
    } finally {
      setDownloadingId(null)
    }
  }

  return (
    <div className="mt-3 space-y-2" data-testid="attachment-list">
      {images.length > 0 && (
        <div className="flex flex-wrap gap-2">
          {images.map(a => (
            <div key={a.attachmentId} className="flex flex-col items-start gap-1">
              <button
                type="button"
                onClick={() => setLightbox(a)}
                className="w-20 h-20 rounded-lg border border-gray-200 overflow-hidden hover:opacity-80 transition-opacity focus:outline-none focus:ring-2 focus:ring-indigo-500"
                aria-label={`Otvori sliku ${a.fileName}`}
              >
                <AuthenticatedImage attachmentId={a.attachmentId} fileName={a.fileName} onClick={() => setLightbox(a)} />
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
              onClick={() => handleDownload(a)}
              disabled={downloadingId === a.attachmentId}
              className="inline-flex items-start gap-2 px-3 py-2 bg-gray-50 hover:bg-gray-100 border border-gray-200 rounded-lg text-sm text-gray-700 transition-colors w-fit max-w-full disabled:opacity-60"
              data-testid="attachment-doc"
              title={`Preuzmi ${a.fileName}`}
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
              {downloadingId === a.attachmentId
                ? <Loader2 size={13} className="text-gray-400 flex-shrink-0 ml-auto self-start animate-spin" />
                : <Download size={13} className="text-gray-400 flex-shrink-0 ml-auto self-start" />}
            </button>
          ))}
        </div>
      )}

      {lightbox && (
        <div
          className="fixed inset-0 bg-black/80 z-50 flex items-center justify-center p-4 backdrop-blur-sm"
          onClick={() => setLightbox(null)}
          data-testid="attachment-lightbox"
        >
          <button
            type="button"
            className="absolute top-4 right-4 text-white hover:text-gray-300 bg-black/40 p-2 rounded-full transition-colors"
            onClick={() => setLightbox(null)}
            aria-label="Zatvori"
          >
            <X size={24} />
          </button>

          <div className="relative max-w-full max-h-[85vh]" onClick={e => e.stopPropagation()}>
            <LightboxImage attachmentId={lightbox.attachmentId} fileName={lightbox.fileName} />
            <p className="mt-2 text-center text-white text-sm opacity-90 font-medium">
              {lightbox.fileName} · <span className="opacity-60">{formatSize(lightbox.size)}</span>
              {buildMeta(lightbox) && (
                <>
                  <br />
                  <span className="opacity-60 text-xs">{buildMeta(lightbox)}</span>
                </>
              )}
            </p>
          </div>
        </div>
      )}
    </div>
  )
}
