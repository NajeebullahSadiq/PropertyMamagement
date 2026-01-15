# Implementation Plan: Document Verification System

## Overview

This implementation plan covers the Document Verification System for PRMIS, starting with the Real Estate (رهنمای معاملات) module. The implementation follows an incremental approach, building core services first, then integrating with the existing print functionality.

## Tasks

- [x] 1. Create database schema and entities
  - [x] 1.1 Create DocumentVerification entity model
    - Create `Backend/Models/Verification/DocumentVerification.cs`
    - Include all fields: Id, VerificationCode, DocumentId, DocumentType, DigitalSignature, DocumentSnapshot, CreatedAt, CreatedBy, IsRevoked, RevokedAt, RevokedBy, RevokedReason
    - _Requirements: 7.1_

  - [x] 1.2 Create VerificationLog entity model
    - Create `Backend/Models/Verification/VerificationLog.cs`
    - Include fields: Id, VerificationCode, AttemptedAt, IpAddress, WasSuccessful, FailureReason
    - _Requirements: 5.1_

  - [x] 1.3 Create database migration script
    - Create SQL migration for DocumentVerifications and VerificationLogs tables
    - Add unique index on VerificationCode
    - Add composite index on (DocumentId, DocumentType)
    - _Requirements: 7.2, 7.3_

  - [x] 1.4 Register entities in AppDbContext
    - Add DbSet properties for DocumentVerification and VerificationLog
    - Configure entity relationships and indexes
    - _Requirements: 7.1_

- [x] 2. Implement core verification services
  - [x] 2.1 Create VerificationCodeGenerator service
    - Implement code generation in format `{PREFIX}-{YEAR}-{RANDOM}`
    - Implement format validation method
    - Use cryptographically secure random generation
    - _Requirements: 1.1, 1.2_

  - [x] 2.2 Write property test for verification code format
    - **Property 1: Verification Code Format Consistency**
    - **Validates: Requirements 1.1**

  - [x] 2.3 Write property test for verification code uniqueness
    - **Property 2: Verification Code Uniqueness**
    - **Validates: Requirements 1.2, 7.2**

  - [x] 2.4 Create SignatureService
    - Implement HMAC-SHA256 signature generation
    - Implement signature verification method
    - Use configuration-based secret key
    - _Requirements: 2.1, 2.3_

  - [x] 2.5 Write property test for signature round-trip
    - **Property 4: Digital Signature Round-Trip**
    - **Validates: Requirements 2.1, 2.3, 2.4**

  - [x] 2.6 Create VerificationService
    - Implement GetOrCreateVerificationAsync method
    - Implement VerifyDocumentAsync method
    - Implement RevokeVerificationAsync method
    - Implement GetVerificationStatsAsync method
    - _Requirements: 1.3, 1.4, 4.1, 4.2, 8.1_

  - [x] 2.7 Write property test for verification code idempotency
    - **Property 3: Verification Code Idempotency**
    - **Validates: Requirements 1.4**

- [x] 3. Checkpoint - Core services complete
  - Ensure all tests pass, ask the user if questions arise.

- [x] 4. Create verification API controller
  - [x] 4.1 Create VerificationController
    - Create `Backend/Controllers/Verification/VerificationController.cs`
    - Implement POST `/api/verification/generate` endpoint (authenticated)
    - Implement GET `/api/verification/verify/{code}` endpoint (public)
    - Implement POST `/api/verification/revoke` endpoint (admin only)
    - Implement GET `/api/verification/stats/{code}` endpoint (authenticated)
    - _Requirements: 4.3, 5.2, 8.3_

  - [x] 4.2 Create DTOs for verification API
    - Create GenerateVerificationRequest DTO
    - Create VerificationResultDto
    - Create RevokeVerificationRequest DTO
    - Create VerificationStatsDto
    - _Requirements: 4.1_

  - [x] 4.3 Write property tests for verification endpoint responses
    - **Property 7: Valid Code Verification Returns Document Data**
    - **Property 8: Invalid Code Verification Returns Error**
    - **Validates: Requirements 4.1, 4.2**

  - [x] 4.4 Write property test for expired document status
    - **Property 9: Expired Document Status**
    - **Validates: Requirements 4.5**

  - [x] 4.5 Write property test for revoked document status
    - **Property 13: Revoked Document Verification Status**
    - **Validates: Requirements 8.2**

- [x] 5. Implement audit logging
  - [x] 5.1 Add verification logging to VerificationService
    - Log all verification attempts with timestamp, code, IP, result
    - _Requirements: 5.1_

  - [x] 5.2 Write property test for audit logging
    - **Property 10: Verification Audit Logging**
    - **Validates: Requirements 5.1**

- [x] 6. Checkpoint - Backend complete
  - Ensure all tests pass, ask the user if questions arise.

- [x] 7. Create frontend verification service
  - [x] 7.1 Create verification service in Angular
    - Create `Frontend/src/app/shared/verification.service.ts`
    - Implement generateVerificationCode method
    - Implement verifyDocument method
    - _Requirements: 6.1_

  - [x] 7.2 Install QR code generation library
    - Add `qrcode` npm package to Frontend
    - Create QrCodeService for generating QR codes
    - _Requirements: 3.1_

  - [x] 7.3 Write property test for QR code URL encoding
    - **Property 6: QR Code URL Encoding**
    - **Validates: Requirements 3.1, 3.2**

- [x] 8. Create public verification portal
  - [x] 8.1 Create verification portal component
    - Create `Frontend/src/app/verify/verify.component.ts`
    - Create `Frontend/src/app/verify/verify.component.html`
    - Display verification form with code input
    - Display verification results with status indicator
    - _Requirements: 4.1, 4.4_

  - [x] 8.2 Create verification portal module and routing
    - Create `Frontend/src/app/verify/verify.module.ts`
    - Add route `/verify/:code` (public, no auth guard)
    - _Requirements: 4.3_

  - [x] 8.3 Style verification portal
    - Create responsive design for mobile scanning
    - Add visual status indicators (green/red/yellow)
    - Support Dari/Pashto language
    - _Requirements: 4.4, 4.5_

- [x] 9. Integrate with print-license component
  - [x] 9.1 Update print-license component to fetch verification code
    - Call verification service on component init
    - Store verification code and URL in component
    - _Requirements: 6.1_

  - [x] 9.2 Add QR code to print-license template
    - Generate QR code from verification URL
    - Position QR code in bottom-right corner
    - Add human-readable verification code below QR
    - _Requirements: 3.3, 3.4, 6.2, 6.3_

  - [x] 9.3 Add error handling for verification failures
    - Display error message if verification code generation fails
    - Prevent printing if verification is unavailable
    - _Requirements: 6.4_

- [x] 10. Checkpoint - Integration complete
  - Ensure all tests pass, ask the user if questions arise.

- [x] 11. Add configuration and documentation
  - [x] 11.1 Add verification configuration to appsettings
    - Add SignatureSecretKey configuration
    - Add VerificationBaseUrl configuration
    - Add DocumentTypePrefix mappings
    - _Requirements: 2.1_

  - [x] 11.2 Write property test for document snapshot serialization
    - **Property 11: Document Snapshot JSON Round-Trip**
    - **Validates: Requirements 7.4**

  - [x] 11.3 Write property test for data persistence
    - **Property 5: Verification Data Persistence**
    - **Validates: Requirements 1.3, 2.2**

  - [x] 11.4 Write property test for revocation state
    - **Property 12: Revocation State Persistence**
    - **Validates: Requirements 8.1**

- [x] 12. Final checkpoint - All features complete
  - Ensure all tests pass, ask the user if questions arise.

## Notes

- All tasks are required for comprehensive implementation
- Each task references specific requirements for traceability
- Checkpoints ensure incremental validation
- Property tests validate universal correctness properties
- Unit tests validate specific examples and edge cases
- The implementation uses C# for backend and TypeScript/Angular for frontend
- FsCheck library will be used for property-based testing in C#
