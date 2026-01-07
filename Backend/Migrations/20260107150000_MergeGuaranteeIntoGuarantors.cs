using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    public partial class MergeGuaranteeIntoGuarantors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add guarantee-related columns to Guarantors table
            migrationBuilder.AddColumn<int>(
                name: "GuaranteeTypeId",
                schema: "org",
                table: "Guarantors",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PropertyDocumentNumber",
                schema: "org",
                table: "Guarantors",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "PropertyDocumentDate",
                schema: "org",
                table: "Guarantors",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenderMaktobNumber",
                schema: "org",
                table: "Guarantors",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "SenderMaktobDate",
                schema: "org",
                table: "Guarantors",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AnswerdMaktobNumber",
                schema: "org",
                table: "Guarantors",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "AnswerdMaktobDate",
                schema: "org",
                table: "Guarantors",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateofGuarantee",
                schema: "org",
                table: "Guarantors",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "GuaranteeDocNumber",
                schema: "org",
                table: "Guarantors",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "GuaranteeDate",
                schema: "org",
                table: "Guarantors",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuaranteeDocPath",
                schema: "org",
                table: "Guarantors",
                type: "text",
                nullable: true);

            // Add index for GuaranteeTypeId
            migrationBuilder.CreateIndex(
                name: "IX_Guarantors_GuaranteeTypeId",
                schema: "org",
                table: "Guarantors",
                column: "GuaranteeTypeId");

            // Add foreign key for GuaranteeType
            migrationBuilder.AddForeignKey(
                name: "Guarantors_GuaranteeTypeId_fkey",
                schema: "org",
                table: "Guarantors",
                column: "GuaranteeTypeId",
                principalSchema: "look",
                principalTable: "GuaranteeType",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            // Migrate existing guarantee data to the first guarantor of each company
            // This safely handles cases where there may be no matching data
            migrationBuilder.Sql(@"
                UPDATE org.""Guarantors"" g
                SET 
                    ""GuaranteeTypeId"" = subquery.""GuaranteeTypeId"",
                    ""PropertyDocumentNumber"" = subquery.""PropertyDocumentNumber"",
                    ""PropertyDocumentDate"" = subquery.""PropertyDocumentDate"",
                    ""SenderMaktobNumber"" = subquery.""SenderMaktobNumber"",
                    ""SenderMaktobDate"" = subquery.""SenderMaktobDate"",
                    ""AnswerdMaktobNumber"" = subquery.""AnswerdMaktobNumber"",
                    ""AnswerdMaktobDate"" = subquery.""AnswerdMaktobDate"",
                    ""DateofGuarantee"" = subquery.""DateofGuarantee"",
                    ""GuaranteeDocNumber"" = subquery.""GuaranteeDocNumber"",
                    ""GuaranteeDate"" = subquery.""GuaranteeDate"",
                    ""GuaranteeDocPath"" = subquery.""DocPath""
                FROM (
                    SELECT 
                        gua.""CompanyId"",
                        gua.""GuaranteeTypeId"",
                        gua.""PropertyDocumentNumber"",
                        gua.""PropertyDocumentDate"",
                        gua.""SenderMaktobNumber"",
                        gua.""SenderMaktobDate"",
                        gua.""AnswerdMaktobNumber"",
                        gua.""AnswerdMaktobDate"",
                        gua.""DateofGuarantee"",
                        gua.""GuaranteeDocNumber"",
                        gua.""GuaranteeDate"",
                        gua.""DocPath"",
                        (SELECT MIN(g2.""Id"") FROM org.""Guarantors"" g2 WHERE g2.""CompanyId"" = gua.""CompanyId"") as min_guarantor_id
                    FROM org.""Gaurantee"" gua
                    WHERE gua.""CompanyId"" IS NOT NULL
                ) subquery
                WHERE g.""Id"" = subquery.min_guarantor_id
                AND subquery.min_guarantor_id IS NOT NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove foreign key
            migrationBuilder.DropForeignKey(
                name: "Guarantors_GuaranteeTypeId_fkey",
                schema: "org",
                table: "Guarantors");

            migrationBuilder.DropIndex(
                name: "IX_Guarantors_GuaranteeTypeId",
                schema: "org",
                table: "Guarantors");

            // Remove guarantee-related columns from Guarantors table
            migrationBuilder.DropColumn(
                name: "GuaranteeTypeId",
                schema: "org",
                table: "Guarantors");

            migrationBuilder.DropColumn(
                name: "PropertyDocumentNumber",
                schema: "org",
                table: "Guarantors");

            migrationBuilder.DropColumn(
                name: "PropertyDocumentDate",
                schema: "org",
                table: "Guarantors");

            migrationBuilder.DropColumn(
                name: "SenderMaktobNumber",
                schema: "org",
                table: "Guarantors");

            migrationBuilder.DropColumn(
                name: "SenderMaktobDate",
                schema: "org",
                table: "Guarantors");

            migrationBuilder.DropColumn(
                name: "AnswerdMaktobNumber",
                schema: "org",
                table: "Guarantors");

            migrationBuilder.DropColumn(
                name: "AnswerdMaktobDate",
                schema: "org",
                table: "Guarantors");

            migrationBuilder.DropColumn(
                name: "DateofGuarantee",
                schema: "org",
                table: "Guarantors");

            migrationBuilder.DropColumn(
                name: "GuaranteeDocNumber",
                schema: "org",
                table: "Guarantors");

            migrationBuilder.DropColumn(
                name: "GuaranteeDate",
                schema: "org",
                table: "Guarantors");

            migrationBuilder.DropColumn(
                name: "GuaranteeDocPath",
                schema: "org",
                table: "Guarantors");
        }
    }
}
