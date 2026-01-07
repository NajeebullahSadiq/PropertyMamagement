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
    public partial class UpdateGuaranteeTypeLookup : Migration
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

            // Delete all existing guarantee types
            migrationBuilder.Sql(@"DELETE FROM look.""GuaranteeType"";");

            // Insert the 3 new guarantee types
            migrationBuilder.Sql(@"
                INSERT INTO look.""GuaranteeType"" (""Id"", ""Name"", ""Des"") VALUES
                (1, 'Cash', 'پول نقد'),
                (2, 'ShariaDeed', 'قباله شرعی'),
                (3, 'CustomaryDeed', 'قباله عرفی');
            ");

            // Reset the sequence to start after 3
            migrationBuilder.Sql(@"
                SELECT setval('look.""GuaranteeType_Id_seq""', 3, true);
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Restore original guarantee types
            migrationBuilder.Sql(@"DELETE FROM look.""GuaranteeType"";");

            migrationBuilder.Sql(@"
                INSERT INTO look.""GuaranteeType"" (""Id"", ""Name"", ""Des"") VALUES
                (1, 'Bank Guarantee', 'ضمانت بانکی'),
                (2, 'Personal Guarantee', 'ضمانت شخصی'),
                (3, 'Corporate Guarantee', 'ضمانت دفتری'),
                (4, 'Property Guarantee', 'ضمانت ملکی'),
                (5, 'Cash Deposit', 'سپردهٔ نقد'),
                (6, 'Government Guarantee', 'ضمانت دولتی'),
                (7, 'Insurance Guarantee', 'ضمانت بیمهٔ');
            ");

            migrationBuilder.Sql(@"
                SELECT setval('look.""GuaranteeType_Id_seq""', 7, true);
            ");
        }
    }
}
