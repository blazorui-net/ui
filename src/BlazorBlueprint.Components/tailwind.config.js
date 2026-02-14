/** @type {import('tailwindcss').Config} */
module.exports = {
  darkMode: ["class"],
  content: [
    './Components/**/*.{razor,html,cs}',
  ],
  theme: {
    extend: {
      fontFamily: {
        sans: "var(--font-sans, ui-sans-serif, system-ui, sans-serif)",
        serif: "var(--font-serif, ui-serif, Georgia, serif)",
        mono: "var(--font-mono, ui-monospace, monospace)",
      },
      colors: {
        border: "var(--border)",
        input: "var(--input)",
        ring: "var(--ring)",
        background: "var(--background)",
        foreground: "var(--foreground)",
        primary: {
          DEFAULT: "var(--primary)",
          foreground: "var(--primary-foreground)",
        },
        secondary: {
          DEFAULT: "var(--secondary)",
          foreground: "var(--secondary-foreground)",
        },
        destructive: {
          DEFAULT: "var(--destructive)",
          foreground: "var(--destructive-foreground)",
        },
        muted: {
          DEFAULT: "var(--muted)",
          foreground: "var(--muted-foreground)",
        },
        accent: {
          DEFAULT: "var(--accent)",
          foreground: "var(--accent-foreground)",
        },
        popover: {
          DEFAULT: "var(--popover)",
          foreground: "var(--popover-foreground)",
        },
        card: {
          DEFAULT: "var(--card)",
          foreground: "var(--card-foreground)",
        },
        sidebar: {
          DEFAULT: "var(--sidebar)",
          foreground: "var(--sidebar-foreground)",
          primary: "var(--sidebar-primary)",
          "primary-foreground": "var(--sidebar-primary-foreground)",
          accent: "var(--sidebar-accent)",
          "accent-foreground": "var(--sidebar-accent-foreground)",
          border: "var(--sidebar-border)",
          ring: "var(--sidebar-ring)",
        },
        alert: {
          success: {
            DEFAULT: "var(--alert-success)",
            foreground: "var(--alert-success-foreground)",
            bg: "var(--alert-success-bg)",
          },
          info: {
            DEFAULT: "var(--alert-info)",
            foreground: "var(--alert-info-foreground)",
            bg: "var(--alert-info-bg)",
          },
          warning: {
            DEFAULT: "var(--alert-warning)",
            foreground: "var(--alert-warning-foreground)",
            bg: "var(--alert-warning-bg)",
          },
          danger: {
            DEFAULT: "var(--alert-danger)",
            foreground: "var(--alert-danger-foreground)",
            bg: "var(--alert-danger-bg)",
          },
        },
      },
      borderRadius: {
        lg: "var(--radius)",
        md: "calc(var(--radius) - 2px)",
        sm: "calc(var(--radius) - 4px)",
      },
      width: {
        "sidebar": "var(--sidebar-width)",
        "sidebar-mobile": "var(--sidebar-width-mobile)",
        "sidebar-icon": "var(--sidebar-width-icon)",
      },
      keyframes: {
        // Accordion/Collapsible animations - height-based
        "accordion-down": {
          from: { height: "0" },
          to: { height: "var(--radix-accordion-content-height, auto)" },
        },
        "accordion-up": {
          from: { height: "var(--radix-accordion-content-height, auto)" },
          to: { height: "0" },
        },
        "collapsible-down": {
          from: { height: "0" },
          to: { height: "var(--radix-collapsible-content-height, auto)" },
        },
        "collapsible-up": {
          from: { height: "var(--radix-collapsible-content-height, auto)" },
          to: { height: "0" },
        },
        
        // Dialog animations - zoom with fade
        "dialog-zoom-in": {
          from: { opacity: "0", transform: "scale(0.95)" },
          to: { opacity: "1", transform: "scale(1)" },
        },
        "dialog-zoom-out": {
          from: { opacity: "1", transform: "scale(1)" },
          to: { opacity: "0", transform: "scale(0.95)" },
        },
        
        // Overlay animations - fade only
        "overlay-fade-in": {
          from: { opacity: "0" },
          to: { opacity: "1" },
        },
        "overlay-fade-out": {
          from: { opacity: "1" },
          to: { opacity: "0" },
        },
        
        // Popover/Dropdown/Combobox animations - small slide with fade
        "content-slide-in-from-top": {
          from: { opacity: "0", transform: "translateY(-0.5rem)" },
          to: { opacity: "1", transform: "translateY(0)" },
        },
        "content-slide-out-to-top": {
          from: { opacity: "1", transform: "translateY(0)" },
          to: { opacity: "0", transform: "translateY(-0.5rem)" },
        },
        "content-slide-in-from-bottom": {
          from: { opacity: "0", transform: "translateY(0.5rem)" },
          to: { opacity: "1", transform: "translateY(0)" },
        },
        "content-slide-out-to-bottom": {
          from: { opacity: "1", transform: "translateY(0)" },
          to: { opacity: "0", transform: "translateY(0.5rem)" },
        },
        "content-slide-in-from-left": {
          from: { opacity: "0", transform: "translateX(-0.5rem)" },
          to: { opacity: "1", transform: "translateX(0)" },
        },
        "content-slide-out-to-left": {
          from: { opacity: "1", transform: "translateX(0)" },
          to: { opacity: "0", transform: "translateX(-0.5rem)" },
        },
        "content-slide-in-from-right": {
          from: { opacity: "0", transform: "translateX(0.5rem)" },
          to: { opacity: "1", transform: "translateX(0)" },
        },
        "content-slide-out-to-right": {
          from: { opacity: "1", transform: "translateX(0)" },
          to: { opacity: "0", transform: "translateX(0.5rem)" },
        },
        
        // Sheet/Drawer animations - slide from edges (100%)
        "slide-in-from-top": {
          from: { transform: "translateY(-100%)" },
          to: { transform: "translateY(0)" },
        },
        "slide-in-from-bottom": {
          from: { transform: "translateY(100%)" },
          to: { transform: "translateY(0)" },
        },
        "slide-in-from-left": {
          from: { transform: "translateX(-100%)" },
          to: { transform: "translateX(0)" },
        },
        "slide-in-from-right": {
          from: { transform: "translateX(100%)" },
          to: { transform: "translateX(0)" },
        },
        "slide-out-to-top": {
          from: { transform: "translateY(0)" },
          to: { transform: "translateY(-100%)" },
        },
        "slide-out-to-bottom": {
          from: { transform: "translateY(0)" },
          to: { transform: "translateY(100%)" },
        },
        "slide-out-to-left": {
          from: { transform: "translateX(0)" },
          to: { transform: "translateX(-100%)" },
        },
        "slide-out-to-right": {
          from: { transform: "translateX(0)" },
          to: { transform: "translateX(100%)" },
        },
        
        // Toast animations - slide with fade (direction-aware entrance)
        "toast-slide-in-from-top": {
          from: { opacity: "0", transform: "translateY(-100%)" },
          to: { opacity: "1", transform: "translateY(0)" },
        },
        "toast-slide-in-from-bottom": {
          from: { opacity: "0", transform: "translateY(100%)" },
          to: { opacity: "1", transform: "translateY(0)" },
        },
      },
      animation: {
        // Accordion/Collapsible
        "accordion-down": "accordion-down 0.2s ease-out",
        "accordion-up": "accordion-up 0.2s ease-out",
        "collapsible-down": "collapsible-down 0.2s ease-out",
        "collapsible-up": "collapsible-up 0.2s ease-out",
        
        // Dialog/AlertDialog - zoom with fade (modal in center)
        "dialog-zoom-in": "dialog-zoom-in 0.2s ease-out",
        "dialog-zoom-out": "dialog-zoom-out 0.2s ease-out",
        
        // Overlays - fade only (backgrounds/backdrops)
        "overlay-fade-in": "overlay-fade-in 0.2s ease-out",
        "overlay-fade-out": "overlay-fade-out 0.2s ease-out",
        
        // Popover/Dropdown/Combobox - small slide with fade
        "content-slide-in-from-top": "content-slide-in-from-top 0.2s ease-out",
        "content-slide-out-to-top": "content-slide-out-to-top 0.2s ease-out",
        "content-slide-in-from-bottom": "content-slide-in-from-bottom 0.2s ease-out",
        "content-slide-out-to-bottom": "content-slide-out-to-bottom 0.2s ease-out",
        "content-slide-in-from-left": "content-slide-in-from-left 0.2s ease-out",
        "content-slide-out-to-left": "content-slide-out-to-left 0.2s ease-out",
        "content-slide-in-from-right": "content-slide-in-from-right 0.2s ease-out",
        "content-slide-out-to-right": "content-slide-out-to-right 0.2s ease-out",
        
        // Sheet/Drawer - slide from edges (100%)
        "slide-in-from-top": "slide-in-from-top 0.2s ease-out",
        "slide-in-from-bottom": "slide-in-from-bottom 0.2s ease-out",
        "slide-in-from-left": "slide-in-from-left 0.2s ease-out",
        "slide-in-from-right": "slide-in-from-right 0.2s ease-out",
        "slide-out-to-top": "slide-out-to-top 0.2s ease-out",
        "slide-out-to-bottom": "slide-out-to-bottom 0.2s ease-out",
        "slide-out-to-left": "slide-out-to-left 0.2s ease-out",
        "slide-out-to-right": "slide-out-to-right 0.2s ease-out",
        
        // Toast - slide with fade (direction-aware entrance only)
        "toast-slide-in-from-top": "toast-slide-in-from-top 0.35s ease-out",
        "toast-slide-in-from-bottom": "toast-slide-in-from-bottom 0.35s ease-out",
      },
    },
  },
  plugins: [],
}
