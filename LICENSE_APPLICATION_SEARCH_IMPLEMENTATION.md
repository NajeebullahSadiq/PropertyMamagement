# License Application Search Implementation

## Summary
Implemented comprehensive search functionality for the License Application module with support for 7 search fields.

---

## Backend Changes

### 1. Controller: `Backend/Controllers/LicenseApplication/LicenseApplicationController.cs`

**Added new endpoint:** `GET /api/LicenseApplication/search`

**Search Parameters:**
- `serialNumber` - نمبر مسلسل (Request Serial Number)
- `requestDate` - تاریخ درخواست (Request Date)
- `applicantName` - شهرت متقاضی (Applicant Name)
- `proposedGuideName` - نام پیشنهادی رهنما (Proposed Guide Name)
- `shariaDeedNumber` - نمبر قباله شرعی (Sharia Deed Number)
- `customaryDeedSerial` - سریال نمبر سته قباله عرفی (Customary Deed Serial)
- `guarantorName` - شهرت تضمین‌کننده (Guarantor Name)
- `page` - Page number (default: 1)
- `pageSize` - Items per page (default: 10)
- `calendarType` - Calendar type for date formatting

**Features:**
- Searches across main application fields
- Joins with guarantors table for deed numbers and guarantor names
- Returns applications with guarantor information included
- Supports pagination
- Supports both Jalali and Gregorian calendar dates

**Example API Calls:**
```
GET /api/LicenseApplication/search?applicantName=احمد&page=1&pageSize=10
GET /api/LicenseApplication/search?serialNumber=KBL-00001
GET /api/LicenseApplication/search?guarantorName=محمد&shariaDeedNumber=123
GET /api/LicenseApplication/search?requestDate=1403-10-15&calendarType=jalali
```

---

## Frontend Changes

### 1. Service: `Frontend/src/app/shared/license-application.service.ts`

**Added new method:** `search()`

```typescript
search(
    serialNumber?: string,
    requestDate?: string,
    applicantName?: string,
    proposedGuideName?: string,
    shariaDeedNumber?: string,
    customaryDeedSerial?: string,
    guarantorName?: string,
    page: number = 1,
    pageSize: number = 10,
    calendarType?: string
): Observable<LicenseApplicationListResponse>
```

### 2. Component: `Frontend/src/app/license-applications/license-application-list/license-application-list.component.ts`

**Added properties:**
- `showAdvancedSearch` - Toggle for advanced search panel
- `searchSerialNumber` - نمبر مسلسل
- `searchRequestDate` - تاریخ درخواست
- `searchApplicantName` - شهرت متقاضی
- `searchProposedGuideName` - نام پیشنهادی رهنما
- `searchShariaDeedNumber` - نمبر قباله شرعی
- `searchCustomaryDeedSerial` - سریال نمبر سته قباله عرفی
- `searchGuarantorName` - شهرت تضمین‌کننده

**Added methods:**
- `toggleAdvancedSearch()` - Show/hide advanced search panel
- `performAdvancedSearch()` - Execute advanced search
- `clearAdvancedSearch()` - Clear all search fields and reload data

### 3. Template: `Frontend/src/app/license-applications/license-application-list/license-application-list.component.html`

**Added UI elements:**
- "جستجوی پیشرفته" button to toggle advanced search panel
- Advanced search panel with 7 input fields
- "جستجو" button to perform search
- "پاک کردن" button to clear search and reload

**Search Fields Layout:**
```
Grid Layout (3 columns on large screens):
┌─────────────────────┬─────────────────────┬─────────────────────┐
│ نمبر مسلسل          │ تاریخ درخواست       │ شهرت متقاضی         │
├─────────────────────┼─────────────────────┼─────────────────────┤
│ نام پیشنهادی رهنما  │ نمبر قباله شرعی     │ سریال قباله عرفی    │
├─────────────────────┼─────────────────────┼─────────────────────┤
│ شهرت تضمین‌کننده    │                     │                     │
└─────────────────────┴─────────────────────┴─────────────────────┘
```

---

## Features

### 1. Quick Search (Existing)
- Simple text input at the top
- Searches: Serial Number, Applicant Name, Proposed Guide Name
- Client-side filtering

### 2. Advanced Search (New)
- Expandable panel with 7 search fields
- Server-side search with database queries
- Supports partial matching (LIKE queries)
- Can combine multiple search criteria
- Searches in guarantors table for deed numbers and guarantor names

### 3. Search Behavior
- All fields are optional
- Multiple fields can be combined (AND logic)
- Partial text matching supported
- Date search supports exact match
- Results include guarantor information
- Pagination supported

---

## Usage Examples

### Example 1: Search by Applicant Name
1. Click "جستجوی پیشرفته" button
2. Enter applicant name in "شهرت متقاضی" field
3. Click "جستجو"

### Example 2: Search by Sharia Deed Number
1. Click "جستجوی پیشرفته" button
2. Enter deed number in "نمبر قباله شرعی" field
3. Click "جستجو"

### Example 3: Combined Search
1. Click "جستجوی پیشرفته" button
2. Enter applicant name in "شهرت متقاضی"
3. Enter guarantor name in "شهرت تضمین‌کننده"
4. Click "جستجو"

### Example 4: Clear Search
1. Click "پاک کردن" button
2. All fields are cleared and full list is reloaded

---

## Response Format

```json
{
  "items": [
    {
      "id": 1,
      "requestDate": "2024-01-15",
      "requestDateFormatted": "1403-10-15",
      "requestSerialNumber": "KBL-00001",
      "applicantName": "احمد محمد علی",
      "proposedGuideName": "دارالامان",
      "permanentProvinceId": 1,
      "permanentProvinceName": "کابل",
      "isWithdrawn": false,
      "status": true,
      "guarantors": [
        {
          "id": 1,
          "guarantorName": "محمد حسن",
          "guarantorFatherName": "حسن علی",
          "guaranteeTypeId": 2,
          "shariaDeedNumber": "123456",
          "shariaDeedDate": "2024-01-10",
          "shariaDeedDateFormatted": "1403-10-10"
        }
      ]
    }
  ],
  "totalCount": 1,
  "page": 1,
  "pageSize": 10,
  "searchCriteria": {
    "serialNumber": null,
    "requestDate": null,
    "applicantName": "احمد",
    "proposedGuideName": null,
    "shariaDeedNumber": null,
    "customaryDeedSerial": null,
    "guarantorName": null
  }
}
```

---

## Testing

### Backend Testing
```bash
# Test search by serial number
curl -X GET "http://localhost:5000/api/LicenseApplication/search?serialNumber=KBL-00001"

# Test search by applicant name
curl -X GET "http://localhost:5000/api/LicenseApplication/search?applicantName=احمد"

# Test combined search
curl -X GET "http://localhost:5000/api/LicenseApplication/search?applicantName=احمد&guarantorName=محمد"

# Test with pagination
curl -X GET "http://localhost:5000/api/LicenseApplication/search?applicantName=احمد&page=1&pageSize=10"
```

### Frontend Testing
1. Navigate to License Applications list page
2. Click "جستجوی پیشرفته" button
3. Enter search criteria in any field
4. Click "جستجو" button
5. Verify results are filtered correctly
6. Click "پاک کردن" to clear and reload

---

## Database Queries

The search endpoint performs the following queries:

1. **Main Application Search:**
```sql
SELECT * FROM org."LicenseApplications"
WHERE "Status" = true
  AND ("RequestSerialNumber" LIKE '%search%' OR ...)
```

2. **Guarantor Search:**
```sql
SELECT DISTINCT "LicenseApplicationId" 
FROM org."LicenseApplicationGuarantors"
WHERE "ShariaDeedNumber" LIKE '%search%'
   OR "CustomaryDeedSerialNumber" LIKE '%search%'
   OR "GuarantorName" LIKE '%search%'
```

3. **Combined Results:**
```sql
SELECT la.*, g.*
FROM org."LicenseApplications" la
LEFT JOIN org."LicenseApplicationGuarantors" g ON g."LicenseApplicationId" = la."Id"
WHERE la."Id" IN (filtered_ids)
ORDER BY la."CreatedAt" DESC
```

---

## Notes

1. **Performance:** The search uses database indexes on commonly searched fields
2. **Security:** All inputs are parameterized to prevent SQL injection
3. **Pagination:** Server-side pagination reduces data transfer
4. **Calendar Support:** Supports both Jalali and Gregorian calendars
5. **Guarantor Data:** Automatically includes guarantor information in results
6. **Partial Matching:** All text searches use LIKE with wildcards for flexibility

---

## Files Modified

### Backend
- `Backend/Controllers/LicenseApplication/LicenseApplicationController.cs`

### Frontend
- `Frontend/src/app/shared/license-application.service.ts`
- `Frontend/src/app/license-applications/license-application-list/license-application-list.component.ts`
- `Frontend/src/app/license-applications/license-application-list/license-application-list.component.html`

---

## Deployment Steps

1. **Backend:**
   ```bash
   cd Backend
   dotnet build
   dotnet run
   ```

2. **Frontend:**
   ```bash
   cd Frontend
   npm install
   ng serve
   ```

3. **Test the search functionality:**
   - Navigate to License Applications list
   - Click "جستجوی پیشرفته"
   - Test each search field individually
   - Test combined searches
   - Verify pagination works correctly

---

## Future Enhancements

1. Add date range search (from-to dates)
2. Add export to Excel functionality for search results
3. Add saved search filters
4. Add search history
5. Add autocomplete for frequently searched terms
6. Add province/district filters
7. Add status filters (active/withdrawn)
