---
name: Emerald Enterprise
colors:
  surface: '#f7f9fb'
  surface-dim: '#d8dadc'
  surface-bright: '#f7f9fb'
  surface-container-lowest: '#ffffff'
  surface-container-low: '#f2f4f6'
  surface-container: '#eceef0'
  surface-container-high: '#e6e8ea'
  surface-container-highest: '#e0e3e5'
  on-surface: '#191c1e'
  on-surface-variant: '#3f4944'
  inverse-surface: '#2d3133'
  inverse-on-surface: '#eff1f3'
  outline: '#6f7973'
  outline-variant: '#bec9c2'
  surface-tint: '#1b6b51'
  primary: '#004532'
  on-primary: '#ffffff'
  primary-container: '#065f46'
  on-primary-container: '#8bd6b7'
  inverse-primary: '#8bd6b6'
  secondary: '#55615a'
  on-secondary: '#ffffff'
  secondary-container: '#d9e6dd'
  on-secondary-container: '#5b6760'
  tertiary: '#2d3d52'
  on-tertiary: '#ffffff'
  tertiary-container: '#44546a'
  on-tertiary-container: '#b8c8e2'
  error: '#ba1a1a'
  on-error: '#ffffff'
  error-container: '#ffdad6'
  on-error-container: '#93000a'
  primary-fixed: '#a6f2d1'
  primary-fixed-dim: '#8bd6b6'
  on-primary-fixed: '#002116'
  on-primary-fixed-variant: '#00513b'
  secondary-fixed: '#d9e6dd'
  secondary-fixed-dim: '#bdcac1'
  on-secondary-fixed: '#131e19'
  on-secondary-fixed-variant: '#3e4943'
  tertiary-fixed: '#d3e4fe'
  tertiary-fixed-dim: '#b7c8e1'
  on-tertiary-fixed: '#0b1c30'
  on-tertiary-fixed-variant: '#38485d'
  background: '#f7f9fb'
  on-background: '#191c1e'
  surface-variant: '#e0e3e5'
typography:
  display-lg:
    fontFamily: Plus Jakarta Sans
    fontSize: 48px
    fontWeight: '700'
    lineHeight: '1.2'
    letterSpacing: -0.02em
  headline-lg:
    fontFamily: Plus Jakarta Sans
    fontSize: 32px
    fontWeight: '600'
    lineHeight: '1.3'
  headline-md:
    fontFamily: Plus Jakarta Sans
    fontSize: 24px
    fontWeight: '600'
    lineHeight: '1.4'
  headline-sm:
    fontFamily: Plus Jakarta Sans
    fontSize: 18px
    fontWeight: '600'
    lineHeight: '1.5'
  body-lg:
    fontFamily: Inter
    fontSize: 16px
    fontWeight: '400'
    lineHeight: '1.6'
  body-md:
    fontFamily: Inter
    fontSize: 14px
    fontWeight: '400'
    lineHeight: '1.5'
  body-sm:
    fontFamily: Inter
    fontSize: 12px
    fontWeight: '400'
    lineHeight: '1.5'
  label-md:
    fontFamily: Inter
    fontSize: 13px
    fontWeight: '600'
    lineHeight: '1'
    letterSpacing: 0.05em
  headline-lg-mobile:
    fontFamily: Plus Jakarta Sans
    fontSize: 28px
    fontWeight: '600'
    lineHeight: '1.3'
rounded:
  sm: 0.25rem
  DEFAULT: 0.5rem
  md: 0.75rem
  lg: 1rem
  xl: 1.5rem
  full: 9999px
spacing:
  unit: 4px
  container-padding: 24px
  gutter: 16px
  margin-sm: 12px
  margin-md: 24px
  sidebar-width: 260px
  sidebar-collapsed: 72px
---

## Brand & Style

The design system is engineered for high-stakes property management, prioritizing clarity, speed of intent, and a premium enterprise aesthetic. It blends the reliability of traditional corporate software with the fluid, high-performance feel of modern SaaS dashboards.

The visual narrative is defined by "The Emerald Standard": a philosophy of precision where every element serves a functional purpose. The style utilizes a **Corporate Modern** approach with subtle **Glassmorphism** accents to signify layers of information without sacrificing legibility. The interface should feel expansive and high-end, utilizing generous whitespace to reduce the cognitive load inherent in complex data management.

## Colors

The palette is anchored by a deep, authoritative Emerald Green, symbolizing growth and stability. 

- **Primary:** Deep Emerald (#065F46) is reserved for primary actions, active navigation states, and key brand moments.
- **Secondary (Mint):** Soft Mint is used as a background for badges and subtle highlights, providing a fresh contrast to the deep primary.
- **Neutrals (Slate):** A sophisticated range of Slates handles the interface's structural elements, from borders to secondary text.
- **Backgrounds:** The interface primarily uses a crisp White (#FFFFFF) for cards and main content areas, with a very light Slate (#F8FAFC) for the application background to create a subtle separation between the UI and the data containers.

## Typography

This design system employs a dual-font strategy. **Plus Jakarta Sans** is used for headings and display elements to provide a modern, slightly geometric warmth that feels premium. **Inter** is used for all body text, data tables, and UI controls to ensure maximum legibility at high densities.

Hierarchy is established primarily through weight and size rather than color. Labels and metadata should use slightly increased letter-spacing to improve scanability in data-heavy contexts.

## Layout & Spacing

The design system utilizes a **12-column fluid grid** for main content areas, with a fixed sidebar for navigation. 

- **Sidebar:** Positioned on the left, it is collapsible to maximize the workspace for data tables. 
- **Rhythm:** An 8px/4px base unit maintains vertical rhythm. Large KPI cards use 24px internal padding, while data table cells use 12px vertical padding for high-density viewing.
- **Adaptive Strategy:** On tablet, the sidebar collapses automatically. On mobile, the grid reflows to a single column, and the navigation moves to a bottom bar or a full-screen overlay menu.

## Elevation & Depth

Visual hierarchy is managed through **Tonal Layers** and **Ambient Shadows**. 

1.  **Level 0 (Base):** Slate-50 background for the application canvas.
2.  **Level 1 (Surface):** White cards with a 1px Slate-200 border and a subtle, large-radius shadow (0px 4px 20px rgba(0, 0, 0, 0.05)).
3.  **Level 2 (Navigation):** The top header utilizes **Glassmorphism** (Background Blur: 12px, Opacity: 80% White) to maintain context while scrolling.
4.  **Level 3 (Overlays):** Modals and dropdowns use a deeper shadow (0px 10px 30px rgba(0, 0, 0, 0.1)) to clearly separate from the functional workspace.

## Shapes

The shape language is consistently **Rounded**, utilizing a base radius of 12px for primary containers and cards. This softens the "enterprise" feel, making the software more approachable. 

- **Inputs & Buttons:** Use the base 12px radius.
- **Badges/Chips:** Use a fully rounded (pill) shape for status indicators.
- **Selection Indicators:** Active states in the sidebar navigation use a 0px left-border or a subtle rounded background inset to indicate focus.

## Components

### Navigation
- **Left Sidebar:** Collapsible with icons only in the collapsed state. Active items are highlighted with the Primary Emerald color and a subtle mint-tinted background.

### Data Tables
- **High-Density:** Alternating row highlights are not used; instead, use 1px subtle borders between rows.
- **Status Badges:** Use the "Soft Mint" secondary for positive statuses (e.g., "Paid"), Slate for neutral (e.g., "Draft"), and muted red for alerts.

### Cards & Widgets
- **KPI Cards:** Feature a large "Plus Jakarta" headline for the primary metric, a small sparkline for trends, and a clear label.
- **Glassmorphism:** Apply to global headers and floating action button backgrounds.

### Forms & Inputs
- **Inputs:** 12px rounded corners with a 1px Slate-300 border. Focus state switches the border to Primary Emerald with a 3px soft mint glow.
- **Validation:** Error messages appear below the input in 12px Inter, using the semantic error red.

### Authentication
- **Split-Screen:** The left side features a high-quality property lifestyle image with an emerald overlay, while the right side is a clean, centered white form on a slate-50 background.