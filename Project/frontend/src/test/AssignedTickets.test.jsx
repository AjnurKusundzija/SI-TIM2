import { describe, it, expect, vi } from 'vitest'
import { render, screen } from '@testing-library/react'
import { BrowserRouter } from 'react-router-dom'
import AssignedTickets from '../pages/AssignedTickets'
import Tickets from '../pages/Tickets'

vi.mock('../pages/Tickets', () => ({
  default: vi.fn(({ assignedOnly }) => (
    <div>Mocked Tickets Component - assignedOnly: {assignedOnly ? 'true' : 'false'}</div>
  ))
}))

describe('AssignedTickets', () => {
  it('should render Tickets component with assignedOnly prop set to true', () => {
    render(
      <BrowserRouter>
        <AssignedTickets />
      </BrowserRouter>
    )

    expect(screen.getByText(/assignedOnly: true/i)).toBeInTheDocument()
    expect(Tickets).toHaveBeenCalledTimes(1)
  })

  it('should pass assignedOnly=true prop to Tickets component', () => {
    render(
      <BrowserRouter>
        <AssignedTickets />
      </BrowserRouter>
    )

    const calls = Tickets.mock.calls
    expect(calls.length).toBeGreaterThan(0)
    expect(calls[0][0].assignedOnly).toBe(true)
  })
})
