# URGENT: Photo 404 Fix Instructions

## The Problem

The error shows the OLD JavaScript is still running:
```
http://103.132.98.92/api/Upload/view//api/Resources/Documents/Company/profile_xxx.jpg
```

This double `/api/Upload/view//api/Resources/` means **the browser is using cached JavaScript**.

## The Solution (3 Steps)

### Step 1: Run Diagnostic

```bash
cd ~/PropertyMamagement
chmod +x diagnose_photo_issue.sh
./diagnose_photo_issue.sh
```

This will tell you:
- ✓ If Resources folder exists in deployment
- ✓ What's in the database
- ✓ If files are accessible

### Step 2: Copy Resources Folder

```bash
cd ~/PropertyMamagement
chmod +x fix_photo_complete.sh
./fix_photo_complete.sh
```

This will:
- Copy all photos to deployment location
- Set correct permissions
- Test if files are accessible

### Step 3: Clear Browser Cache (CRITICAL!)

The frontend IS deployed, but your browser is using OLD cached JavaScript files.

**You MUST do this:**

1. **Close the browser completely**
2. **Reopen browser**
3. **Press Ctrl+Shift+Delete**
4. **Select "Cached images and files"**
5. **Select "All time"**
6. **Click "Clear data"**
7. **Close browser again**
8. **Reopen and test**

OR use DevTools:
1. Open DevTools (F12)
2. Go to Network tab
3. Check "Disable cache"
4. Right-click refresh button
5. Select "Empty Cache and Hard Reload"

## How to Verify It's Fixed

### Before Fix (OLD - WRONG):
```
http://103.132.98.92/api/Upload/view//api/Resources/Documents/Company/profile_xxx.jpg
                                    ^^
                                    Double slash - WRONG!
```

### After Fix (NEW - CORRECT):
```
http://103.132.98.92/api/Resources/Documents/Company/profile_xxx.jpg
                         ^
                         Single path - CORRECT!
```

## Why This Happened

1. **Frontend WAS deployed** - The new code is on the server
2. **Browser cache NOT cleared** - Browser is using old JavaScript from cache
3. **Resources folder missing** - Photos weren't copied to deployment location

## Quick Test

After clearing cache, open browser console (F12) and check the Network tab:

1. Go to Company Owner form
2. Look at the image request URL
3. It should be: `http://103.132.98.92/api/Resources/Documents/Company/profile_xxx.jpg`
4. NOT: `http://103.132.98.92/api/Upload/view//api/Resources/...`

If you still see the double path, **your browser cache is NOT cleared**.

## Alternative: Test in Incognito/Private Window

1. Open browser in Incognito/Private mode
2. Navigate to your site
3. Login
4. Go to Company Owner form
5. Check if photo loads

If it works in Incognito but not in normal browser, **it's definitely a cache issue**.

## Still Not Working?

Run this to check everything:

```bash
cd ~/PropertyMamagement
./test_photo_fix.sh
```

This will verify:
- Resources folder exists
- Frontend is deployed
- Database has correct paths
- Files are accessible via HTTP

## Need Help?

If after following ALL steps above it still doesn't work, provide:

1. Output of `./diagnose_photo_issue.sh`
2. Screenshot of browser console showing the URL being requested
3. Confirmation that you cleared browser cache completely
4. Test result from Incognito/Private window
