import { describe, it, expect, vi } from 'vitest'
import { render, screen, fireEvent } from '@testing-library/react'
import Modal from '../components/common/Modal'

const baseProps = {
  isOpen: true,
  onClose: vi.fn(),
  title: 'Test Modal',
  children: <div>Modal Content</div>,
}

describe('Modal', () => {
  it('renders nothing when isOpen is false', () => {
    render(<Modal {...baseProps} isOpen={false} />)
    expect(screen.queryByText('Test Modal')).not.toBeInTheDocument()
    expect(screen.queryByText('Modal Content')).not.toBeInTheDocument()
  })

  it('renders title and children when isOpen is true', () => {
    render(<Modal {...baseProps} />)
    expect(screen.getByText('Test Modal')).toBeInTheDocument()
    expect(screen.getByText('Modal Content')).toBeInTheDocument()
  })

  it('calls onClose when the X button is clicked', () => {
    const onClose = vi.fn()
    render(<Modal {...baseProps} onClose={onClose} />)
    fireEvent.click(screen.getByRole('button'))
    expect(onClose).toHaveBeenCalledOnce()
  })

  it('calls onClose when the backdrop overlay is clicked', () => {
    const onClose = vi.fn()
    const { container } = render(<Modal {...baseProps} onClose={onClose} />)
    const backdrop = container.querySelector('.bg-black\\/50')
    fireEvent.click(backdrop)
    expect(onClose).toHaveBeenCalledOnce()
  })

  it('applies md size class by default', () => {
    const { container } = render(<Modal {...baseProps} />)
    expect(container.querySelector('.max-w-lg')).toBeInTheDocument()
  })

  it('applies correct size class for lg size', () => {
    const { container } = render(<Modal {...baseProps} size="lg" />)
    expect(container.querySelector('.max-w-2xl')).toBeInTheDocument()
  })
})