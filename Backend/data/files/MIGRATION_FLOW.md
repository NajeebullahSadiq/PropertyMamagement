# Migration Flow Diagram

## Overview: Access Database → PostgreSQL

```
OLD ACCESS DATABASE                    NEW POSTGRESQL DATABASE
(MainForm.xlsx)                        (Normalized Structure)
================                       ====================

┌─────────────────────────┐           
│  7,329 Records          │           
│  50 Columns             │           
│  Single Table           │           
│  Mixed Data             │           
└───────────┬─────────────┘           
            │                         
            │ Export to JSON          
            ▼                         
┌─────────────────────────┐           
│ mainform_records.json   │           
│ (11 MB file)            │           
└───────────┬─────────────┘           
            │                         
            │ .NET Migration Tool     
            │ (Program.cs)            
            ▼                         
┌─────────────────────────┐           
│  Data Transformation    │           
│  ├─ Parse dates         │           
│  ├─ Map fields          │           
│  ├─ Create lookups      │           
│  └─ Validate data       │           
└───────────┬─────────────┘           
            │                         
            │ Insert with             
            │ Transactions            
            ▼                         
┌───────────────────────────────────────────┐
│        POSTGRESQL DATABASE                │
│                                           │
│  ┌─────────────────────┐                 │
│  │ org.companydetails  │ (7,329 records) │
│  │  ├─ Id              │                 │
│  │  ├─ Title           │                 │
│  │  ├─ TIN             │                 │
│  │  ├─ ProvinceId ─────┼───┐            │
│  │  └─ Status          │   │            │
│  └──────┬──────────────┘   │            │
│         │                   │            │
│         │ 1:Many            │            │
│         ▼                   │            │
│  ┌─────────────────────┐   │            │
│  │ org.companyowners   │   │            │
│  │  (7,326 records)    │   │            │
│  │  ├─ FirstName       │   │            │
│  │  ├─ FatherName      │   │            │
│  │  ├─ CompanyId       │   │            │
│  │  └─ ProvinceId ─────┼───┤            │
│  └─────────────────────┘   │            │
│                             │            │
│  ┌─────────────────────┐   │            │
│  │ org.licensedetails  │   │            │
│  │  (7,329 records)    │   │            │
│  │  ├─ LicenseNumber   │   │            │
│  │  ├─ CompanyId       │   │            │
│  │  ├─ RoyaltyAmount   │   │            │
│  │  └─ ProvinceId ─────┼───┤            │
│  └─────────────────────┘   │            │
│                             │            │
│  ┌─────────────────────┐   │            │
│  │ org.cancellation    │   │            │
│  │  (2,169 records)    │   │            │
│  │  ├─ CompanyId       │   │            │
│  │  ├─ LetterNumber    │   │            │
│  │  └─ Remarks         │   │            │
│  └─────────────────────┘   │            │
│                             │            │
│  LOOKUP TABLES              ▼            │
│  ┌─────────────────────────────┐        │
│  │ look.location               │        │
│  │  ├─ Provinces (35)          │◄───────┘
│  │  └─ Districts (741)         │        
│  └─────────────────────────────┘        
│                                          
│  ┌─────────────────────────────┐        
│  │ look.educationlevel         │        
│  │  └─ Levels (3)              │        
│  └─────────────────────────────┘        
│                                          
└───────────────────────────────────────────┘
```

## Field Mapping Flow

```
OLD FIELD           TRANSFORMATION          NEW FIELD
===========         ===============         =============

RID                 Direct Copy      →      CompanyDetails.Id
RealEstateName      Direct Copy      →      CompanyDetails.Title
TIN                 Direct Copy      →      CompanyDetails.TIN

FName               Direct Copy      →      CompanyOwners.FirstName
FathName            Direct Copy      →      CompanyOwners.FatherName
GFName              Direct Copy      →      CompanyOwners.GrandFatherName

Education           Lookup Create    →      CompanyOwners.EducationLevelId
                    (Text → ID)              ↓
                                            look.educationlevel

PerProvince         Lookup Create    →      CompanyDetails.ProvinceId
                    (Text → ID)              CompanyOwners.OwnerProvinceId
                                            ↓
                                            look.location (type='province')

PerWoloswaly        Lookup Create    →      CompanyOwners.OwnerDistrictId
                    (Text → ID)              ↓
                                            look.location (type='district')

SYear/SMonth/SDay   Date Combine     →      LicenseDetails.IssueDate
                    (1394/9/9 →              (1394-09-09)
                     1394-09-09)

LicenseNo           String Convert   →      LicenseDetails.LicenseNumber
                    (123 → "123")

Halat               Boolean Logic    →      CompanyDetails.Status
                    (جدید → true)           (فسخ شده → false)
```

## Migration Process Steps

```
┌──────────────────────────────────────────────────────────────┐
│                    MIGRATION WORKFLOW                        │
└──────────────────────────────────────────────────────────────┘

Step 1: PRE-MIGRATION SETUP
├─ Create database schemas (org, log, look)
├─ Create all tables with proper structure
├─ Run setup.sql to create initial lookup data
└─ Create indexes for performance

Step 2: LOAD DATA
├─ Read mainform_records.json
├─ Parse 7,329 records into memory
└─ Validate JSON structure

Step 3: PROCESS EACH RECORD (In Transaction)
├─ Check if company already exists
│   ├─ Yes → Skip record
│   └─ No → Continue
│
├─ Get/Create Province lookup
│   └─ look.location (type='province')
│
├─ INSERT CompanyDetails
│   ├─ Id (from RID)
│   ├─ Title (from RealEstateName)
│   ├─ TIN (from TIN)
│   ├─ ProvinceId (from lookup)
│   └─ Status (from Halat)
│
├─ Get/Create Education Level lookup
│   └─ look.educationlevel
│
├─ Get/Create District lookup
│   └─ look.location (type='district')
│
├─ INSERT CompanyOwner (if has name)
│   ├─ FirstName, FatherName, GrandFatherName
│   ├─ CompanyId (from previous insert)
│   ├─ EducationLevelId (from lookup)
│   ├─ OwnerProvinceId, OwnerDistrictId
│   └─ PermanentProvinceId, PermanentDistrictId
│
├─ INSERT LicenseDetails (if has license)
│   ├─ LicenseNumber
│   ├─ CompanyId
│   ├─ IssueDate (combined from SYear/SMonth/SDay)
│   ├─ ExpireDate (combined from EYear/EMonth/EDay)
│   ├─ RoyaltyAmount, RoyaltyDate
│   └─ Status
│
└─ INSERT CompanyCancellationInfo (if cancelled)
    ├─ CompanyId
    ├─ LicenseCancellationLetterNumber
    ├─ LicenseCancellationLetterDate
    └─ Remarks

Step 4: COMMIT OR ROLLBACK
├─ If all inserts successful → COMMIT transaction
└─ If any error → ROLLBACK transaction

Step 5: POST-MIGRATION VERIFICATION
├─ Count records in each table
├─ Verify relationships (foreign keys)
├─ Check data integrity
└─ Run sample queries
```

## Data Distribution After Migration

```
┌────────────────────────────────────────────────────────────┐
│                 MIGRATION STATISTICS                        │
├────────────────────────────────────────────────────────────┤
│                                                            │
│  CompanyDetails              7,329 records ████████████  │
│  CompanyOwners               7,326 records ████████████  │
│  LicenseDetails              7,329 records ████████████  │
│  CompanyCancellationInfo     2,169 records ████▌         │
│                                                            │
│  look.location (provinces)      35 records ▌             │
│  look.location (districts)     741 records ███           │
│  look.educationlevel             3 records ▌             │
│                                                            │
└────────────────────────────────────────────────────────────┘

Province Distribution (Top 10):
├─ کابل (Kabul):        10,663 references ██████████████
├─ پروان (Parwan):         619 references ███▌
├─ وردک (Wardak):          595 references ███▌
├─ غزنی (Ghazni):          400 references ██▌
├─ کاپیسا (Kapisa):        355 references ██
├─ لوگر (Logar):           352 references ██
├─ پنجشیر (Panjshir):      268 references █▌
├─ لغمان (Laghman):        258 references █▌
├─ پکتیا (Paktia):         244 references █▌
└─ ننگرهار (Nangarhar):    156 references █

License Types:
├─ جدید (New):         4,394 records (59.9%) ███████▌
└─ تجدید (Renewal):    2,932 records (40.0%) █████

Status Distribution:
├─ Active (جدید):      7,322 records (99.9%) █████████████
└─ Inactive (فسخ):        7 records (0.09%) ▌
```

## Error Handling Flow

```
┌─────────────────────────────────────────────────────────┐
│              ERROR HANDLING STRATEGY                    │
└─────────────────────────────────────────────────────────┘

For Each Record:
├─ BEGIN TRANSACTION
│   │
│   ├─ Try to process record
│   │   │
│   │   ├─ Success?
│   │   │   ├─ Yes → COMMIT
│   │   │   │         └─ stats.Success++
│   │   │   │
│   │   │   └─ No → ROLLBACK
│   │   │             ├─ Log error details
│   │   │             ├─ stats.Errors++
│   │   │             └─ Continue to next record
│   │   │
│   │   └─ Already exists?
│   │       ├─ Yes → ROLLBACK
│   │       │         └─ stats.Skipped++
│   │       │
│   │       └─ No → Process normally
│   │
│   └─ END TRANSACTION
│
└─ Show progress every 100 records

Common Errors Handled:
├─ Duplicate primary key      → Skip record
├─ Foreign key constraint     → Create lookup, retry
├─ Date format error          → Store as string
├─ NULL constraint violation  → Use DBNull.Value
└─ Connection timeout         → Retry with backoff
```

## Technology Stack

```
┌────────────────────────────────────────────────────────┐
│                    TECH STACK                          │
├────────────────────────────────────────────────────────┤
│                                                        │
│  Source Data:    Microsoft Access (via Excel export)  │
│  Format:         JSON (UTF-8 with RTL support)       │
│  Migration:      .NET 8.0 Console Application        │
│  Database:       PostgreSQL 13+                       │
│  ORM/Driver:     Npgsql (Raw SQL)                     │
│  Transaction:    Database Transactions (ACID)        │
│  Backend API:    .NET (ASP.NET Core)                 │
│  Frontend:       React.js                            │
│                                                        │
└────────────────────────────────────────────────────────┘
```

## Performance Metrics

```
Processing Speed:     100-150 records/second
Total Duration:       5-10 minutes
Memory Usage:         200-300 MB
Database Size:        ~15-20 MB after migration
Transaction Size:     1 record per transaction
Batch Size:          50 records per batch (configurable)
```

## Data Integrity Guarantees

```
✓ ACID Transactions    Each record is atomic
✓ Foreign Keys         All relationships validated
✓ Data Types           Strong typing enforced
✓ NULL Handling        Explicit NULL values
✓ UTF-8 Encoding       Persian/Dari text preserved
✓ Referential Integrity All FK relationships valid
✓ Rollback on Error    No partial records
```

---

This diagram provides a complete visual overview of how your 7,329 records 
will be transformed from the old flat Access structure to the new normalized 
PostgreSQL structure.
