import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

// Aspire service discovery: services__api__https__0 or services__api__http__0
// Fallback to standard API port from launchSettings.json
const apiTarget = process.env.services__api__https__0
  || process.env.services__api__http__0
  || 'http://localhost:5041';

// https://vite.dev/config/
export default defineConfig({
  plugins: [react(), tailwindcss()],
  server: {
    port: parseInt(process.env.PORT || '5173'),
    proxy: {
      '/api': {
        target: apiTarget,
        changeOrigin: true,
        secure: false,
      },
    },
  },
})
