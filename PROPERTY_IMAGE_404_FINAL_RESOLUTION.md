# Property Module Image 404 Error - Final Resolution

## Issue Confirmed

The 404 errors are caused by **stale database references** to files that no longer exist:

### Database References (OLD - December 2025)
```
profile_20251227_092027_197.jpg
profile_20251227_055335_033.jpg
```

### Actual Files on Disk (NEW - January 2026)
```
profile_20260127_100310_489.jpg
profile_20260127_113948_847.jpg
profile_20260127_114059_541.jpg
profile_20260127_120435_305.jpg
profile_20260127_160452_695.jpg
profile_20260127_160512_566.jpg
```

## Root Cause

The database contains paths to photos that were uploaded in December 2025, but those files have been deleted or the database was restored from an old backup. The current photos on disk are from January 2026.

## Solution Options

### Option 1: Clean Up Database (Recommended)

Set the photo field to NULL for records that reference non-existent files:

```sql
-- Check which seller photos don't exist
SELECT 
    "Id",
    "Name",
    "Photo"
FROM tr."SellerDetails"
WHERE "Photo" LIKE '%20251227%';

-- Set them to NULL
UPDATE tr."SellerDetails"
SET "Photo" = NULL
WHERE "Photo" LIKE '%20251227%';

-- Same for buyers
UPDATE tr."BuyerDetails"
SET "Photo" = NULL
WHERE "Photo" LIKE '%20251227%';
```

After this, users will see the default avatar icon and can re-upload their photos.

### Option 2: Have Users Re-upload

Simply have users:
1. Navigate to Property Details
2. Edit the seller/buyer
3. Upload a new photo
4. Save

The new photo will overwrite the NULL value and display correctly.

### Option 3: Map Old References to New Files (Not Recommended)

If you know which old photo corresponds to which new photo, you could update the database:

```sql
-- Example: Map old photo to new photo
UPDATE property."SellerDetail"
SET photo = 'Resources/Documents/Profile/profile_20260127_100310_489.jpg'
WHERE photo = 'Resources/Documents/Profile/profile_20251227_092027_197.jpg';
```

This is error-prone and not recommended unless you have a clear mapping.

## System Status

### ✅ What's Working

1. **Backend Configuration**: Program.cs correctly:
   - Creates Resources folder structure on startup
   - Serves static files at `/api/Resources/`
   - Saves uploads to `bin/Debug/net9.0/Resources/Documents/Profile/`

2. **Frontend URL Construction**: Components correctly:
   - Use `constructImageUrl()` helper
   - Handle `Resources/` paths properly
   - Pass raw paths to `profile-image-cropper`

3. **File Storage**: 
   - Profile folder exists with 6 photos
   - Property folder exists with 9 images
   - Company folder exists with 26 photos
   - Identity folder exists with 5 documents

### ⚠️ What Needs Attention

1. **Database Cleanup**: Remove references to non-existent December 2025 photos
2. **User Re-upload**: Have users upload new photos for affected records

## Verification Steps

### 1. Check Database for Stale References

```sql
-- Find all stale photo references
SELECT 
    'Seller' as type,
    "Id" as id,
    "Name" as name,
    "Photo" as photo
FROM tr."SellerDetails"
WHERE "Photo" LIKE '%2025%'

UNION ALL

SELECT 
    'Buyer' as type,
    "Id" as id,
    "Name" as name,
    "Photo" as photo
FROM tr."BuyerDetails"
WHERE "Photo" LIKE '%2025%';
```

### 2. Clean Up Stale References

```sql
-- Clean up sellers
UPDATE tr."SellerDetails"
SET "Photo" = NULL
WHERE "Photo" LIKE '%2025%';

-- Clean up buyers
UPDATE tr."BuyerDetails"
SET "Photo" = NULL
WHERE "Photo" LIKE '%2025%';
```

### 3. Test New Upload

1. Navigate to Property module
2. Edit a seller/buyer that had a stale photo reference
3. Upload a new photo
4. Save and verify it displays correctly
5. Check browser console - should be no 404 errors

### 4. Verify Existing Photos

For records with January 2026 photos, verify they display correctly:
1. Navigate to Property Details
2. Check seller/buyer tabs
3. Photos should display without 404 errors

## Technical Details

### Backend File Serving

The backend serves files from:
```
{AppContext.BaseDirectory}/Resources/
```

In Debug mode, this is:
```
C:\Users\Najib\OneDrive\Desktop\PropertyMamagement\Backend\bin\Debug\net9.0\Resources\
```

Static files are served at:
- `/Resources/*` (direct)
- `/api/Resources/*` (with API prefix)

### Frontend URL Construction

When the database has:
```
Resources/Documents/Profile/profile_20260127_100310_489.jpg
```

The frontend constructs:
```
http://localhost:5143/api/Resources/Documents/Profile/profile_20260127_100310_489.jpg
```

The backend serves this file via the static file middleware.

### Upload Flow

1. User uploads photo via frontend
2. Frontend sends to `/api/Upload` endpoint
3. Backend saves to: `{AppContext.BaseDirectory}/Resources/Documents/Profile/profile_{timestamp}.jpg`
4. Backend stores in DB: `Resources/Documents/Profile/profile_{timestamp}.jpg`
5. Frontend displays using: `http://localhost:5143/api/Resources/Documents/Profile/profile_{timestamp}.jpg`

## Files Modified

- ✅ Frontend/src/app/print/print.component.ts
- ✅ Frontend/src/app/estate/propertydetails/buyerdetail/buyerdetail.component.ts
- ✅ Frontend/src/app/estate/propertydetails/sellerdetail/sellerdetail.component.ts
- ✅ Frontend/src/app/shared/profile-image-cropper/profile-image-cropper.component.ts
- ✅ Backend/Scripts/check_resource_files.sql (fixed table names)
- ✅ Backend/Scripts/verify_resources_setup.ps1 (verification script)
- ✅ Backend/Resources/Documents/Profile/ (folder created)
- ✅ Backend/Resources/Documents/Identity/ (folder created)
- ✅ Backend/Resources/Images/ (folder created)

## Recommended Action Plan

1. **Run database cleanup** (5 minutes):
   ```sql
   UPDATE property."SellerDetail" SET photo = NULL WHERE photo LIKE '%2025%';
   UPDATE property."BuyerDetail" SET photo = NULL WHERE photo LIKE '%2025%';
   ```

2. **Restart backend** (if not already running):
   - This ensures all folders are created
   - Static file serving is active

3. **Test with one property** (5 minutes):
   - Navigate to Property Details
   - Edit seller/buyer
   - Upload new photo
   - Verify it displays correctly

4. **Notify users** (optional):
   - Let them know they need to re-upload photos
   - Or do it for them if there are only a few records

## Expected Results

After cleanup:
- ✅ No more 404 errors in browser console
- ✅ Records with January 2026 photos display correctly
- ✅ Records with NULL photos show default avatar
- ✅ New uploads work correctly
- ✅ Print view displays photos correctly

## Production Considerations

For production deployment:

1. **Configure persistent storage** in appsettings.Production.json:
   ```json
   {
     "FileStorage": {
       "RootPath": "/var/www/prmis/storage"
     }
   }
   ```

2. **Set up backup** for the storage directory

3. **Ensure permissions** are correct:
   ```bash
   sudo chown -R www-data:www-data /var/www/prmis/storage
   sudo chmod -R 755 /var/www/prmis/storage
   ```

4. **Test file uploads** after deployment

This ensures uploaded files persist across deployments and rebuilds.
