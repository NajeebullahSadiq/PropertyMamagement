# Migration Strategy Guide

## Overview
This document describes the module-based migration strategy for the PRMIS application.

## Module → DbContext Mapping

| Module | Schema | Tables |
|--------|--------|--------|
| **Shared/Lookup** | `look` | AddressType, Area, EducationLevel, FormsReference, GuaranteeType, IdentityCardType, Location, LostdocumentsType, PropertyType, PUnitType, TransactionType, ViolationType |
| **UserManagement** | `public` | AspNetUsers, AspNetRoles, AspNetUserRoles, AspNetUserClaims, AspNetRoleClaims, AspNetUserLogins, AspNetUserTokens |
| **Company** | `org` | CompanyDetails, CompanyOwner, CompanyOwnerAddress, CompanyOwnerAddressHistory, LicenseDetails, Gaurantee, Guarantors, Haqulemtyaz, PeriodicForm, CompanyAccountInfo, CompanyCancellationInfo |
| **Property** | `tr` | PropertyDetails, PropertyAddress, BuyerDetails, SellerDetails, WitnessDetails, PropertyCancellations, PropertyCancellationDocuments |
| **Vehicle** | `tr` | VehiclesPropertyDetails, VehiclesBuyerDetails, VehiclesSellerDetails, VehiclesWitnessDetails |
| **Securities** | `org` | SecuritiesDistribution, PetitionWriterSecurities, SecuritiesControl |
| **Audit** | `log` | Propertyaudit, Propertybuyeraudit, Propertyselleraudit, Vehicleaudit, Vehicleselleraudit, Vehiclebuyeraudit, Licenseaudit, Guarantorsaudit, Graunteeaudit, Companyowneraudit, Companydetailsaudit |

## Migration Execution Order

1. **Shared/Lookup** - Base lookup tables (no dependencies)
2. **UserManagement** - Identity tables (no dependencies)
3. **Company** - Organization tables (depends on Lookup)
4. **Property** - Property transaction tables (depends on Lookup, Company)
5. **Vehicle** - Vehicle transaction tables (depends on Lookup, Company)
6. **Securities** - Securities tables (depends on Company)
7. **Audit** - Audit log tables (depends on all above)

## Migration Naming Convention

Format: `YYYYMMDD_Module_Action`

Examples:
- `20260114_Shared_Initial`
- `20260114_Company_Initial`
- `20260114_Property_AddWitnessTable`
- `20260114_Securities_AddSerialRange`

## How to Apply Migrations Safely

### Development Environment
```bash
cd Backend
dotnet ef database update
```

### Production Environment
1. Generate SQL script first:
```bash
dotnet ef migrations script --idempotent -o Scripts/deploy_migration.sql
```

2. Review the script manually
3. Apply via database tool or:
```bash
psql -h hostname -U username -d database -f Scripts/deploy_migration.sql
```

## How to Generate Future Migrations

### Adding a new migration:
```bash
cd Backend
dotnet ef migrations add YYYYMMDD_Module_ActionDescription
```

### Best Practices:
- Always prefix with date in YYYYMMDD format
- Include module name (Shared, Company, Property, Vehicle, Securities, User)
- Use descriptive action (Initial, Add, Update, Remove, Fix)
- Test on empty database AND existing production copy

## Server Deployment Without Downtime

### Blue-Green Deployment:
1. Apply backward-compatible migrations first
2. Deploy new application version
3. Apply breaking migrations (if any) during maintenance window

### Rolling Deployment:
1. Ensure all migrations are backward-compatible
2. Apply migrations before deploying new code
3. Deploy application instances one by one

## Consolidated Migration Files

The following consolidated migrations replace the scattered individual migrations:

| Consolidated Migration | Replaces |
|----------------------|----------|
| `20260114_Shared_Initial` | Initial lookup table creation |
| `20260114_UserManagement_Initial` | Identity tables + RBAC columns |
| `20260114_Company_Initial` | All company-related tables |
| `20260114_Property_Initial` | All property transaction tables |
| `20260114_Vehicle_Initial` | All vehicle transaction tables |
| `20260114_Securities_Initial` | All securities tables |
| `20260114_Audit_Initial` | All audit log tables |

## Removed/Deprecated Migrations

The following migrations have been consolidated and should not be applied individually:
- All migrations before 20260114 are now consolidated
- Raw SQL migrations have been converted to EF Core migrations
- Duplicate table creations have been removed
