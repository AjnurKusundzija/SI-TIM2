import { describe, it, expect, vi } from 'vitest'
import { render, screen, fireEvent } from '@testing-library/react'
import EmptyState from '../components/common/EmptyState'

describe('EmptyState', () => {
  it('renders default title and description', () => {
    render(<EmptyState />)
    expect(screen.getByText('Nema podataka')).toBeInTheDocument()
    expect(screen.getByText('Trenutno nema ništa za prikaz.')).toBeInTheDocument()
  })

  it('renders custom title and description', () => {
    render(<EmptyState title="Nema tiketa" description="Nemate otvorenih tiketa." />)
    expect(screen.getByText('Nema tiketa')).toBeInTheDocument()
    expect(screen.getByText('Nemate otvorenih tiketa.')).toBeInTheDocument()
  })

  it('does not render action button when action prop is missing', () => {
    render(<EmptyState />)
    expect(screen.queryByRole('button')).not.toBeInTheDocument()
  })

  it('renders action button when action and actionLabel are provided', () => {
    render(<EmptyState action={vi.fn()} actionLabel="Kreiraj tiket" />)
    expect(screen.getByRole('button', { name: 'Kreiraj tiket' })).toBeInTheDocument()
  })

  it('calls action callback when button is clicked', () => {
    const action = vi.fn()
    render(<EmptyState action={action} actionLabel="Kreiraj tiket" />)
    fireEvent.click(screen.getByRole('button'))
    expect(action).toHaveBeenCalledOnce()
  })
})
