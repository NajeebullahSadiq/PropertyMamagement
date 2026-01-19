# Design Document

## Overview

This design document outlines the approach for merging three separate tabs in the Real Estate Module into a single unified tab. The refactoring focuses exclusively on UI reorganization while preserving all existing business logic, validation rules, data persistence, and component interactions.

The three tabs to be merged are:
1. **دفتر رهنمایی معاملات** (Company Details) - Office registration information
2. **مشخصات مالک** (Owner Details) - Owner/applicant information  
3. **مشخصات جواز** (License Details) - License information

The merged tab will be named **"اطلاعات دفتر، مالک و جواز"** (Office, Owner & License Information).

## Architecture

### Current Architecture

The Real Estate module currently uses Angular Material tabs (`mat-tab-group`) with six separate tabs:
- Tab 1: Company Details (`app-companydetails`)
- Tab 2: Owner Details (`app-companyowner`)
- Tab 3: License Details (`app-licensedetails`)
- Tab 4: Guarantors (`app-guaranators`)
- Tab 5: Account Info (`app-accountinfo`)
- Tab 6: Cancellation Info (`app-cancellationinfo`)

Each tab contains a standalone component with its own:
- Reactive form (`FormGroup`)
- Validation logic
- API service calls
- File upload functionality
- Date handling with multi-calendar support

### Target Architecture

The refactored module will have four tabs:
- **Tab 1: Unified Tab** - Contains all three merged sections in a single scrollable view
- Tab 2: Guarantors (unchanged)
- Tab 3: Account Info (unchanged)
- Tab 4: Cancellation Info (unchanged)

The unified tab will embed the three existing components but present them in a single continuous view with visual section separators.

### Design Pattern

**Component Composition Pattern**: The unified tab will use Angular's component composition to embed the three existing child components within a single parent container. This approach:
- Preserves all existing component logic
- Maintains component isolation and testability
- Requires minimal code changes
- Allows independent component updates

## Components and Interfaces

### Modified Components

#### 1. RealestateComponent (Parent Container)

**File**: `Frontend/src/app/realestate/realestate.component.html`

**Changes**:
- Replace first three `<mat-tab>` elements with a single unified tab
- Embed three child components within the unified tab
- Maintain all `@ViewChild` references
- Preserve tab navigation logic for remaining tabs

**Template Structure**:
```html
<mat-tab-group #tabGroup>
  <!-- UNIFIED TAB -->
  <mat-tab>
    <ng-template matTabLabel>
      <mat-icon>description</mat-icon>
      اطلاعات دفتر، مالک و جواز
    </ng-template>
    
    <div class="unified-tab-container">
      <!-- Section 1: Company Details -->
      <app-companydetails #companydetails [id]="PropertyId" (next)="scrollToNextSection('owner')"></app-companydetails>
      
      <!-- Section 2: Owner Details -->
      <app-companyowner #companyowner [id]="PropertyId" (next)="scrollToNextSection('license')"></app-companyowner>
      
      <!-- Section 3: License Details -->
      <app-licensedetails #licensedetails [id]="PropertyId" (next)="nextTab()"></app-licensedetails>
    </div>
  </mat-tab>
  
  <!-- REMAINING TABS (unchanged) -->
  <mat-tab [disabled]="comservice.mainTableId === 0">...</mat-tab>
  ...
</mat-tab-group>
```

**TypeScript Changes**:
```typescript
// Add scroll navigation method
scrollToNextSection(sectionId: string): void {
  const element = document.getElementById(sectionId);
  if (element) {
    element.scrollIntoView({ behavior: 'smooth', block: 'start' });
  }
}
```

#### 2. CompanydetailsComponent (Section 1)

**File**: `Frontend/src/app/realestate/companydetails/companydetails.component.html`

**Changes**:
- Wrap entire template in a section container with ID
- Remove page-level headers (will be replaced with section headers)
- Adjust spacing for compact layout
- Maintain all form fields and functionality

**Template Structure**:
```html
<section id="company" class="unified-section">
  <div class="section-header">
    <h2>دفتر رهنمایی معاملات</h2>
  </div>
  
  <form [formGroup]="companyForm" (ngSubmit)="...">
    <!-- All existing form fields -->
  </form>
</section>
```

**No TypeScript Changes Required** - All business logic remains unchanged

#### 3. CompanyownerComponent (Section 2)

**File**: `Frontend/src/app/realestate/companyowner/companyowner.component.html`

**Changes**:
- Wrap entire template in a section container with ID
- Remove page-level headers
- Adjust spacing for compact layout
- Maintain all form fields, address management, and profile image functionality

**Template Structure**:
```html
<section id="owner" class="unified-section">
  <div class="section-header">
    <h2>مشخصات مالک</h2>
  </div>
  
  <form [formGroup]="ownerForm" (ngSubmit)="...">
    <!-- All existing form fields -->
  </form>
</section>
```

**No TypeScript Changes Required** - All business logic remains unchanged

#### 4. LicensedetailsComponent (Section 3)

**File**: `Frontend/src/app/realestate/licensedetails/licensedetails.component.html`

**Changes**:
- Wrap entire template in a section container with ID
- Remove page-level headers
- Adjust spacing for compact layout
- Maintain all form fields and print functionality

**Template Structure**:
```html
<section id="license" class="unified-section">
  <div class="section-header">
    <h2>مشخصات جواز</h2>
  </div>
  
  <form [formGroup]="licenseForm" (ngSubmit)="...">
    <!-- All existing form fields -->
  </form>
</section>
```

**No TypeScript Changes Required** - All business logic remains unchanged

### Unchanged Components

The following components require **no modifications**:
- `GuaranatorsComponent`
- `AccountinfoComponent`
- `CancellationinfoComponent`
- `FileuploadComponent`
- `ProfileImageCropperComponent`
- `MultiCalendarDatepickerComponent`

## Data Models

**No changes to data models** - All existing interfaces and models remain unchanged:
- `companydetails`
- `companyowner`
- `companyOwnerAddressHistory`
- `LicenseDetail`

## Styling and Layout

### Tailwind CSS Classes

#### Unified Tab Container
```css
.unified-tab-container {
  @apply w-full bg-gray-50 py-6;
}
```

#### Section Styling
```css
.unified-section {
  @apply max-w-7xl mx-auto px-4 mb-8;
}

.unified-section:not(:last-child) {
  @apply border-b-4 border-gray-300 pb-8;
}
```

#### Section Headers
```css
.section-header {
  @apply mb-6 pb-4 border-b-2 border-blue-500;
}

.section-header h2 {
  @apply text-3xl font-bold text-gray-800 flex items-center gap-3;
}
```

#### Compact Form Spacing
```css
/* Reduce vertical spacing between form sections */
.unified-section .bg-white {
  @apply mb-4; /* Reduced from mb-6 */
}

/* Reduce padding in form sections */
.unified-section .px-6.py-6 {
  @apply px-6 py-4; /* Reduced vertical padding */
}
```

### Responsive Design

All existing responsive grid layouts will be maintained:
- `grid-cols-1 md:grid-cols-3` for three-column layouts
- `grid-cols-1 md:grid-cols-2` for two-column layouts
- Mobile-first approach with breakpoints at `md` (768px)

## Form Submission Flow

### Current Flow
1. User fills Company Details → Clicks "ثبت معلومات" → Saves to backend → Navigates to Owner tab
2. User fills Owner Details → Clicks "ثبت معلومات" → Saves to backend → Navigates to License tab
3. User fills License Details → Clicks "ثبت معلومات" → Saves to backend → Navigates to Guarantors tab

### New Flow
1. User fills Company Details → Clicks "ثبت معلومات" → Saves to backend → **Scrolls to Owner section**
2. User fills Owner Details → Clicks "ثبت معلومات" → Saves to backend → **Scrolls to License section**
3. User fills License Details → Clicks "ثبت معلومات" → Saves to backend → Navigates to Guarantors tab

**Implementation**:
- Modify `(next)` event handlers to call `scrollToNextSection()` instead of `nextTab()`
- Only the License Details component will call `nextTab()` to move to Guarantors tab
- Smooth scroll behavior for better UX

## Validation and Error Handling

**No changes to validation logic** - All existing validators remain:

### Company Details Validators
- `title`: Required
- `phoneNumber`: Required
- `licenseNumber`: Required
- `petitionDate`: Required
- `petitionNumber`: Required
- `tin`: Required

### Owner Details Validators
- `firstName`: Required
- `fatherName`: Required
- `grandFatherName`: Required
- `educationLevelId`: Required
- `dateofBirth`: Required
- `identityCardTypeId`: Required
- `indentityCardNumber`: Required
- `jild`, `safha`, `sabtNumber`: Conditionally required (not for electronic ID)
- `ownerProvinceId`, `ownerDistrictId`, `ownerVillage`: Required
- `permanentProvinceId`, `permanentDistrictId`, `permanentVillage`: Required

### License Details Validators
- `licenseNumber`: Required
- `issueDate`: Required
- `expireDate`: Required
- `areaId`: Required
- `officeAddress`: Required
- `licenseType`: Required

## API Integration

**No changes to API calls** - All existing service methods remain unchanged:

### CompnaydetailService Methods
- `addcompanies(companyDetail: companydetails)`
- `updatecompanies(companyDetail: companydetails)`
- `getCompanyById(id: number)`
- `addcompanyOwner(owner: companyowner)`
- `updateowner(owner: companyowner)`
- `getOwnerById(id: number)`
- `getOwnerAddressHistory(id: number)`
- `addLicenseDetails(license: LicenseDetail)`
- `updateLicenseDetails(license: LicenseDetail)`
- `getLicenseById(id: number)`

### Shared State Management
- `comservice.mainTableId`: Tracks the company ID across components
- `comservice.ownerId`: Tracks the owner ID across components

## File Upload Functionality

**No changes to file upload** - All existing upload components remain:

### Company Details
- Uses `FileuploadComponent` for document upload
- Stores path in `docPath` field
- Supports image/*, .pdf, .doc, .docx

### Owner Details
- Uses `ProfileImageCropperComponent` for profile photo
- Stores path in `pothoPath` field
- Supports circular cropping

### License Details
- Uses `FileuploadComponent` for license document upload
- Stores path in `docPath` field
- Supports image/*, .pdf, .doc, .docx

## Calendar and Date Handling

**No changes to calendar logic** - All existing multi-calendar support remains:

### Supported Calendars
- Solar Hijri (Persian/Afghan)
- Gregorian
- Lunar Hijri

### Date Conversion
- `CalendarConversionService`: Converts between calendar types
- `CalendarService`: Manages selected calendar type
- Dates stored in backend in selected calendar format

### Date Fields
- Company: `petitionDate`
- Owner: `dateofBirth`
- License: `issueDate`, `expireDate`, `royaltyDate`, `penaltyDate`, `hrLetterDate`

## Address Management

**No changes to address logic** - All existing address functionality remains:

### Owner Address Types
1. **Owner's Own Address** (آدرس اصلی مالک): Province, District, Village
2. **Current Residence** (آدرس فعلی سکونت): Province, District, Village
3. **Temporary Address** (آدرس موقت): Province, District, Village (optional)

### Address Change Mode
- Allows updating addresses while preserving history
- Stores old addresses in `companyOwnerAddressHistory` table
- Displays address history in collapsible table

## Print Functionality

**No changes to print logic** - Existing print functionality remains:

### License Print
- Accessed via "چاپ جواز" button in License Details section
- Navigates to `/printLicense/:id` route
- Displays formatted license document

## Testing Strategy

### Manual Testing Checklist

#### Visual Verification
- [ ] All three sections visible in unified tab
- [ ] Section headers clearly distinguish content areas
- [ ] Visual separators (borders/dividers) between sections
- [ ] Consistent styling with existing modules
- [ ] Responsive layout on mobile/tablet/desktop

#### Functional Testing
- [ ] Company Details form submission saves correctly
- [ ] Owner Details form submission saves correctly
- [ ] License Details form submission saves correctly
- [ ] Smooth scroll navigation between sections
- [ ] File uploads work in all sections
- [ ] Date pickers work in all sections
- [ ] Address dropdowns populate correctly
- [ ] Address change mode functions properly
- [ ] Validation errors display correctly
- [ ] Success/error toasts appear as expected

#### Data Persistence
- [ ] New records save to database correctly
- [ ] Existing records load in edit mode
- [ ] All fields populate with correct data
- [ ] Updates save without data loss
- [ ] Address history preserved correctly

#### Navigation Testing
- [ ] Remaining tabs (Guarantors, Account, Cancellation) unchanged
- [ ] Tab disable logic works (tabs disabled until company saved)
- [ ] Print navigation works from License section
- [ ] Reset functionality clears all forms

#### Cross-Browser Testing
- [ ] Chrome
- [ ] Firefox
- [ ] Safari
- [ ] Edge

## Migration and Rollback

### Migration Steps
1. Create backup of current `realestate.component.html`
2. Update parent component template
3. Update child component templates (add section wrappers)
4. Add scroll navigation method to parent component
5. Test thoroughly in development environment
6. Deploy to staging for user acceptance testing
7. Deploy to production

### Rollback Plan
If issues arise:
1. Restore backed-up `realestate.component.html`
2. Restore child component templates
3. Remove scroll navigation method
4. Redeploy previous version

**No database rollback needed** - No schema changes

## Performance Considerations

### Initial Load
- All three components load simultaneously (vs. lazy loading per tab)
- Minimal performance impact - components are lightweight
- Forms initialize only once

### Scroll Performance
- Smooth scroll uses native browser API
- No performance concerns for typical form sizes

### Memory Usage
- Three components in memory vs. one at a time
- Negligible impact - forms are not memory-intensive

## Accessibility

### Keyboard Navigation
- Tab key navigates through all form fields sequentially
- Enter key submits active form
- Escape key clears focus

### Screen Readers
- Section headers provide clear landmarks
- Form labels properly associated with inputs
- Error messages announced on validation failure

### ARIA Attributes
- Maintain existing ARIA labels on Material components
- Add `role="region"` to section containers
- Add `aria-labelledby` to link sections with headers

## Security

**No security changes** - All existing security measures remain:
- Form validation on client and server
- File upload restrictions (type, size)
- Authentication/authorization unchanged
- CSRF protection maintained

## Correctness Properties

*A property is a characteristic or behavior that should hold true across all valid executions of a system—essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*

### Property 1: UI Component Preservation
*For any* form field that exists in the original three-tab layout, that field must be present and functional in the unified tab layout with identical validation rules and data binding.

**Validates: Requirements 3.1, 3.2, 3.3, 3.4, 3.5, 3.6, 3.7, 3.8**

### Property 2: Data Persistence Equivalence
*For any* form submission (add or update) in the unified tab, the data saved to the backend must be identical to the data that would have been saved in the original three-tab layout.

**Validates: Requirements 4.1, 4.2, 4.3, 4.4, 4.5**

### Property 3: Navigation Flow Consistency
*For any* successful form submission in sections 1 or 2, the system must navigate to the next section within the unified tab; for section 3, the system must navigate to the next tab (Guarantors).

**Validates: Requirements 6.1, 6.2, 6.3, 6.4, 6.5, 6.6**

### Property 4: Visual Section Separation
*For any* two adjacent sections in the unified tab, there must be a visible separator (border, divider, or spacing) that clearly distinguishes the sections.

**Validates: Requirements 2.5**

### Property 5: Form Validation Preservation
*For any* form field with validation rules in the original layout, the same validation rules must trigger in the unified layout with identical error messages.

**Validates: Requirements 3.4**

### Property 6: File Upload Functionality
*For any* file upload operation in the unified tab, the file must be uploaded to the same backend endpoint and stored in the same database field as in the original layout.

**Validates: Requirements 3.6**

### Property 7: Address Management Preservation
*For any* owner address operation (add, update, change), the address data must be saved and address history must be maintained identically to the original layout.

**Validates: Requirements 5.1, 5.2, 5.3, 5.4**

### Property 8: Responsive Layout Consistency
*For any* screen size (mobile, tablet, desktop), the unified tab must display all form fields in a readable and accessible manner consistent with the original responsive behavior.

**Validates: Requirements 9.1, 9.2, 9.3, 9.4**

### Property 9: Component Communication Preservation
*For any* parent-child component interaction (@ViewChild, @Output events, service state), the communication must function identically in the unified layout as in the original layout.

**Validates: Requirements 10.1, 10.2, 10.3, 10.4, 10.5**

### Property 10: Print Output Consistency
*For any* license print operation, the printed document must contain the same data in the same format as in the original layout.

**Validates: Requirements 7.1, 7.2, 7.3, 7.4**

## Error Handling

**No changes to error handling** - All existing error handling remains:

### Toast Notifications
- Success: "معلومات موفقانه ثبت شد" (Data saved successfully)
- Update: "معلومات موفقانه تغیر یافت" (Data updated successfully)
- Error: "خرابی در ثبت معلومات" (Error saving data)
- Validation: Field-specific error messages

### Network Errors
- HTTP error responses handled by service layer
- User-friendly error messages displayed
- Console logging for debugging

## Implementation Notes

### Development Order
1. Update parent component template (realestate.component.html)
2. Add section wrappers to child component templates
3. Add scroll navigation method to parent component TypeScript
4. Create/update CSS classes for unified layout
5. Test each section independently
6. Test integrated flow
7. Test responsive behavior
8. Test with existing data

### Code Review Checklist
- [ ] No business logic changes
- [ ] All form fields present
- [ ] All validations intact
- [ ] All API calls unchanged
- [ ] Styling consistent with design system
- [ ] Responsive behavior maintained
- [ ] Accessibility attributes present
- [ ] Comments added for clarity

### Deployment Checklist
- [ ] Backup current version
- [ ] Deploy to development environment
- [ ] Run manual test suite
- [ ] Deploy to staging environment
- [ ] User acceptance testing
- [ ] Deploy to production
- [ ] Monitor for errors
- [ ] Verify with production data

## Conclusion

This design provides a comprehensive approach to merging three tabs into a single unified tab while maintaining all existing functionality. The component composition pattern ensures minimal code changes and preserves the integrity of business logic, validation, and data persistence. The implementation focuses on UI reorganization using Tailwind CSS for consistent, professional styling.
