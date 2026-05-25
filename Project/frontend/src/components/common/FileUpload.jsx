import { useState, useRef } from 'react'
import { Upload, X, AlertCircle, CheckCircle, Loader } from 'lucide-react'

const MAX_FILE_SIZE = 5 * 1024 * 1024 // 5 MB
const MAX_FILES = 5
const ALLOWED_EXTENSIONS = ['.png', '.jpg', '.jpeg', '.pdf', '.docx', '.txt']
const FORBIDDEN_EXTENSIONS = ['.exe', '.bat', '.sh', '.cmd', '.com']

function formatSize(bytes) {
  if (bytes < 1024) return `${bytes} B`
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`
  return `${(bytes / (1024 * 1024)).toFixed(1)} MB`
}

export default function FileUpload({ onFilesSelected, maxFiles = MAX_FILES, compact = false }) {
  const [files, setFiles] = useState([])
  const [errors, setErrors] = useState([])
  const fileInputRef = useRef(null)

  const validateFile = (file) => {
    const fileErrors = []
    
    // Check file size
    if (file.size > MAX_FILE_SIZE) {
      fileErrors.push(`Fajl '${file.name}' prelazi maksimalnu veličinu od 5 MB`)
    }

    // Check extension
    const extension = file.name.substring(file.name.lastIndexOf('.')).toLowerCase()
    
    if (FORBIDDEN_EXTENSIONS.includes(extension)) {
      fileErrors.push(`Upload izvršnih fajlova (${extension}) je zabranjen`)
    } else if (!ALLOWED_EXTENSIONS.includes(extension)) {
      fileErrors.push(`Format '${extension}' nije podržan. Dozvoljeni: PNG, JPG, JPEG, PDF, DOCX, TXT`)
    }

    return fileErrors
  }

  const handleFileChange = (e) => {
    const selectedFiles = Array.from(e.target.files || [])
    const newErrors = []
    const validFiles = []

    // Check total number of files
    if (files.length + selectedFiles.length > maxFiles) {
      newErrors.push(`Maksimalan broj fajlova je ${maxFiles}`)
    } else {
      selectedFiles.forEach(file => {
        const fileErrors = validateFile(file)
        if (fileErrors.length > 0) {
          newErrors.push(...fileErrors)
        } else {
          validFiles.push(file)
        }
      })
    }

    const updatedFiles = [...files, ...validFiles]
    setFiles(updatedFiles)
    setErrors(newErrors)
    onFilesSelected(updatedFiles)

    // Reset input
    if (fileInputRef.current) {
      fileInputRef.current.value = ''
    }
  }

  const handleRemoveFile = (index) => {
    const updatedFiles = files.filter((_, i) => i !== index)
    setFiles(updatedFiles)
    setErrors([])
    onFilesSelected(updatedFiles)
  }

  const handleDragOver = (e) => {
    e.preventDefault()
    e.stopPropagation()
  }

  const handleDrop = (e) => {
    e.preventDefault()
    e.stopPropagation()
    
    const droppedFiles = Array.from(e.dataTransfer.files || [])
    const fileInputEvent = new DataTransfer()
    droppedFiles.forEach(file => fileInputEvent.items.add(file))
    
    if (fileInputRef.current) {
      fileInputRef.current.files = fileInputEvent.files
      handleFileChange({ target: fileInputRef.current })
    }
  }

  if (compact) {
    return (
      <div className="space-y-2">
        <div
          onClick={() => fileInputRef.current?.click()}
          onDragOver={handleDragOver}
          onDrop={handleDrop}
          className="border-2 border-dashed border-gray-300 rounded-lg p-3 cursor-pointer hover:border-navy-500 hover:bg-navy-50 transition-colors"
        >
          <div className="flex items-center gap-2 justify-center">
            <Upload size={16} className="text-gray-400" />
            <span className="text-sm text-gray-600">
              {files.length === 0 ? 'Klikni ili povuci fajlove' : `${files.length} fajl(ova)`}
            </span>
          </div>
          <input
            ref={fileInputRef}
            type="file"
            multiple
            onChange={handleFileChange}
            className="hidden"
            accept={ALLOWED_EXTENSIONS.join(',')}
          />
        </div>

        {errors.length > 0 && (
          <div className="bg-red-50 border border-red-200 rounded-lg p-2 text-xs text-red-700">
            {errors.map((err, i) => (
              <div key={i} className="flex items-start gap-2">
                <AlertCircle size={12} className="mt-0.5 flex-shrink-0" />
                <span>{err}</span>
              </div>
            ))}
          </div>
        )}

        {files.length > 0 && (
          <div className="space-y-1.5">
            {files.map((file, i) => (
              <div key={i} className="flex items-center justify-between bg-gray-50 p-2 rounded-lg border border-gray-200">
                <span className="text-xs text-gray-600 truncate">{file.name}</span>
                <div className="flex items-center gap-2">
                  <span className="text-xs text-gray-400">{formatSize(file.size)}</span>
                  <button
                    type="button"
                    onClick={() => handleRemoveFile(i)}
                    className="text-gray-400 hover:text-red-500 transition-colors"
                  >
                    <X size={14} />
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    )
  }

  return (
    <div className="space-y-4">
      <div
        onClick={() => fileInputRef.current?.click()}
        onDragOver={handleDragOver}
        onDrop={handleDrop}
        className="border-2 border-dashed border-gray-300 rounded-lg p-6 cursor-pointer hover:border-navy-500 hover:bg-navy-50 transition-colors text-center"
      >
        <Upload size={32} className="mx-auto text-gray-400 mb-2" />
        <p className="text-sm font-medium text-gray-700">Klikni ili povuci fajlove ovdje</p>
        <p className="text-xs text-gray-500 mt-1">
          PNG, JPG, JPEG, PDF, DOCX, TXT - maks. 5 MB po fajlu ({files.length}/{maxFiles})
        </p>
        <input
          ref={fileInputRef}
          type="file"
          multiple
          onChange={handleFileChange}
          className="hidden"
          accept={ALLOWED_EXTENSIONS.join(',')}
        />
      </div>

      {errors.length > 0 && (
        <div className="bg-red-50 border border-red-200 rounded-lg p-3 space-y-1">
          {errors.map((err, i) => (
            <div key={i} className="flex items-start gap-2 text-sm text-red-700">
              <AlertCircle size={14} className="mt-0.5 flex-shrink-0" />
              <span>{err}</span>
            </div>
          ))}
        </div>
      )}

      {files.length > 0 && (
        <div className="space-y-2">
          <h4 className="text-sm font-medium text-gray-700">Odabrani fajlovi:</h4>
          {files.map((file, i) => (
            <div key={i} className="flex items-center justify-between bg-gray-50 p-3 rounded-lg border border-gray-200">
              <div className="flex-1">
                <p className="text-sm text-gray-700 font-medium truncate">{file.name}</p>
                <p className="text-xs text-gray-500">{formatSize(file.size)}</p>
              </div>
              <button
                type="button"
                onClick={() => handleRemoveFile(i)}
                className="ml-3 text-gray-400 hover:text-red-500 transition-colors flex-shrink-0"
              >
                <X size={18} />
              </button>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}
