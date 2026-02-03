# Vehicle Module QR Code Verification Implementation

## Overview
Implemented QR code verification system for vehicle print documents, matching the exact functionality from the property module.

## Changes Made

### 1. TypeScript Component Updates
**File**: `Frontend/src/app/printvehicledata/printvehicledata.component.ts`

#### Added Imports
```typescript
import { VerificationService } from '../shared/verification.service';
import { catchError, of } from 'rxjs';
```

#### Added Properties
```typescript
isLoading: boolean = true;
loadError: boolean = false;
errorMessage: string = '';

// Verification properties
verificationCode: string = '';
verificationUrl: string = '';
qrCodeUrl: string = '';
verificationError: string | null = null;
```

#### Added VerificationService to Constructor
```typescript
constructor(
  public service: AuthService,
  private route: ActivatedRoute,
  private pservice: VehicleService,
  private verificationService: VerificationService
) { }
```

#### Refactored ngOnInit
- Changed from direct data loading to `loadPrintData()` method
- Added proper error handling with user-friendly messages
- Added loading states

#### Added New Methods

1. **loadPrintData()**: Main data loading method with error handling
2. **waitForImagesToLoad()**: Waits for images to load before printing
3. **handleError()**: Centralized error handling
4. **constructImageUrlPublic()**: Public method for template access
5. **fetchVerificationCode()**: Generates QR code for vehicle document

### 2. HTML Template Updates
**File**: `Frontend/src/app/printvehicledata/printvehicledata.component.html`

#### Added Loading State
```html
<div *ngIf="isLoading" style="display: flex; justify-content: center; align-items: center; min-height: 100vh; flex-direction: column;">
    <div style="text-align: center;">
        <div style="border: 4px solid #f3f3f3; border-top: 4px solid #3498db; border-radius: 50%; width: 50px; height: 50px; animation: spin 1s linear infinite; margin: 0 auto;"></div>
        <p style="margin-top: 20px; font-size: 16px; color: #555;">در حال بارگذاری اطلاعات واسطه...</p>
    </div>
</div>
```

#### Added Error State
```html
<div *ngIf="loadError" style="display: flex; justify-content: center; align-items: center; min-height: 100vh; flex-direction: column;">
    <div style="text-align: center; padding: 20px;">
        <i class="fas fa-exclamation-triangle" style="font-size: 48px; color: #e74c3c; margin-bottom: 20px;"></i>
        <h3 style="color: #e74c3c; margin-bottom: 10px;">خطا در بارگذاری</h3>
        <p style="color: #555;">{{errorMessage}}</p>
        <button onclick="window.close()" style="margin-top: 20px; padding: 10px 20px; background-color: #3498db; color: white; border: none; border-radius: 4px; cursor: pointer;">بستن</button>
    </div>
</div>
```

#### Added QR Code Display
```html
<!-- QR Code in Header - Centered -->
<div *ngIf="verificationCode" style="margin: 5px auto; display: flex; flex-direction: column; align-items: center; justify-content: center;">
    <img [src]="qrCodeUrl" alt="QR Code" style="width: 85px; height: 85px; border: 2px solid #1976d2; padding: 2px; background: white; display: block;" *ngIf="qrCodeUrl">
    <p style="margin: 2px 0; font-size: 10.5px; font-weight: bold; color: #1976d2; text-align: center;">{{ verificationCode }}</p>
    <p style="margin: 1px 0; font-size: 9.5px; color: #555; text-align: center;">کود تصدیق سند</p>
</div>
```

#### Wrapped Content with Conditional Display
```html
<div class="container" *ngIf="!isLoading && !loadError">
    <!-- All existing content -->
</div>
```

## Features Implemented

### 1. QR Code Generation
- Automatically generates unique verification code for each vehicle document
- Creates QR code image using Google Charts API
- Document type: `VehicleDocument`
- Uses vehicle ID from the print data

### 2. Loading States
- Shows spinner while loading vehicle data
- Waits for images to load before triggering print
- Smooth user experience

### 3. Error Handling
- Network errors (status 0)
- Not found errors (status 404)
- Authentication errors (status 401)
- Authorization errors (status 403)
- Server errors (status 500)
- User-friendly error messages in Dari

### 4. Image Loading
- Tracks seller and buyer photo loading
- Falls back to default avatar if images fail
- Only triggers print after all images are loaded

### 5. Photo Path Handling
- Uses direct static file serving for `Resources/` paths
- Matches property module implementation exactly
- Handles both full paths and relative paths

## How It Works

1. **Page Load**: Component shows loading spinner
2. **Data Fetch**: Retrieves vehicle print data from API
3. **Photo Setup**: Constructs proper URLs for seller/buyer photos
4. **Verification**: Calls verification service to generate QR code
5. **Image Loading**: Waits for all images (photos + QR code) to load
6. **Auto Print**: Triggers print dialog after 500ms delay
7. **Error Handling**: Shows user-friendly error if anything fails

## QR Code Details

- **Size**: 85x85 pixels
- **Border**: 2px solid blue (#1976d2)
- **Position**: Centered in header, below company info
- **Content**: 
  - QR code image (scannable)
  - Verification code text (readable)
  - Label: "کود تصدیق سند" (Document Verification Code)

## Verification Service Integration

The component uses the existing `VerificationService` which:
- Generates unique verification codes
- Stores verification records in database
- Creates QR code URLs using Google Charts API
- Supports document verification via web interface

## Testing

To test the QR code:
1. Navigate to vehicle print page
2. Wait for loading to complete
3. QR code should appear in the header
4. Scan QR code with phone - should open verification page
5. Verification code should be displayed below QR code
6. Print should trigger automatically after images load

## Compatibility

- Matches property module implementation exactly
- Uses same verification service and database tables
- Consistent UI/UX across property and vehicle modules
- Same error handling patterns

## Files Modified

1. `Frontend/src/app/printvehicledata/printvehicledata.component.ts` - Complete refactor
2. `Frontend/src/app/printvehicledata/printvehicledata.component.html` - Added QR code and states

## Dependencies

- `VerificationService` - Already exists, shared with property module
- `rxjs` operators (`catchError`, `of`) - Already installed
- Google Charts API - External service for QR code generation
