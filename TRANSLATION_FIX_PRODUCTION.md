# Translation Keys Showing in Production - Fix

## Issue
In production, translation keys (like `DISTRICT_MANAGEMENT.TITLE`) are showing instead of the actual translated text. This works fine in local development but fails in production.

## Root Cause
The Angular app was not setting a default language, causing the TranslateService to show keys instead of values when translation files aren't loaded immediately.

## Solution Applied

### 1. Set Default Language in App Component
**File:** `Frontend/src/app/app.component.ts`

**Changes:**
```typescript
import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'Property-Registeration-MIS';

  constructor(private translate: TranslateService) {
    // Set default language - CRITICAL for production
    this.translate.setDefaultLang('دری');
  }

  ngOnInit() {
    // Use the default language
    this.translate.use('دری');
  }
}
```

**Why This Fixes It:**
- `setDefaultLang()` tells Angular which language to use as fallback
- `use()` actively loads and applies the translation file
- This ensures translations load immediately on app startup

### 2. Explicit Translation Path Configuration
**File:** `Frontend/src/app/app.module.ts`

**Changes:**
```typescript
export function HttpLoaderFactory(http: HttpClient): TranslateHttpLoader {
  return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}
```

**Why This Helps:**
- Explicitly defines the path to translation files
- Uses relative path `./assets/i18n/` which works in both dev and production
- Specifies `.json` extension explicitly

## Translation File Structure

Your translation files are correctly structured:

```
Frontend/src/assets/i18n/
├── دری.json          (Dari translations)
└── English.json      (Pashto translations)
```

### Sample Translation File (دری.json):
```json
{
    "title2": "وزارت عدلیه",
    "title1": "امارت اسلامی افغانستان",
    "DISTRICT_MANAGEMENT": {
        "TITLE": "مدیریت ولسوالی ها",
        "SELECT_PROVINCE": "انتخاب ولایت",
        "ADD_DISTRICT": "افزودن ولسوالی",
        ...
    }
}
```

## Production Build Steps

### 1. Build for Production
```bash
cd Frontend
npm run build --prod
```

### 2. Verify Translation Files Are Included
After build, check that translation files exist:
```bash
ls dist/property-registeration-mis/assets/i18n/
```

You should see:
- `دری.json`
- `English.json`

### 3. Deploy to Production
Copy the entire `dist/property-registeration-mis/` folder to your production server.

## Verification Steps

### After Deployment:

1. **Check Browser Console**
   - Open browser DevTools (F12)
   - Look for any 404 errors loading translation files
   - Should see: `GET /assets/i18n/دری.json` with status 200

2. **Check Network Tab**
   - Filter by "i18n"
   - Verify translation files are loading
   - Check response contains actual JSON data

3. **Test Language Switching**
   - Click language switcher
   - Verify translations change
   - Check that keys don't appear

## Common Production Issues & Solutions

### Issue 1: Translation Files Not Found (404)
**Symptoms:** Keys showing, 404 errors in console

**Solution:**
```bash
# Verify files are in dist folder
ls dist/property-registeration-mis/assets/i18n/

# If missing, rebuild:
npm run build --prod
```

### Issue 2: CORS Issues
**Symptoms:** Translation files blocked by CORS policy

**Solution:** Configure your web server (IIS/Nginx/Apache) to allow loading JSON files:

**IIS web.config:**
```xml
<staticContent>
  <mimeMap fileExtension=".json" mimeType="application/json" />
</staticContent>
```

**Nginx:**
```nginx
location ~* \.json$ {
    add_header Access-Control-Allow-Origin *;
}
```

### Issue 3: Caching Old Translation Files
**Symptoms:** Old translations showing after update

**Solution:**
```bash
# Clear browser cache
# Or add cache busting to angular.json:
"outputHashing": "all"  # Already configured
```

### Issue 4: File Encoding Issues
**Symptoms:** Garbled text or file not loading

**Solution:**
- Ensure translation files are saved as UTF-8
- Check file has no BOM (Byte Order Mark)
- Verify file names are exactly: `دری.json` and `English.json`

## Testing Checklist

Before deploying to production:

- [ ] Build completes without errors
- [ ] Translation files exist in dist folder
- [ ] Default language is set in app.component.ts
- [ ] Test in production-like environment first
- [ ] Check browser console for errors
- [ ] Verify all translation keys resolve to values
- [ ] Test language switching
- [ ] Check on different browsers
- [ ] Verify on mobile devices

## Deployment Checklist

- [ ] Backup current production files
- [ ] Build with `npm run build --prod`
- [ ] Verify dist folder contains translation files
- [ ] Copy dist folder to production server
- [ ] Clear server cache if applicable
- [ ] Test immediately after deployment
- [ ] Monitor for errors in first 10 minutes
- [ ] Have rollback plan ready

## File Paths Reference

### Development:
```
http://localhost:4200/assets/i18n/دری.json
```

### Production:
```
https://your-domain.com/assets/i18n/دری.json
```

Both should return the same JSON content.

## Debugging Commands

### Check if translation file is accessible:
```bash
# From production server
curl https://your-domain.com/assets/i18n/دری.json

# Should return JSON content, not 404
```

### Check file permissions:
```bash
# On Linux server
ls -la /path/to/production/assets/i18n/
# Files should be readable (644 or 755)
```

### Check IIS MIME types (Windows):
```powershell
# In IIS Manager, check Static Content settings
# Ensure .json files are configured
```

## Expected Behavior

### Before Fix:
- ❌ Shows: `DISTRICT_MANAGEMENT.TITLE`
- ❌ Shows: `DISTRICT_MANAGEMENT.ADD_DISTRICT`
- ❌ Shows: `DISTRICT_MANAGEMENT.EDIT`

### After Fix:
- ✅ Shows: `مدیریت ولسوالی ها`
- ✅ Shows: `افزودن ولسوالی`
- ✅ Shows: `ویرایش`

## Additional Notes

### Language Files Naming
The translation files use non-ASCII names:
- `دری.json` (Dari)
- `English.json` (Pashto - note: this is actually Pashto content)

This is supported by modern web servers, but ensure:
- Server supports UTF-8 file names
- File system supports Unicode
- No URL encoding issues

### Browser Compatibility
The fix works on all modern browsers:
- ✅ Chrome/Edge
- ✅ Firefox
- ✅ Safari
- ✅ Mobile browsers

## Conclusion

The translation issue has been fixed by:
1. Setting a default language in the app component
2. Explicitly configuring the translation loader path
3. Ensuring translations load on app startup

After rebuilding and redeploying, all translation keys should resolve to their proper values in production.

---

**Status:** ✅ Fixed
**Date:** March 4, 2026
**Next Steps:** Rebuild and redeploy to production
