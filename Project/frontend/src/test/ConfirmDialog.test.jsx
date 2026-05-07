import { describe, it, expect, vi } from 'vitest'
import { render, screen, fireEvent } from '@testing-library/react'
import ConfirmDialog from '../components/common/ConfirmDialog'

const baseProps = {
  isOpen: true,
  onClose: vi.fn(),
  onConfirm: vi.fn(),
  title: 'Potvrdi akciju',
  message: 'Da li ste sigurni?',
  confirmText: 'Potvrdi',
  cancelText: 'Odustani',
}

describe('ConfirmDialog', () => {
  it('does not render when isOpen is false', () => {
    render(<ConfirmDialog {...baseProps} isOpen={false} />)
    expect(screen.queryByText('Da li ste sigurni?')).not.toBeInTheDocument()
  })

  it('renders title and message', () => {
    render(<ConfirmDialog {...baseProps} />)
    expect(screen.getByText('Potvrdi akciju')).toBeInTheDocument()
    expect(screen.getByText('Da li ste sigurni?')).toBeInTheDocument()
  })

  it('renders custom confirm and cancel text', () => {
    render(<ConfirmDialog {...baseProps} />)
    expect(screen.getByRole('button', { name: 'Potvrdi' })).toBeInTheDocument()
    expect(screen.getByRole('button', { name: 'Odustani' })).toBeInTheDocument()
  })

  it('calls onConfirm and onClose when confirm button is clicked', () => {
    const onClose = vi.fn()
    const onConfirm = vi.fn()
    render(<ConfirmDialog {...baseProps} onClose={onClose} onConfirm={onConfirm} />)
    fireEvent.click(screen.getByRole('button', { name: 'Potvrdi' }))
    expect(onConfirm).toHaveBeenCalledOnce()
    expect(onClose).toHaveBeenCalledOnce()
  })

  it('calls onClose when cancel button is clicked', () => {
    const onClose = vi.fn()
    render(<ConfirmDialog {...baseProps} onClose={onClose} />)
    fireEvent.click(screen.getByRole('button', { name: 'Odustani' }))
    expect(onClose).toHaveBeenCalledOnce()
  })

  it('applies red button class for danger variant', () => {
    render(<ConfirmDialog {...baseProps} variant="danger" />)
    expect(screen.getByRole('button', { name: 'Potvrdi' })).toHaveClass('bg-red-600')
  })

  it('applies navy button class for primary variant', () => {
    render(<ConfirmDialog {...baseProps} variant="primary" />)
    expect(screen.getByRole('button', { name: 'Potvrdi' })).toHaveClass('bg-navy-700')
  })
})
