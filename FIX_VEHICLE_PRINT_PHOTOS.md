# Fix Vehicle Print Photos Not Displaying

## Problem
Photos for seller and buyer are not showing in the vehicle print page, even though they are correctly stored in the database and display properly in the edit view.

## Root Cause
1. The database view `getVehiclePrintData` needs to be recreated to properly include the `Photo` column from seller and buyer tables
2. The frontend was looking for photos in the wrong folder (`Identity` instead of `Profile`)

## Solution Applied

### 1. Frontend Fix (TypeScript)
**File**: `Frontend/src/app/printvehicledata/printvehicledata.component.ts`

Updated the photo path handling to:
- Check if photo path already starts with `Resources/` (full path)
- If not, prepend `Resources/Documents/Profile/` (correct folder for vehicle photos)
- Added console logging to debug photo loading
- Changed from `Identity` folder to `Profile` folder

```typescript
// Handle seller photo - check if it's a full path or just filename
if (res.sellerPhoto) {
  const sellerPhotoPath = res.sellerPhoto.startsWith('Resources/') 
    ? res.sellerPhoto 
    : `Resources/Documents/Profile/${res.sellerPhoto}`;
  this.SellerfilePath = `${this.baseUrl}api/Upload/view/${sellerPhotoPath}`;
} else {
  this.SellerfilePath = 'assets/img/avatar2.png';
}
```

### 2. Database View Recreation Required
**File**: `Backend/Scripts/create_vehicle_print_view.sql`

The view already has the correct SQL to select Photo columns:
```sql
s."Photo" AS "SellerPhoto",
b."Photo" AS "BuyerPhoto",
```

**ACTION REQUIRED**: Run this SQL script in PostgreSQL to recreate the view:
```bash
psql -U your_username -d your_database -f Backend/Scripts/create_vehicle_print_view.sql
```

Or execute the SQL directly in pgAdmin or your PostgreSQL client.

### 3. Test Query Created
**File**: `Backend/Scripts/test_vehicle_print_view.sql`

Created a test query to verify the view is returning photo data correctly after recreation.

## Data Verification
From the API responses provided:
- Seller photo: `Resources/Documents/Profile/profile_20260203_050123_020.jpg`
- Buyer photo: `Resources/Documents/Profile/profile_20260203_051035_273.jpg`
- Photos are correctly stored in the database
- Photos display correctly in edit view
- Backend controller returns `SellerPhoto` and `BuyerPhoto` fields

## How Photos Are Stored
Vehicle photos are stored in: `Resources/Documents/Profile/`
- This is different from property photos which may use `Identity` folder
- The upload controller returns the full path: `Resources/Documents/Profile/filename.jpg`
- The API endpoint to view photos: `/api/Upload/view/{fullPath}`

## Testing Steps
1. Run the `create_vehicle_print_view.sql` script to recreate the database view
2. Open browser developer console (F12)
3. Navigate to vehicle print page
4. Check console logs for:
   - "Print data received:" - should show the full response
   - "Seller photo:" - should show the photo path
   - "Buyer photo:" - should show the photo path
   - "Seller file path:" - should show the complete URL
   - "Buyer file path:" - should show the complete URL
5. Check Network tab for any 404 errors on photo requests
6. Verify photos display correctly

## Expected Result
After running the SQL script and with the frontend fix:
- Seller photo should display in the print page
- Buyer photo should display in the print page
- No 404 errors in browser console
- Photos load from: `http://localhost:5143/api/Upload/view/Resources/Documents/Profile/filename.jpg`

## Files Modified
1. `Frontend/src/app/printvehicledata/printvehicledata.component.ts` - Fixed photo path handling
2. `Backend/Scripts/test_vehicle_print_view.sql` - Created test query

## Files to Execute
1. `Backend/Scripts/create_vehicle_print_view.sql` - **MUST RUN** to recreate the database view
