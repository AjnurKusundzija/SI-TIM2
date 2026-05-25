import { describe, it, expect, vi } from 'vitest'
import { render, screen, fireEvent } from '@testing-library/react'
import FileUpload from '../components/common/FileUpload'

// PB-56 / US-80: validacija formata, veličine i broja fajlova mora ostati usklađena sa backendom.
function makeFile(name, size, type = 'application/octet-stream') {
  const blob = new Blob([new Uint8Array(size)], { type })
  return new File([blob], name, { type })
}

describe('FileUpload', () => {
  it('accepts allowed formats (png, jpg, jpeg, pdf, docx, txt)', () => {
    const onFilesSelected = vi.fn()
    const { container } = render(<FileUpload onFilesSelected={onFilesSelected} />)
    const input = container.querySelector('input[type="file"]')

    const files = [
      makeFile('a.png', 100, 'image/png'),
      makeFile('b.pdf', 100, 'application/pdf'),
      makeFile('c.txt', 100, 'text/plain'),
    ]
    fireEvent.change(input, { target: { files } })

    expect(onFilesSelected).toHaveBeenCalled()
    const last = onFilesSelected.mock.calls.at(-1)[0]
    expect(last).toHaveLength(3)
  })

  it('rejects files larger than 5 MB', () => {
    const onFilesSelected = vi.fn()
    const { container } = render(<FileUpload onFilesSelected={onFilesSelected} />)
    const input = container.querySelector('input[type="file"]')

    const tooBig = makeFile('huge.png', 5 * 1024 * 1024 + 1, 'image/png')
    fireEvent.change(input, { target: { files: [tooBig] } })

    expect(screen.getByText(/prelazi maksimalnu veličinu/i)).toBeInTheDocument()
    const last = onFilesSelected.mock.calls.at(-1)[0]
    expect(last).toHaveLength(0)
  })

  it('rejects unsupported format like .zip', () => {
    const onFilesSelected = vi.fn()
    const { container } = render(<FileUpload onFilesSelected={onFilesSelected} />)
    const input = container.querySelector('input[type="file"]')

    fireEvent.change(input, { target: { files: [makeFile('arhiva.zip', 100, 'application/zip')] } })

    expect(screen.getByText(/nije podržan/i)).toBeInTheDocument()
  })

  it('explicitly blocks executable files (.exe)', () => {
    const onFilesSelected = vi.fn()
    const { container } = render(<FileUpload onFilesSelected={onFilesSelected} />)
    const input = container.querySelector('input[type="file"]')

    fireEvent.change(input, { target: { files: [makeFile('virus.exe', 100, 'application/octet-stream')] } })

    expect(screen.getByText(/izvršnih fajlova/i)).toBeInTheDocument()
  })

  it('rejects more than 5 files', () => {
    const onFilesSelected = vi.fn()
    const { container } = render(<FileUpload onFilesSelected={onFilesSelected} />)
    const input = container.querySelector('input[type="file"]')

    const files = Array.from({ length: 6 }).map((_, i) => makeFile(`f${i}.png`, 100, 'image/png'))
    fireEvent.change(input, { target: { files } })

    expect(screen.getByText(/Maksimalan broj fajlova/i)).toBeInTheDocument()
  })

  it('rejects .xlsx (sklonjeno iz dozvoljenih formata)', () => {
    const onFilesSelected = vi.fn()
    const { container } = render(<FileUpload onFilesSelected={onFilesSelected} />)
    const input = container.querySelector('input[type="file"]')

    fireEvent.change(input, {
      target: { files: [makeFile('tabela.xlsx', 100, 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet')] }
    })

    expect(screen.getByText(/nije podržan/i)).toBeInTheDocument()
  })
})
