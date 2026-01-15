-- =============================================
-- Document Verification Module - Initial Schema
-- Version: 1.0
-- Date: 2026-01-15
-- Description: Creates tables for document verification system
-- =============================================

-- Ensure org schema exists
CREATE SCHEMA IF NOT EXISTS org;

-- =============================================
-- Table: DocumentVerifications
-- Purpose: Stores verification codes and digital signatures for printed documents
-- =============================================
CREATE TABLE IF NOT EXISTS org."DocumentVerifications" (
    "Id" SERIAL PRIMARY KEY,
    "VerificationCode" VARCHAR(20) NOT NULL,
    "DocumentId" INTEGER NOT NULL,
    "DocumentType" VARCHAR(50) NOT NULL,
    "DigitalSignature" VARCHAR(128) NOT NULL,
    "DocumentSnapshot" JSONB,
    "CreatedAt" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    "IsRevoked" BOOLEAN DEFAULT FALSE,
    "RevokedAt" TIMESTAMP WITH TIME ZONE,
    "RevokedBy" VARCHAR(50),
    "RevokedReason" VARCHAR(500)
);

-- Unique index on VerificationCode for fast lookups and uniqueness constraint
CREATE UNIQUE INDEX IF NOT EXISTS "IX_DocumentVerifications_VerificationCode" 
ON org."DocumentVerifications"("VerificationCode");

-- Composite index for document lookup by ID and type
CREATE INDEX IF NOT EXISTS "IX_DocumentVerifications_DocumentId_DocumentType" 
ON org."DocumentVerifications"("DocumentId", "DocumentType");

-- Index for finding non-revoked verifications
CREATE INDEX IF NOT EXISTS "IX_DocumentVerifications_IsRevoked" 
ON org."DocumentVerifications"("IsRevoked") WHERE "IsRevoked" = FALSE;

-- =============================================
-- Table: VerificationLogs
-- Purpose: Audit trail for all verification attempts
-- =============================================
CREATE TABLE IF NOT EXISTS org."VerificationLogs" (
    "Id" SERIAL PRIMARY KEY,
    "VerificationCode" VARCHAR(20) NOT NULL,
    "AttemptedAt" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "IpAddress" VARCHAR(45),
    "WasSuccessful" BOOLEAN NOT NULL,
    "FailureReason" VARCHAR(50)
);

-- Index for verification logs by code and time (for statistics and suspicious activity detection)
CREATE INDEX IF NOT EXISTS "IX_VerificationLogs_VerificationCode_AttemptedAt" 
ON org."VerificationLogs"("VerificationCode", "AttemptedAt" DESC);

-- Index for finding failed attempts (for suspicious activity monitoring)
CREATE INDEX IF NOT EXISTS "IX_VerificationLogs_WasSuccessful" 
ON org."VerificationLogs"("WasSuccessful") WHERE "WasSuccessful" = FALSE;

-- =============================================
-- Comments for documentation
-- =============================================
COMMENT ON TABLE org."DocumentVerifications" IS 'Stores verification codes and digital signatures for printed documents';
COMMENT ON COLUMN org."DocumentVerifications"."VerificationCode" IS 'Unique code in format: PREFIX-YEAR-RANDOM (e.g., LIC-2026-A7X9K2)';
COMMENT ON COLUMN org."DocumentVerifications"."DocumentId" IS 'ID of the source document (e.g., LicenseDetail.Id)';
COMMENT ON COLUMN org."DocumentVerifications"."DocumentType" IS 'Type of document: RealEstateLicense, PetitionWriterLicense, Securities, etc.';
COMMENT ON COLUMN org."DocumentVerifications"."DigitalSignature" IS 'HMAC-SHA256 signature of key document fields';
COMMENT ON COLUMN org."DocumentVerifications"."DocumentSnapshot" IS 'JSON snapshot of document data at verification code generation time';

COMMENT ON TABLE org."VerificationLogs" IS 'Audit trail for all document verification attempts';
COMMENT ON COLUMN org."VerificationLogs"."VerificationCode" IS 'The verification code that was attempted';
COMMENT ON COLUMN org."VerificationLogs"."IpAddress" IS 'IP address of the requester (IPv4 or IPv6)';
COMMENT ON COLUMN org."VerificationLogs"."FailureReason" IS 'Reason for failure: NotFound, Revoked, Expired, SignatureMismatch';
