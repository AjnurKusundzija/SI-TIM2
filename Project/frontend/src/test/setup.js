import '@testing-library/jest-dom'
import { cleanup } from '@testing-library/react'
import { afterEach } from 'vitest'

afterEach(cleanup)

// jsdom ne implementira blob URL API — neki testovi koji renderuju komponente sa attachmentima
// pozivaju URL.revokeObjectURL pri unmount-u, pa stubujemo bezbjedne no-op verzije.
if (typeof URL.createObjectURL !== 'function') {
  URL.createObjectURL = () => 'blob:mock'
}
if (typeof URL.revokeObjectURL !== 'function') {
  URL.revokeObjectURL = () => {}
}
