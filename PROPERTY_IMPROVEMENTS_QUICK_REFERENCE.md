# Property Database Improvements - Quick Reference

## 🚀 Quick Start

```bash
# 1. Backup database
pg_dump -U prmis_user -d PRMIS > backup.sql

# 2. Apply migration
cd Backend
dotnet ef database update

# 3. Done!
```

---

## 📋 What's New?

### PropertyDetails
```csharp
Status              // "Draft", "Pending Review", "Approved", "Completed", "Cancelled"
VerifiedBy          // Who verified
VerifiedAt          // When verified
ApprovedBy          // Who approved
ApprovedAt          // When approved
```

### BuyerDetails & SellerDetails
```csharp
SharePercentage     // e.g., 50.0 for 50%
ShareAmount         // e.g., 50000.00
```

### WitnessDetails
```csharp
PaddressProvinceId  // Witness address
PaddressDistrictId
PaddressVillage
RelationshipToParties  // e.g., "Brother of Seller"
WitnessType         // "Transaction" or "Document"
```

### New Tables

**PropertyOwnershipHistory**
```csharp
PropertyDetailsId
OwnerName
OwnerFatherName
OwnershipStartDate
OwnershipEndDate
TransferDocumentPath
```

**PropertyPayment**
```csharp
PropertyDetailsId
PaymentDate
AmountPaid
PaymentMethod
ReceiptNumber
BalanceRemaining
```

**PropertyValuation**
```csharp
PropertyDetailsId
ValuationDate
ValuationAmount
ValuatorName
ValuationDocumentPath
```

**PropertyDocument**
```csharp
PropertyDetailsId
DocumentCategory    // "Deed", "Title", "Survey", "Tax Certificate", etc.
FilePath
OriginalFileName
```

---

## 🔄 Status Workflow

```
Draft → Pending Review → Approved → Completed
                    ↓
                Rejected
```

---

## 💡 Usage Examples

### Example 1: Property with Multiple Buyers
```csharp
// Property
PropertyDetails.Price = 100000;

// Buyer 1 (60% share)
BuyerDetails[0].SharePercentage = 60;
BuyerDetails[0].ShareAmount = 60000;

// Buyer 2 (40% share)
BuyerDetails[1].SharePercentage = 40;
BuyerDetails[1].ShareAmount = 40000;
```

### Example 2: Payment Installments
```csharp
// First payment
PropertyPayment[0].AmountPaid = 30000;
PropertyPayment[0].BalanceRemaining = 70000;

// Second payment
PropertyPayment[1].AmountPaid = 40000;
PropertyPayment[1].BalanceRemaining = 30000;

// Final payment
PropertyPayment[2].AmountPaid = 30000;
PropertyPayment[2].BalanceRemaining = 0;
```

### Example 3: Document Categories
```csharp
PropertyDocument[0].DocumentCategory = "Deed";
PropertyDocument[1].DocumentCategory = "Title";
PropertyDocument[2].DocumentCategory = "Survey";
PropertyDocument[3].DocumentCategory = "Tax Certificate";
```

### Example 4: Ownership History
```csharp
// Previous owner
PropertyOwnershipHistory[0].OwnerName = "Ahmad Khan";
PropertyOwnershipHistory[0].OwnershipStartDate = 2020-01-01;
PropertyOwnershipHistory[0].OwnershipEndDate = 2025-01-15;

// Current owner (from transaction)
PropertyOwnershipHistory[1].OwnerName = "Hassan Ali";
PropertyOwnershipHistory[1].OwnershipStartDate = 2025-01-15;
PropertyOwnershipHistory[1].OwnershipEndDate = null; // Current owner
```

---

## 🎯 Common Queries

### Get properties by status
```sql
SELECT * FROM tr."PropertyDetails" WHERE "Status" = 'Pending Review';
```

### Get payment history for property
```sql
SELECT * FROM tr."PropertyPayment" 
WHERE "PropertyDetailsId" = 123 
ORDER BY "PaymentDate";
```

### Get ownership chain
```sql
SELECT * FROM tr."PropertyOwnershipHistory" 
WHERE "PropertyDetailsId" = 123 
ORDER BY "OwnershipStartDate";
```

### Get documents by category
```sql
SELECT * FROM tr."PropertyDocument" 
WHERE "PropertyDetailsId" = 123 AND "DocumentCategory" = 'Deed';
```

---

## 📊 Indexes (Auto-created)

- `PropertyDetails.Pnumber` - Fast property number lookup
- `PropertyDetails.Status` - Fast status filtering
- `PropertyDetails.CreatedBy` - Fast user queries
- All foreign keys - Fast joins

---

## ⚠️ Important Notes

1. **Status field is required** - Default is "Draft"
2. **Share fields are optional** - Use when multiple buyers/sellers
3. **New tables are optional** - Use as needed
4. **All existing data preserved** - Migration is backward compatible
5. **Indexes improve performance** - Queries will be faster

---

## 🔧 Troubleshooting

### Migration fails?
```bash
# Check connection
dotnet ef database update --verbose

# Rollback
dotnet ef database update <PreviousMigration>
```

### Need to revert?
```bash
# Restore backup
psql -U prmis_user -d PRMIS < backup.sql
```

---

## 📞 Quick Help

- **Documentation:** `PROPERTY_DATABASE_STRUCTURE_IMPROVED.md`
- **Full Summary:** `PROPERTY_IMPROVEMENTS_SUMMARY.md`
- **Migration File:** `Backend/Migrations/20260115000000_ImprovePropertyManagementStructure.cs`
