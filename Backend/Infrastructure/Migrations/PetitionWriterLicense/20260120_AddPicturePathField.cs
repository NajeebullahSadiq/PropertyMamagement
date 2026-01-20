using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Infrastructure.Migrations.PetitionWriterLicense
{
    /// <summary>
    /// Add PicturePath field to PetitionWriterLicenses table
    /// </summary>
    public partial class AddPicturePathField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PicturePath",
                schema: "org",
                table: "PetitionWriterLicenses",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PicturePath",
                schema: "org",
                table: "PetitionWriterLicenses");
        }
    }
}
