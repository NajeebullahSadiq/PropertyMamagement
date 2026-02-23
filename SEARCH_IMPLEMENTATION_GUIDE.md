# Search Implementation Guide - License Applications

## Overview
All search functionality is implemented as API calls, not client-side filtering. This ensures:
- ✅ Better performance with large datasets
- ✅ Consistent pagination
- ✅ Server-side filtering and sorting
- ✅ Reduced data transfer

## Search Types

### 1. Simple Search (Quick Search)
**Location:** Top of the list page
**Endpoint:** `GET /api/LicenseApplication?search={term}`
**Searches across:**
- Request Serial Number (نمبر مسلسل)
- Applicant Name (نام متقاضی)
- Applicant Father Name (نام پدر)
- Applicant Grandfather Name (نام پدرکلان)
- Applicant Electronic Number (نمبر الکترونیکی)
- Proposed Guide Name (نام پیشنهادی رهنما)

**How it works:**
1. User types in the search box
2. On input change, calls API with search term
3. API returns filtered results with pagination
4. UI displays results

**Code Flow:**
```typescript
// Frontend: license-application-list.component.ts
onSearch(): void {
    this.page = 1;
    this.loadData(); // Calls API with searchTerm
}

loadData(): void {
    this.licenseAppService.getAll(this.page, this.pageSize, this.searchTerm, calendar)
        .subscribe(response => {
            this.items = response.items; // Direct from API
            this.totalCount = response.totalCount;
        });
}
```

```csharp
// Backend: LicenseApplicationController.cs
[HttpGet]
public async Task<IActionResult> GetAll(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? search = null)
{
    var query = _context.LicenseApplications.Where(x => x.Status == true);
    
    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(x =>
            x.RequestSerialNumber.Contains(search) ||
            x.ApplicantName.Contains(search) ||
            (x.ApplicantFatherName != null && x.ApplicantFatherName.Contains(search)) ||
            (x.ApplicantGrandfatherName != null && x.ApplicantGrandfatherName.Contains(search)) ||
            (x.ApplicantElectronicNumber != null && x.ApplicantElectronicNumber.Contains(search)) ||
            x.ProposedGuideName.Contains(search));
    }
    
    // Apply pagination and return
}
```

### 2. Advanced Search
**Location:** Expandable panel below simple search
**Endpoint:** `GET /api/LicenseApplication/search?{parameters}`
**Search Fields:**
- Serial Number (نمبر مسلسل)
- Request Date (تاریخ درخواست)
- Applicant Name (نام متقاضی)
- Proposed Guide Name (نام پیشنهادی رهنما)
- Electronic Number (نمبر الکترونیکی متقاضی) ⭐ NEW
- Sharia Deed Number (نمبر قباله شرعی)
- Customary Deed Serial (سریال نمبر سته قباله عرفی)
- Guarantor Name (شهرت تضمین‌کننده)

**How it works:**
1. User clicks "جستجوی پیشرفته" button
2. Panel expands with multiple search fields
3. User fills in one or more fields
4. Clicks "جستجو" button
5. API is called with all filled parameters
6. Results are displayed with pagination

**Code Flow:**
```typescript
// Frontend: license-application-list.component.ts
performAdvancedSearch(): void {
    this.licenseAppService.search(
        this.searchSerialNumber || undefined,
        this.searchRequestDate || undefined,
        this.searchApplicantName || undefined,
        this.searchProposedGuideName || undefined,
        this.searchElectronicNumber || undefined, // NEW
        this.searchShariaDeedNumber || undefined,
        this.searchCustomaryDeedSerial || undefined,
        this.searchGuarantorName || undefined,
        this.page,
        this.pageSize,
        calendar
    ).subscribe(response => {
        this.items = response.items; // Direct from API
        this.totalCount = response.totalCount;
    });
}
```

```csharp
// Backend: LicenseApplicationController.cs
[HttpGet("search")]
public async Task<IActionResult> Search(
    [FromQuery] string? serialNumber = null,
    [FromQuery] string? requestDate = null,
    [FromQuery] string? applicantName = null,
    [FromQuery] string? proposedGuideName = null,
    [FromQuery] string? electronicNumber = null, // NEW
    [FromQuery] string? shariaDeedNumber = null,
    [FromQuery] string? customaryDeedSerial = null,
    [FromQuery] string? guarantorName = null,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
{
    var query = _context.LicenseApplications.Where(x => x.Status == true);
    
    // Apply each filter if provided
    if (!string.IsNullOrWhiteSpace(serialNumber))
        query = query.Where(x => x.RequestSerialNumber.Contains(serialNumber));
    
    if (!string.IsNullOrWhiteSpace(applicantName))
        query = query.Where(x => x.ApplicantName.Contains(applicantName));
    
    if (!string.IsNullOrWhiteSpace(electronicNumber))
        query = query.Where(x => x.ApplicantElectronicNumber != null && 
                                  x.ApplicantElectronicNumber.Contains(electronicNumber));
    
    // ... other filters
    
    // Apply pagination and return
}
```

## Key Implementation Details

### No Client-Side Filtering
❌ **WRONG (Old Way):**
```typescript
// DON'T DO THIS
this.filteredItems = this.items.filter(item => 
    item.applicantName.includes(searchTerm)
);
```

✅ **CORRECT (Current Implementation):**
```typescript
// DO THIS - Let API handle filtering
this.licenseAppService.getAll(page, pageSize, searchTerm, calendar)
    .subscribe(response => {
        this.items = response.items; // Already filtered by API
    });
```

### Pagination Works with Search
- Total count reflects filtered results
- Page numbers adjust based on filtered count
- Changing page maintains search criteria

### Search State Management
```typescript
// Simple search state
searchTerm: string = '';

// Advanced search state
searchSerialNumber: string = '';
searchRequestDate: string = '';
searchApplicantName: string = '';
searchProposedGuideName: string = '';
searchElectronicNumber: string = ''; // NEW
searchShariaDeedNumber: string = '';
searchCustomaryDeedSerial: string = '';
searchGuarantorName: string = '';
```

### Clear Search
```typescript
clearAdvancedSearch(): void {
    // Clear all advanced search fields
    this.searchSerialNumber = '';
    this.searchRequestDate = '';
    this.searchApplicantName = '';
    this.searchProposedGuideName = '';
    this.searchElectronicNumber = '';
    this.searchShariaDeedNumber = '';
    this.searchCustomaryDeedSerial = '';
    this.searchGuarantorName = '';
    this.searchTerm = ''; // Also clear simple search
    this.page = 1;
    this.loadData(); // Reload without filters
}
```

## API Response Format

### Simple Search Response
```json
{
  "items": [
    {
      "id": 1,
      "requestSerialNumber": "KBL-001",
      "applicantName": "احمد",
      "applicantFatherName": "محمد",
      "applicantGrandfatherName": "عبدالله",
      "applicantElectronicNumber": "123456",
      "proposedGuideName": "کابل استیت",
      ...
    }
  ],
  "totalCount": 45,
  "page": 1,
  "pageSize": 10
}
```

### Advanced Search Response
```json
{
  "items": [...],
  "totalCount": 12,
  "page": 1,
  "pageSize": 10,
  "searchCriteria": {
    "serialNumber": null,
    "requestDate": null,
    "applicantName": "احمد",
    "proposedGuideName": null,
    "electronicNumber": "123456",
    "shariaDeedNumber": null,
    "customaryDeedSerial": null,
    "guarantorName": null
  }
}
```

## Performance Considerations

### Database Indexing
Recommended indexes for search performance:
```sql
-- Already exists
CREATE INDEX IX_LicenseApplications_RequestSerialNumber 
ON org."LicenseApplications" ("RequestSerialNumber");

-- Recommended for search
CREATE INDEX IX_LicenseApplications_ApplicantName 
ON org."LicenseApplications" ("ApplicantName");

-- Already created (unique)
CREATE UNIQUE INDEX IX_LicenseApplications_ApplicantElectronicNumber 
ON org."LicenseApplications" ("ApplicantElectronicNumber") 
WHERE "ApplicantElectronicNumber" IS NOT NULL;
```

### Query Optimization
- Uses `Contains()` for partial matching
- Null checks for optional fields
- Pagination applied at database level
- Only includes necessary related data

## Testing Scenarios

### Simple Search
- [ ] Search by serial number
- [ ] Search by applicant name
- [ ] Search by father name
- [ ] Search by grandfather name
- [ ] Search by electronic number
- [ ] Search by guide name
- [ ] Verify pagination works
- [ ] Verify total count updates
- [ ] Clear search returns all results

### Advanced Search
- [ ] Search with single criterion
- [ ] Search with multiple criteria
- [ ] Search by electronic number only
- [ ] Combine electronic number with other fields
- [ ] Verify pagination with filtered results
- [ ] Clear advanced search
- [ ] Switch between simple and advanced search

### Edge Cases
- [ ] Empty search (returns all)
- [ ] No results found
- [ ] Special characters in search
- [ ] Very long search terms
- [ ] Search with pagination
- [ ] Search then change page size

## User Experience

### Search Feedback
- Loading indicator while searching
- "هیچ نتیجه‌ای یافت نشد" message when no results
- Total count shows filtered results
- Pagination adjusts to filtered count

### Search Behavior
- Simple search: Real-time as user types
- Advanced search: On button click
- Clear button resets all filters
- Maintains search when changing pages
- Resets to page 1 on new search

## Migration from Client-Side to API-Based

### What Changed
1. ❌ Removed `filteredItems` array
2. ✅ Use `items` directly from API
3. ❌ Removed client-side `.filter()` logic
4. ✅ All filtering happens on server
5. ✅ Added electronic number to search

### Benefits
- Faster with large datasets
- Consistent pagination
- Better performance
- Reduced memory usage
- Server-side validation

---

**Status:** ✅ COMPLETE
**Implementation:** 100% API-Based
**Client-Side Filtering:** NONE
**Last Updated:** 2026-02-23
