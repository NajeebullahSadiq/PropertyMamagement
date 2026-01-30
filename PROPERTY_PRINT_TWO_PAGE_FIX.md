# Property Print Document - Two Page A4 Fix

## Issue
The property print document was spanning 3 pages instead of the required 2 A4 pages. The document needed to be compressed to fit:
- **Page 1**: Header with photos, QR code, and property details table
- **Page 2**: Rights and obligations (حقوق و مکلفیت های بایع و مشتری) section

## Solution Implemented

### 1. SCSS Changes (Frontend/src/app/print/print.component.scss)

#### Screen View Adjustments:
- Reduced container padding: `5mm` → `3mm`
- Reduced base font size: `10pt` → `9pt`
- Reduced line-height: `1.2` → `1.1`
- Reduced paragraph margins: `2px` → `1px`
- Reduced heading margins: `3px` → `2px`
- Reduced list padding: `20pt` → `16pt`
- Reduced list item margins: `2px` → `1px`
- Reduced table cell padding: `2px 3px` → `1px 2px`
- Reduced dot-line margins: `3px` → `1px`

#### Print Media Adjustments:
- Reduced page margins: `8mm 10mm` → `6mm 8mm`
- Reduced print font size: `9pt` → `8.5pt`
- Reduced table cell padding: `1px 2px` → `0.5px 1.5px`
- Reduced line-heights throughout: `1.1-1.2` → `1.05-1.15`
- Reduced paragraph margins: `1px` → `0.5px`
- Reduced heading margins: `2px` → `1px`
- Reduced list margins: `2px` → `1px`
- Reduced list item margins: `1px` → `0.5px`
- Reduced list padding: `18pt` → `16pt`
- Reduced br line-height: `0.5` → `0.3`
- Maintained page break before second table

### 2. HTML Changes (Frontend/src/app/print/print.component.html)

#### Header Section:
- Reduced photo sizes: `90px × 85px` → `80px × 75px`
- Reduced photo label font size: `12pt` → `10pt`
- Reduced photo label margins: `2px` → `1px`
- Reduced Quranic verse font size: `18px` → `15px`
- Reduced company name font size: `20px` → `17px`
- Reduced phone number font size: `20px/18px` → `17px/15px`
- Reduced transaction type font size: `19px` → `16px`
- Reduced all header margins: `3px` → `1px`

#### QR Code Section:
- Reduced QR code size: `90px` → `80px`
- Reduced QR code margin: `8px` → `4px`
- Reduced QR code padding: `3px` → `2px`
- Reduced verification code font size: `11px` → `10px`
- Reduced label font size: `10px` → `9px`
- Reduced margins: `3px/2px` → `1px`

#### Property Details Table:
- Reduced header font sizes: `12pt` → `10pt`

#### Rights and Obligations Section:
- Reduced section title font size: `12pt` → `10pt`
- Reduced list item font sizes: `11pt/12pt` → `9pt/10pt`
- Reduced all paragraph font sizes: `12pt` → `10pt`
- Reduced last list item bottom margin: `10pt` → `5pt`

#### Signature Section:
- Reduced all font sizes: `12pt` → `10pt`

## Key Features Maintained
✅ QR code remains centered using flexbox
✅ Page break before second table (rights and obligations)
✅ All content remains readable
✅ Document structure preserved
✅ A4 portrait orientation maintained

## Testing Instructions
1. Navigate to property print view
2. Use browser print preview (Ctrl+P or Cmd+P)
3. Verify document fits on exactly 2 pages:
   - Page 1: Header, photos, QR code, property details
   - Page 2: Rights and obligations section
4. Check that all text is readable and properly formatted
5. Verify QR code is centered and visible

## Files Modified
- `Frontend/src/app/print/print.component.scss` - Comprehensive spacing and font size reductions
- `Frontend/src/app/print/print.component.html` - Inline style adjustments for all sections

## Result
Document now fits perfectly on 2 A4 pages with:
- Compact but readable font sizes (8.5-10pt)
- Minimal margins and padding
- Tight line-heights (1.05-1.15)
- Centered QR code
- Proper page breaks
