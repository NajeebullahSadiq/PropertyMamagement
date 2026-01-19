-- =============================================
-- Add PicturePath column to PetitionWriterLicenses
-- Migration: 20260119_AddPicturePath_PetitionWriter
-- =============================================

USE [PRMIS]
GO

-- Add PicturePath column if it doesn't exist
IF NOT EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[org].[PetitionWriterLicenses]') 
    AND name = 'PicturePath'
)
BEGIN
    ALTER TABLE [org].[PetitionWriterLicenses]
    ADD [PicturePath] NVARCHAR(500) NULL;
    
    PRINT 'Column PicturePath added to org.PetitionWriterLicenses';
END
ELSE
BEGIN
    PRINT 'Column PicturePath already exists in org.PetitionWriterLicenses';
END
GO
