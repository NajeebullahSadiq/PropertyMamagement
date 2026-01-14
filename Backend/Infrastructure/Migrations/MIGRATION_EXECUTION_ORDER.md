# Migration Execution Order

## Safe Deployment Sequence

Execute migrations in this exact order to ensure all dependencies are satisfied:

### Order 1: Shared/Lookup Tables
- **Schema:** `look`
- **Dependencies:** None
- **Tables:** AddressType, Area, EducationLevel, FormsReference, GuaranteeType, IdentityCardType, Location, LostdocumentsType, PropertyType, PUnitType, TransactionType, ViolationType

### Order 2: User Management
- **Schema:** `public`
- **Dependencies:** None
- **Tables:** AspNetUsers, AspNetRoles, AspNetUserRoles, AspNetUserClaims, AspNetRoleClaims, AspNetUserLogins, AspNetUserTokens

### Order 3: Company
- **Schema:** `org`
- **Dependencies:** Shared (look schema)
- **Tables:** CompanyDetails, CompanyOwner, CompanyOwnerAddress, CompanyOwnerAddressHistory, LicenseDetails, Gaurantee, Guarantors, Haqulemtyaz, PeriodicForm, Seta, CompanyAccountInfo, CompanyCancellationInfo

### Order 4: Property
- **Schema:** `tr`
- **Dependencies:** Shared (look), Company (org)
- **Tables:** PropertyDetails, PropertyAddress, BuyerDetails, SellerDetails, WitnessDetails, PropertyCancellations, PropertyCancellationDocuments

### Order 5: Vehicle
- **Schema:** `tr`
- **Dependencies:** Shared (look), Company (org)
- **Tables:** VehiclesPropertyDetails, VehiclesBuyerDetails, VehiclesSellerDetails, VehiclesWitnessDetails

### Order 6: Securities
- **Schema:** `org`
- **Dependencies:** Company (org)
- **Tables:** SecuritiesDistribution, PetitionWriterSecurities, SecuritiesControl

### Order 7: Audit
- **Schema:** `log`
- **Dependencies:** All above modules
- **Tables:** Propertyaudit, Propertybuyeraudit, Propertyselleraudit, Vehicleaudit, Vehicleselleraudit, Vehiclebuyeraudit, Licenseaudit, Guarantorsaudit, Graunteeaudit, Companyowneraudit, Companydetailsaudit

## Deployment Commands

### Using EF Core CLI
```bash
cd Backend
dotnet ef database update
```

### Using SQL Scripts (Production)
```bash
cd Backend/Scripts/Modules
psql -h hostname -U username -d database -f deploy_all_modules.sql
```

### Individual Module Deployment
```bash
psql -h hostname -U username -d database -f 01_Shared_Initial.sql
psql -h hostname -U username -d database -f 02_UserManagement_Initial.sql
psql -h hostname -U username -d database -f 03_Company_Initial.sql
psql -h hostname -U username -d database -f 04_Property_Initial.sql
psql -h hostname -U username -d database -f 05_Vehicle_Initial.sql
psql -h hostname -U username -d database -f 06_Securities_Initial.sql
psql -h hostname -U username -d database -f 07_Audit_Initial.sql
```

## Rollback Strategy

Each module can be rolled back independently (in reverse order):
1. Audit → 2. Securities → 3. Vehicle → 4. Property → 5. Company → 6. UserManagement → 7. Shared

**Warning:** Rolling back will delete data in those tables. Always backup first.
