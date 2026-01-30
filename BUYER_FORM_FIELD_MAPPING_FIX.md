# Buyer Form Field Name Standardization

## Issue
Error: "Must supply a value for form control with name: 'indentityCardNumber'"

The buyer form had a field name mismatch between frontend and backend due to a legacy typo:
- **Frontend form control**: `indentityCardNumber` (typo: "indent" instead of "ident")
- **Backend model field**: `ElectronicNationalIdNumber` (correct name)

## Solution Implemented

Completely removed the typo `indentityCardNumber` and standardized on `electronicNationalIdNumber` throughout the entire codebase.

### 1. Frontend Changes

#### A. TypeScript Component (`Frontend/src/app/estate/propertydetails/buyerdetail/buyerdetail.component.ts`)

**Form Definition (line 112):**
```typescript
// BEFORE
indentityCardNumber: ['', Validators.required],

// AFTER
electronicNationalIdNumber: ['', Validators.required],
```

**Data Loading - loadBuyerDetails() (line 296):**
```typescript
// BEFORE
indentityCardNumber: (firstBuyer as any).electronicNationalIdNumber || firstBuyer.indentityCardNumber || '',

// AFTER
electronicNationalIdNumber: firstBuyer.electronicNationalIdNumber || '',
```

**Data Loading - BindValue() (line 472):**
```typescript
// BEFORE
indentityCardNumber: (selectedBuyer as any).electronicNationalIdNumber || selectedBuyer.indentityCardNumber || '',

// AFTER
electronicNationalIdNumber: selectedBuyer.electronicNationalIdNumber || '',
```

**Getter Method (line 733):**
```typescript
// BEFORE
get indentityCardNumber() { return this.sellerForm.get('indentityCardNumber'); }

// AFTER
get electronicNationalIdNumber() { return this.sellerForm.get('electronicNationalIdNumber'); }
```

#### B. HTML Template (`Frontend/src/app/estate/propertydetails/buyerdetail/buyerdetail.component.html`)

**Form Input (line 221-230):**
```html
<!-- BEFORE -->
<label for="indentityCardNumber" class="block text-sm font-semibold text-gray-700 mb-2">شماره تذکره</label>
<input type="text" appNumericInput id="indentityCardNumber" 
    formControlName="indentityCardNumber">
<div *ngIf="indentityCardNumber?.invalid && (indentityCardNumber?.dirty || indentityCardNumber?.touched)">
    <small class="text-red-600 text-xs mt-1 block" *ngIf="indentityCardNumber?.errors?.['required']">این فیلد الزامی است</small>
</div>

<!-- AFTER -->
<label for="electronicNationalIdNumber" class="block text-sm font-semibold text-gray-700 mb-2">شماره تذکره</label>
<input type="text" appNumericInput id="electronicNationalIdNumber" 
    formControlName="electronicNationalIdNumber">
<div *ngIf="electronicNationalIdNumber?.invalid && (electronicNationalIdNumber?.dirty || electronicNationalIdNumber?.touched)">
    <small class="text-red-600 text-xs mt-1 block" *ngIf="electronicNationalIdNumber?.errors?.['required']">این فیلد الزامی است</small>
</div>
```

**Table Display (line 561):**
```html
<!-- BEFORE -->
<td>{{ b.indentityCardNumber }}</td>

<!-- AFTER -->
<td>{{ b.electronicNationalIdNumber }}</td>
```

#### C. TypeScript Model (`Frontend/src/app/models/SellerDetail.ts`)

```typescript
// BEFORE
export interface SellerDetail {
    indentityCardNumber:number;
    // ...
}

// AFTER
export interface SellerDetail {
    electronicNationalIdNumber?:string;
    // ...
}
```

### 2. Backend Changes

#### A. Controller (`Backend/Controllers/SellerDetailsController.cs`)

**GetByerById() Method:**
```csharp
// Returns data with electronicNationalIdNumber field
var mappedBuyers = buyers.Select(b => new
{
    // ... other fields ...
    electronicNationalIdNumber = b.ElectronicNationalIdNumber,
    // ... other fields ...
}).ToList();
```

**SaveBuyer() and UpdateBuyer() Methods:**
- Changed parameter type from `BuyerDetailRequest` back to `BuyerDetail`
- Removed field mapping logic
- Directly use `request.ElectronicNationalIdNumber`

**Removed BuyerDetailRequest Class:**
- Deleted the temporary DTO class that was accepting both field names
- No longer needed since frontend now uses correct field name

## Files Modified

### Frontend
- `Frontend/src/app/estate/propertydetails/buyerdetail/buyerdetail.component.ts`
  - Updated form definition (line 112)
  - Updated loadBuyerDetails() method (line 296)
  - Updated BindValue() method (line 472)
  - Updated getter method (line 733)
- `Frontend/src/app/estate/propertydetails/buyerdetail/buyerdetail.component.html`
  - Updated form input field (lines 221-230)
  - Updated table display (line 561)
- `Frontend/src/app/models/SellerDetail.ts`
  - Updated interface field name and type

### Backend
- `Backend/Controllers/SellerDetailsController.cs`
  - Updated `GetByerById()` method (line 59)
  - Updated `SaveBuyer()` method (line 445)
  - Updated `UpdateBuyer()` method (line 625)
  - Removed `BuyerDetailRequest` class

## Testing Steps

1. **Create a new property with buyer**:
   - Fill in all property details
   - Add a buyer with electronic national ID number
   - Save and verify no errors

2. **Edit existing buyer**:
   - Open an existing property
   - Click edit on a buyer
   - Verify the form loads without errors
   - Verify the national ID number displays correctly

3. **Load buyer list**:
   - Navigate to property details
   - Verify buyer list displays correctly
   - Verify national ID numbers show in the list

## Benefits

1. **Consistency**: Single field name used throughout the entire application
2. **Correctness**: Uses proper spelling (electronic, not "indent")
3. **Maintainability**: No more confusion about which field name to use
4. **Type Safety**: Changed from `number` to `string` type (more appropriate for ID numbers)
5. **Clean Code**: Removed unnecessary compatibility layer and DTO classes

## Related Issues Fixed

This fix is part of a series of fixes for the Property module:
1. ✅ Property form validation error ('des' field)
2. ✅ PostgreSQL DateTime timezone errors
3. ✅ PNumber NOT NULL constraint
4. ✅ Buyer form JSON type mismatch (royaltyAmount)
5. ✅ **Form control name standardization (electronicNationalIdNumber)** ← Current fix

## Date
January 27, 2026
