import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, fireEvent } from '@testing-library/react'

import AuditLogFilters from '../pages/AuditLog/AuditLogFilters'

const FAKE_ACTION_TYPES = ['USER_LOGIN', 'USER_LOGOUT', 'TICKET_CREATED', 'TICKET_CLOSED', 'TICKET_FORWARDED']

const FAKE_USERS = [
  { id: 5, fullName: 'Ajdin Hodžić', email: 'ajdin@example.com' },
  { id: 1, fullName: 'Admin', email: 'admin@example.com' },
  { id: 3, fullName: 'Hana Hasanović', email: 'hana@example.com' },
]

const INITIAL_FILTERS = {
  search: '',
  actionType: '',
  userId: '',
  dateFrom: '',
  dateTo: '',
  page: 1,
  pageSize: 20,
}

function renderFilters(onApply = vi.fn(), onReset = vi.fn()) {
  return render(
    <AuditLogFilters
      filters={INITIAL_FILTERS}
      actionTypes={FAKE_ACTION_TYPES}
      users={FAKE_USERS}
      onApply={onApply}
      onReset={onReset}
      filterError={null}
    />
  )
}

describe('AuditLogFilters', () => {
  it('renders all filter controls', () => {
    renderFilters()

    expect(screen.getByPlaceholderText(/Pretraži po opisu/)).toBeInTheDocument()
    expect(screen.getByLabelText('Tip akcije')).toBeInTheDocument()
    expect(screen.getByLabelText('Korisnik')).toBeInTheDocument()
    expect(screen.getByLabelText('Od datuma')).toBeInTheDocument()
    expect(screen.getByLabelText('Do datuma')).toBeInTheDocument()
  })

  it('calls onApply with filters on "Primijeni" button click', () => {
    const onApply = vi.fn()
    renderFilters(onApply)

    const applyButton = screen.getByText('Primijeni')
    fireEvent.click(applyButton)

    expect(onApply).toHaveBeenCalledWith(expect.objectContaining({
      search: '',
      actionType: '',
      userId: '',
      dateFrom: '',
      dateTo: '',
    }))
  })

  it('calls onReset when "Resetuj filtere" button is clicked', () => {
    const onReset = vi.fn()
    renderFilters(vi.fn(), onReset)

    const resetButton = screen.getByText('Resetuj filtere')
    fireEvent.click(resetButton)

    expect(onReset).toHaveBeenCalled()
  })

  it('updates search filter on input change', () => {
    const onApply = vi.fn()
    renderFilters(onApply)

    const searchInput = screen.getByPlaceholderText(/Pretraži po opisu/)
    fireEvent.change(searchInput, { target: { value: 'korisnik' } })
    fireEvent.click(screen.getByText('Primijeni'))

    expect(onApply).toHaveBeenCalledWith(expect.objectContaining({
      search: 'korisnik',
    }))
  })

  it('allows action type selection', () => {
    const onApply = vi.fn()
    renderFilters(onApply)

    const actionTypeSelect = screen.getByLabelText('Tip akcije')
    fireEvent.change(actionTypeSelect, { target: { value: 'TICKET_CREATED' } })
    fireEvent.click(screen.getByText('Primijeni'))

    expect(onApply).toHaveBeenCalledWith(expect.objectContaining({
      actionType: 'TICKET_CREATED',
    }))
  })

  it('allows user selection', () => {
    const onApply = vi.fn()
    renderFilters(onApply)

    const userSelect = screen.getByLabelText('Korisnik')
    fireEvent.change(userSelect, { target: { value: '5' } })
    fireEvent.click(screen.getByText('Primijeni'))

    expect(onApply).toHaveBeenCalledWith(expect.objectContaining({
      userId: '5',
    }))
  })

  it('allows date range filtering', () => {
    const onApply = vi.fn()
    renderFilters(onApply)

    const dateFromInput = screen.getByLabelText('Od datuma')
    const dateToInput = screen.getByLabelText('Do datuma')

    fireEvent.change(dateFromInput, { target: { value: '2025-05-01' } })
    fireEvent.change(dateToInput, { target: { value: '2025-05-31' } })
    fireEvent.click(screen.getByText('Primijeni'))

    expect(onApply).toHaveBeenCalledWith(expect.objectContaining({
      dateFrom: '2025-05-01',
      dateTo: '2025-05-31',
    }))
  })

  it('displays error when dateTo < dateFrom', () => {
    const onApply = vi.fn()
    const onReset = vi.fn()
    
    render(
      <AuditLogFilters
        filters={INITIAL_FILTERS}
        actionTypes={FAKE_ACTION_TYPES}
        users={FAKE_USERS}
        onApply={onApply}
        onReset={onReset}
        filterError={'Datum "Do" mora biti nakon datuma "Od"'}
      />
    )

    expect(screen.getByText(/Datum "Do" mora biti/)).toBeInTheDocument()
  })

  it('clears all filters on reset', () => {
    const onApply = vi.fn()
    const onReset = vi.fn()
    renderFilters(onApply, onReset)

    const searchInput = screen.getByPlaceholderText(/Pretraži po opisu/)
    fireEvent.change(searchInput, { target: { value: 'test' } })

    fireEvent.click(screen.getByText('Resetuj filtere'))

    expect(onReset).toHaveBeenCalled()
    // After reset, inputs should be cleared (in next render)
  })

  it('applies filters on Enter key press in search input', () => {
    const onApply = vi.fn()
    renderFilters(onApply)

    const searchInput = screen.getByPlaceholderText(/Pretraži po opisu/)
    fireEvent.change(searchInput, { target: { value: 'test' } })
    fireEvent.keyDown(searchInput, { key: 'Enter' })

    expect(onApply).toHaveBeenCalledWith(expect.objectContaining({
      search: 'test',
    }))
  })

  it('populates action type dropdown with provided options', () => {
    renderFilters()

    const actionTypeSelect = screen.getByLabelText('Tip akcije')
    const options = actionTypeSelect.querySelectorAll('option')

    // Should have "Sve akcije" + all provided action types
    expect(options.length).toBe(FAKE_ACTION_TYPES.length + 1)
  })

  it('populates user dropdown with provided options', () => {
    renderFilters()

    const userSelect = screen.getByLabelText('Korisnik')
    const options = userSelect.querySelectorAll('option')

    // Should have "Svi korisnici" + all provided users
    expect(options.length).toBe(FAKE_USERS.length + 1)
  })
})
