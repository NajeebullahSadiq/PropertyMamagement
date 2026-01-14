# Backend Architecture

## Overview

This backend follows a Clean Modular Architecture with clear separation of concerns. The refactoring maintains full backward compatibility with existing code while providing a clean structure for future development.

## Directory Structure

```
Backend/
├── Configuration/          # Core configuration (actively used)
│   ├── AppDbContext.cs     # Main EF Core DbContext
│   ├── CustomClaimTypes.cs # Custom claim type definitions
│   ├── DatabaseSeeder.cs   # Database seeding
│   ├── PolicyTypes.cs      # Authorization policies
│   └── UserRoles.cs        # Role definitions & permissions
│
├── Controllers/            # API Controllers
│   ├── Companies/          # Company module controllers
│   ├── Report/             # Dashboard controller
│   ├── Securities/         # Securities module controllers
│   ├── Vehicles/           # Vehicle module controllers
│   └── *.cs                # Other controllers
│
├── Helpers/                # Utility helpers
│   ├── CalendarType.cs     # Calendar type enum
│   ├── DateConversionHelper.cs
│   └── RbacHelper.cs
│
├── Models/                 # Entity models (organized by module)
│   ├── Company/            # Company registration models & audits
│   ├── Identity/           # User/auth related models
│   ├── Lookup/             # Reference/lookup tables
│   ├── Property/           # Real estate transaction models & audits
│   ├── RequestData/        # Request DTOs (Company/, Securities/)
│   ├── Securities/         # Securities distribution models
│   ├── Vehicles/           # Vehicle transaction models & audits
│   └── ViewModels/         # View models for print/display
│
├── Services/               # Business services
│   └── AuthorizationService.cs
│
├── src/                    # Clean Architecture extensions
│   ├── API/                # API Layer
│   │   └── Controllers/    # Base controller
│   │
│   ├── Application/        # Application Layer
│   │   ├── Common/         # Shared interfaces, models, validators
│   │   ├── Company/        # Company module DTOs & interfaces
│   │   ├── Dashboard/      # Dashboard DTOs & interfaces
│   │   ├── Property/       # Property module DTOs & interfaces
│   │   ├── Reports/        # Reporting DTOs & interfaces
│   │   ├── Securities/     # Securities DTOs, interfaces, validators
│   │   ├── UserManagement/ # User management DTOs & interfaces
│   │   └── Vehicle/        # Vehicle module DTOs & interfaces
│   │
│   ├── Domain/             # Domain Layer
│   │   └── Common/         # Base entities
│   │
│   ├── Infrastructure/     # Infrastructure Layer
│   │   ├── DependencyInjection.cs
│   │   └── Services/       # Service implementations
│   │
│   └── Shared/             # Shared Layer
│       ├── Constants/      # LicenseTypes, ModuleNames, DocumentTypes
│       └── Extensions/     # Query extensions, pagination
│
└── Migrations/             # EF Core migrations
```

## Modules

| Module | Description | Controllers |
|--------|-------------|-------------|
| User Management | User CRUD, auth, roles | ApplicationUserController, UserProfileController |
| Dashboard | Statistics, reports | DashboardController |
| Company | Company registration | CompanyDetailsController, CompanyOwnerController, etc. |
| Property | Real estate transactions | PropertyDetailsController, SellerDetailsController |
| Vehicle | Vehicle transactions | VehiclesController, VehiclesSubController |
| Securities - Rahnamayi | Transaction guide securities | SecuritiesDistributionController |
| Securities - Petition | Petition writer securities | PetitionWriterSecuritiesController |
| Securities - Control | Entry/exit control | SecuritiesControlController |

## Authorization Roles

| Role | Dari Name | Access |
|------|-----------|--------|
| ADMIN | مدیر سیستم | Full access |
| AUTHORITY | مقام / رهبری | View-only all modules |
| COMPANY_REGISTRAR | کاربر ثبت جواز شرکت | Company module |
| LICENSE_REVIEWER | ریاست بررسی و ثبت جواز | View-only company list |
| PROPERTY_OPERATOR | کاربر عملیاتی املاک | Property module |
| VEHICLE_OPERATOR | کاربر عملیاتی موتر فروشی | Vehicle module |

## Key Design Decisions

1. **Backward Compatibility**: All existing code continues to work unchanged
2. **Gradual Migration**: New features can use the clean architecture
3. **No Breaking Changes**: API contracts remain identical
4. **Module Isolation**: Each module has its own DTOs and interfaces
5. **Centralized Authorization**: RbacHelper provides consistent access control

## New Components

### Application Layer (src/Application/)
- **DTOs**: Type-safe data transfer objects for each module
- **Interfaces**: Service contracts for dependency injection
- **Validators**: Input validation logic

### Infrastructure Layer (src/Infrastructure/)
- **Services**: Implementation of application interfaces
- **CurrentUserService**: Access to authenticated user info
- **AppAuthorizationService**: Centralized authorization checks

### Shared Layer (src/Shared/)
- **Constants**: LicenseTypes, ModuleNames, DocumentTypes
- **Extensions**: QueryableExtensions for pagination
