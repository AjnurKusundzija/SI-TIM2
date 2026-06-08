import { describe, it, expect } from 'vitest'
import { render, screen } from '@testing-library/react'
import SlaIndicator from '../components/common/SlaIndicator'

describe('SlaIndicator (US-115)', () => {
  // US-115: bez slaStatus — prikazuje tihu zamjensku vrijednost (zatvoreni tiketi)
  it('renders dash placeholder when slaStatus is not provided', () => {
    render(<SlaIndicator />)
    expect(screen.getByText('—')).toBeInTheDocument()
  })

  // US-115: GREEN status — nije prekoračen, prikazuje preostalo vrijeme
  it('shows remaining time when SLA is not breached (GREEN)', () => {
    render(<SlaIndicator slaStatus="GREEN" slaRemainingMinutes={90} slaIsBreached={false} />)
    expect(screen.getByText(/1h 30m/i)).toBeInTheDocument()
  })

  // US-115: RED status s prekoračenjem — prikazuje "SLA prekoračen"
  it('shows breach label when slaIsBreached is true', () => {
    render(<SlaIndicator slaStatus="RED" slaRemainingMinutes={-30} slaIsBreached={true} />)
    expect(screen.getByText(/SLA prekoračen/i)).toBeInTheDocument()
  })

  // US-115: kompaktni prikaz — prekoračenje prikazuje "Prekoračeno"
  it('compact mode shows "Prekoračeno" on breach', () => {
    render(<SlaIndicator slaStatus="RED" slaRemainingMinutes={-45} slaIsBreached={true} compact />)
    expect(screen.getByText(/Prekoračeno/i)).toBeInTheDocument()
  })

  // US-115: kompaktni prikaz — nije prekoračen, prikazuje preostalo
  it('compact mode shows remaining time when not breached', () => {
    render(<SlaIndicator slaStatus="GREEN" slaRemainingMinutes={45} slaIsBreached={false} compact />)
    expect(screen.getByText(/45 min/i)).toBeInTheDocument()
  })

  // US-115: YELLOW boja — prikazuje preostalo bez breasha
  it('shows remaining time for YELLOW status', () => {
    render(<SlaIndicator slaStatus="YELLOW" slaRemainingMinutes={120} slaIsBreached={false} />)
    expect(screen.getByText(/2h/i)).toBeInTheDocument()
  })

  // US-115: preostalo manje od 60 min — prikazuje minute
  it('shows minutes when remaining is under 60 minutes', () => {
    render(<SlaIndicator slaStatus="RED" slaRemainingMinutes={25} slaIsBreached={false} />)
    expect(screen.getByText(/25 min/i)).toBeInTheDocument()
  })

  // US-115: prekoračenje prikazuje koliko je minuta prošlo (kompakt)
  it('compact breach tooltip title shows overdue duration', () => {
    const { container } = render(
      <SlaIndicator slaStatus="RED" slaRemainingMinutes={-90} slaIsBreached={true} compact />
    )
    const el = container.querySelector('[title]')
    expect(el?.getAttribute('title')).toMatch(/Prekoračeno/)
  })
})
