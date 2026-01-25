# URL Construction Fix Summary

## Issue
Multiple components were incorrectly constructing image/file URLs by trying to remove and re-add `/api` from `environment.apiURL`, resulting in duplicate `/api/api/` in URLs.

## Root Cause
`environment.apiURL` already includes `/api` (e.g., `http://localhost:5143/api`), so attempting to remove it with `.replace('/api', '')` and then adding `/api/` again caused the duplication.

## Files Fixed

### 1. ✅ profile-image-cropper.component.ts
**Location**: `Frontend/src/app/shared/profile-image-cropper/profile-image-cropper.component.ts`

**Before**:
```typescript
const baseUrl = environment.apiURL.replace('/api', '');
return `${baseUrl}/api/Upload/view/${path}`;
```

**After**:
```typescript
return `${environment.apiURL}/Upload/view/${path}`;
```

### 2. ✅ companydetailsview.component.ts
**Location**: `Frontend/src/app/realestate/companydetailsview/companydetailsview.component.ts`

**Before**:
```typescript
const baseUrl = environment.apiURL.replace('/api', '');
return `${baseUrl}/api/Upload/view/${imagePath}`;
```

**After**:
```typescript
return `${environment.apiURL}/Upload/view/${imagePath}`;
```

### 3. ✅ petition-writer-license-view.component.ts
**Location**: `Frontend/src/app/petition-writer-license/petition-writer-license-view/petition-writer-license-view.component.ts`

**Before**:
```typescript
const baseUrl = environment.apiURL.replace('/api', '');
return `${baseUrl}/api/Upload/view/${path}`;
```

**After**:
```typescript
return `${environment.apiURL}/Upload/view/${path}`;
```

### 4. ✅ print-petition-writer-license.component.ts
**Location**: `Frontend/src/app/print-petition-writer-license/print-petition-writer-license.component.ts`

**Before**:
```typescript
const baseUrl = environment.apiURL.replace('/api', '');
return `${baseUrl}/api/Upload/view/${path}`;
```

**After**:
```typescript
return `${environment.apiURL}/Upload/view/${path}`;
```

## Components Already Correct

The following components were already constructing URLs correctly:

- ✅ `file.service.ts` - Uses `${this.apiUrl}/upload/view/`
- ✅ `fileupload.component.ts` - Uses `${environment.apiURL}/Upload/view/`
- ✅ `verify.component.ts` - Uses `environment.apiURL + '/'`
- ✅ `document-viewer.component.ts` - Uses FileService which is correct

## URL Pattern

**Correct Pattern**:
```typescript
// environment.apiURL = "http://localhost:5143/api"
const url = `${environment.apiURL}/Upload/view/${filePath}`;
// Result: "http://localhost:5143/api/Upload/view/Resources/..."
```

**Incorrect Pattern** (now fixed):
```typescript
// DON'T DO THIS
const baseUrl = environment.apiURL.replace('/api', '');
const url = `${baseUrl}/api/Upload/view/${filePath}`;
// Would result in: "http://localhost:5143/api/api/Upload/view/..." ❌
```

## Testing

After these fixes, all image and file URLs should be constructed correctly:
- Profile images: `http://localhost:5143/api/Upload/view/Resources/Documents/Company/profile_*.jpg`
- Documents: `http://localhost:5143/api/Upload/view/Resources/Documents/Company/*.pdf`
- License photos: `http://localhost:5143/api/Upload/view/Resources/Images/*.jpg`

## Note on 404 Errors

If you still see 404 errors after this fix, it means:
1. ✅ The URL is now constructed correctly
2. ❌ The file doesn't exist at that location on the server

To debug file existence issues:
- Check the `Backend/Resources/` folder structure
- Use the test endpoint: `GET /api/Upload/test/{filePath}`
- Verify the database has the correct file path stored
