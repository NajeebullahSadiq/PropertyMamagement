-- Create SetaDocuments table for document uploads
CREATE TABLE SetaDocuments (
    Id SERIAL PRIMARY KEY,
    SetaNumber VARCHAR(100) NOT NULL,
    FilePath VARCHAR(500) NOT NULL,
    OriginalFileName VARCHAR(100) NULL,
    FileType VARCHAR(50) NULL,
    FileSize BIGINT NOT NULL DEFAULT 0,
    UploadedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UploadedBy VARCHAR(256) NULL
);

-- Create index on SetaNumber for faster lookups
CREATE INDEX IX_SetaDocuments_SetaNumber ON SetaDocuments(SetaNumber);

-- Create index on UploadedAt for sorting
CREATE INDEX IX_SetaDocuments_UploadedAt ON SetaDocuments(UploadedAt DESC);
