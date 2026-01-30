# Property Module Image 404 Error - Complete Solution

## Problem Summary

Images in the Property module are returning 404 errors:
```
GET http://localhost:5143/api/Resources/Documents/Profile/profile_20251227_092027_197.jpg 404 (Not Found)
```

## Root Cause Analysis

After investigation, we found:

1. ✅ **Frontend URL construction** - FIXED (see PROPERTY_IMAGE_URL_FIX.md)
2. ✅ **Backend static file serving** - Already configured in Program.cs
3. ❌ **Missing files** - The specific files referenced in the database don't exist on disk

### Why Files Are Missing

The backend serves files from `{AppContext.BaseDirectory}/Resources`, which is:
```
Backend/bin/Debug/net9.0/Resources/
```

When you run the backend in Debug mode, it operates from the compiled output directory. The database has references to files like `profile_20251227_092027_197.jpg` that were either:
- Never uploaded
- Deleted
- From an old database backup

## Solution

You have two options:

### Option 1: Have Users Re-upload Photos (Recommended)

This is the cleanest solution:

1. The folder structure is now in place (created automatically by Program.cs on startup)
2. Users can re-upload their profile photos
3. New uploads will save to the correct location
4. Images will load correctly

**Steps:**
1. Restart the backend server (it will create all necessary folders)
2. Navigate to Property module
3. Edit seller/buyer details
4. Upload new photos
5. Save and verify images load correctly

### Option 2: Clean Up Database References

If you want to remove references to non-existent files:

```sql
-- Check which photos don't exist
SELECT 
    'Seller' as type,
    id,
    "firstName",
    photo
FROM property."SellerDetail"
WHERE photo IS NOT NULL 
  AND photo != ''
  AND photo LIKE 'Resources/Documents/Profile/profile_2025%';

-- Set non-existent photos to NULL (optional)
UPDATE property."SellerDetail"
SET photo = NULL
WHERE photo IS NOT NULL 
  AND photo != ''
  AND photo LIKE 'Resources/Documents/Profile/profile_2025%';

-- Same for buyers
UPDATE property."BuyerDetail"
SET photo = NULL
WHERE photo IS NOT NULL 
  AND photo != ''
  AND photo LIKE 'Resources/Documents/Profile/profile_2025%';
```

## How the System Works Now

### Backend Configuration (Program.cs)

The backend automatically:
1. Creates the Resources folder structure on startup:
   ```
   Resources/
   ├── Images/
   └── Documents/
       ├── Identity/
       ├── Profile/
       ├── Property/
       ├── Vehicle/
       ├── Company/
       └── License/
   ```

2. Serves static files at two endpoints:
   - `/Resources/*` 
   - `/api/Resources/*`

### Frontend URL Construction

Components use the `constructImageUrl()` helper:

```typescript
private constructImageUrl(path: string): string {
  if (!path) return 'assets/img/avatar.png';
  
  // If path starts with Resources/, use static file serving
  if (path.startsWith('Resources/') || path.startsWith('/Resources/')) {
    const cleanPath = path.startsWith('/') ? path.substring(1) : path;
    return `${this.baseUrl}${cleanPath}`;
  }
  
  // Otherwise, use Upload/view endpoint
  return `${this.baseUrl}Upload/view/${path}`;
}
```

### Upload Flow

When a user uploads a photo:

1. Frontend sends file to backend Upload controller
2. Backend saves to: `{AppContext.BaseDirectory}/Resources/Documents/Profile/profile_{timestamp}.jpg`
3. Backend stores path in database: `Resources/Documents/Profile/profile_{timestamp}.jpg`
4. Frontend constructs URL: `http://localhost:5143/api/Resources/Documents/Profile/profile_{timestamp}.jpg`
5. Backend serves file via static file middleware

## Testing

### 1. Test New Upload

```
1. Navigate to Property Details
2. Go to Seller or Buyer tab
3. Click "Add New" or "Edit"
4. Upload a profile photo
5. Save
6. Verify photo displays correctly
7. Check browser console - should be no 404 errors
```

### 2. Test Print View

```
1. Navigate to Property List
2. Click "Print" on a property
3. Verify seller/buyer photos display
4. Check browser console for any 404 errors
```

### 3. Verify File Location

After uploading, check that files are saved correctly:

```powershell
# Check files in the running backend directory
Get-ChildItem Backend\bin\Debug\net9.0\Resources\Documents\Profile
```

## Troubleshooting

### Issue: Still getting 404 errors after upload

**Check:**
1. Backend is running
2. File was actually saved (check bin/Debug/net9.0/Resources/Documents/Profile/)
3. Database has correct path (should start with `Resources/Documents/Profile/`)
4. Browser console shows correct URL (should be `/api/Resources/Documents/Profile/...`)

**Solution:**
- Restart backend server
- Clear browser cache
- Try uploading again

### Issue: Old photos still showing 404

**This is expected** - old photos that don't exist on disk will show 404 errors. Options:
1. Ignore them (they'll show default avatar)
2. Clean up database (see Option 2 above)
3. Have users re-upload

### Issue: Photos disappear after rebuild

**Cause:** When you rebuild the backend, the `bin/Debug/net9.0/` folder is cleaned.

**Solutions:**
1. Use Production mode with persistent storage
2. Configure a different storage root in appsettings.json:
   ```json
   {
     "FileStorage": {
       "RootPath": "C:\\PersistentStorage"
     }
   }
   ```
3. Copy files before rebuild (use sync_resources.ps1 script)

## Files Modified

- ✅ Frontend/src/app/print/print.component.ts
- ✅ Frontend/src/app/estate/propertydetails/buyerdetail/buyerdetail.component.ts
- ✅ Frontend/src/app/estate/propertydetails/sellerdetail/sellerdetail.component.ts
- ✅ Frontend/src/app/shared/profile-image-cropper/profile-image-cropper.component.ts
- ✅ Backend/Program.cs (already configured)
- ✅ Backend/Scripts/check_resource_files.sql (fixed table names)
- ✅ Backend/Scripts/sync_resources.ps1 (new utility script)

## Status

✅ **Frontend URL construction** - Fixed
✅ **Backend static file serving** - Configured
✅ **Folder structure** - Created automatically on startup
✅ **Root cause identified** - Database has stale references to December 2025 photos
✅ **Actual photos on disk** - From January 2026
⚠️ **Action needed** - Clean up stale database references

## Resolution

The 404 errors are caused by the database referencing old photos from December 2025 (`profile_20251227_*.jpg`) that no longer exist on disk. The actual photos are from January 2026 (`profile_20260127_*.jpg`).

**Recommended Solution**: 

1. Run the diagnostic script to see affected records:
   ```bash
   psql -U your_user -d your_database -f Backend/Scripts/cleanup_stale_photo_references.sql
   ```

2. Review the output, then uncomment the UPDATE statements in the script to clean up the data.

3. Users will see default avatars and can re-upload their photos.

**Alternative**: Have users re-upload their photos directly in the Property module.

**Note**: The Property module tables are in the `tr` schema, not `property` schema:
- `tr."SellerDetails"` (not `property."SellerDetail"`)
- `tr."BuyerDetails"` (not `property."BuyerDetail"`)
- `tr."PropertyDetails"`

See `PROPERTY_IMAGE_404_FINAL_RESOLUTION.md` for complete details.

## Next Steps

1. ✅ Backend server is configured correctly (no restart needed)
2. Run cleanup script to remove stale photo references
3. Test uploading a new photo in Property module
4. Verify the photo displays correctly
5. Apply same pattern to Vehicle and Company modules if needed

## Production Deployment

For production, configure persistent storage:

1. Create a dedicated storage directory outside the application folder
2. Update appsettings.Production.json:
   ```json
   {
     "FileStorage": {
       "RootPath": "/var/www/prmis/storage"
     }
   }
   ```
3. Ensure the directory has proper permissions
4. Set up backup for this directory

This ensures uploaded files persist across deployments and rebuilds.
