# Verification UI - Seller and Buyer Information Display

## Overview
Updated the verification UI to display detailed seller and buyer information for Property and Vehicle documents.

## Changes Made

### 1. Frontend Service - `Frontend/src/app/shared/verification.service.ts`

#### Added New Interfaces
```typescript
export interface SellerInfoDto {
  firstName?: string;
  fatherName?: string;
  grandFatherName?: string;
  electronicNationalIdNumber?: string;
  phoneNumber?: string;
  photo?: string;
  province?: string;
  district?: string;
  village?: string;
}

export interface BuyerInfoDto {
  firstName?: string;
  fatherName?: string;
  grandFatherName?: string;
  electronicNationalIdNumber?: string;
  phoneNumber?: string;
  photo?: string;
  province?: string;
  district?: string;
  village?: string;
}
```

#### Updated DocumentVerificationDto
```typescript
export interface DocumentVerificationDto {
  // ... existing properties ...
  sellerInfo?: SellerInfoDto;
  buyerInfo?: BuyerInfoDto;
}
```

### 2. Component TypeScript - `Frontend/src/app/verify/verify.component.ts`

#### Updated Document Type Labels
Added labels for Property and Vehicle documents:
```typescript
case 'PropertyDocument':
  return 'سند معامله ملکیت';
case 'VehicleDocument':
  return 'سند معامله واسطه';
```

### 3. Component HTML - `Frontend/src/app/verify/verify.component.html`

#### Added Seller Information Section
```html
<div *ngIf="result.sellerInfo" class="party-details seller-details">
  <div class="party-header">
    <h4>معلومات فروشنده</h4>
  </div>
  <div class="party-content">
    <!-- Seller Photo -->
    <div class="party-photo-section" *ngIf="result.sellerInfo.photo">
      <img [src]="baseUrl + result.sellerInfo.photo" alt="عکس فروشنده" class="party-photo">
    </div>
    
    <!-- Seller Info Grid -->
    <div class="party-info-grid">
      <div class="info-item" *ngIf="result.sellerInfo.firstName">
        <label>نام:</label>
        <span>{{ result.sellerInfo.firstName }}</span>
      </div>
      <!-- ... other fields ... -->
    </div>
  </div>
</div>
```

#### Added Buyer Information Section
Similar structure as seller section with buyer-specific styling.

### 4. Component SCSS - `Frontend/src/app/verify/verify.component.scss`

#### Added Party Details Styling
```scss
.party-details {
  margin-top: 25px;
  border-top: 2px solid #e2e8f0;
  
  &.seller-details {
    .party-header {
      background: linear-gradient(135deg, #fef5e7 0%, #fdebd0 100%);
      border-right: 4px solid #f39c12;
      
      h4 {
        color: #d68910;
      }
    }
  }
  
  &.buyer-details {
    .party-header {
      background: linear-gradient(135deg, #e8f8f5 0%, #d1f2eb 100%);
      border-right: 4px solid #1abc9c;
      
      h4 {
        color: #138d75;
      }
    }
  }
  
  // ... more styles ...
}
```

## UI Features

### Seller Section
- **Header**: Orange/yellow gradient background with "معلومات فروشنده" (Seller Information)
- **Photo**: 110x135px photo with rounded corners and shadow
- **Information Grid**: 2-column responsive grid showing:
  - نام (Name)
  - نام پدر (Father Name)
  - نام پدر کلان (Grandfather Name)
  - شماره تذکره (National ID Number)
  - شماره تماس (Phone Number)
  - ولایت (Province)
  - ولسوالی (District)
  - قریه/گذر (Village/Street)

### Buyer Section
- **Header**: Green/teal gradient background with "معلومات خریدار" (Buyer Information)
- **Photo**: Same styling as seller photo
- **Information Grid**: Same structure as seller section

### Responsive Design
- Desktop: 2-column grid for information
- Mobile: Single column layout
- Photos centered on mobile devices

### Visual Hierarchy
1. **Status Banner**: Shows document validity (Valid/Invalid/Expired/Revoked)
2. **Document Details**: General document information
3. **Seller Information**: Detailed seller data with photo
4. **Buyer Information**: Detailed buyer data with photo
5. **Verification Info**: Verification code and timestamp

## Document Type Display

### Company-Based Documents
- **RealEstateLicense**: Shows company info, no seller/buyer
- **PetitionWriterLicense**: Shows individual info, no seller/buyer
- **Securities**: Shows company info, no seller/buyer

### Transaction Documents
- **PropertyDocument**: Shows seller and buyer information
- **VehicleDocument**: Shows seller and buyer information

## Color Scheme

### Seller Section
- Background: Warm orange/yellow gradient (#fef5e7 to #fdebd0)
- Border: Orange (#f39c12)
- Text: Dark orange (#d68910)

### Buyer Section
- Background: Cool green/teal gradient (#e8f8f5 to #d1f2eb)
- Border: Teal (#1abc9c)
- Text: Dark teal (#138d75)

## Testing

### Test Property Document Verification
1. Scan QR code from property print
2. Verify seller section appears with:
   - Seller photo
   - Complete seller information
3. Verify buyer section appears with:
   - Buyer photo
   - Complete buyer information

### Test Vehicle Document Verification
1. Scan QR code from vehicle print
2. Verify seller section appears
3. Verify buyer section appears
4. Verify all fields display correctly

### Test Company License Verification
1. Scan QR code from company license
2. Verify NO seller/buyer sections appear
3. Verify company information displays correctly

## Files Modified

1. `Frontend/src/app/shared/verification.service.ts` - Added interfaces
2. `Frontend/src/app/verify/verify.component.ts` - Added document type labels
3. `Frontend/src/app/verify/verify.component.html` - Added seller/buyer sections
4. `Frontend/src/app/verify/verify.component.scss` - Added styling

## Benefits

1. **Complete Information**: Users can see full details of both parties
2. **Visual Distinction**: Different colors for seller vs buyer
3. **Photos**: Visual verification with party photos
4. **Contact Info**: Phone numbers for verification purposes
5. **Address Details**: Complete address information
6. **Responsive**: Works on all device sizes
7. **Conditional Display**: Only shows for transaction documents

## Next Steps

1. Test on mobile devices
2. Verify photo loading from different paths
3. Test with real data
4. Gather user feedback
5. Consider adding print functionality for verification page
