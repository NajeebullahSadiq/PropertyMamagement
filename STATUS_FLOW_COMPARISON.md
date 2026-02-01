# Status Flow Comparison: Property vs Company

## Property Module (Reference Implementation)

### Database
- Table: `PropertyDetail`
- Column: `iscomplete` (boolean, default: false)

### Status Update Logic
- Location: `Backend/Controllers/SellerDetailsController.cs`
- Method: `UpdatePropertyCompletionStatus()`
- Triggered when: Seller, Buyer, Witness, or Property Address is saved/updated

### Required Fields
1. PropertyTypeId
2. SerialNumber
3. At least one Seller with required fields
4. At least one Buyer with required fields
5. Exactly 2 Witnesses (one from each side)
6. At least one Property Address

### Frontend Display
- File: `Frontend/src/app/estate/propertydetailslist/propertydetailslist.component.html`
- Status column shows: "تکمیل شده" (Complete) or "ناقص" (Incomplete)
- Print button: `[disabled]="!property.iscomplete"`

---

## Company Module (New Implementation)

### Database
- Table: `LicenseDetails`
- Column: `IsComplete` (boolean, default: false)

### Status Update Logic
- Location: `Backend/Services/CompanyService.cs`
- Method: `UpdateLicenseCompletionStatusAsync()`
- Triggered when: CompanyOwner, LicenseDetail, or Guarantor is saved/updated

### Required Fields
1. Company Title
2. At least one Owner with:
   - FirstName
   - FatherName
   - ElectronicNationalIdNumber
3. At least one License with:
   - LicenseNumber
   - IssueDate
   - ExpireDate
   - OfficeAddress
4. At least one Guarantor with:
   - FirstName
   - FatherName
   - ElectronicNationalIdNumber

### Frontend Display
- File: `Frontend/src/app/realestate/realestatelist/realestatelist.component.html`
- Status column shows: "تکمیل شده" (Complete) or "ناقص" (Incomplete)
- Print button: `[disabled]="!property.isComplete"`
- Missing fields show: "تکیمل ناشده" (Incomplete) in red

---

## Key Differences

| Aspect | Property Module | Company Module |
|--------|----------------|----------------|
| Status Field Name | `iscomplete` | `IsComplete` |
| Status Location | PropertyDetail table | LicenseDetails table |
| Service Layer | Direct in controller | Centralized in CompanyService |
| Update Trigger | SellerDetailsController | Multiple controllers via service |
| Witness Requirement | Exactly 2 (strict) | Not applicable |
| Guarantor Requirement | Not applicable | At least 1 |

---

## Implementation Pattern

Both modules follow the same pattern:

1. **Database Column**: Boolean flag to track completion status
2. **Validation Logic**: Check all required fields are present
3. **Auto-Update**: Status updates automatically when related data changes
4. **Frontend Display**: Visual indicator (badge) showing status
5. **Print Restriction**: Button disabled until complete

This ensures data quality and prevents printing incomplete records.
