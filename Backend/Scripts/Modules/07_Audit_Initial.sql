-- =====================================================
-- Module: Audit/Log Tables
-- Schema: log
-- Dependencies: All other modules
-- Execution Order: 7
-- =====================================================

-- Create schema if not exists
CREATE SCHEMA IF NOT EXISTS log;
CREATE SCHEMA IF NOT EXISTS audit;

-- Comprehensive audit logs used by /audit-log
CREATE TABLE IF NOT EXISTS audit."ComprehensiveAuditLogs" (
    "Id" BIGSERIAL PRIMARY KEY,
    "UserId" VARCHAR(450) NOT NULL,
    "UserName" VARCHAR(200),
    "UserRole" VARCHAR(100),
    "ActionType" VARCHAR(50) NOT NULL,
    "Module" VARCHAR(100) NOT NULL,
    "EntityType" VARCHAR(100),
    "EntityId" VARCHAR(100),
    "Description" VARCHAR(1000) NOT NULL,
    "DescriptionDari" VARCHAR(1000),
    "OldValues" TEXT,
    "NewValues" TEXT,
    "IpAddress" VARCHAR(45),
    "UserAgent" VARCHAR(500),
    "RequestUrl" VARCHAR(500),
    "HttpMethod" VARCHAR(10),
    "Status" VARCHAR(50) NOT NULL DEFAULT 'Success',
    "ErrorMessage" VARCHAR(1000),
    "Metadata" TEXT,
    "UserProvince" VARCHAR(100),
    "Timestamp" TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
    "DurationMs" BIGINT
);

CREATE INDEX IF NOT EXISTS "IX_ComprehensiveAuditLogs_UserId"
    ON audit."ComprehensiveAuditLogs"("UserId");
CREATE INDEX IF NOT EXISTS "IX_ComprehensiveAuditLogs_ActionType"
    ON audit."ComprehensiveAuditLogs"("ActionType");
CREATE INDEX IF NOT EXISTS "IX_ComprehensiveAuditLogs_Module"
    ON audit."ComprehensiveAuditLogs"("Module");
CREATE INDEX IF NOT EXISTS "IX_ComprehensiveAuditLogs_Timestamp"
    ON audit."ComprehensiveAuditLogs"("Timestamp" DESC);
CREATE INDEX IF NOT EXISTS "IX_ComprehensiveAuditLogs_Status"
    ON audit."ComprehensiveAuditLogs"("Status");
CREATE INDEX IF NOT EXISTS "IX_ComprehensiveAuditLogs_EntityType_EntityId"
    ON audit."ComprehensiveAuditLogs"("EntityType", "EntityId");
CREATE INDEX IF NOT EXISTS "IX_ComprehensiveAuditLogs_EntityType_EntityId_ActionType"
    ON audit."ComprehensiveAuditLogs"("EntityType", "EntityId", "ActionType");
CREATE INDEX IF NOT EXISTS "IX_ComprehensiveAuditLogs_UserProvince"
    ON audit."ComprehensiveAuditLogs"("UserProvince");
CREATE INDEX IF NOT EXISTS "IX_ComprehensiveAuditLogs_Module_ActionType_Timestamp"
    ON audit."ComprehensiveAuditLogs"("Module", "ActionType", "Timestamp" DESC);

CREATE OR REPLACE VIEW audit."AuditStatistics" AS
SELECT
    DATE("Timestamp") AS "Date",
    "Module",
    "ActionType",
    COUNT(*) AS "TotalCount",
    COUNT(*) FILTER (WHERE "Status" = 'Success') AS "SuccessCount",
    COUNT(*) FILTER (WHERE "Status" IN ('Failed', 'Error')) AS "FailedCount"
FROM audit."ComprehensiveAuditLogs"
GROUP BY DATE("Timestamp"), "Module", "ActionType";

-- Propertyaudit
CREATE TABLE IF NOT EXISTS log."Propertyaudit" (
    "Id" SERIAL PRIMARY KEY,
    "PropertyDetailsId" INTEGER,
    "Action" TEXT,
    "OldValue" TEXT,
    "NewValue" TEXT,
    "ChangedBy" TEXT,
    "ChangedAt" TIMESTAMP WITHOUT TIME ZONE
);

-- Propertybuyeraudit
CREATE TABLE IF NOT EXISTS log."Propertybuyeraudit" (
    "Id" SERIAL PRIMARY KEY,
    "BuyerDetailsId" INTEGER,
    "Action" TEXT,
    "OldValue" TEXT,
    "NewValue" TEXT,
    "ChangedBy" TEXT,
    "ChangedAt" TIMESTAMP WITHOUT TIME ZONE
);

-- Propertyselleraudit
CREATE TABLE IF NOT EXISTS log."Propertyselleraudit" (
    "Id" SERIAL PRIMARY KEY,
    "SellerDetailsId" INTEGER,
    "Action" TEXT,
    "OldValue" TEXT,
    "NewValue" TEXT,
    "ChangedBy" TEXT,
    "ChangedAt" TIMESTAMP WITHOUT TIME ZONE
);

-- Vehicleaudit
CREATE TABLE IF NOT EXISTS log."Vehicleaudit" (
    "Id" SERIAL PRIMARY KEY,
    "VehiclesPropertyDetailsId" INTEGER,
    "Action" TEXT,
    "OldValue" TEXT,
    "NewValue" TEXT,
    "ChangedBy" TEXT,
    "ChangedAt" TIMESTAMP WITHOUT TIME ZONE
);

-- Vehicleselleraudit
CREATE TABLE IF NOT EXISTS log."Vehicleselleraudit" (
    "Id" SERIAL PRIMARY KEY,
    "VehiclesSellerDetailsId" INTEGER,
    "Action" TEXT,
    "OldValue" TEXT,
    "NewValue" TEXT,
    "ChangedBy" TEXT,
    "ChangedAt" TIMESTAMP WITHOUT TIME ZONE
);

-- Vehiclebuyeraudit
CREATE TABLE IF NOT EXISTS log."Vehiclebuyeraudit" (
    "Id" SERIAL PRIMARY KEY,
    "VehiclesBuyerDetailsId" INTEGER,
    "Action" TEXT,
    "OldValue" TEXT,
    "NewValue" TEXT,
    "ChangedBy" TEXT,
    "ChangedAt" TIMESTAMP WITHOUT TIME ZONE
);

-- Licenseaudit
CREATE TABLE IF NOT EXISTS log."licenseaudit" (
    "Id" SERIAL PRIMARY KEY,
    "LicenseId" INTEGER NOT NULL,
    "PropertyName" TEXT,
    "OldValue" TEXT,
    "NewValue" TEXT,
    "UpdatedBy" TEXT,
    "UpdatedAt" TIMESTAMP WITHOUT TIME ZONE,
    CONSTRAINT "FK_licenseaudit_LicenseDetails" FOREIGN KEY ("LicenseId") 
        REFERENCES company."LicenseDetails"("Id") ON DELETE CASCADE
);

-- Guarantorsaudit
CREATE TABLE IF NOT EXISTS log."Guarantorsaudit" (
    "Id" SERIAL PRIMARY KEY,
    "GuarantorsId" INTEGER,
    "Action" TEXT,
    "OldValue" TEXT,
    "NewValue" TEXT,
    "ChangedBy" TEXT,
    "ChangedAt" TIMESTAMP WITHOUT TIME ZONE
);

-- Graunteeaudit
CREATE TABLE IF NOT EXISTS log."Graunteeaudit" (
    "Id" SERIAL PRIMARY KEY,
    "GauranteeId" INTEGER,
    "Action" TEXT,
    "OldValue" TEXT,
    "NewValue" TEXT,
    "ChangedBy" TEXT,
    "ChangedAt" TIMESTAMP WITHOUT TIME ZONE
);

-- Companyowneraudit
CREATE TABLE IF NOT EXISTS log."Companyowneraudit" (
    "Id" SERIAL PRIMARY KEY,
    "CompanyOwnerId" INTEGER,
    "Action" TEXT,
    "OldValue" TEXT,
    "NewValue" TEXT,
    "ChangedBy" TEXT,
    "ChangedAt" TIMESTAMP WITHOUT TIME ZONE
);

-- Companydetailsaudit
CREATE TABLE IF NOT EXISTS log."Companydetailsaudit" (
    "Id" SERIAL PRIMARY KEY,
    "CompanyDetailsId" INTEGER,
    "Action" TEXT,
    "OldValue" TEXT,
    "NewValue" TEXT,
    "ChangedBy" TEXT,
    "ChangedAt" TIMESTAMP WITHOUT TIME ZONE
);

-- =====================================================
-- End of Audit Module
-- =====================================================
