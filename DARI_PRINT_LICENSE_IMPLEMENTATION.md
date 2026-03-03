# Dari Print License Implementation

## Overview
Added Dari language support to the print license feature, allowing users to select between Pashto and Dari versions when printing licenses.

## Changes Made

### 1. TypeScript Component (`print-license.component.ts`)

#### Added Properties:
- `selectedLanguage: 'pashto' | 'dari'` - Tracks the selected language (default: 'pashto')
- `showLanguageSelector: boolean` - Controls the visibility of the language selector modal (default: true)

#### Added Methods:
- `selectLanguage(language: 'pashto' | 'dari')` - Handles language selection and triggers print
- `getLicenseTypeInDari(licenseType: string)` - Translates license types to Dari

#### Modified Methods:
- `triggerPrint()` - Removed auto-print functionality to allow language selection first

#### Translation Mappings:

**Pashto Translations:**
- Real Estate → ملکیت
- Vehicle/Car Sale → موټر پلورنه

**Dari Translations:**
- Real Estate → املاک
- Vehicle/Car Sale → فروش موتر

### 2. HTML Template (`print-license.component.html`)

#### Added Language Selector Modal:
```html
<div class="language-selector-overlay" *ngIf="!isLoading && !error && showLanguageSelector">
  <div class="language-selector-modal">
    <h2>د چاپ ژبه انتخاب کړئ / زبان چاپ را انتخاب کنید</h2>
    <p>Select Print Language</p>
    <div class="language-buttons">
      <button class="language-btn pashto-btn" (click)="selectLanguage('pashto')">
        <span class="lang-icon">🇦🇫</span>
        <span class="lang-name">پښتو</span>
        <span class="lang-name-en">Pashto</span>
      </button>
      <button class="language-btn dari-btn" (click)="selectLanguage('dari')">
        <span class="lang-icon">🇦🇫</span>
        <span class="lang-name">دری</span>
        <span class="lang-name-en">Dari</span>
      </button>
    </div>
  </div>
</div>
```

#### Modified Certificate Display:
- Pashto version: `*ngIf="!isLoading && !error && selectedLanguage === 'pashto'"`
- Dari version: `*ngIf="!isLoading && !error && selectedLanguage === 'dari'"`

#### Complete Dari Version Created:
Full certificate template with all text translated to Dari, including:
- Header labels and ministry names
- License metadata labels
- Personal information text
- Authorization statements
- Footer text and notices
- Verification labels

### 3. SCSS Styles (`print-license.component.scss`)

#### Added Language Selector Styles:
- `.language-selector-overlay` - Full-screen modal overlay with backdrop blur
- `.language-selector-modal` - Centered modal with gradient background
- `.language-buttons` - Flex container for language buttons
- `.language-btn` - Individual language button with hover effects
- `.pashto-btn` and `.dari-btn` - Specific styling for each language button

#### Key Features:
- Smooth hover animations with transform and shadow effects
- Responsive design with flex layout
- RTL support for Pashto/Dari text
- Print media query to hide selector when printing

### 4. License Metadata Alignment Fix

Updated `.license-meta` and `.meta-item` styles to properly align license numbers and dates:
- Right-aligned layout with RTL direction
- Values displayed with LTR direction for proper number formatting
- Consistent spacing and alignment

## Key Translations (Pashto → Dari)

| Element | Pashto | Dari |
|---------|--------|------|
| License Number | د جوازلیک شمېره | شماره جوازلیسانس |
| Issue Date | د صدور نېټه | تاریخ صدور |
| Expiry Date | د پای نېټه | تاریخ ختم |
| Ministry of Justice | د عدلیې وزارت | وزارت عدلیه |
| Financial & Admin Deputy | مالي او اداري معینیت | معینیت مالی و اداری |
| General Directorate | د انسجام عمومی ریاست | ریاست عمومی انسجام |
| License Type | د جوازلیک ډول | نوع جوازلیسانس |
| Real Estate | ملکیت | املاک |
| Vehicle Sale | موټر پلورنه | فروش موتر |
| Mr./Sir | ښاغلی | آقای |
| Son of | د ... زوی | فرزند |
| Grandson of | د ... لمسی | نواسه |
| Born in year | کال کی زېږېدلی | متولد سال |
| Electronic ID Number | الکترونیکي تذکرې شمېره | شماره تذکره الکترونیکی |
| Province | ولایت | ولایت |
| District | ناحې/ ولسوالۍ | ناحیه/ ولسوالی |
| Original Resident | اصلي اوسیدونکې | باشنده اصلی |
| Current Resident | اوسنی اوسیدونکې | باشنده فعلی |
| With Respect | په درنښت | با احترام |
| General Director | د انسجام عمومی رئیس | رئیس عمومی انسجام |
| Note | یادښت | یادداشت |
| Validity Period | د اعتبار موده | مدت اعتبار |
| Three Years | درېو کلونو | سه سال |
| Contact Number | د اړیکې شمېره | شماره تماس |
| License Fee | د جوازلیک بیه | قیمت جوازلیسانس |
| Twenty Thousand | شل زره افغانۍ | بیست هزار افغانی |
| Twenty-Five Thousand | پنځویشت زره افغانۍ | بیست و پنج هزار افغانی |
| Verification Code | د تصدیق کوډ | کود تصدیق |
| Fame/Reputation | شهرت | شهرت |

## User Flow

1. User navigates to print license page
2. License data loads from backend
3. Language selector modal appears with two options:
   - Pashto (پښتو)
   - Dari (دری)
4. User clicks desired language button
5. Modal closes and print dialog opens automatically
6. Selected language version is displayed and printed

## Features

- **Bilingual Support**: Complete Pashto and Dari versions
- **User Choice**: Modal allows users to select their preferred language
- **Auto-Print**: Print dialog opens automatically after language selection
- **Consistent Layout**: Both versions maintain the same professional design
- **Dynamic Data**: All data fields work identically in both languages
- **Print Optimization**: Language selector hidden during print

## Testing Checklist

- [ ] Language selector appears on page load
- [ ] Pashto button displays Pashto version
- [ ] Dari button displays Dari version
- [ ] Print dialog opens after language selection
- [ ] All dynamic data displays correctly in both versions
- [ ] License type translations work correctly
- [ ] Date formatting works in both versions
- [ ] QR code and verification display correctly
- [ ] Print output matches screen display
- [ ] Modal is hidden during print

## Notes

- Default language is Pashto if no selection is made
- Language selector only appears once per page load
- Both versions use the same dynamic data source
- All styling and layout are consistent between versions
- Print functionality works identically for both languages
