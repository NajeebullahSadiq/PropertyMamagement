# Implementation Plan: Guarantee Type Logic Enhancement

## Overview

This implementation plan covers the enhancement of the Guarantors section with dynamic conditional fields based on guarantee type selection. The implementation follows an incremental approach, starting with database changes, then backend logic, and finally frontend UI updates.

## Tasks

- [x] 1. Database Schema Extension
  - [x] 1.1 Create migration to add new columns to Guarantors table
    - Add CourtName (varchar 255, nullable) for Sharia Deed
    - Add CollateralNumber (varchar 100, nullable) for Sharia Deed
    - Add SetSerialNumber (varchar 100, nullable) for Customary Deed
    - Add GuaranteeDistrictId (int, nullable, FK to Location) for Customary Deed
    - Add BankName (varchar 255, nullable) for Cash
    - Add DepositNumber (varchar 100, nullable) for Cash
    - Add DepositDate (date, nullable) for Cash
    - Add foreign key constraint for GuaranteeDistrictId
    - _Requirements: 6.1, 6.2, 6.3, 6.4, 6.5, 6.6, 6.7, 6.8_

  - [x] 1.2 Create migration to update GuaranteeType lookup table
    - Delete all existing guarantee types
    - Insert exactly 3 types: Cash (1), ShariaDeed (2), CustomaryDeed (3)
    - _Requirements: 1.1, 1.3_

- [x] 2. Backend Model Updates
  - [x] 2.1 Update Guarantor entity model
    - Add new properties for all conditional fields
    - Add navigation property for GuaranteeDistrict
    - _Requirements: 6.1, 6.2, 6.3, 6.4, 6.5, 6.6, 6.7_

  - [x] 2.2 Update GuarantorData request model
    - Add new properties matching entity model
    - Add DepositDate as string for calendar conversion
    - _Requirements: 6.1, 6.2, 6.3, 6.4, 6.5, 6.6, 6.7_

  - [x] 2.3 Update AppDbContext with new entity configuration
    - Configure GuaranteeDistrict navigation property
    - Add index for GuaranteeDistrictId
    - _Requirements: 6.4_

- [x] 3. Backend Validation Logic
  - [x] 3.1 Implement guarantee type validation in GuaranatorController
    - Create ValidateGuaranteeFields method
    - Validate guarantee type is 1, 2, or 3
    - Validate required fields based on selected type
    - Return appropriate error messages
    - _Requirements: 1.4, 5.1, 5.2, 5.3, 8.1_

  - [ ]* 3.2 Write property test for guarantee type validation
    - **Property 1: Guarantee Type Validation**
    - **Validates: Requirements 1.4, 8.1**

  - [ ]* 3.3 Write property test for Sharia Deed field validation
    - **Property 2: Sharia Deed Field Validation**
    - **Validates: Requirements 2.3, 5.1**

  - [ ]* 3.4 Write property test for Customary Deed field validation
    - **Property 3: Customary Deed Field Validation**
    - **Validates: Requirements 3.3, 5.2**

  - [ ]* 3.5 Write property test for Cash field validation
    - **Property 4: Cash Field Validation**
    - **Validates: Requirements 4.4, 5.3**

- [x] 4. Backend CRUD Operations Update
  - [x] 4.1 Update POST endpoint to handle new fields
    - Parse DepositDate using multi-calendar helper
    - Store only relevant fields based on guarantee type
    - Clear irrelevant fields before saving
    - _Requirements: 5.4, 5.5_

  - [x] 4.2 Update PUT endpoint to handle new fields
    - Parse DepositDate using multi-calendar helper
    - Update only relevant fields based on guarantee type
    - Clear irrelevant fields on type change
    - _Requirements: 5.4, 5.5_

  - [x] 4.3 Update GET endpoint to return new fields
    - Convert DepositDate to requested calendar type
    - Return all fields for frontend to display conditionally
    - _Requirements: 7.1, 8.5_

  - [ ]* 4.4 Write property test for relevant fields storage
    - **Property 5: Relevant Fields Storage**
    - **Validates: Requirements 5.4, 5.5**

- [x] 5. Checkpoint - Backend Complete
  - Ensure all backend tests pass
  - Verify database migration applies correctly
  - Ask the user if questions arise

- [x] 6. Frontend Model Updates
  - [x] 6.1 Update Guarantor TypeScript interface
    - Add new properties for all conditional fields
    - _Requirements: 6.1, 6.2, 6.3, 6.4, 6.5, 6.6, 6.7_

  - [x] 6.2 Update LocalizationService with new guarantee types
    - Replace existing guaranteeTypes array with 3 new types
    - Use Dari labels: پول نقد, قباله شرعی, قباله عرفی
    - _Requirements: 1.1, 1.2_

- [-] 7. Frontend Form Logic
  - [x] 7.1 Add new form controls to guaranatorForm
    - Add courtName, collateralNumber for Sharia Deed
    - Add setSerialNumber, guaranteeDistrictId for Customary Deed
    - Add bankName, depositNumber, depositDate for Cash
    - _Requirements: 2.1, 2.2, 3.1, 3.2, 4.1, 4.2, 4.3_

  - [x] 7.2 Implement onGuaranteeTypeChange method
    - Clear all conditional fields when type changes
    - Update field visibility flags
    - Update validators dynamically
    - _Requirements: 2.4, 2.5, 3.4, 3.5, 4.5, 4.6_

  - [x] 7.3 Implement conditional validation logic
    - Add validators only for visible fields
    - Remove validators when fields are hidden
    - _Requirements: 8.3_

  - [ ]* 7.4 Write property test for conditional validation scope
    - **Property 8: Conditional Validation Scope**
    - **Validates: Requirements 8.3**

- [x] 8. Frontend UI Implementation
  - [x] 8.1 Update guarantee type dropdown in template
    - Use localizedGuaranteeTypes with 3 options only
    - Add (change) handler for onGuaranteeTypeChange
    - Make guarantee type required
    - _Requirements: 1.1, 1.2, 8.1, 8.2_

  - [x] 8.2 Add Sharia Deed conditional fields section
    - Add Court Name input field with Dari label
    - Add Collateral Number input field with Dari label
    - Use *ngIf to show only when ShariaDeed selected
    - _Requirements: 2.1, 2.2_

  - [x] 8.3 Add Customary Deed conditional fields section
    - Add Set Serial Number input field with Dari label
    - Add District dropdown using ng-select
    - Use *ngIf to show only when CustomaryDeed selected
    - _Requirements: 3.1, 3.2_

  - [x] 8.4 Add Cash conditional fields section
    - Add Bank Name input field with Dari label
    - Add Deposit Number input field with Dari label
    - Add Deposit Date using multi-calendar-datepicker
    - Use *ngIf to show only when Cash selected
    - _Requirements: 4.1, 4.2, 4.3_

  - [x] 8.5 Add validation error messages in Dari
    - Add error messages for all required conditional fields
    - Display only when field is invalid and touched
    - _Requirements: 8.2_

- [x] 9. Frontend Data Binding
  - [x] 9.1 Update BindValu method for edit mode
    - Load and display correct conditional fields
    - Populate field values from existing record
    - Trigger onGuaranteeTypeChange to set visibility
    - _Requirements: 8.5_

  - [x] 9.2 Update addData method
    - Include new fields in request payload
    - Format DepositDate for backend
    - _Requirements: 5.4_

  - [x] 9.3 Update updateData method
    - Include new fields in request payload
    - Format DepositDate for backend
    - _Requirements: 5.4_

  - [ ]* 9.4 Write property test for edit mode field loading
    - **Property 7: Edit Mode Field Loading**
    - **Validates: Requirements 8.5**

- [x] 10. Checkpoint - Frontend Complete
  - Ensure all frontend tests pass
  - Verify form behavior for all guarantee types
  - Ask the user if questions arise

- [x] 11. View and Print Updates
  - [x] 11.1 Update guarantors list table to show guarantee type
    - Display localized guarantee type name
    - _Requirements: 7.2_

  - [x] 11.2 Update print template for conditional fields
    - Show only relevant fields based on guarantee type
    - Hide unused conditional fields completely
    - Display guarantee type label in Dari
    - _Requirements: 7.2, 7.3, 7.4_
    - Note: No separate print template exists for guarantors. The form itself serves as view mode with conditional field visibility.

  - [ ]* 11.3 Write property test for view/print field visibility
    - **Property 6: View/Print Field Visibility**
    - **Validates: Requirements 7.1, 7.3, 7.4**

- [x] 12. Final Checkpoint
  - Ensure all tests pass
  - Verify end-to-end flow for all guarantee types
  - Ask the user if questions arise

## Notes

- Tasks marked with `*` are optional and can be skipped for faster MVP
- Each task references specific requirements for traceability
- Checkpoints ensure incremental validation
- Property tests validate universal correctness properties
- Unit tests validate specific examples and edge cases
- The implementation uses existing patterns from the codebase (multi-calendar dates, ng-select dropdowns, Tailwind styling)
