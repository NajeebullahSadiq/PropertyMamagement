using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    public partial class ChangeGuaranteeNumbersToBigInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Change PropertyDocumentNumber, AnswerdMaktobNumber, and GuaranteeDocNumber from integer to bigint
            migrationBuilder.Sql(@"
                ALTER TABLE org.""Gaurantee"" 
                    ALTER COLUMN ""PropertyDocumentNumber"" TYPE bigint,
                    ALTER COLUMN ""AnswerdMaktobNumber"" TYPE bigint,
                    ALTER COLUMN ""GuaranteeDocNumber"" TYPE bigint;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert back to integer (note: this may fail if data exceeds int range)
            migrationBuilder.Sql(@"
                ALTER TABLE org.""Gaurantee"" 
                    ALTER COLUMN ""PropertyDocumentNumber"" TYPE integer,
                    ALTER COLUMN ""AnswerdMaktobNumber"" TYPE integer,
                    ALTER COLUMN ""GuaranteeDocNumber"" TYPE integer;
            ");
        }
    }
}
