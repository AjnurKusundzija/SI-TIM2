import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'

const mocks = vi.hoisted(() => ({ adminCopilotQuery: vi.fn() }))

vi.mock('../services/aiService', () => ({
  adminCopilotQuery: mocks.adminCopilotQuery,
}))

import AdminCopilotPanel from '../components/admin/AdminCopilotPanel'

const WORKLOAD_RESPONSE = {
  answer: 'Tim Internet je trenutno najopterećeniji.',
  intent: 'team_workload',
  metrics: [
    { label: 'Najopterećeniji tim', value: 'Internet Tim' },
    { label: 'Otvoreni tiketi', value: '5' },
  ],
  recommendations: [
    { title: 'Rasteretiti tim: Internet Tim', description: 'Razmotrite preraspodjelu.', teamFilter: '1' },
  ],
  sources: [{ tool: 'team.workload' }],
  usedTools: ['team.workload', 'ticket.analytics'],
  relatedTickets: [
    { ticketId: 7, title: 'Internet ne radi', status: 'OPEN', priority: 'HIGH', minutesWithoutResponse: 200 },
  ],
  faqCoverage: [],
  message: null,
}

const FAQ_RESPONSE = {
  answer: 'Analiza FAQ pokrivenosti.',
  intent: 'faq_coverage',
  metrics: [{ label: 'Ponavljani problemi', value: '2' }],
  recommendations: [],
  sources: [{ tool: 'faq.search' }],
  usedTools: ['ticket.analytics', 'faq.search'],
  relatedTickets: [],
  faqCoverage: [
    { problem: 'internet', occurrenceCount: 4, covered: false, suggestedQuestion: 'Kako riješiti internet problem?', suggestedAnswer: 'Nacrt odgovora.' },
    { problem: 'racun', occurrenceCount: 2, covered: true, matchedFaqQuestion: 'Pogrešan iznos na računu' },
  ],
  message: null,
}

function renderPanel() {
  return render(
    <MemoryRouter>
      <AdminCopilotPanel onClose={vi.fn()} context={{}} />
    </MemoryRouter>
  )
}

describe('AdminCopilotPanel (PB-70 / US-108)', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('renderuje chat input i prijedloge pitanja', () => {
    renderPanel()
    expect(screen.getByLabelText('Pitanje za MCP Admin Copilot')).toBeInTheDocument()
    expect(screen.getByText('Koji tim je najopterećeniji?')).toBeInTheDocument()
  })

  it('slanje pitanja poziva adminCopilotQuery i prikazuje odgovor u chat formatu', async () => {
    mocks.adminCopilotQuery.mockResolvedValueOnce(WORKLOAD_RESPONSE)
    renderPanel()

    fireEvent.click(screen.getByText('Koji tim je najopterećeniji?'))

    await waitFor(() =>
      expect(mocks.adminCopilotQuery).toHaveBeenCalledWith('Koji tim je najopterećeniji?', {})
    )
    // korisnička poruka + odgovor
    expect(await screen.findByText('Tim Internet je trenutno najopterećeniji.')).toBeInTheDocument()
  })

  it('prikazuje loading stanje dok se pitanje obrađuje', async () => {
    let resolvePromise
    mocks.adminCopilotQuery.mockReturnValueOnce(new Promise((res) => { resolvePromise = res }))
    renderPanel()

    fireEvent.click(screen.getByText('Koji tim je najopterećeniji?'))

    expect(await screen.findByText(/Copilot analizira/i)).toBeInTheDocument()
    resolvePromise(WORKLOAD_RESPONSE)
    // sačekaj da se odgovor renderuje (izbjegava act() upozorenje)
    await screen.findByText('Tim Internet je trenutno najopterećeniji.')
  })

  it('prikazuje grešku kao alert kada servis nije dostupan', async () => {
    mocks.adminCopilotQuery.mockRejectedValueOnce({
      response: { data: { message: 'MCP server nije dostupan' } },
    })
    renderPanel()

    fireEvent.click(screen.getByText('Koji tim je najopterećeniji?'))

    const alert = await screen.findByRole('alert')
    expect(alert).toHaveTextContent('MCP server nije dostupan')
  })

  it('slanje preko input polja prosljeđuje upisani tekst', async () => {
    mocks.adminCopilotQuery.mockResolvedValueOnce(WORKLOAD_RESPONSE)
    renderPanel()

    const input = screen.getByLabelText('Pitanje za MCP Admin Copilot')
    fireEvent.change(input, { target: { value: 'Prikaži tikete bez odgovora' } })
    fireEvent.click(screen.getByLabelText('Pošalji'))

    await waitFor(() =>
      expect(mocks.adminCopilotQuery).toHaveBeenCalledWith('Prikaži tikete bez odgovora', {})
    )
  })

  it('US-110: prikazuje najopterećeniji tim, relevantne tikete i preporuku', async () => {
    mocks.adminCopilotQuery.mockResolvedValueOnce(WORKLOAD_RESPONSE)
    renderPanel()

    fireEvent.click(screen.getByText('Koji tim je najopterećeniji?'))

    expect(await screen.findByText('Internet Tim')).toBeInTheDocument()
    expect(screen.getByText('Rasteretiti tim: Internet Tim')).toBeInTheDocument()
    expect(screen.getByText(/Internet ne radi/)).toBeInTheDocument()
  })

  it('US-111: prikazuje listu ponavljanih problema i FAQ status', async () => {
    mocks.adminCopilotQuery.mockResolvedValueOnce(FAQ_RESPONSE)
    renderPanel()

    fireEvent.click(screen.getByText('Koji problemi se ponavljaju, a nisu pokriveni FAQ-om?'))

    expect(await screen.findByText('Nije pokriveno')).toBeInTheDocument()
    expect(screen.getByText('FAQ postoji')).toBeInTheDocument()
    expect(screen.getByText('Kako riješiti internet problem?')).toBeInTheDocument()
  })
})
