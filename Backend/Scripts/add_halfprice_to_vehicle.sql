-- =====================================================
-- Script: Add HalfPrice field to VehiclesPropertyDetails
-- Description: Adds HalfPrice (مناصف قیمت) column to vehicle table
-- Date: 2026-02-02
-- =====================================================

-- Add HalfPrice column
ALTER TABLE tr."VehiclesPropertyDetails" 
ADD COLUMN IF NOT EXISTS "HalfPrice" TEXT;

-- =====================================================
-- End of Script
-- =====================================================
