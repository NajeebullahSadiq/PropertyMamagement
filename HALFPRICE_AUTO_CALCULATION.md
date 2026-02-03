# HalfPrice Auto-Calculation Implementation

## Overview
The HalfPrice (مناصف قیمت) field in the vehicle module now automatically calculates as half of the entered price, matching the behavior in the real estate module.

## Date
February 2, 2026

## Implementation Details

### Calculation Logic
```typescript
halfPrice = price / 2
```

### How It Works

1. **User enters price**: When the user types a price in the "قیمت به عدد" field
2. **Automatic calculation**: The `updateOnePercent()` method is triggered
3. **Half price computed**: Price is divided by 2
4. **Field updated**: The halfPrice field is automatically populated
5. **Display**: Shows in a disabled (read-only) input field with gray background

### Code Changes

#### Component TypeScript
```typescript
// Added property
halfPriceValue: number = 0;

// Updated method
updateOnePercent() {
  const priceControl = this.price;
  if (priceControl && priceControl.value) {
    const normalizedPrice = this.numeralService.parseNumber(priceControl.value);
    this.onePercent = normalizedPrice * 0.01;
    
    // Calculate half price automatically
    this.halfPriceValue = normalizedPrice / 2;
    this.vehicleForm.patchValue(
      { halfPrice: this.halfPriceValue },
      { emitEvent: false }
    );
  } else {
    this.onePercent = 0;
    this.halfPriceValue = 0;
    this.vehicleForm.patchValue(
      { halfPrice: null },
      { emitEvent: false }
    );
  }
}
```

#### Component HTML
```html
<div>
  <label for="halfPrice" class="block text-sm font-semibold text-gray-700 mb-2">مناصف قیمت</label>
  <div class="relative">
    <span class="absolute inset-y-0 left-0 flex items-center pl-3 text-gray-500">
      <i class="fas fa-divide"></i>
    </span>
    <input type="number" id="halfPrice" [value]="halfPriceValue" 
           class="w-full px-4 py-2.5 pl-10 border border-gray-300 rounded-lg bg-gray-50 cursor-not-allowed" 
           disabled>
  </div>
</div>
```

### Field Characteristics

| Aspect | Value |
|--------|-------|
| **Editable** | No (disabled/read-only) |
| **Auto-calculated** | Yes |
| **Trigger** | Price field change |
| **Formula** | Price ÷ 2 |
| **Display** | Disabled input with gray background |
| **Icon** | fa-divide |

### Comparison with Real Estate Module

Both modules now have identical behavior:

| Feature | Real Estate | Vehicle |
|---------|-------------|---------|
| Auto-calculation | ✅ Yes | ✅ Yes |
| Formula | Price ÷ 2 | Price ÷ 2 |
| Read-only | ✅ Yes | ✅ Yes |
| Updates on price change | ✅ Yes | ✅ Yes |

## User Experience

### Before
- User had to manually enter half price
- Risk of calculation errors
- Extra data entry step

### After
- Half price automatically calculated
- No manual entry needed
- Consistent with real estate module
- Reduces user effort and errors

## Testing

### Test Scenarios

1. **Enter new price**
   - Enter: 10000
   - Expected HalfPrice: 5000

2. **Change existing price**
   - Original: 10000 (HalfPrice: 5000)
   - Change to: 20000
   - Expected HalfPrice: 10000

3. **Clear price**
   - Clear price field
   - Expected HalfPrice: 0 or null

4. **Load existing vehicle**
   - Vehicle with price: 15000
   - Expected HalfPrice: 7500 (calculated from stored value or price)

5. **Decimal prices**
   - Enter: 10500.50
   - Expected HalfPrice: 5250.25

## Files Modified

1. **Frontend/src/app/vehicle/vehicle-submit/vehicle-submit.component.ts**
   - Added `halfPriceValue` property
   - Updated `updateOnePercent()` method to calculate half price
   - Updated data loading to set halfPriceValue

2. **Frontend/src/app/vehicle/vehicle-submit/vehicle-submit.component.html**
   - Changed input from editable to disabled
   - Bound to `halfPriceValue` property instead of form control
   - Added gray background styling

3. **ADD_HALFPRICE_FIELD.md**
   - Updated documentation to reflect auto-calculation behavior

## Benefits

✅ **Consistency**: Matches real estate module behavior
✅ **Accuracy**: Eliminates manual calculation errors
✅ **Efficiency**: Reduces data entry time
✅ **User-friendly**: Automatic calculation is intuitive
✅ **Maintainability**: Single source of truth for calculation

## Notes

- The field is still stored in the database for data persistence
- Backend receives the calculated value from frontend
- The calculation happens in real-time as user types
- The field cannot be manually edited by users
- Existing vehicles will show calculated half price when loaded
