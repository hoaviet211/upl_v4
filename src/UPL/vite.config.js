import { defineConfig } from 'vite'
import { resolve } from 'node:path'

// Build directly into ASP.NET Core wwwroot for easy serving
export default defineConfig({
  base: '/dist/',
  root: '.',
  publicDir: false,
  build: {
    outDir: 'wwwroot/dist',
    emptyOutDir: true,
    sourcemap: true,
    rollupOptions: {
      input: {
        main: resolve(__dirname, 'frontend/main.js')
      },
      output: {
        // Stable filenames so we can reference without manifest plumbing
        entryFileNames: 'assets/[name].js',
        chunkFileNames: 'assets/[name].js',
        assetFileNames: ({ name }) => {
          return 'assets/[name][extname]'
        }
      }
    }
  },
  server: {
    port: 5173,
    strictPort: true
  }
})
