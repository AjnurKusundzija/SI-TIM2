import { describe, it, expect, vi } from 'vitest'
import { render, screen, fireEvent } from '@testing-library/react'
import StarRating from '../components/common/StarRating'

describe('StarRating', () => {
  it('renders 5 star buttons', () => {
    render(<StarRating />)
    expect(screen.getAllByRole('button')).toHaveLength(5)
  })

  it('calls onChange with the correct star value when clicked', () => {
    const onChange = vi.fn()
    render(<StarRating onChange={onChange} />)
    fireEvent.click(screen.getAllByRole('button')[2]) // 3rd star → value 3
    expect(onChange).toHaveBeenCalledWith(3)
  })

  it('calls onChange with 1 when first star is clicked', () => {
    const onChange = vi.fn()
    render(<StarRating onChange={onChange} />)
    fireEvent.click(screen.getAllByRole('button')[0])
    expect(onChange).toHaveBeenCalledWith(1)
  })

  it('calls onChange with 5 when last star is clicked', () => {
    const onChange = vi.fn()
    render(<StarRating onChange={onChange} />)
    fireEvent.click(screen.getAllByRole('button')[4])
    expect(onChange).toHaveBeenCalledWith(5)
  })

  it('does not call onChange when readonly', () => {
    const onChange = vi.fn()
    render(<StarRating onChange={onChange} readonly />)
    fireEvent.click(screen.getAllByRole('button')[0])
    expect(onChange).not.toHaveBeenCalled()
  })

  it('disables all buttons when readonly', () => {
    render(<StarRating readonly />)
    screen.getAllByRole('button').forEach(btn => expect(btn).toBeDisabled())
  })
})
