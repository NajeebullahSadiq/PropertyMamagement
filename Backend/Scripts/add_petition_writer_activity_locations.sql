-- Migration: Add PetitionWriterActivityLocations table
-- This table stores admin-configurable activity locations for petition writer licenses

-- Create the table in the org schema
CREATE TABLE IF NOT EXISTS org."PetitionWriterActivityLocations" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(500) NOT NULL,
    "DariName" VARCHAR(500) NOT NULL,
    "IsActive" boolean NOT NULL DEFAULT true,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" VARCHAR(50),
    "UpdatedAt" timestamp without time zone,
    "UpdatedBy" VARCHAR(50)
);

-- Create unique index on DariName
CREATE UNIQUE INDEX IF NOT EXISTS "IX_PetitionWriterActivityLocations_DariName" 
    ON org."PetitionWriterActivityLocations" ("DariName");

-- Insert some default activity locations
INSERT INTO org."PetitionWriterActivityLocations" ("Name", "DariName", "IsActive", "CreatedAt")
VALUES 
    ('District Center', 'مرکز ولسوالی', true, NOW()),
    ('Provincial Center', 'مرکز ولایت', true, NOW()),
    ('City Center', 'مرکز شهر', true, NOW())
ON CONFLICT ("DariName") DO NOTHING;
