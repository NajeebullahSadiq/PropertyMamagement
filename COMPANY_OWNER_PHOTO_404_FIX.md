# Company Owner Photo 404 Error Fix

## Problem
When viewing or editing company owner details, the owner's photo fails to load with a 404 error. The URL being constructed is incorrect:

**Wrong URL**: `http://103.132.98.92/api/Upload/view//api/Resources/Documents/Company/profile_20260126_073209_448.jpg`

**Should be**: `http://103.132.98.92/api/Resources/Documents/Company/profile_20260126_073209_448.jpg`

## Root Cause
The database stores the full path: `Resources/Documents/Company/profile_20260126_073209_448.jpg`

But the frontend code was:
1. Adding `/api/` prefix in companyowner component
2. Then passing that constructed URL to profile-image-cropper component
3. Profile-image-cropper was adding `/Upload/view/` prefix again

This created a double path construction issue.

## Solution

### Files Changed

1. **Frontend/src/app/realestate/companyowner/companyowner.component.ts**
   - Pass RAW database path to `setExistingImage()` instead of constructed URL
   - Let the child component handle URL construction

2. **Frontend/src/app/shared/profile-image-cropper/profile-image-cropper.component.ts**
   - Updated `getImageUrl()` method to detect if path starts with `Resources/`
   - If yes: use static file serving (`/api/Resources/...`)
   - If no: use Upload/view endpoint (`/api/Upload/view/...`)

3. **Frontend/src/app/print-license/print-license.component.ts**
   - Same logic as profile-image-cropper for handling photo paths

## QUICK FIX GUIDE

### The Problem
Photos fail to load with 404 error because:
1. **Frontend not deployed** - The fix is in code but not on production server
2. **Resources folder missing** - Uploaded photos are in `~/PropertyMamagement/Backend/Resources/` but not in `/var/www/prmis/backend/Resources/`

### The Solution (3 Steps)

**Step 1: Run the deployment script**
```bash
cd ~/PropertyMamagement
chmod +x deploy_photo_fix.sh
./deploy_photo_fix.sh
```

This script will:
- Copy Resources folder to deployment location
- Deploy the fixed frontend code
- Set proper permissions

**Step 2: Clear browser cache**
- Press `Ctrl+Shift+R` (hard refresh)
- Or `Ctrl+F5`
- Or clear cache manually in browser settings

**Step 3: Test**
```bash
cd ~/PropertyMamagement
chmod +x test_photo_fix.sh
./test_photo_fix.sh
```

This will verify:
- Resources folder exists with photos
- Frontend is deployed
- Photos are accessible via HTTP

### Manual Deployment (if script fails)

If the script doesn't work, run these commands manually:

### Manual Deployment (if script fails)

If the script doesn't work, run these commands manually:

#### Manual Step 1: Copy Resources Folder
The uploaded images are in the development location but not in the deployment location. Copy them:

```bash
# Check if files exist in development location
ls -la ~/PropertyMamagement/Backend/Resources/Documents/Company/

# Copy all Resources to deployment location
sudo cp -r ~/PropertyMamagement/Backend/Resources/* /var/www/prmis/backend/Resources/

# Set proper permissions
sudo chown -R www-data:www-data /var/www/prmis/backend/Resources
sudo chmod -R 755 /var/www/prmis/backend/Resources

# Verify files were copied
ls -la /var/www/prmis/backend/Resources/Documents/Company/
```

#### Manual Step 2: Deploy Frontend
The frontend code has been fixed but NOT deployed yet. Deploy it now:

```bash
cd ~/PropertyMamagement/Frontend
git pull origin main
npx ng build --configuration production
sudo rm -rf /var/www/prmis/frontend/*
sudo cp -r dist/property-registeration-mis/* /var/www/prmis/frontend/
sudo chown -R www-data:www-data /var/www/prmis/frontend
sudo chmod -R 755 /var/www/prmis/frontend
```

#### Manual Step 3: Clear Browser Cache
After deployment, users MUST clear their browser cache or do a hard refresh (Ctrl+F5) to load the new JavaScript files.

**Important**: The old JavaScript files are cached in the browser. Without clearing cache, users will still see the old broken behavior.

## Verification

After deployment and cache clear, check:
1. ✓ Photo loads in company owner form (edit mode)
2. ✓ Photo loads in print license view
3. ✓ URL should be: `http://103.132.98.92/api/Resources/Documents/Company/profile_xxx.jpg`
4. ✓ No 404 errors in browser console

## Technical Details

### Database Path Format
- Stored as: `Resources/Documents/Company/profile_20260126_073209_448.jpg`
- This is the FULL path relative to the backend root

### Static File Serving (Program.cs)
```csharp
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(resourcesPath),
    RequestPath = new PathString("/Resources")
});
```

This means files in `Backend/Resources/` are served at `/Resources/` URL path.

### URL Construction Logic
```typescript
// If path starts with "Resources/"
if (path.startsWith('Resources/')) {
    // Use: http://103.132.98.92/api/Resources/Documents/Company/profile_xxx.jpg
    return `${environment.apiURL}/${path}`;
} else {
    // Use: http://103.132.98.92/api/Upload/view/profile_xxx.jpg
    return `${environment.apiURL}/Upload/view/${path}`;
}
```

## Status
- ✅ Backend: Deployed
- ✅ Frontend Code: Fixed and committed
- ❌ Frontend Deployment: **NOT DEPLOYED YET - NEEDS DEPLOYMENT**
- ⏳ Browser Cache: Users need to clear cache after deployment
- ❌ Resources Folder: **Files missing on server - needs copy**

## Troubleshooting

If issue persists after deployment:

1. **Verify frontend files were actually updated**:
   ```bash
   ls -la /var/www/prmis/frontend/main*.js
   # Should show recent timestamp
   ```

2. **Check browser is loading new files**:
   - Open browser DevTools → Network tab
   - Hard refresh (Ctrl+Shift+R or Ctrl+F5)
   - Check if `main.*.js` file has new hash in filename

3. **Verify file exists on server**:
   ```bash
   ls -la /var/www/prmis/backend/Resources/Documents/Company/
   # Should show profile_*.jpg files
   ```

4. **Test direct URL access**:
   ```
   http://103.132.98.92/api/Resources/Documents/Company/profile_20260126_073209_448.jpg
   ```
   Should return the image file, not 404.
