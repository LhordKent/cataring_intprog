/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./**/*.{razor,html,cshtml}"],
  theme: {
    extend: {
      colors: {
        'temu-orange': '#ff4700',
        'temu-black': '#000000',
      }
    },
  },
  plugins: [],
}
