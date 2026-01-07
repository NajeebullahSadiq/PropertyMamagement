using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <summary>
    /// Migration to update GuaranteeType lookup table with exactly 3 options:
    /// 1 - Cash (پول نقد)
    /// 2 - ShariaDeed (قباله شرعی)
    /// 3 - CustomaryDeed (قباله عرفی)
    /// </summary>
    public partial class UpdateGuaranteeTypesTo3Options : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First, update any existing Guarantors to use valid type IDs
            // Map old types to new ones (default to Cash if unknown)
            migrationBuilder.Sql(@"
                UPDATE org.""Guarantors"" 
                SET ""GuaranteeTypeId"" = CASE 
                    WHEN ""GuaranteeTypeId"" IN (1, 5) THEN 1  -- Cash/Cash Deposit -> Cash
                    WHEN ""GuaranteeTypeId"" IN (4) THEN 2     -- Property Guarantee -> Sharia Deed
                    ELSE 1                                      -- Default to Cash
                END
                WHERE ""GuaranteeTypeId"" IS NOT NULL;
            ");

            // Also update the Gaurantee table (note: typo in original table name)
            migrationBuilder.Sql(@"
                UPDATE org.""Gaurantee"" 
                SET ""GuaranteeTypeId"" = CASE 
                    WHEN ""GuaranteeTypeId"" IN (1, 5) THEN 1  -- Cash/Cash Deposit -> Cash
                    WHEN ""GuaranteeTypeId"" IN (4) THEN 2     -- Property Guarantee -> Sharia Deed
                    ELSE 1                                      -- Default to Cash
                END
                WHERE ""GuaranteeTypeId"" IS NOT NULL;
            ");

            // Update the guarantee types in place instead of deleting
            // This avoids foreign key constraint issues
            migrationBuilder.Sql(@"
                UPDATE look.""GuaranteeType"" SET ""Name"" = 'پول نقد' WHERE ""Id"" = 1;
                UPDATE look.""GuaranteeType"" SET ""Name"" = 'قباله شرعی' WHERE ""Id"" = 2;
                UPDATE look.""GuaranteeType"" SET ""Name"" = 'قباله عرفی' WHERE ""Id"" = 3;
            ");

            // Delete types with Id > 3 (they should no longer be referenced)
            migrationBuilder.Sql(@"
                DELETE FROM look.""GuaranteeType"" WHERE ""Id"" > 3;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Restore original guarantee types
            migrationBuilder.Sql(@"DELETE FROM look.""GuaranteeType"";");

            migrationBuilder.Sql(@"
                INSERT INTO look.""GuaranteeType"" (""Id"", ""Name"") VALUES
                (1, 'Bank Guarantee'),
                (2, 'Personal Guarantee'),
                (3, 'Corporate Guarantee'),
                (4, 'Property Guarantee'),
                (5, 'Cash Deposit'),
                (6, 'Government Guarantee'),
                (7, 'Insurance Guarantee');
            ");
        }
    }
}
