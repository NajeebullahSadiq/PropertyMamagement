-- Add contract start and end date fields to BuyerDetails table for rental transactions
-- These fields are required when transaction type is "Rent" (کرایه)

ALTER TABLE tr."BuyerDetails"
ADD COLUMN IF NOT EXISTS "ContractStartDate" TIMESTAMP WITHOUT TIME ZONE,
ADD COLUMN IF NOT EXISTS "ContractEndDate" TIMESTAMP WITHOUT TIME ZONE;

-- Add comments for the new fields
COMMENT ON COLUMN tr."BuyerDetails"."ContractStartDate" IS 'تاریخ اغاز قرار داد - Contract start date for rental transactions';
COMMENT ON COLUMN tr."BuyerDetails"."ContractEndDate" IS 'تاریخ ختم قرار داد - Contract end date for rental transactions';
