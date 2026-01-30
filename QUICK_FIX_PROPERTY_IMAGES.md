# Quick Fix: Property Module Image 404 Errors

## Problem
Images showing 404 errors in Property module browser console.

## Root Cause
Database references old photos from December 2025 that no longer exist. Actual photos on disk are from January 2026.

## Quick Solution (5 minutes)

### Step 1: Check Affected Records

Run this SQL to see which records are affected:

```sql
-- Check sellers (Photo column is capitalized, FirstName not Name)
SELECT "Id", "FirstName", "FatherName", "Photo"
FROM tr."SellerDetails"
WHERE "Photo" LIKE '%2025%';

-- Check buyers (photo column is lowercase, FirstName not Name)
SELECT "Id", "FirstName", "FatherName", "photo"
FROM tr."BuyerDetails"
WHERE "photo" LIKE '%2025%';
```

### Step 2: Clean Up Stale References

Run this SQL to remove stale photo references:

```sql
-- Clean up sellers
UPDATE tr."SellerDetails"
SET "Photo" = NULL
WHERE "Photo" LIKE '%2025%';

-- Clean up buyers
UPDATE tr."BuyerDetails"
SET "photo" = NULL
WHERE "photo" LIKE '%2025%';
```

### Step 3: Verify

1. Refresh the Property module in your browser
2. Check browser console - 404 errors should be gone
3. Records will show default avatar icon
4. Users can now re-upload their photos

## Alternative: Use the Script

We've created a script that does this for you:

```bash
# Connect to your database
psql -U your_user -d your_database

# Run the CORRECTED script (has actual column names)
\i Backend/Scripts/cleanup_stale_photos_CORRECTED.sql

# Review the output, then uncomment the UPDATE statements and run again
```

**Note**: Use `cleanup_stale_photos_CORRECTED.sql` which has the actual column names from your database:
- SellerDetails: `"FirstName"`, `"Photo"` (capitalized)
- BuyerDetails: `"FirstName"`, `"photo"` (lowercase)

## What This Does

- Sets `photo` field to `NULL` for records with non-existent December 2025 photos
- Records will show default avatar icon
- No data is lost (just the reference to a non-existent file)
- Users can re-upload photos anytime

## Expected Results

✅ No more 404 errors in browser console
✅ Records with January 2026 photos still work correctly
✅ Records with NULL photos show default avatar
✅ New uploads work correctly

## Files Created

- `Backend/Scripts/cleanup_stale_photo_references.sql` - SQL cleanup script
- `Backend/Scripts/verify_resources_setup.ps1` - Verification script
- `PROPERTY_IMAGE_404_SOLUTION.md` - Detailed documentation
- `PROPERTY_IMAGE_404_FINAL_RESOLUTION.md` - Complete analysis

## Need More Details?

See `PROPERTY_IMAGE_404_FINAL_RESOLUTION.md` for complete technical details and analysis.
