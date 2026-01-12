# Implementation Plan: Dari/Pashto Number Input Support

## Overview

This plan implements support for Eastern Arabic numerals (۰۱۲۳۴۵۶۷۸۹) in all numeric input fields. The implementation creates a shared directive and service, then updates all existing numeric input fields to use the new approach.

## Tasks

- [-] 1. Create NumeralService for numeral conversion
  - Create `Frontend/src/app/shared/numeral.service.ts`
  - Implement `toWesternArabic()` method with character mapping
  - Implement `toEasternArabic()` method for reverse conversion
  - Implement `parseNumber()` method for parsing mixed numeral strings
  - Implement `isNumeral()` helper method
  - _Requirements: 2.1, 2.3, 5.2_

- [ ] 1.1 Write property test for numeral normalization
  - **Property 3: Numeral Normalization Preserves Value**
  - **Validates: Requirements 2.1, 2.3, 5.2**

- [-] 2. Create NumericInputDirective for input validation
  - Create `Frontend/src/app/shared/directives/numeric-input.directive.ts`
  - Implement keypress handler to validate Eastern and Western Arabic numerals
  - Implement paste handler to validate pasted content
  - Add `allowDecimal` input property for decimal support
  - Add `allowNegative` input property for negative number support
  - _Requirements: 1.1, 1.2, 1.3, 1.4, 5.1_

- [ ] 2.1 Write property test for valid numeral acceptance
  - **Property 1: Valid Numeral Acceptance**
  - **Validates: Requirements 1.1, 1.2, 5.1**

- [ ] 2.2 Write property test for invalid character rejection
  - **Property 2: Invalid Character Rejection**
  - **Validates: Requirements 1.3**

- [-] 3. Export directive and service from SharedModule
  - Add NumeralService to SharedModule providers
  - Add NumericInputDirective to SharedModule declarations and exports
  - _Requirements: 4.1, 4.3_

- [ ] 4. Checkpoint - Ensure all tests pass
  - Ensure all tests pass, ask the user if questions arise.

- [-] 5. Update property details component
  - Replace `type="number"` with `type="text"` and add `appNumericInput` directive
  - Remove `onlyNumberKey` function from `propertydetails.component.ts`
  - Update template inputs: parea, numofFloor, numofRooms
  - _Requirements: 3.1, 3.2, 4.2_

- [x] 6. Update property buyer detail component
  - Replace `type="number"` with `type="text"` and add `appNumericInput` directive
  - Remove `onlyNumberKey` function from `buyerdetail.component.ts`
  - Update template inputs: price, halfPrice, royaltyAmount
  - Add `[allowDecimal]="true"` for price fields
  - _Requirements: 3.1, 3.2, 4.2_

- [x] 7. Update property seller detail component
  - Replace `type="number"` with `type="text"` and add `appNumericInput` directive
  - Remove `onlyNumberKey` function from `sellerdetail.component.ts`
  - _Requirements: 3.1, 3.2, 4.2_

- [x] 8. Update property witness detail component
  - Replace `type="number"` with `type="text"` and add `appNumericInput` directive
  - Remove `onlyNumberKey` function from `witnessdetail.component.ts`
  - _Requirements: 3.1, 3.2, 4.2_

- [-] 9. Update vehicle submit component
  - Replace `type="number"` with `type="text"` and add `appNumericInput` directive
  - Remove `onlyNumberKey` function from `vehicle-submit.component.ts`
  - Update template inputs: pilateNo, enginNo, shasiNo, price
  - Add `[allowDecimal]="true"` for price field
  - _Requirements: 3.1, 3.2, 4.2_

- [x] 10. Update vehicle buyer detail component
  - Replace `type="number"` with `type="text"` and add `appNumericInput` directive
  - Remove `onlyNumberKey` function from `buyerdetail.component.ts`
  - _Requirements: 3.1, 3.2, 4.2_

- [x] 11. Update vehicle seller detail component
  - Replace `type="number"` with `type="text"` and add `appNumericInput` directive
  - Remove `onlyNumberKey` function from `sellerdetail.component.ts`
  - _Requirements: 3.1, 3.2, 4.2_

- [x] 12. Update vehicle witness detail component
  - Replace `type="number"` with `type="text"` and add `appNumericInput` directive
  - Remove `onlyNumberKey` function from `witnessdetail.component.ts`
  - _Requirements: 3.1, 3.2, 4.2_

- [-] 13. Update license details component
  - Replace `type="number"` with `type="text"` and add `appNumericInput` directive
  - Update template inputs: royaltyAmount, penaltyAmount
  - Add `[allowDecimal]="true"` for amount fields
  - _Requirements: 3.1, 3.2_

- [-] 14. Update account info component
  - Replace `type="number"` with `type="text"` and add `appNumericInput` directive
  - Update template inputs: taxPaymentAmount, settlementYear, transactionCount, companyCommission
  - Add `[allowDecimal]="true"` for amount fields
  - _Requirements: 3.1, 3.2_

- [ ] 15. Normalize numeric values in form submissions
  - Update form submission handlers to use NumeralService.toWesternArabic()
  - Apply to property details, vehicle, and company forms
  - _Requirements: 2.1_

- [ ] 16. Final checkpoint - Ensure all tests pass
  - Ensure all tests pass, ask the user if questions arise.

## Notes

- All tasks are required for comprehensive implementation
- Each task references specific requirements for traceability
- The directive approach allows gradual migration without breaking existing functionality
- Property tests validate universal correctness properties
- Unit tests validate specific examples and edge cases
