import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import basicSsl from '@vitejs/plugin-basic-ssl' // SLL plugin for https verification

export default defineConfig({
  plugins: [
    react(),
    basicSsl() // SSL
  ],
  server: {
    proxy: {
      '/api': {
        // Point to "https" backend endpoint
        target: 'https://localhost:7148',
        changeOrigin: true,
        // IMPORTANT: Tells Vite to accept the local .NET self-signed certificate
        secure: false,
      },
    },
  },
  ...(process.env.VITEST && { esbuild: { jsx: 'automatic' } }),
  test: {
    environment: 'jsdom',
    globals: true,
    setupFiles: ['./src/test/setup.js'],
  },
})