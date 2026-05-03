import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import basicSsl from '@vitejs/plugin-basic-ssl'
import tailwindcss from '@tailwindcss/vite'

export default defineConfig({
  plugins: [
    react(),
    tailwindcss(),
    basicSsl()
  ],
  server: {
    proxy: {
      '/api': {
        // Point to "https" backend endpoint
        target: 'http://localhost:7149',
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