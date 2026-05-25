import { FileText, Download, X } from 'lucide-react'
import { useState } from 'react'

const IMAGE_TYPES = ['image/png', 'image/jpeg', 'image/jpg']

function formatSize(bytes) {
  if (bytes < 1024) return `${bytes} B`
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`
  return `${(bytes / (1024 * 1024)).toFixed(1)} MB`
}

export default function AttachmentList({ attachments }) {
  const [lightbox, setLightbox] = useState(null)

  if (!attachments || attachments.length === 0) return null

  const images = attachments.filter(a => IMAGE_TYPES.includes(a.contentType?.toLowerCase()))
  const docs = attachments.filter(a => !IMAGE_TYPES.includes(a.contentType?.toLowerCase()))

  return (
    <div className="mt-3 space-y-2">
      {/* Slike — thumbnails */}
      {images.length > 0 && (
        <div className="flex flex-wrap gap-2">
          {images.map(a => (
            <button
              key={a.attachmentId}
              type="button"
              onClick={() => setLightbox(a)}
              className="w-20 h-20 rounded-lg border border-gray-200 overflow-hidden hover:opacity-80 transition-opacity focus:outline-none focus:ring-2 focus:ring-indigo-500"
            >
              <img
                src={a.downloadUrl}
                alt={a.fileName}
                className="w-full h-full object-cover"
              />
            </button>
          ))}
        </div>
      )}

      {/* Dokumenti */}
      {docs.length > 0 && (
        <div className="flex flex-col gap-1.5">
          {docs.map(a => (
            <a
              key={a.attachmentId}
              href={a.downloadUrl}
              download={a.fileName}
              target="_blank"
              rel="noopener noreferrer"
              className="inline-flex items-center gap-2 px-3 py-2 bg-gray-50 hover:bg-gray-100 border border-gray-200 rounded-lg text-sm text-gray-700 transition-colors w-fit max-w-full"
            >
              <FileText size={14} className="text-gray-400 flex-shrink-0" />
              <span className="truncate max-w-[200px] md:max-w-[300px]" title={a.fileName}>
                {a.fileName}
              </span>
              <span className="text-xs text-gray-400 ml-1 flex-shrink-0">{formatSize(a.size)}</span>
              <Download size={13} className="text-gray-400 flex-shrink-0 ml-auto" />
            </a>
          ))}
        </div>
      )}

      {/* Lightbox za slike */}
      {lightbox && (
        <div
          className="fixed inset-0 bg-black/80 z-50 flex items-center justify-center p-4 backdrop-blur-sm"
          onClick={() => setLightbox(null)}
        >
          <button
            type="button"
            className="absolute top-4 right-4 text-white hover:text-gray-300 bg-black/40 p-2 rounded-full transition-colors"
            onClick={() => setLightbox(null)}
          >
            <X size={24} />
          </button>
          
          <div className="relative max-w-full max-h-[85vh]" onClick={e => e.stopPropagation()}>
            <img
              src={lightbox.downloadUrl}
              alt={lightbox.fileName}
              className="max-w-full max-h-[80vh] rounded-lg shadow-2xl object-contain"
            />
            <p className="mt-2 text-center text-white text-sm opacity-90 font-medium">
              {lightbox.fileName} · <span className="opacity-60">{formatSize(lightbox.size)}</span>
            </p>
          </div>
        </div>
      )}
    </div>
  )
}