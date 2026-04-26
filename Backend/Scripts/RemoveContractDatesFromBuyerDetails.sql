-- Remove contract start and end date fields from BuyerDetails table
-- These fields are no longer needed; rental dates (RentStartDate/RentEndDate) are used instead

ALTER TABLE tr."BuyerDetails"
DROP COLUMN IF EXISTS "ContractStartDate",
DROP COLUMN IF EXISTS "ContractEndDate";
