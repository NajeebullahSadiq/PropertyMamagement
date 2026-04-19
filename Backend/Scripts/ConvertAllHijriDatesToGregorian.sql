-- =====================================================
-- Convert All Hijri Shamsi Dates to Gregorian in Database
-- =====================================================
-- This script converts all dates stored in Hijri Shamsi format (year 1300-1500)
-- to proper Gregorian dates using PostgreSQL date conversion
-- 
-- IMPORTANT: Backup your database before running this script!
-- =====================================================

-- Create a helper function to convert Hijri Shamsi to Gregorian
-- This uses the Persian calendar conversion formula
CREATE OR REPLACE FUNCTION convert_hijri_shamsi_to_gregorian(hijri_date DATE)
RETURNS DATE AS $$
DECLARE
    hijri_year INTEGER;
    hijri_month INTEGER;
    hijri_day INTEGER;
    gregorian_date DATE;
    days_since_epoch INTEGER;
    persian_epoch INTEGER := 1948321; -- Julian day number for 1/1/1 Persian calendar
BEGIN
    -- Extract year, month, day from the Hijri Shamsi date
    hijri_year := EXTRACT(YEAR FROM hijri_date);
    hijri_month := EXTRACT(MONTH FROM hijri_date);
    hijri_day := EXTRACT(DAY FROM hijri_date);
    
    -- Only convert if year is in Hijri Shamsi range (1300-1500)
    IF hijri_year < 1300 OR hijri_year > 1500 THEN
        RETURN hijri_date; -- Already Gregorian, return as-is
    END IF;
    
    -- Calculate days from Persian epoch
    -- This is a simplified conversion - for production use a proper library
    days_since_epoch := (hijri_year - 1) * 365 
                      + FLOOR((hijri_year - 1) / 33) * 8 
                      + FLOOR(((hijri_year - 1) % 33 + 3) / 4);
    
    -- Add days for complete months
    IF hijri_month > 1 THEN
        IF hijri_month <= 7 THEN
            days_since_epoch := days_since_epoch + (hijri_month - 1) * 31;
        ELSE
            days_since_epoch := days_since_epoch + 6 * 31 + (hijri_month - 7) * 30;
        END IF;
    END IF;
    
    -- Add remaining days
    days_since_epoch := days_since_epoch + hijri_day;
    
    -- Convert Julian day to Gregorian date
    gregorian_date := TO_DATE('0622-03-22', 'YYYY-MM-DD') + days_since_epoch - 1;
    
    RETURN gregorian_date;
END;
$$ LANGUAGE plpgsql IMMUTABLE;

-- =====================================================
-- Convert LicenseDetails dates
-- =====================================================
DO $$
DECLARE
    rec RECORD;
    new_issue_date DATE;
    new_expire_date DATE;
    new_royalty_date DATE;
    new_penalty_date DATE;
    new_hr_letter_date DATE;
    converted_count INTEGER := 0;
BEGIN
    RAISE NOTICE 'Starting conversion of LicenseDetails dates...';
    
    FOR rec IN 
        SELECT "Id", "IssueDate", "ExpireDate", "RoyaltyDate", "PenaltyDate", "HrLetterDate"
        FROM org."LicenseDetails"
        WHERE "IssueDate" IS NOT NULL OR "ExpireDate" IS NOT NULL 
           OR "RoyaltyDate" IS NOT NULL OR "PenaltyDate" IS NOT NULL 
           OR "HrLetterDate" IS NOT NULL
    LOOP
        -- Convert IssueDate if it's Hijri Shamsi
        IF rec."IssueDate" IS NOT NULL AND EXTRACT(YEAR FROM rec."IssueDate") BETWEEN 1300 AND 1500 THEN
            new_issue_date := convert_hijri_shamsi_to_gregorian(rec."IssueDate");
        ELSE
            new_issue_date := rec."IssueDate";
        END IF;
        
        -- Convert ExpireDate if it's Hijri Shamsi
        IF rec."ExpireDate" IS NOT NULL AND EXTRACT(YEAR FROM rec."ExpireDate") BETWEEN 1300 AND 1500 THEN
            new_expire_date := convert_hijri_shamsi_to_gregorian(rec."ExpireDate");
        ELSE
            new_expire_date := rec."ExpireDate";
        END IF;
        
        -- Convert RoyaltyDate if it's Hijri Shamsi
        IF rec."RoyaltyDate" IS NOT NULL AND EXTRACT(YEAR FROM rec."RoyaltyDate") BETWEEN 1300 AND 1500 THEN
            new_royalty_date := convert_hijri_shamsi_to_gregorian(rec."RoyaltyDate");
        ELSE
            new_royalty_date := rec."RoyaltyDate";
        END IF;
        
        -- Convert PenaltyDate if it's Hijri Shamsi
        IF rec."PenaltyDate" IS NOT NULL AND EXTRACT(YEAR FROM rec."PenaltyDate") BETWEEN 1300 AND 1500 THEN
            new_penalty_date := convert_hijri_shamsi_to_gregorian(rec."PenaltyDate");
        ELSE
            new_penalty_date := rec."PenaltyDate";
        END IF;
        
        -- Convert HrLetterDate if it's Hijri Shamsi
        IF rec."HrLetterDate" IS NOT NULL AND EXTRACT(YEAR FROM rec."HrLetterDate") BETWEEN 1300 AND 1500 THEN
            new_hr_letter_date := convert_hijri_shamsi_to_gregorian(rec."HrLetterDate");
        ELSE
            new_hr_letter_date := rec."HrLetterDate";
        END IF;
        
        -- Update the record
        UPDATE org."LicenseDetails"
        SET "IssueDate" = new_issue_date,
            "ExpireDate" = new_expire_date,
            "RoyaltyDate" = new_royalty_date,
            "PenaltyDate" = new_penalty_date,
            "HrLetterDate" = new_hr_letter_date
        WHERE "Id" = rec."Id";
        
        converted_count := converted_count + 1;
    END LOOP;
    
    RAISE NOTICE 'Converted % records in LicenseDetails', converted_count;
END $$;

-- =====================================================
-- Convert CompanyOwner dates
-- =====================================================
DO $$
DECLARE
    rec RECORD;
    new_date_of_birth DATE;
    converted_count INTEGER := 0;
BEGIN
    RAISE NOTICE 'Starting conversion of CompanyOwner dates...';
    
    FOR rec IN 
        SELECT "Id", "DateofBirth"
        FROM org."CompanyOwner"
        WHERE "DateofBirth" IS NOT NULL
    LOOP
        -- Convert DateofBirth if it's Hijri Shamsi
        IF EXTRACT(YEAR FROM rec."DateofBirth") BETWEEN 1300 AND 1500 THEN
            new_date_of_birth := convert_hijri_shamsi_to_gregorian(rec."DateofBirth");
            
            UPDATE org."CompanyOwner"
            SET "DateofBirth" = new_date_of_birth
            WHERE "Id" = rec."Id";
            
            converted_count := converted_count + 1;
        END IF;
    END LOOP;
    
    RAISE NOTICE 'Converted % records in CompanyOwner', converted_count;
END $$;

-- =====================================================
-- Convert Guarantors dates
-- =====================================================
DO $$
DECLARE
    rec RECORD;
    new_property_doc_date DATE;
    new_sender_maktob_date DATE;
    new_answerd_maktob_date DATE;
    new_date_of_guarantee DATE;
    new_guarantee_date DATE;
    new_deposit_date DATE;
    converted_count INTEGER := 0;
BEGIN
    RAISE NOTICE 'Starting conversion of Guarantors dates...';
    
    FOR rec IN 
        SELECT "Id", "PropertyDocumentDate", "SenderMaktobDate", "AnswerdMaktobDate",
               "DateofGuarantee", "GuaranteeDate", "DepositDate"
        FROM org."Guarantors"
        WHERE "PropertyDocumentDate" IS NOT NULL OR "SenderMaktobDate" IS NOT NULL 
           OR "AnswerdMaktobDate" IS NOT NULL OR "DateofGuarantee" IS NOT NULL
           OR "GuaranteeDate" IS NOT NULL OR "DepositDate" IS NOT NULL
    LOOP
        -- Convert PropertyDocumentDate
        IF rec."PropertyDocumentDate" IS NOT NULL AND EXTRACT(YEAR FROM rec."PropertyDocumentDate") BETWEEN 1300 AND 1500 THEN
            new_property_doc_date := convert_hijri_shamsi_to_gregorian(rec."PropertyDocumentDate");
        ELSE
            new_property_doc_date := rec."PropertyDocumentDate";
        END IF;
        
        -- Convert SenderMaktobDate
        IF rec."SenderMaktobDate" IS NOT NULL AND EXTRACT(YEAR FROM rec."SenderMaktobDate") BETWEEN 1300 AND 1500 THEN
            new_sender_maktob_date := convert_hijri_shamsi_to_gregorian(rec."SenderMaktobDate");
        ELSE
            new_sender_maktob_date := rec."SenderMaktobDate";
        END IF;
        
        -- Convert AnswerdMaktobDate
        IF rec."AnswerdMaktobDate" IS NOT NULL AND EXTRACT(YEAR FROM rec."AnswerdMaktobDate") BETWEEN 1300 AND 1500 THEN
            new_answerd_maktob_date := convert_hijri_shamsi_to_gregorian(rec."AnswerdMaktobDate");
        ELSE
            new_answerd_maktob_date := rec."AnswerdMaktobDate";
        END IF;
        
        -- Convert DateofGuarantee
        IF rec."DateofGuarantee" IS NOT NULL AND EXTRACT(YEAR FROM rec."DateofGuarantee") BETWEEN 1300 AND 1500 THEN
            new_date_of_guarantee := convert_hijri_shamsi_to_gregorian(rec."DateofGuarantee");
        ELSE
            new_date_of_guarantee := rec."DateofGuarantee";
        END IF;
        
        -- Convert GuaranteeDate
        IF rec."GuaranteeDate" IS NOT NULL AND EXTRACT(YEAR FROM rec."GuaranteeDate") BETWEEN 1300 AND 1500 THEN
            new_guarantee_date := convert_hijri_shamsi_to_gregorian(rec."GuaranteeDate");
        ELSE
            new_guarantee_date := rec."GuaranteeDate";
        END IF;
        
        -- Convert DepositDate
        IF rec."DepositDate" IS NOT NULL AND EXTRACT(YEAR FROM rec."DepositDate") BETWEEN 1300 AND 1500 THEN
            new_deposit_date := convert_hijri_shamsi_to_gregorian(rec."DepositDate");
        ELSE
            new_deposit_date := rec."DepositDate";
        END IF;
        
        -- Update the record
        UPDATE org."Guarantors"
        SET "PropertyDocumentDate" = new_property_doc_date,
            "SenderMaktobDate" = new_sender_maktob_date,
            "AnswerdMaktobDate" = new_answerd_maktob_date,
            "DateofGuarantee" = new_date_of_guarantee,
            "GuaranteeDate" = new_guarantee_date,
            "DepositDate" = new_deposit_date
        WHERE "Id" = rec."Id";
        
        converted_count := converted_count + 1;
    END LOOP;
    
    RAISE NOTICE 'Converted % records in Guarantors', converted_count;
END $$;

-- =====================================================
-- Convert CompanyCancellationInfo dates
-- =====================================================
DO $$
DECLARE
    rec RECORD;
    new_cancellation_date DATE;
    converted_count INTEGER := 0;
BEGIN
    RAISE NOTICE 'Starting conversion of CompanyCancellationInfo dates...';
    
    FOR rec IN 
        SELECT "Id", "LicenseCancellationLetterDate"
        FROM org."CompanyCancellationInfo"
        WHERE "LicenseCancellationLetterDate" IS NOT NULL
    LOOP
        -- Convert LicenseCancellationLetterDate if it's Hijri Shamsi
        IF EXTRACT(YEAR FROM rec."LicenseCancellationLetterDate") BETWEEN 1300 AND 1500 THEN
            new_cancellation_date := convert_hijri_shamsi_to_gregorian(rec."LicenseCancellationLetterDate");
            
            UPDATE org."CompanyCancellationInfo"
            SET "LicenseCancellationLetterDate" = new_cancellation_date
            WHERE "Id" = rec."Id";
            
            converted_count := converted_count + 1;
        END IF;
    END LOOP;
    
    RAISE NOTICE 'Converted % records in CompanyCancellationInfo', converted_count;
END $$;

-- =====================================================
-- Convert CompanyAccountInfo dates
-- =====================================================
DO $$
DECLARE
    rec RECORD;
    new_tax_payment_date DATE;
    converted_count INTEGER := 0;
BEGIN
    RAISE NOTICE 'Starting conversion of CompanyAccountInfo dates...';
    
    FOR rec IN 
        SELECT "Id", "TaxPaymentDate"
        FROM org."CompanyAccountInfo"
        WHERE "TaxPaymentDate" IS NOT NULL
    LOOP
        -- Convert TaxPaymentDate if it's Hijri Shamsi
        IF EXTRACT(YEAR FROM rec."TaxPaymentDate") BETWEEN 1300 AND 1500 THEN
            new_tax_payment_date := convert_hijri_shamsi_to_gregorian(rec."TaxPaymentDate");
            
            UPDATE org."CompanyAccountInfo"
            SET "TaxPaymentDate" = new_tax_payment_date
            WHERE "Id" = rec."Id";
            
            converted_count := converted_count + 1;
        END IF;
    END LOOP;
    
    RAISE NOTICE 'Converted % records in CompanyAccountInfo', converted_count;
END $$;

-- =====================================================
-- Verification Query
-- =====================================================
-- Run this to verify the conversion worked
-- All years should now be > 1500 (Gregorian range)
SELECT 
    'LicenseDetails' as table_name,
    COUNT(*) as total_records,
    COUNT(CASE WHEN EXTRACT(YEAR FROM "IssueDate") BETWEEN 1300 AND 1500 THEN 1 END) as hijri_issue_dates,
    COUNT(CASE WHEN EXTRACT(YEAR FROM "ExpireDate") BETWEEN 1300 AND 1500 THEN 1 END) as hijri_expire_dates
FROM org."LicenseDetails"
UNION ALL
SELECT 
    'CompanyOwner' as table_name,
    COUNT(*) as total_records,
    COUNT(CASE WHEN EXTRACT(YEAR FROM "DateofBirth") BETWEEN 1300 AND 1500 THEN 1 END) as hijri_dates,
    0 as hijri_expire_dates
FROM org."CompanyOwner"
UNION ALL
SELECT 
    'Guarantors' as table_name,
    COUNT(*) as total_records,
    COUNT(CASE WHEN EXTRACT(YEAR FROM "PropertyDocumentDate") BETWEEN 1300 AND 1500 THEN 1 END) as hijri_dates,
    0 as hijri_expire_dates
FROM org."Guarantors";

-- =====================================================
-- Cleanup
-- =====================================================
-- Optionally drop the helper function after conversion
-- DROP FUNCTION IF EXISTS convert_hijri_shamsi_to_gregorian(DATE);
