-- Comprehensive Audit Log System Migration
-- Creates audit schema and table for tracking all system activities

-- Create audit schema if not exists
CREATE SCHEMA IF NOT EXISTS audit;

-- Create comprehensive audit logs table
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

-- Create indexes for efficient querying
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

CREATE INDEX IF NOT EXISTS "IX_ComprehensiveAuditLogs_UserProvince" 
    ON audit."ComprehensiveAuditLogs"("UserProvince");

-- Create composite index for common queries
CREATE INDEX IF NOT EXISTS "IX_ComprehensiveAuditLogs_Module_ActionType_Timestamp" 
    ON audit."ComprehensiveAuditLogs"("Module", "ActionType", "Timestamp" DESC);

-- Add comments to columns for documentation
COMMENT ON TABLE audit."ComprehensiveAuditLogs" IS 'Comprehensive audit log tracking all system activities';
COMMENT ON COLUMN audit."ComprehensiveAuditLogs"."UserId" IS 'ID of user who performed the action';
COMMENT ON COLUMN audit."ComprehensiveAuditLogs"."ActionType" IS 'Type of action: Create, Update, Delete, Login, Logout, etc.';
COMMENT ON COLUMN audit."ComprehensiveAuditLogs"."Module" IS 'System module: Property, Vehicle, User, License, etc.';
COMMENT ON COLUMN audit."ComprehensiveAuditLogs"."EntityType" IS 'Specific entity type affected';
COMMENT ON COLUMN audit."ComprehensiveAuditLogs"."EntityId" IS 'ID of the affected entity';
COMMENT ON COLUMN audit."ComprehensiveAuditLogs"."Description" IS 'Human-readable description in English';
COMMENT ON COLUMN audit."ComprehensiveAuditLogs"."DescriptionDari" IS 'Human-readable description in Dari';
COMMENT ON COLUMN audit."ComprehensiveAuditLogs"."OldValues" IS 'Previous values before change (JSON)';
COMMENT ON COLUMN audit."ComprehensiveAuditLogs"."NewValues" IS 'New values after change (JSON)';
COMMENT ON COLUMN audit."ComprehensiveAuditLogs"."Status" IS 'Status: Success, Failed, Error';
COMMENT ON COLUMN audit."ComprehensiveAuditLogs"."Metadata" IS 'Additional metadata (JSON)';

-- Create a view for quick statistics
CREATE OR REPLACE VIEW audit."AuditStatistics" AS
SELECT 
    DATE("Timestamp") as "Date",
    "Module",
    "ActionType",
    COUNT(*) as "TotalCount",
    COUNT(*) FILTER (WHERE "Status" = 'Success') as "SuccessCount",
    COUNT(*) FILTER (WHERE "Status" IN ('Failed', 'Error')) as "FailedCount"
FROM audit."ComprehensiveAuditLogs"
GROUP BY DATE("Timestamp"), "Module", "ActionType";

-- Create a function to clean old audit logs (optional - run manually or schedule)
CREATE OR REPLACE FUNCTION audit.CleanOldAuditLogs(daysToKeep INTEGER DEFAULT 90)
RETURNS INTEGER AS $$
DECLARE
    deletedCount INTEGER;
BEGIN
    DELETE FROM audit."ComprehensiveAuditLogs"
    WHERE "Timestamp" < NOW() - INTERVAL '1 day' * daysToKeep;
    
    GET DIAGNOSTICS deletedCount = ROW_COUNT;
    RETURN deletedCount;
END;
$$ LANGUAGE plpgsql;

-- Grant permissions (adjust as needed for your security setup)
-- GRANT ALL ON SCHEMA audit TO your_app_user;
-- GRANT ALL ON ALL TABLES IN SCHEMA audit TO your_app_user;
-- GRANT ALL ON ALL SEQUENCES IN SCHEMA audit TO your_app_user;
