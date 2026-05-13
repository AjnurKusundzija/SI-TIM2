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
        // Point to backend endpoint
        target: 'http://localhost:5122',
        changeOrigin: true,
        // IMPORTANT: Tells Vite to accept the local .NET self-signed certificate
        secure: false,
      },
      '/chathub': {
        target: 'http://localhost:5122',
        ws: true,
        secure: false,
      },
    },
  },
  esbuild: { jsx: 'automatic' },
  test: {
    environment: 'jsdom',
    globals: true,
    setupFiles: ['./src/test/setup.js'],
    coverage: {
      provider: 'v8',
      exclude: [
        'src/main.jsx',
        'src/App.jsx',
        'src/components/layout/**',
        'src/services/api.js',
        'socket-server.js',
        'vite.config.js',
        'eslint.config.js',
        'src/test/**',
      ],
    },
  },
})