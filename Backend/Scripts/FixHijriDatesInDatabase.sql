-- Script to convert Hijri Shamsi dates stored in database to Gregorian format
-- This fixes the issue where dates were incorrectly stored in Hijri Shamsi format

-- Function to convert Hijri Shamsi to Gregorian (approximate conversion)
-- Hijri Shamsi year 1400 ≈ Gregorian year 2021
-- Formula: Gregorian Year ≈ Hijri Year + 621

-- Fix LicenseDetails table
UPDATE org."LicenseDetails"
SET 
    "IssueDate" = CASE 
        -- If year is between 1300-1500, it's Hijri Shamsi, convert to Gregorian
        WHEN EXTRACT(YEAR FROM "IssueDate") BETWEEN 1300 AND 1500 THEN
            ("IssueDate" + INTERVAL '621 years')::date
        -- If year is 2000+, it's already Gregorian
        ELSE "IssueDate"
    END,
    "ExpireDate" = CASE 
        WHEN EXTRACT(YEAR FROM "ExpireDate") BETWEEN 1300 AND 1500 THEN
            ("ExpireDate" + INTERVAL '621 years')::date
        ELSE "ExpireDate"
    END,
    "RoyaltyDate" = CASE 
        WHEN "RoyaltyDate" IS NOT NULL AND EXTRACT(YEAR FROM "RoyaltyDate") BETWEEN 1300 AND 1500 THEN
            ("RoyaltyDate" + INTERVAL '621 years')::date
        ELSE "RoyaltyDate"
    END,
    "PenaltyDate" = CASE 
        WHEN "PenaltyDate" IS NOT NULL AND EXTRACT(YEAR FROM "PenaltyDate") BETWEEN 1300 AND 1500 THEN
            ("PenaltyDate" + INTERVAL '621 years')::date
        ELSE "PenaltyDate"
    END,
    "HrLetterDate" = CASE 
        WHEN "HrLetterDate" IS NOT NULL AND EXTRACT(YEAR FROM "HrLetterDate") BETWEEN 1300 AND 1500 THEN
            ("HrLetterDate" + INTERVAL '621 years')::date
        ELSE "HrLetterDate"
    END
WHERE 
    EXTRACT(YEAR FROM "IssueDate") BETWEEN 1300 AND 1500
    OR EXTRACT(YEAR FROM "ExpireDate") BETWEEN 1300 AND 1500
    OR (EXTRACT(YEAR FROM "RoyaltyDate") BETWEEN 1300 AND 1500)
    OR (EXTRACT(YEAR FROM "PenaltyDate") BETWEEN 1300 AND 1500)
    OR (EXTRACT(YEAR FROM "HrLetterDate") BETWEEN 1300 AND 1500);

-- Fix CompanyOwner table
UPDATE org."CompanyOwner"
SET 
    "DateofBirth" = CASE 
        WHEN EXTRACT(YEAR FROM "DateofBirth") BETWEEN 1300 AND 1500 THEN
            ("DateofBirth" + INTERVAL '621 years')::date
        ELSE "DateofBirth"
    END
WHERE 
    EXTRACT(YEAR FROM "DateofBirth") BETWEEN 1300 AND 1500;

-- Fix Guarantors table
UPDATE org."Guarantors"
SET 
    "DateofGuarantee" = CASE 
        WHEN "DateofGuarantee" IS NOT NULL AND EXTRACT(YEAR FROM "DateofGuarantee") BETWEEN 1300 AND 1500 THEN
            ("DateofGuarantee" + INTERVAL '621 years')::date
        ELSE "DateofGuarantee"
    END,
    "DepositDate" = CASE 
        WHEN "DepositDate" IS NOT NULL AND EXTRACT(YEAR FROM "DepositDate") BETWEEN 1300 AND 1500 THEN
            ("DepositDate" + INTERVAL '621 years')::date
        ELSE "DepositDate"
    END,
    "SenderMaktobDate" = CASE 
        WHEN "SenderMaktobDate" IS NOT NULL AND EXTRACT(YEAR FROM "SenderMaktobDate") BETWEEN 1300 AND 1500 THEN
            ("SenderMaktobDate" + INTERVAL '621 years')::date
        ELSE "SenderMaktobDate"
    END,
    "AnswerdMaktobDate" = CASE 
        WHEN "AnswerdMaktobDate" IS NOT NULL AND EXTRACT(YEAR FROM "AnswerdMaktobDate") BETWEEN 1300 AND 1500 THEN
            ("AnswerdMaktobDate" + INTERVAL '621 years')::date
        ELSE "AnswerdMaktobDate"
    END,
    "ExpiredAt" = CASE 
        WHEN "ExpiredAt" IS NOT NULL AND EXTRACT(YEAR FROM "ExpiredAt") BETWEEN 1300 AND 1500 THEN
            ("ExpiredAt" + INTERVAL '621 years')::date
        ELSE "ExpiredAt"
    END
WHERE 
    (EXTRACT(YEAR FROM "DateofGuarantee") BETWEEN 1300 AND 1500)
    OR (EXTRACT(YEAR FROM "DepositDate") BETWEEN 1300 AND 1500)
    OR (EXTRACT(YEAR FROM "SenderMaktobDate") BETWEEN 1300 AND 1500)
    OR (EXTRACT(YEAR FROM "AnswerdMaktobDate") BETWEEN 1300 AND 1500)
    OR (EXTRACT(YEAR FROM "ExpiredAt") BETWEEN 1300 AND 1500);

-- Fix AccountInfo table
UPDATE org."AccountInfo"
SET 
    "TaxPaymentDate" = CASE 
        WHEN "TaxPaymentDate" IS NOT NULL AND EXTRACT(YEAR FROM "TaxPaymentDate") BETWEEN 1300 AND 1500 THEN
            ("TaxPaymentDate" + INTERVAL '621 years')::date
        ELSE "TaxPaymentDate"
    END
WHERE 
    EXTRACT(YEAR FROM "TaxPaymentDate") BETWEEN 1300 AND 1500;

-- Fix CancellationInfo table
UPDATE org."CancellationInfo"
SET 
    "LicenseCancellationLetterDate" = CASE 
        WHEN "LicenseCancellationLetterDate" IS NOT NULL AND EXTRACT(YEAR FROM "LicenseCancellationLetterDate") BETWEEN 1300 AND 1500 THEN
            ("LicenseCancellationLetterDate" + INTERVAL '621 years')::date
        ELSE "LicenseCancellationLetterDate"
    END
WHERE 
    EXTRACT(YEAR FROM "LicenseCancellationLetterDate") BETWEEN 1300 AND 1500;

-- Display summary of changes
SELECT 
    'LicenseDetails' as table_name,
    COUNT(*) as total_records,
    COUNT(CASE WHEN EXTRACT(YEAR FROM "IssueDate") >= 2000 THEN 1 END) as gregorian_dates,
    COUNT(CASE WHEN EXTRACT(YEAR FROM "IssueDate") < 2000 THEN 1 END) as remaining_hijri_dates
FROM org."LicenseDetails"
UNION ALL
SELECT 
    'CompanyOwner' as table_name,
    COUNT(*) as total_records,
    COUNT(CASE WHEN EXTRACT(YEAR FROM "DateofBirth") >= 1900 THEN 1 END) as gregorian_dates,
    COUNT(CASE WHEN EXTRACT(YEAR FROM "DateofBirth") < 1900 THEN 1 END) as remaining_hijri_dates
FROM org."CompanyOwner";
