const defaultTheme = require('tailwindcss/defaultTheme');

module.exports = {
  purge: [
    './Pages/*.razor',
    './Shared/*.razor',
    './App.razor',
    './wwwroot/index.html',
  ],
  theme: {
    extend: {
      fontFamily: {
        sans: ['Inter var', ...defaultTheme.fontFamily.sans],
      },
    },
  },
  variants: {},
  plugins: [
    require('@tailwindcss/ui')
  ],
}
