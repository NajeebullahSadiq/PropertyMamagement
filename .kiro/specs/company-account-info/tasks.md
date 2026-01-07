# Implementation Plan: Company Account Info (مالیه)

## Overview

This implementation plan breaks down the Company Account Info feature into discrete coding tasks. Each task builds incrementally on previous work, ensuring no orphaned code. The implementation follows the existing patterns in the Company module.

## Tasks

- [x] 1. Create database migration and entity model
  - [x] 1.1 Create CompanyAccountInfo entity model
    - Create `Backend/Models/CompanyAccountInfo.cs` with all fields
    - Include navigation property to CompanyDetail
    - Add audit fields (CreatedAt, CreatedBy, Status)
    - _Requirements: 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8_

  - [x] 1.2 Create database migration
    - Create migration file in `Backend/Migrations/`
    - Create CompanyAccountInfo table in org schema
    - Add foreign key constraint to CompanyDetails
    - Add index on CompanyId
    - _Requirements: 7.1, 7.3, 7.4_

  - [x] 1.3 Update AppDbContext
    - Add DbSet<CompanyAccountInfo> to AppDbContext
    - Configure entity in OnModelCreating
    - Set up foreign key relationship
    - _Requirements: 1.1_

  - [ ]* 1.4 Write property test for migration data preservation
    - **Property 7: Migration Data Preservation**
    - Verify existing CompanyDetails records are preserved after migration
    - **Validates: Requirements 7.2**

- [x] 2. Create backend API endpoints
  - [x] 2.1 Create CompanyAccountInfoData DTO
    - Create `Backend/Models/RequestData/CompanyAccountInfoData.cs`
    - Include all fields for API requests
    - Add data annotations for validation
    - _Requirements: 2.4, 2.6, 2.7, 2.8_

  - [x] 2.2 Create CompanyAccountInfoController
    - Create `Backend/Controllers/Companies/CompanyAccountInfoController.cs`
    - Implement GET endpoint by company ID
    - Implement POST endpoint for creation
    - Implement PUT endpoint for updates
    - Add validation and error handling
    - _Requirements: 2.1, 2.2, 2.3, 2.5_

  - [ ]* 2.3 Write property test for negative value rejection
    - **Property 3: Negative Numeric Value Rejection**
    - Generate random negative values for numeric fields
    - Verify all return 400 Bad Request
    - **Validates: Requirements 2.6, 2.7, 2.8**

  - [ ]* 2.4 Write property test for automatic timestamp
    - **Property 2: Automatic Timestamp Population**
    - Create records and verify CreatedAt is populated
    - **Validates: Requirements 1.9**

- [x] 3. Checkpoint - Backend complete
  - Ensure all backend tests pass
  - Verify API endpoints work with Postman/curl
  - Ask the user if questions arise

- [x] 4. Create frontend model and service
  - [x] 4.1 Create AccountInfo TypeScript model
    - Create `Frontend/src/app/models/AccountInfo.ts`
    - Define interface with all fields
    - _Requirements: 3.3_

  - [x] 4.2 Extend company service with AccountInfo methods
    - Add getAccountInfo(companyId) method
    - Add createAccountInfo(data) method
    - Add updateAccountInfo(id, data) method
    - _Requirements: 2.1, 2.2, 2.3_

- [x] 5. Create AccountInfo component
  - [x] 5.1 Generate AccountInfo component
    - Create component in `Frontend/src/app/realestate/accountinfo/`
    - Set up component structure with inputs/outputs
    - _Requirements: 3.1_

  - [x] 5.2 Implement AccountInfo form template
    - Create form with all six financial fields
    - Add Dari labels for each field
    - Use multi-calendar date picker for tax_payment_date
    - Apply Tailwind styling matching existing components
    - _Requirements: 3.3, 3.6, 4.1, 4.2, 4.3, 4.4_

  - [x] 5.3 Implement AccountInfo component logic
    - Handle form initialization and data binding
    - Implement save/update functionality
    - Handle view mode (read-only) and edit mode
    - Wire up service calls
    - _Requirements: 3.4, 3.5_

  - [x] 5.4 Add form validation
    - Add numeric validation for amount fields
    - Add max length validation for settlement_info (500 chars)
    - Add required validation for tax_payment_amount
    - Display inline validation errors
    - _Requirements: 5.1, 5.2, 5.3, 5.4, 5.5, 5.6_

  - [ ]* 5.5 Write property test for non-numeric input validation
    - **Property 4: Non-Numeric Input Validation**
    - Generate random non-numeric strings
    - Verify validation errors display
    - **Validates: Requirements 5.1, 5.2, 5.3**

  - [ ]* 5.6 Write property test for settlement info length
    - **Property 5: Settlement Info Length Validation**
    - Generate strings > 500 characters
    - Verify validation errors display
    - **Validates: Requirements 5.4**

- [x] 6. Integrate AccountInfo into Company module
  - [x] 6.1 Add AccountInfo tab to realestate component
    - Add new mat-tab for Account Info (مالیه)
    - Set disabled state based on company selection
    - Add icon and Dari label
    - _Requirements: 3.1, 3.2_

  - [x] 6.2 Register AccountInfo component in module
    - Add AccountInfo component to realestate.module.ts declarations
    - Import required modules (FormsModule, ReactiveFormsModule, etc.)
    - _Requirements: 3.1_

  - [ ]* 6.3 Write property test for date round-trip
    - **Property 6: Date Calendar Round-Trip**
    - Generate random dates
    - Test conversion across calendar systems
    - Verify equivalence after storage and retrieval
    - **Validates: Requirements 4.5**

- [x] 7. Checkpoint - Frontend complete
  - Ensure all frontend tests pass
  - Verify form works in create, edit, and view modes
  - Ask the user if questions arise

- [x] 8. Implement print integration
  - [x] 8.1 Update print template for Account Info
    - Add Account Info section to company print template
    - Display all fields with Dari labels
    - Format as professional table layout
    - Handle empty data with placeholder text
    - _Requirements: 6.1, 6.2, 6.4_

- [x] 9. Final checkpoint
  - Ensure all tests pass
  - Verify complete workflow: create, edit, view, print
  - Ask the user if questions arise

## Notes

- Tasks marked with `*` are optional and can be skipped for faster MVP
- Each task references specific requirements for traceability
- Checkpoints ensure incremental validation
- Property tests validate universal correctness properties
- Unit tests validate specific examples and edge cases
