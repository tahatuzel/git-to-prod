// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  modules: [
    '@nuxt/eslint',
    '@nuxt/ui'
  ],

  css: ['~/assets/css/main.css'],

  runtimeConfig: {
    apiBaseUrl: 'http://localhost:5157'
  },

  compatibilityDate: '2026-06-30',

  vite: {
    server: {
      proxy: {
        '/api/todo-items': {
          target: process.env.NUXT_API_BASE_URL || 'http://localhost:5157',
          changeOrigin: true
        }
      }
    }
  },

  eslint: {
    config: {
      stylistic: {
        commaDangle: 'never',
        braceStyle: '1tbs'
      }
    }
  },

  fonts: {
    provider: 'local'
  }
})
