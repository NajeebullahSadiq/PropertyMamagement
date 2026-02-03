# Verification System Testing Guide

## Overview
This guide provides step-by-step instructions to test the complete verification system for both Property and Vehicle documents.

## Prerequisites
- Backend server running on `http://localhost:5143`
- Frontend server running on `http://localhost:4200`
- At least one completed property or vehicle transaction in the database

## Test Scenarios

### Scenario 1: Vehicle Document Verification

#### Step 1: Create/View a Vehicle Document
1. Log in to the system
2. Navigate to Vehicle module
3. Either create a new vehicle transaction or select an existing one
4. Ensure the transaction has:
   - Seller information with photo
   - Buyer information with photo
   - All required fields filled

#### Step 2: Print the Document
1. Click "Print" button for the vehicle transaction
2. The print preview should display:
   - ✅ Vehicle details
   - ✅ Seller photo and information
   - ✅ Buyer photo and information
   - ✅ QR code in the header (85x85px with blue border)
   - ✅ Verification code below QR code (format: VEH-YYYY-XXXXXX)

#### Step 3: Test Manual Verification
1. Open a new browser tab/window
2. Navigate to: `http://localhost:4200/verify`
3. Copy the verification code from the printed document
4. Paste it into the input field
5. Click "تصدیق" (Verify) button

**Expected Result:**
- ✅ Green success banner: "معتبر" (Valid)
- ✅ Document type: "سند معامله واسطه" (Vehicle Document)
- ✅ Seller section with:
  - Orange/yellow gradient background
  - Seller photo displayed
  - Full name, father name, grandfather name
  - National ID number
  - Phone number
  - Province, district, village
- ✅ Buyer section with:
  - Green/teal gradient background
  - Buyer photo displayed
  - Full name, father name, grandfather name
  - National ID number
  - Phone number
  - Province, district, village
- ✅ Verification code and timestamp at bottom

#### Step 4: Test QR Code Scanning
1. On the verify page, click the "اسکن QR کود" (Scan QR Code) tab
2. Allow camera access when prompted
3. Point camera at the QR code on the printed document
4. The system should automatically:
   - Detect the QR code
   - Extract the verification code
   - Switch to manual mode
   - Display verification results

**Expected Result:** Same as Step 3

#### Step 5: Test Direct URL Access
1. Copy the verification URL from the QR code or construct it manually:
   - Format: `http://localhost:4200/verify/VEH-2026-XXXXXX`
2. Open the URL in a new browser tab
3. The page should automatically verify and display results

**Expected Result:** Same as Step 3

### Scenario 2: Property Document Verification

#### Step 1: Create/View a Property Document
1. Navigate to Property module (Estate)
2. Either create a new property transaction or select an existing one
3. Ensure the transaction has:
   - Seller information with photo
   - Buyer information with photo
   - All required fields filled

#### Step 2: Print the Document
1. Click "Print" button for the property transaction
2. The print preview should display:
   - ✅ Property details
   - ✅ Seller photo and information
   - ✅ Buyer photo and information
   - ✅ QR code in the header
   - ✅ Verification code (format: PRO-YYYY-XXXXXX)

#### Step 3: Test Verification
Follow the same steps as Vehicle Scenario (Steps 3-5)

**Expected Result:**
- Document type: "سند معامله ملکیت" (Property Document)
- All other results same as vehicle verification

### Scenario 3: Error Handling Tests

#### Test 3.1: Invalid Verification Code
1. Navigate to verify page
2. Enter an invalid code: `XXX-9999-INVALID`
3. Click verify

**Expected Result:**
- ❌ Red error banner
- Message: "این کود تصدیق معتبر نیست" (This verification code is invalid)

#### Test 3.2: Empty Code
1. Navigate to verify page
2. Leave input field empty
3. Click verify

**Expected Result:**
- ❌ Error message: "لطفاً کود تصدیق را وارد کنید" (Please enter verification code)

#### Test 3.3: Malformed URL in Route
1. Navigate to: `http://localhost:4200/verify/http://localhost:4200/verify`
2. The system should extract the code or show appropriate error

**Expected Result:**
- System attempts to extract valid code pattern
- If no valid pattern found, shows invalid code error

### Scenario 4: Photo Display Tests

#### Test 4.1: Missing Seller Photo
1. Create a transaction without seller photo
2. Print and verify the document

**Expected Result:**
- ✅ Seller section displays with default avatar image
- ✅ All other seller information displays correctly

#### Test 4.2: Missing Buyer Photo
1. Create a transaction without buyer photo
2. Print and verify the document

**Expected Result:**
- ✅ Buyer section displays with default avatar image
- ✅ All other buyer information displays correctly

#### Test 4.3: Photo Path Formats
Test different photo path formats:
- `Resources/Documents/Profile/photo.jpg` ✅ Should work
- `/Resources/Documents/Profile/photo.jpg` ✅ Should work
- `http://localhost:5143/Resources/Documents/Profile/photo.jpg` ✅ Should work

### Scenario 5: Mobile Responsiveness

#### Test 5.1: Mobile View
1. Open verify page on mobile device or use browser dev tools
2. Test both manual input and QR scanner modes
3. Verify a document

**Expected Result:**
- ✅ Layout adapts to mobile screen
- ✅ Photos display correctly
- ✅ All information is readable
- ✅ Buttons are easily tappable

## Common Issues and Solutions

### Issue 1: Photos Not Displaying
**Symptoms:** Broken image icons or 404 errors for photos

**Solutions:**
1. Check that photos are stored in `Backend/Resources/Documents/Profile/`
2. Verify backend static file serving is configured
3. Check browser console for 404 errors
4. Verify photo paths in database don't have extra slashes

### Issue 2: QR Code Not Generating
**Symptoms:** No QR code appears on print page

**Solutions:**
1. Check browser console for errors
2. Verify verification service is generating codes
3. Check that document ID is being passed correctly
4. Verify QR Server API is accessible

### Issue 3: Verification Returns 404
**Symptoms:** Error: `/api/Verification/verify/...` returns 404

**Solutions:**
1. ✅ **FIXED:** Verify component now extracts code from URL properly
2. Check backend verification controller is running
3. Verify route is registered in backend
4. Check that verification code exists in database

### Issue 4: Seller/Buyer Info Not Showing
**Symptoms:** Verification works but seller/buyer sections are empty

**Solutions:**
1. Check that backend is including seller/buyer data in response
2. Verify database has seller/buyer records linked to document
3. Check browser console for JavaScript errors
4. Verify `result.sellerInfo` and `result.buyerInfo` are populated

## API Endpoints Reference

### Generate Verification Code
```
POST /api/Verification/generate
Authorization: Bearer {token}
Body: {
  "documentId": 123,
  "documentType": "VehicleDocument" | "PropertyDocument"
}
Response: {
  "verificationCode": "VEH-2026-ABC123",
  "verificationUrl": "https://prmis.gov.af/verify/VEH-2026-ABC123",
  "isNew": true
}
```

### Verify Document (Public)
```
GET /api/Verification/verify/{code}
No authentication required
Response: {
  "isValid": true,
  "status": "Valid",
  "verificationCode": "VEH-2026-ABC123",
  "documentType": "VehicleDocument",
  "licenseNumber": "232",
  "holderName": "نجیب الله - نجیب الله",
  "holderPhoto": "Resources/Documents/Profile/photo.jpg",
  "issueDate": "2026-02-03T05:01:06.760509",
  "officeAddress": "کابل, ناحیه اول",
  "sellerInfo": {
    "firstName": "نجیب الله",
    "fatherName": "صادق",
    "grandFatherName": "صدبر",
    "electronicNationalIdNumber": "1400030063636",
    "phoneNumber": "0744444444",
    "photo": "Resources/Documents/Profile/photo.jpg",
    "province": "کابل",
    "district": "ناحیه اول",
    "village": "ناحیه سوم"
  },
  "buyerInfo": {
    "firstName": "نجیب الله",
    "fatherName": "صادق",
    "grandFatherName": "صدبر",
    "electronicNationalIdNumber": "1400030063636",
    "phoneNumber": "0700363636",
    "photo": "Resources/Documents/Profile/photo.jpg",
    "province": "بدخشان",
    "district": "کشم",
    "village": "ناحیه سوم"
  },
  "verifiedAt": "2026-02-03T08:28:10.460895Z"
}
```

## Success Criteria

All tests should pass with:
- ✅ QR codes generate and display on print pages
- ✅ Manual verification works with code input
- ✅ QR code scanning works (if camera available)
- ✅ Direct URL access works
- ✅ Seller information displays with photo
- ✅ Buyer information displays with photo
- ✅ All photos load correctly
- ✅ Mobile responsive layout works
- ✅ Error handling works for invalid codes
- ✅ URL parsing handles edge cases

## Status
✅ **ALL FEATURES IMPLEMENTED AND TESTED**
