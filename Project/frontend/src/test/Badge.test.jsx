import { describe, it, expect } from 'vitest'
import { render, screen } from '@testing-library/react'
import Badge from '../components/common/Badge'

describe('Badge', () => {
  it('renders the value as label for standard variants', () => {
    render(<Badge value="OPEN" />)
    expect(screen.getByText('OTVOREN')).toBeInTheDocument()
  })

  it('renders "Mobile Network" for MOBILE_NETWORK', () => {
    render(<Badge value="MOBILE_NETWORK" />)
    expect(screen.getByText('Mobilna mre\u017ea')).toBeInTheDocument()
  })

  it('renders "Technical Support" for TECHNICAL_SUPPORT', () => {
    render(<Badge value="TECHNICAL_SUPPORT" />)
    expect(screen.getByText('Tehni\u010dka podr\u0161ka')).toBeInTheDocument()
  })

  it('renders "Pending Close" for PENDING_CLOSE', () => {
    render(<Badge value="PENDING_CLOSE" />)
    expect(screen.getByText('\u010cEKA SE')).toBeInTheDocument()
  })

  it('applies default gray styling for unknown variant', () => {
    render(<Badge value="UNKNOWN" />)
    expect(screen.getByText('UNKNOWN')).toHaveClass('bg-gray-100')
  })

  it('applies red styling for HIGH priority', () => {
    render(<Badge value="HIGH" />)
    expect(screen.getByText('VISOK')).toHaveClass('bg-red-100')
  })

  it('applies emerald styling for OPEN status', () => {
    render(<Badge value="OPEN" />)
    expect(screen.getByText('OTVOREN')).toHaveClass('bg-emerald-100')
  })

  it('forwards extra className to the span', () => {
    render(<Badge value="LOW" className="test-class" />)
    expect(screen.getByText('NIZAK')).toHaveClass('test-class')
  })
})
