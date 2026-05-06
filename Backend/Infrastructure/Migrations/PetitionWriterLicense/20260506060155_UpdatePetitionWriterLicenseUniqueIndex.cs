using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Infrastructure.Migrations.PetitionWriterLicense
{
    /// <inheritdoc />
    public partial class UpdatePetitionWriterLicenseUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop old ProvinceId FK (auto-generated name) to replace with named constraint
            migrationBuilder.DropForeignKey(
                name: "FK_PetitionWriterLicenses_Location_ProvinceId",
                schema: "org",
                table: "PetitionWriterLicenses");

            // Drop old unique index on LicenseNumber (unfiltered)
            migrationBuilder.DropIndex(
                name: "IX_PetitionWriterLicenses_LicenseNumber",
                schema: "org",
                table: "PetitionWriterLicenses");

            // Create new filtered unique index on LicenseNumber (only active records)
            migrationBuilder.CreateIndex(
                name: "IX_PetitionWriterLicenses_LicenseNumber",
                schema: "org",
                table: "PetitionWriterLicenses",
                column: "LicenseNumber",
                unique: true,
                filter: "\"Status\" = true");

            // Add ProvinceId FK with named constraint (matching other FK pattern)
            migrationBuilder.AddForeignKey(
                name: "PetitionWriterLicenses_ProvinceId_fkey",
                schema: "org",
                table: "PetitionWriterLicenses",
                column: "ProvinceId",
                principalSchema: "look",
                principalTable: "Location",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "PetitionWriterLicenses_ProvinceId_fkey",
                schema: "org",
                table: "PetitionWriterLicenses");

            migrationBuilder.DropIndex(
                name: "IX_PetitionWriterLicenses_LicenseNumber",
                schema: "org",
                table: "PetitionWriterLicenses");

            migrationBuilder.CreateIndex(
                name: "IX_PetitionWriterLicenses_LicenseNumber",
                schema: "org",
                table: "PetitionWriterLicenses",
                column: "LicenseNumber",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PetitionWriterLicenses_Location_ProvinceId",
                schema: "org",
                table: "PetitionWriterLicenses",
                column: "ProvinceId",
                principalSchema: "look",
                principalTable: "Location",
                principalColumn: "ID");
        }
    }
}
