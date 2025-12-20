using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using WebAPIBackend.Configuration;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AppDbContext))]
    [Migration("20251218090000_AddPropertyDocumentCategories")]
    public partial class AddPropertyDocumentCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    -- PreviousDocumentsPath
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'tr'
                        AND table_name = 'PropertyDetails'
                        AND column_name = 'PreviousDocumentsPath'
                    ) THEN
                        IF EXISTS (
                            SELECT 1 FROM information_schema.columns
                            WHERE table_schema = 'tr'
                            AND table_name = 'PropertyDetails'
                            AND column_name = 'previousdocumentspath'
                        ) THEN
                            ALTER TABLE tr.""PropertyDetails""
                            RENAME COLUMN previousdocumentspath TO ""PreviousDocumentsPath"";
                        ELSE
                            ALTER TABLE tr.""PropertyDetails""
                            ADD COLUMN ""PreviousDocumentsPath"" text NULL;
                        END IF;
                    END IF;

                    -- ExistingDocumentsPath
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'tr'
                        AND table_name = 'PropertyDetails'
                        AND column_name = 'ExistingDocumentsPath'
                    ) THEN
                        IF EXISTS (
                            SELECT 1 FROM information_schema.columns
                            WHERE table_schema = 'tr'
                            AND table_name = 'PropertyDetails'
                            AND column_name = 'existingdocumentspath'
                        ) THEN
                            ALTER TABLE tr.""PropertyDetails""
                            RENAME COLUMN existingdocumentspath TO ""ExistingDocumentsPath"";
                        ELSE
                            ALTER TABLE tr.""PropertyDetails""
                            ADD COLUMN ""ExistingDocumentsPath"" text NULL;
                        END IF;
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'tr'
                        AND table_name = 'PropertyDetails'
                        AND column_name = 'PreviousDocumentsPath'
                    ) THEN
                        ALTER TABLE tr.""PropertyDetails"" DROP COLUMN ""PreviousDocumentsPath"";
                    END IF;

                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'tr'
                        AND table_name = 'PropertyDetails'
                        AND column_name = 'previousdocumentspath'
                    ) THEN
                        ALTER TABLE tr.""PropertyDetails"" DROP COLUMN previousdocumentspath;
                    END IF;

                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'tr'
                        AND table_name = 'PropertyDetails'
                        AND column_name = 'ExistingDocumentsPath'
                    ) THEN
                        ALTER TABLE tr.""PropertyDetails"" DROP COLUMN ""ExistingDocumentsPath"";
                    END IF;

                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'tr'
                        AND table_name = 'PropertyDetails'
                        AND column_name = 'existingdocumentspath'
                    ) THEN
                        ALTER TABLE tr.""PropertyDetails"" DROP COLUMN existingdocumentspath;
                    END IF;
                END $$;
            ");
        }
    }
}
