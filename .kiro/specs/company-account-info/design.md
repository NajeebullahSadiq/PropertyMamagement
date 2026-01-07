# Design Document: Company Account Info (مالیه)

## Overview

This design document describes the implementation of a new Account / Financial Information section in the Company module. The feature adds a dedicated tab for capturing and managing financial data including tax settlements, payment information, transaction counts, and commission details. The implementation follows the existing patterns established in the Company module and integrates with the multi-calendar date system.

## Architecture

The feature follows the existing three-tier architecture:

```
┌─────────────────────────────────────────────────────────────────┐
│                        Frontend (Angular)                        │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │  RealEstate Module                                       │    │
│  │  ├── CompanyDetails Tab                                  │    │
│  │  ├── CompanyOwner Tab                                    │    │
│  │  ├── LicenseDetails Tab                                  │    │
│  │  ├── Guarantors Tab                                      │    │
│  │  └── AccountInfo Tab (NEW)                               │    │
│  └─────────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                    Backend (ASP.NET Core)                        │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │  Controllers/Companies/                                  │    │
│  │  └── CompanyAccountInfoController.cs (NEW)               │    │
│  └─────────────────────────────────────────────────────────┘    │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │  Models/                                                 │    │
│  │  ├── CompanyAccountInfo.cs (NEW)                         │    │
│  │  └── RequestData/CompanyAccountInfoData.cs (NEW)         │    │
│  └─────────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                    Database (PostgreSQL)                         │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │  Schema: org                                             │    │
│  │  └── CompanyAccountInfo (NEW TABLE)                      │    │
│  │      ├── FK → CompanyDetails                             │    │
│  └─────────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────────┘
```

## Components and Interfaces

### Backend Components

#### 1. CompanyAccountInfo Entity (Model)

Location: `Backend/Models/CompanyAccountInfo.cs`

```csharp
public class CompanyAccountInfo
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string? SettlementInfo { get; set; }
    public decimal TaxPaymentAmount { get; set; }
    public int? SettlementYear { get; set; }
    public DateOnly? TaxPaymentDate { get; set; }
    public int? TransactionCount { get; set; }
    public decimal? CompanyCommission { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public bool? Status { get; set; }
    
    // Navigation property
    public virtual CompanyDetail Company { get; set; }
}
```

#### 2. CompanyAccountInfoData DTO

Location: `Backend/Models/RequestData/CompanyAccountInfoData.cs`

```csharp
public class CompanyAccountInfoData
{
    public int? Id { get; set; }
    public int CompanyId { get; set; }
    public string? SettlementInfo { get; set; }
    public decimal TaxPaymentAmount { get; set; }
    public int? SettlementYear { get; set; }
    public DateOnly? TaxPaymentDate { get; set; }
    public int? TransactionCount { get; set; }
    public decimal? CompanyCommission { get; set; }
}
```

#### 3. CompanyAccountInfoController

Location: `Backend/Controllers/Companies/CompanyAccountInfoController.cs`

Endpoints:
- `GET /api/CompanyAccountInfo/{companyId}` - Retrieve account info by company ID
- `POST /api/CompanyAccountInfo` - Create new account info
- `PUT /api/CompanyAccountInfo/{id}` - Update existing account info

### Frontend Components

#### 1. AccountInfo Component

Location: `Frontend/src/app/realestate/accountinfo/`

Files:
- `accountinfo.component.ts` - Component logic
- `accountinfo.component.html` - Template with form
- `accountinfo.component.scss` - Styles

#### 2. AccountInfo Model

Location: `Frontend/src/app/models/AccountInfo.ts`

```typescript
export interface AccountInfo {
    id?: number;
    companyId: number;
    settlementInfo?: string;
    taxPaymentAmount: number;
    settlementYear?: number;
    taxPaymentDate?: Date;
    transactionCount?: number;
    companyCommission?: number;
    createdAt?: Date;
    createdBy?: string;
    status?: boolean;
}
```

#### 3. Service Extension

Location: `Frontend/src/app/shared/compnaydetail.service.ts`

New methods:
- `getAccountInfo(companyId: number): Observable<AccountInfo>`
- `createAccountInfo(data: AccountInfo): Observable<AccountInfo>`
- `updateAccountInfo(id: number, data: AccountInfo): Observable<AccountInfo>`

## Data Models

### Database Schema

```sql
CREATE TABLE org."CompanyAccountInfo" (
    "Id" SERIAL PRIMARY KEY,
    "CompanyId" INTEGER NOT NULL,
    "SettlementInfo" TEXT,
    "TaxPaymentAmount" DECIMAL(18,2) NOT NULL,
    "SettlementYear" INTEGER,
    "TaxPaymentDate" DATE,
    "TransactionCount" INTEGER,
    "CompanyCommission" DECIMAL(18,2),
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    "Status" BOOLEAN DEFAULT TRUE,
    CONSTRAINT "FK_CompanyAccountInfo_CompanyDetails" 
        FOREIGN KEY ("CompanyId") REFERENCES org."CompanyDetails"("Id")
);

CREATE INDEX "IX_CompanyAccountInfo_CompanyId" 
    ON org."CompanyAccountInfo"("CompanyId");
```

### Entity Relationship

```
┌─────────────────────┐         ┌─────────────────────────┐
│   CompanyDetails    │         │   CompanyAccountInfo    │
├─────────────────────┤         ├─────────────────────────┤
│ Id (PK)             │◄────────│ CompanyId (FK)          │
│ Title               │    1:N  │ Id (PK)                 │
│ PhoneNumber         │         │ SettlementInfo          │
│ LicenseNumber       │         │ TaxPaymentAmount        │
│ ...                 │         │ SettlementYear          │
└─────────────────────┘         │ TaxPaymentDate          │
                                │ TransactionCount        │
                                │ CompanyCommission       │
                                │ CreatedAt               │
                                │ CreatedBy               │
                                │ Status                  │
                                └─────────────────────────┘
```

## Correctness Properties

*A property is a characteristic or behavior that should hold true across all valid executions of a system—essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*

### Property 1: Foreign Key Integrity

*For any* CompanyAccountInfo record, the CompanyId must reference an existing CompanyDetails record. Attempting to create an AccountInfo with a non-existent CompanyId should fail with a constraint violation.

**Validates: Requirements 1.1**

### Property 2: Automatic Timestamp Population

*For any* CompanyAccountInfo record created through the API, the CreatedAt field should be automatically populated with a timestamp within 5 seconds of the creation request time.

**Validates: Requirements 1.9**

### Property 3: Negative Numeric Value Rejection

*For any* negative numeric value submitted for TaxPaymentAmount, TransactionCount, or CompanyCommission fields, the backend SHALL return a 400 Bad Request with appropriate validation errors.

**Validates: Requirements 2.6, 2.7, 2.8**

### Property 4: Non-Numeric Input Validation

*For any* non-numeric string submitted to the TaxPaymentAmount, TransactionCount, or CompanyCommission fields, the system SHALL display a validation error and prevent form submission.

**Validates: Requirements 5.1, 5.2, 5.3**

### Property 5: Settlement Info Length Validation

*For any* string exceeding 500 characters submitted to the SettlementInfo field, the system SHALL display a validation error and prevent form submission.

**Validates: Requirements 5.4**

### Property 6: Date Calendar Round-Trip

*For any* date selected using any of the three calendar systems (Georgian, Hijri Qamari, Hijri Shamsi), storing the date and retrieving it should produce an equivalent date value regardless of the calendar used for display.

**Validates: Requirements 4.5**

### Property 7: Migration Data Preservation

*For any* existing CompanyDetails records in the database, running the CompanyAccountInfo migration SHALL preserve all existing data without modification or deletion.

**Validates: Requirements 7.2**

## Error Handling

### Backend Error Handling

| Error Condition | HTTP Status | Response |
|----------------|-------------|----------|
| Company not found | 404 | `{ "message": "Company not found" }` |
| Invalid numeric value | 400 | `{ "errors": { "field": ["Must be a valid number"] } }` |
| Negative value | 400 | `{ "errors": { "field": ["Must be non-negative"] } }` |
| Text too long | 400 | `{ "errors": { "settlementInfo": ["Maximum 500 characters"] } }` |
| Missing required field | 400 | `{ "errors": { "taxPaymentAmount": ["Required"] } }` |
| Database constraint violation | 500 | `{ "message": "Database error occurred" }` |

### Frontend Error Handling

- Display inline validation errors below each field
- Show toast notification for API errors
- Disable submit button until form is valid
- Handle network errors with retry option

## Testing Strategy

### Unit Tests

Unit tests will verify specific examples and edge cases:

1. **Model Validation Tests**
   - Test that valid AccountInfo objects pass validation
   - Test that invalid numeric values are rejected
   - Test that text exceeding 500 characters is rejected
   - Test that required fields are enforced

2. **Controller Tests**
   - Test GET endpoint returns correct data for valid company
   - Test GET endpoint returns 404 for non-existent company
   - Test POST endpoint creates record with valid data
   - Test PUT endpoint updates existing record
   - Test validation error responses

3. **Frontend Component Tests**
   - Test form renders all fields
   - Test form validation messages display
   - Test form submission calls service
   - Test read-only mode disables fields

### Property-Based Tests

Property-based tests will use a PBT library (FsCheck for C#, fast-check for TypeScript) to verify universal properties:

1. **Property 1: Foreign Key Integrity**
   - Generate random CompanyAccountInfo with invalid CompanyIds
   - Verify all attempts fail with constraint violation
   - Minimum 100 iterations

2. **Property 3: Negative Value Rejection**
   - Generate random negative decimals and integers
   - Submit to each numeric field
   - Verify all return 400 Bad Request
   - Minimum 100 iterations

3. **Property 4: Non-Numeric Input Validation**
   - Generate random non-numeric strings
   - Submit to each numeric field
   - Verify all trigger validation errors
   - Minimum 100 iterations

4. **Property 5: Settlement Info Length**
   - Generate random strings > 500 characters
   - Submit to SettlementInfo field
   - Verify all trigger validation errors
   - Minimum 100 iterations

5. **Property 6: Date Round-Trip**
   - Generate random valid dates
   - Convert to each calendar format
   - Store and retrieve
   - Verify equivalence
   - Minimum 100 iterations

### Test Configuration

- Backend: xUnit with FsCheck for property-based testing
- Frontend: Jasmine/Karma with fast-check for property-based testing
- Each property test tagged with: `Feature: company-account-info, Property N: {property_text}`
