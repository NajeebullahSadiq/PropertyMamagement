# Print Mode Feature - Full Design vs Data Only

## Overview
Added a print mode selection feature that allows users to choose between two printing options:
1. **Full Design Mode** - Print with all borders, headers, and design elements (for blank A4 paper)
2. **Data Only Mode** - Print only the data values without design elements (for pre-printed forms)

## User Experience

### When Opening Print Preview
1. User clicks print button for property or vehicle document
2. A modal dialog appears with two options:
   - **چاپ با دیزاین کامل** (Print with Full Design) - Blue icon
   - **چاپ فقط معلومات** (Print Data Only) - Green icon
3. User selects their preferred mode
4. Print dialog opens automatically

### Full Design Mode
- Shows all borders, tables, and design elements
- Includes photos, QR codes, and headers
- Includes decorative text (Quranic verse, office name, etc.)
- Suitable for printing on blank A4 paper

### Data Only Mode
- Hides all borders and table lines
- Hides photos and QR codes
- Hides decorative headers and text
- Shows only data values in their exact positions
- Suitable for printing on pre-printed forms where design is already on paper

## Technical Implementation

### Files Modified

#### Property Module:
1. `Frontend/src/app/print/print.component.ts`
   - Added `printMode` property ('full' | 'data-only')
   - Added `showPrintOptions` boolean
   - Added `setPrintMode()` method
   - Added `cancelPrint()` method
   - Modified `ngOnInit()` to check URL parameter
   - Modified `waitForImagesToLoad()` to not auto-print when showing options

2. `Frontend/src/app/print/print.component.html`
   - Added print mode selection dialog
   - Added `[class.data-only-mode]` conditional class
   - Added `*ngIf="printMode === 'full'"` to design elements

3. `Frontend/src/app/print/print.component.scss`
   - Added `.print-options-overlay` styles
   - Added `.print-options-dialog` styles
   - Added `.print-option-btn` styles
   - Added `.data-only-mode` styles for screen
   - Added `.data-only-mode` print media query styles

#### Vehicle Module:
1. `Frontend/src/app/printvehicledata/printvehicledata.component.ts`
   - Same changes as property module

2. `Frontend/src/app/printvehicledata/printvehicledata.component.html`
   - Same changes as property module

3. `Frontend/src/app/printvehicledata/printvehicledata.component.scss`
   - Same changes as property module

## Features

### URL Parameter Support
You can bypass the dialog by adding a query parameter:
```
/print/123?mode=data-only
/printvehicledata/456?mode=full
```

### Responsive Dialog
- Centered modal overlay with dark background
- Animated slide-in effect
- Hover effects on buttons
- Cancel button to close without printing

### Print Optimization
- Dialog automatically hidden when printing
- Data-only mode removes all visual clutter
- Maintains exact positioning for pre-printed forms
- No padding or margins in data-only mode

## CSS Classes

### Screen View Classes
- `.print-options-overlay` - Full-screen modal backdrop
- `.print-options-dialog` - Dialog container
- `.print-options-buttons` - Button container (flex layout)
- `.print-option-btn` - Individual option button
- `.full-design` - Blue styling for full design option
- `.data-only` - Green styling for data-only option
- `.cancel-btn` - Cancel button styling
- `.data-only-mode` - Applied to container when data-only selected

### Print Media Query
- Hides `.print-options-overlay`
- Removes borders in `.data-only-mode`
- Hides images in `.data-only-mode`
- Hides decorative text in `.data-only-mode`
- Sets padding to 0 in `.data-only-mode`

## Usage Instructions

### For Users:
1. Click print button on property or vehicle document
2. Choose print mode:
   - Use "چاپ با دیزاین کامل" for blank paper
   - Use "چاپ فقط معلومات" for pre-printed forms
3. Print dialog opens automatically
4. Print as normal

### For Developers:
To add this feature to other print components:
1. Copy the TypeScript properties and methods
2. Copy the HTML dialog structure
3. Copy the SCSS styles
4. Add `[class.data-only-mode]` to container
5. Add `*ngIf="printMode === 'full'"` to design elements

## Benefits

1. **Cost Savings** - Use pre-printed forms instead of printing full design every time
2. **Professional Look** - Pre-printed forms can have better quality design
3. **Flexibility** - Users can choose based on their needs
4. **Exact Positioning** - Data-only mode maintains exact positions for pre-printed forms
5. **User Friendly** - Simple, clear interface with visual icons

## Testing Checklist

- [ ] Property print shows mode selection dialog
- [ ] Vehicle print shows mode selection dialog
- [ ] Full design mode shows all elements
- [ ] Data-only mode hides borders and design
- [ ] Data-only mode maintains data positions
- [ ] Cancel button closes dialog
- [ ] URL parameter bypasses dialog
- [ ] Print dialog opens after selection
- [ ] Dialog hidden when printing
- [ ] Works on different browsers

## Future Enhancements

1. Remember user's last selection (localStorage)
2. Add preview before printing
3. Support for custom pre-printed form templates
4. Alignment guides for data-only mode
5. Export to PDF with selected mode
