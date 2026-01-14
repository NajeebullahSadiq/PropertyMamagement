-- =====================================================
-- Module: Shared/Lookup Tables
-- Schema: look
-- Dependencies: None
-- Execution Order: 1
-- =====================================================

-- Create schema if not exists
CREATE SCHEMA IF NOT EXISTS look;

-- Create sequences
CREATE SEQUENCE IF NOT EXISTS look.educationlevel_id_seq;
CREATE SEQUENCE IF NOT EXISTS look.location_id_seq;

-- AddressType
CREATE TABLE IF NOT EXISTS look."AddressType" (
    "Id" SERIAL PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "Des" TEXT
);

-- Area
CREATE TABLE IF NOT EXISTS look."Area" (
    "Id" SERIAL PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "Des" TEXT
);

-- EducationLevel
CREATE TABLE IF NOT EXISTS look."EducationLevel" (
    "ID" SMALLINT PRIMARY KEY DEFAULT nextval('look.educationlevel_id_seq'::regclass),
    "Name" VARCHAR(50) NOT NULL,
    "Dari" TEXT,
    "parentid" SMALLINT,
    "Sorter" VARCHAR(50)
);

-- FormsReference
CREATE TABLE IF NOT EXISTS look."FormsReference" (
    "Id" SERIAL PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "Des" TEXT
);

-- GuaranteeType
CREATE TABLE IF NOT EXISTS look."GuaranteeType" (
    "Id" SERIAL PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "Des" TEXT
);

-- IdentityCardType
CREATE TABLE IF NOT EXISTS look."IdentityCardType" (
    "Id" SERIAL PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "Des" TEXT
);

-- Location
CREATE TABLE IF NOT EXISTS look."Location" (
    "ID" INTEGER PRIMARY KEY DEFAULT nextval('look.location_id_seq'::regclass),
    "Dari" VARCHAR(255) NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "Code" CHAR(3),
    "Path" VARCHAR(255),
    "Path_Dari" VARCHAR(255),
    "ParentID" INTEGER,
    "TypeID" INTEGER,
    "Name" VARCHAR(255)
);

-- LostdocumentsType
CREATE TABLE IF NOT EXISTS look."LostdocumentsType" (
    "Id" SERIAL PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "Des" TEXT
);

-- PropertyType
CREATE TABLE IF NOT EXISTS look."PropertyType" (
    "Id" SERIAL PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "Des" TEXT
);

-- PUnitType
CREATE TABLE IF NOT EXISTS look."PUnitType" (
    "Id" SERIAL PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "Des" TEXT
);

-- TransactionType
CREATE TABLE IF NOT EXISTS look."TransactionType" (
    "Id" SERIAL PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "Des" TEXT
);

-- ViolationType
CREATE TABLE IF NOT EXISTS look."ViolationType" (
    "Id" SERIAL PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "Des" TEXT
);

-- =====================================================
-- End of Shared/Lookup Module
-- =====================================================
