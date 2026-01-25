# Dari/Pashto Numeral Support - Account Info Module

## Summary
Added support for entering numbers in Dari/Pashto (Eastern Arabic) numerals (۰۱۲۳۴۵۶۷۸۹) in the account info numeric fields.

## Changes Made

### File: `Frontend/src/app/realestate/accountinfo/accountinfo.component.ts`

1. **Imported NumeralService**
   - Added import for `NumeralService` to handle Eastern Arabic numeral conversion

2. **Injected NumeralService**
   - Added `private numeralService: NumeralService` to constructor

3. **Updated parseNumber() helper in updateData()**
   - Now converts Eastern Arabic numerals to Western using `numeralService.toWesternArabic()` before parsing
   - Supports both ۱۲۳۴ (Dari/Pashto) and 1234 (Western) formats

4. **Updated parseInteger() helper in updateData()**
   - Now converts Eastern Arabic numerals to Western using `numeralService.toWesternArabic()` before parsing
   - Supports both ۱۲۳۴ (Dari/Pashto) and 1234 (Western) formats

5. **Updated parseNumber() helper in addData()**
   - Same conversion logic as updateData()

6. **Updated parseInteger() helper in addData()**
   - Same conversion logic as updateData()

## Affected Fields
The following numeric fields now support Dari/Pashto numerals:
- **settlementYear** (سال تصفيه مالية) - Integer field
- **transactionCount** (تعدادی معامله) - Integer field
- **companyCommission** (كمیشن رهنما) - Decimal field
- **taxPaymentAmount** (تحويل ماليات) - Decimal field

## How It Works

1. **User Input**: The `appNumericInput` directive (already present in HTML) allows users to type Eastern Arabic numerals (۰۱۲۳۴۵۶۷۸۹) or Western numerals (0123456789)

2. **Conversion**: When the form is submitted, the component's helper functions convert Eastern Arabic numerals to Western numerals using `NumeralService.toWesternArabic()`

3. **Parsing**: After conversion, the values are parsed as floats or integers

4. **Backend**: The backend receives standard Western numerals

## Testing

Test with these values:
- **Dari/Pashto**: ۱۲۳۴ → Should save as 1234
- **Western**: 1234 → Should save as 1234
- **Mixed**: Not applicable (directive prevents mixing)
- **Decimal (Dari)**: ۱۲۳۴٫۵۶ → Should save as 1234.56
- **Decimal (Western)**: 1234.56 → Should save as 1234.56

## Example Usage

```typescript
// User enters: ۱۴۰۳ in settlementYear field
// Component converts: "۱۴۰۳" → "1403"
// Backend receives: 1403

// User enters: ۱۲۳۴٫۵۶ in companyCommission field
// Component converts: "۱۲۳۴٫۵۶" → "1234.56"
// Backend receives: 1234.56
```

## Notes
- The `NumeralService` is already available in the shared module
- The `appNumericInput` directive was already applied to all numeric fields
- No changes needed to the HTML template
- No changes needed to the backend
- The conversion is transparent to the user
