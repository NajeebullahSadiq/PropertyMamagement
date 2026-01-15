# Requirements Document

## Introduction

This document defines the requirements for implementing a Document Verification System for the Property Registration MIS (PRMIS). The system will generate unique verification codes and QR codes for printed licenses and documents to prevent fraud and enable public verification. The initial implementation will focus on the Real Estate (رهنمای معاملات) module licenses, with the architecture designed to support other document types in the future.

## Glossary

- **Verification_Code**: A unique alphanumeric identifier (e.g., `LIC-2026-A7X9K2`) assigned to each printed document for verification purposes
- **QR_Code**: A two-dimensional barcode containing the verification URL and document metadata
- **Digital_Signature**: A cryptographic hash generated from document data using HMAC-SHA256 to ensure authenticity
- **Verification_Portal**: A public-facing web page where anyone can verify document authenticity by scanning QR or entering verification code
- **Document_Type**: The category of document being verified (e.g., RealEstateLicense, PetitionWriterLicense, Securities)
- **Verification_Service**: Backend service responsible for generating and validating verification codes and signatures
- **Print_Component**: Angular component that renders the printable document with embedded QR code

## Requirements

### Requirement 1: Verification Code Generation

**User Story:** As a system administrator, I want each printed license to have a unique verification code, so that documents can be individually identified and verified.

#### Acceptance Criteria

1. WHEN a license is printed for the first time, THE Verification_Service SHALL generate a unique Verification_Code in the format `{PREFIX}-{YEAR}-{RANDOM}` where PREFIX is document type specific, YEAR is 4-digit year, and RANDOM is 6 alphanumeric characters
2. WHEN generating a Verification_Code, THE Verification_Service SHALL ensure no duplicate codes exist in the database
3. WHEN a Verification_Code is generated, THE Verification_Service SHALL store it with the associated document ID, document type, generation timestamp, and issuing user
4. IF a document already has a Verification_Code, THEN THE Verification_Service SHALL return the existing code instead of generating a new one

### Requirement 2: Digital Signature Generation

**User Story:** As a security officer, I want documents to have a cryptographic signature, so that any tampering with document data can be detected.

#### Acceptance Criteria

1. WHEN generating a Digital_Signature, THE Verification_Service SHALL create an HMAC-SHA256 hash using document key fields (license number, issue date, expire date, owner name) and a server-side secret key
2. THE Digital_Signature SHALL be stored alongside the Verification_Code in the database
3. WHEN verifying a document, THE Verification_Service SHALL regenerate the signature from current document data and compare with stored signature
4. IF the regenerated signature does not match the stored signature, THEN THE Verification_Service SHALL flag the document as potentially tampered

### Requirement 3: QR Code Generation

**User Story:** As a document holder, I want a QR code on my printed license, so that anyone can quickly scan and verify its authenticity.

#### Acceptance Criteria

1. WHEN rendering a printable document, THE Print_Component SHALL generate a QR_Code containing the verification URL
2. THE QR_Code SHALL encode a URL in the format `{BASE_URL}/verify/{VERIFICATION_CODE}`
3. THE QR_Code SHALL be positioned in a consistent location on the document (bottom-right corner)
4. THE QR_Code SHALL be sized appropriately for scanning (minimum 2cm x 2cm at print resolution)

### Requirement 4: Public Verification Portal

**User Story:** As a member of the public, I want to verify a license by scanning its QR code or entering its verification code, so that I can confirm the document is genuine.

#### Acceptance Criteria

1. WHEN a user accesses the Verification_Portal with a valid Verification_Code, THE System SHALL display the original document details including: license number, holder name, issue date, expiry date, and holder photo
2. WHEN a user accesses the Verification_Portal with an invalid Verification_Code, THE System SHALL display a clear error message indicating the document could not be found
3. THE Verification_Portal SHALL be accessible without authentication (public access)
4. WHEN displaying verification results, THE System SHALL show a clear visual indicator of verification status (valid/invalid/expired)
5. WHEN a document's expiry date has passed, THE Verification_Portal SHALL display the document as "Expired" with appropriate visual styling

### Requirement 5: Verification Audit Trail

**User Story:** As a system administrator, I want to track all verification attempts, so that I can monitor for suspicious activity and generate reports.

#### Acceptance Criteria

1. WHEN a verification attempt is made, THE System SHALL log the timestamp, Verification_Code, IP address, and verification result
2. THE System SHALL provide an API endpoint to retrieve verification statistics for a specific document
3. WHEN multiple failed verification attempts occur for the same code within a short period, THE System SHALL flag this as suspicious activity

### Requirement 6: Integration with Real Estate License Print

**User Story:** As a staff member, I want the existing license print functionality to automatically include verification features, so that all printed licenses are verifiable.

#### Acceptance Criteria

1. WHEN the print-license component loads, THE System SHALL request or retrieve the Verification_Code for the document
2. THE Print_Component SHALL display the Verification_Code in human-readable format on the printed document
3. THE Print_Component SHALL display the QR_Code on the printed document
4. IF Verification_Code generation fails, THEN THE Print_Component SHALL display an error and prevent printing

### Requirement 7: Verification Data Model

**User Story:** As a developer, I want a well-structured data model for verification records, so that the system can efficiently store and retrieve verification data.

#### Acceptance Criteria

1. THE Database SHALL store verification records with fields: Id, VerificationCode, DocumentId, DocumentType, DigitalSignature, CreatedAt, CreatedBy, IsRevoked, RevokedAt, RevokedBy, RevokedReason
2. THE Database SHALL have a unique index on VerificationCode
3. THE Database SHALL have a composite index on (DocumentId, DocumentType) for efficient lookups
4. WHEN storing verification records, THE System SHALL use JSON serialization for the document snapshot data

### Requirement 8: Document Revocation

**User Story:** As an administrator, I want to revoke a verification code if a license is cancelled, so that revoked documents show as invalid when verified.

#### Acceptance Criteria

1. WHEN an administrator revokes a Verification_Code, THE System SHALL mark the record as revoked with timestamp and reason
2. WHEN a revoked document is verified, THE Verification_Portal SHALL display "Document Revoked" status with the revocation reason
3. THE System SHALL provide an API endpoint for administrators to revoke verification codes
