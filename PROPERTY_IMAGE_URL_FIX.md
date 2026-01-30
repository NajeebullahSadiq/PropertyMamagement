# Property Module Image URL Fix

## Issue
Images in the Property module were failing to load with 404 errors due to incorrect URL construction:

### Error Patterns
1. **Profile images**: 
   - `http://localhost:5143/api/api/Upload/view/Resources/Documents/Profile/...` (duplicate `/api`)
   - `http://localhost:5143/api/Resources/Documents/Profile/...` (404 - backend not serving static files)

2. **Property images**:
   - `http://localhost:5143/api/Resources/Images/Resources/Documents/Property/...` (duplicate `/Resources`)

## Root Cause
1. The database stores full paths like `Resources/Documents/Profile/profile_20251227_092027_197.jpg`
2. The frontend was inconsistently constructing URLs
3. **The backend is not configured to serve static files from the Resources folder**

## Solution

### Frontend Changes
Added a centralized `constructImageUrl()` helper method to Property module components and updated the `profile-image-cropper` component:

```typescript
private constructImageUrl(path: string): string {
  if (!path) return 'assets/img/avatar.png';
  
  // If path already starts with http/https or is a blob URL, return as is
  if (path.startsWith('http://') || path.startsWith('https://') || path.startsWith('blob:')) {
    return path;
  }
  
  // If it's an assets path, return as is
  if (path.startsWith('assets/')) {
    return path;
  }
  
  // If path starts with Resources/, it's a full path from DB - use static file serving
  if (path.startsWith('Resources/') || path.startsWith('/Resources/')) {
    const cleanPath = path.startsWith('/') ? path.substring(1) : path;
    return `${this.baseUrl}${cleanPath}`;
  }
  
  // Otherwise, use Upload/view endpoint
  return `${this.baseUrl}Upload/view/${path}`;
}
```

**Important**: When passing paths to `profile-image-cropper.setExistingImage()`, pass the raw path from the database, not a constructed URL. The cropper component will handle URL construction through its own `getImageUrl()` method.

### Backend Configuration Needed

The backend needs to serve static files from the `Resources` folder. Add this to your backend configuration:

**Option 1: ASP.NET Core Static Files Middleware**

In `Program.cs` or `Startup.cs`:

```csharp
// Enable static file serving
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Resources")),
    RequestPath = "/api/Resources"
});
```

**Option 2: Nginx Configuration** (if using Nginx as reverse proxy)

```nginx
location /api/Resources/ {
    alias /path/to/backend/Resources/;
    expires 30d;
    add_header Cache-Control "public, immutable";
}
```

## Files Modified

### Frontend Components

1. **Frontend/src/app/print/print.component.ts**
   - Added `constructImageUrl()` helper method
   - Updated all image path constructions

2. **Frontend/src/app/estate/propertydetails/buyerdetail/buyerdetail.component.ts**
   - Added `constructImageUrl()` helper method
   - Updated image path constructions
   - Fixed `setExistingImage()` calls to pass raw paths

3. **Frontend/src/app/estate/propertydetails/sellerdetail/sellerdetail.component.ts**
   - Added `constructImageUrl()` helper method
   - Updated image path constructions
   - Fixed `setExistingImage()` calls to pass raw paths

4. **Frontend/src/app/shared/profile-image-cropper/profile-image-cropper.component.ts**
   - Updated `getImageUrl()` method with better comments
   - Ensures proper handling of `Resources/` paths

## How It Works

### URL Construction Logic

1. **Full paths from DB** (e.g., `Resources/Documents/Profile/photo.jpg`):
   - Frontend constructs: `http://localhost:5143/api/Resources/Documents/Profile/photo.jpg`
   - Backend must serve static files at this location

2. **Relative paths** (e.g., `photo.jpg`):
   - Uses Upload/view endpoint: `http://localhost:5143/api/Upload/view/photo.jpg`

3. **Already complete URLs** (e.g., `http://...` or `blob:...`):
   - Returns as-is

4. **Asset paths** (e.g., `assets/img/avatar.png`):
   - Returns as-is

### Component Communication

- Parent components (buyerdetail, sellerdetail) pass **raw database paths** to `profile-image-cropper`
- The `profile-image-cropper` component handles URL construction through its `getImageUrl()` method
- This prevents double URL construction and ensures consistency

## Testing

Test the following scenarios:

1. **Print Property Document**:
   ```
   Navigate to: /print/:propertyId
   Verify: Seller photo, buyer photo, and property images load correctly
   ```

2. **Edit Buyer Details**:
   ```
   Navigate to: Property Details > Buyer tab
   - Load existing buyer with photo
   - Edit buyer and verify photo displays
   - Upload new photo and verify it displays
   ```

3. **Edit Seller Details**:
   ```
   Navigate to: Property Details > Seller tab
   - Load existing seller with photo
   - Edit seller and verify photo displays
   - Upload new photo and verify it displays
   ```

## Current Status

✅ Frontend URL construction fixed
✅ Backend static file serving is configured in Program.cs
⚠️ **Files may not exist at the expected location**

The backend is already configured to serve static files from the `Resources` folder at `/api/Resources/`. However, the 404 errors indicate that either:
1. The files don't exist at the expected location
2. The file paths in the database don't match the actual file locations
3. The Resources folder is in a different location than expected

## Troubleshooting Steps

### 1. Check if files exist on disk

Run the PowerShell script to check file existence:

```powershell
cd Backend/Scripts
.\check_files_exist.ps1
```

This will show you:
- Whether the Resources directory exists
- How many files are in each subdirectory
- Whether the specific files from the error logs exist

### 2. Check database paths

Run the SQL script to see what paths are stored in the database:

```sql
psql -U your_user -d your_database -f Backend/Scripts/check_resource_files.sql
```

This will show you the photo paths stored in the database for sellers, buyers, and properties.

### 3. Verify backend configuration

The backend Program.cs should have (already present):

```csharp
var storageRoot = builder.Configuration["FileStorage:RootPath"] ?? AppContext.BaseDirectory;
var resourcesPath = Path.Combine(storageRoot, "Resources");

app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(resourcesPath),
    RequestPath = new PathString("/api/Resources")
});
```

### 4. Common Issues and Solutions

**Issue 1: Files uploaded to wrong location**
- Check where the Upload controller is saving files
- Ensure it's saving to `Resources/Documents/Profile/` not just `Resources/`

**Issue 2: Database has incorrect paths**
- If database has `Resources/Documents/Profile/photo.jpg` but files are at `Documents/Profile/photo.jpg`
- You may need to update database paths or move files

**Issue 3: Resources folder in wrong location**
- Backend expects Resources folder at: `{AppContext.BaseDirectory}/Resources`
- Check if it's actually there or in a parent directory

### 5. Quick Fix: Create symlink or copy files

If files are in a different location, you can:

**Option A: Create symbolic link (Windows)**
```cmd
mklink /D "C:\path\to\backend\Resources" "C:\actual\location\Resources"
```

**Option B: Copy files to correct location**
```powershell
Copy-Item -Path "C:\actual\location\Resources\*" -Destination "C:\path\to\backend\Resources\" -Recurse
```

## Next Steps

1. Add static file serving middleware to the backend (see Backend Configuration Needed section above)
2. Restart the backend server
3. Test image loading in the Property module
4. Apply the same pattern to other modules (Vehicle, Company, etc.) if needed

## Benefits

1. **Consistent URL Construction**: All image URLs are now constructed using the same logic
2. **Handles Multiple Formats**: Works with full paths, relative paths, and URLs
3. **Prevents Duplicates**: Avoids duplicate path segments
4. **Maintainable**: Centralized logic makes future updates easier
5. **Backward Compatible**: Works with both old and new path formats

