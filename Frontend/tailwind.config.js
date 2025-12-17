/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {
      colors: {
        primary: '#2563eb',
        'primary-dark': '#1e40af',
        'primary-light': '#3b82f6',
      },
      fontFamily: {
        dari: ['Gulzar', 'IranNastaliq', 'NotoSansArabic', 'sans-serif'],
        'dari-display': ['Gulzar', 'IranNastaliq', 'sans-serif'],
        'dari-body': ['NotoSansArabic', 'Gulzar', 'sans-serif'],
      },
      fontSize: {
        'dari-xs': ['12px', { lineHeight: '1.6', letterSpacing: '0.3px' }],
        'dari-sm': ['13px', { lineHeight: '1.7', letterSpacing: '0.2px' }],
        'dari-base': ['14px', { lineHeight: '1.8', letterSpacing: '0.15px' }],
        'dari-lg': ['15px', { lineHeight: '1.8', letterSpacing: '0.1px' }],
        'dari-xl': ['16px', { lineHeight: '1.8', letterSpacing: '0px' }],
        'dari-2xl': ['18px', { lineHeight: '1.7', letterSpacing: '-0.1px' }],
        'dari-3xl': ['20px', { lineHeight: '1.6', letterSpacing: '-0.2px' }],
        'dari-4xl': ['24px', { lineHeight: '1.5', letterSpacing: '-0.3px' }],
        'dari-5xl': ['28px', { lineHeight: '1.4', letterSpacing: '-0.4px' }],
      },
      fontWeight: {
        'dari-light': 300,
        'dari-normal': 400,
        'dari-medium': 500,
        'dari-semibold': 600,
      },
      animation: {
        'gradient-shift': 'gradientShift 15s ease infinite',
        'slide-up': 'slideUp 0.6s ease-out',
        'logo-float': 'logoFloat 3s ease-in-out infinite',
      },
      keyframes: {
        gradientShift: {
          '0%': { backgroundPosition: '0% 50%' },
          '50%': { backgroundPosition: '100% 50%' },
          '100%': { backgroundPosition: '0% 50%' },
        },
        slideUp: {
          'from': { opacity: '0', transform: 'translateY(30px)' },
          'to': { opacity: '1', transform: 'translateY(0)' },
        },
        logoFloat: {
          '0%, 100%': { transform: 'translateY(0px)' },
          '50%': { transform: 'translateY(-10px)' },
        },
      },
    },
  },
  plugins: [],
}
