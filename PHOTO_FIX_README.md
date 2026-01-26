# Company Owner Photo 404 Fix - Quick Start

## Problem
When viewing/editing company owner details or printing license, owner photos fail to load with 404 error.

**Error in browser console:**
```
GET http://103.132.98.92/api/Upload/view//api/Upload/view/Resources/Documents/Company/profile_xxx.jpg 404 (Not Found)
```

## Root Cause
1. **Frontend code not deployed** - The fix exists in code but hasn't been deployed to production
2. **Resources folder missing** - Uploaded photos exist in development folder but not in deployment folder

## Solution

### Quick Fix (Recommended)

Run the automated deployment script:

```bash
cd ~/PropertyMamagement
chmod +x deploy_photo_fix.sh
./deploy_photo_fix.sh
```

Then **clear your browser cache** (Ctrl+Shift+R or Ctrl+F5)

### Verify the Fix

Run the test script to verify everything is working:

```bash
cd ~/PropertyMamagement
chmod +x test_photo_fix.sh
./test_photo_fix.sh
```

### What the Script Does

1. **Copies Resources folder** from `~/PropertyMamagement/Backend/Resources/` to `/var/www/prmis/backend/Resources/`
2. **Deploys frontend** with the URL construction fix
3. **Sets permissions** correctly for web server access

### After Deployment

1. **Clear browser cache** - This is CRITICAL! The old JavaScript is cached
   - Press `Ctrl+Shift+R` (hard refresh)
   - Or `Ctrl+F5`
   - Or clear cache in browser settings

2. **Test in browser**:
   - Go to Company Owner form
   - Check if existing photo loads
   - Upload a new photo
   - Try printing license

3. **Expected URL format**:
   ```
   http://103.132.98.92/api/Resources/Documents/Company/profile_20260126_070151_288.jpg
   ```
   
   NOT:
   ```
   http://103.132.98.92/api/Upload/view//api/Upload/view/Resources/...
   ```

## Manual Deployment

If the script fails, see detailed manual steps in `COMPANY_OWNER_PHOTO_404_FIX.md`

## Files Changed

### Frontend (Fixed - Needs Deployment)
- `Frontend/src/app/realestate/companyowner/companyowner.component.ts` - Pass raw DB path to child
- `Frontend/src/app/shared/profile-image-cropper/profile-image-cropper.component.ts` - Smart URL construction
- `Frontend/src/app/print-license/print-license.component.ts` - Same URL logic

### Backend (Already Deployed)
- `Backend/Program.cs` - Static file serving configured
- `Backend/Controllers/UploadController.cs` - File serving endpoints

## Troubleshooting

### Photos still show 404 after deployment

1. **Check browser cache was cleared**:
   - Open DevTools → Network tab
   - Hard refresh (Ctrl+Shift+R)
   - Check if `main.*.js` has new timestamp

2. **Verify Resources folder exists**:
   ```bash
   ls -la /var/www/prmis/backend/Resources/Documents/Company/
   ```

3. **Test direct file access**:
   ```bash
   curl -I http://103.132.98.92/api/Resources/Documents/Company/profile_20260126_070151_288.jpg
   ```
   Should return `HTTP/1.1 200 OK`

4. **Check file permissions**:
   ```bash
   sudo chown -R www-data:www-data /var/www/prmis/backend/Resources
   sudo chmod -R 755 /var/www/prmis/backend/Resources
   ```

### Frontend not updated

1. **Verify deployment**:
   ```bash
   ls -lh /var/www/prmis/frontend/main*.js
   ```
   Should show recent timestamp

2. **Rebuild and redeploy**:
   ```bash
   cd ~/PropertyMamagement/Frontend
   npx ng build --configuration production
   sudo rm -rf /var/www/prmis/frontend/*
   sudo cp -r dist/property-registeration-mis/* /var/www/prmis/frontend/
   ```

3. **Clear browser cache again**

## Support

For detailed technical information, see:
- `COMPANY_OWNER_PHOTO_404_FIX.md` - Complete technical documentation
- `deploy_photo_fix.sh` - Automated deployment script
- `test_photo_fix.sh` - Verification test script
