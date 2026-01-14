using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebAPIBackend.Infrastructure.Migrations.Company
{
    /// <summary>
    /// Initial migration for Company/Organization tables.
    /// Schema: org
    /// Dependencies: Shared (look schema), UserManagement
    /// </summary>
    public partial class Company_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(name: "org");

            // CompanyDetails - Main company table
            migrationBuilder.CreateTable(
                name: "CompanyDetails",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: true),
                    LicenseNumber = table.Column<string>(type: "text", nullable: true),
                    PetitionDate = table.Column<DateOnly>(type: "date", nullable: true),
                    PetitionNumber = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: true),
                    DocPath = table.Column<string>(type: "text", nullable: true),
                    TIN = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("CompanyDetails_pkey", x => x.Id);
                });

            // CompanyOwner
            migrationBuilder.CreateTable(
                name: "CompanyOwner",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    FatherName = table.Column<string>(type: "text", nullable: true),
                    GrandFatherName = table.Column<string>(type: "text", nullable: true),
                    TazkiraNumber = table.Column<string>(type: "text", nullable: true),
                    TazkiraPage = table.Column<string>(type: "text", nullable: true),
                    TazkiraVolume = table.Column<string>(type: "text", nullable: true),
                    TazkiraRegNumber = table.Column<string>(type: "text", nullable: true),
                    NationalIdNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    WhatsAppNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Photo = table.Column<string>(type: "text", nullable: true),
                    CompanyId = table.Column<int>(type: "integer", nullable: true),
                    EducationLevelId = table.Column<short>(type: "smallint", nullable: true),
                    IdentityCardTypeId = table.Column<int>(type: "integer", nullable: true),
                    OwnerProvinceId = table.Column<int>(type: "integer", nullable: true),
                    OwnerDistrictId = table.Column<int>(type: "integer", nullable: true),
                    OwnerVillage = table.Column<string>(type: "text", nullable: true),
                    PermanentProvinceId = table.Column<int>(type: "integer", nullable: true),
                    PermanentDistrictId = table.Column<int>(type: "integer", nullable: true),
                    PermanentVillage = table.Column<string>(type: "text", nullable: true),
                    TemporaryProvinceId = table.Column<int>(type: "integer", nullable: true),
                    TemporaryDistrictId = table.Column<int>(type: "integer", nullable: true),
                    TemporaryVillage = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("CompanyOwner_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "CompanyOwner_CompanyId_fkey",
                        column: x => x.CompanyId,
                        principalSchema: "org",
                        principalTable: "CompanyDetails",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "CompanyOwner_EducationLevelId_fkey",
                        column: x => x.EducationLevelId,
                        principalSchema: "look",
                        principalTable: "EducationLevel",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "CompanyOwner_IdentityCardTypeId_fkey",
                        column: x => x.IdentityCardTypeId,
                        principalSchema: "look",
                        principalTable: "IdentityCardType",
                        principalColumn: "Id");
                });

            // CompanyOwnerAddress
            migrationBuilder.CreateTable(
                name: "CompanyOwnerAddress",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyOwnerId = table.Column<int>(type: "integer", nullable: true),
                    AddressTypeId = table.Column<int>(type: "integer", nullable: true),
                    ProvinceId = table.Column<int>(type: "integer", nullable: true),
                    DistrictId = table.Column<int>(type: "integer", nullable: true),
                    Village = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("CompanyOwnerAddress_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "CompanyOwnerAddress_CompanyOwnerId_fkey",
                        column: x => x.CompanyOwnerId,
                        principalSchema: "org",
                        principalTable: "CompanyOwner",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "CompanyOwnerAddress_AddressTypeId_fkey",
                        column: x => x.AddressTypeId,
                        principalSchema: "look",
                        principalTable: "AddressType",
                        principalColumn: "Id");
                });

            // CompanyOwnerAddressHistory
            migrationBuilder.CreateTable(
                name: "CompanyOwnerAddressHistory",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyOwnerId = table.Column<int>(type: "integer", nullable: false),
                    AddressType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ProvinceId = table.Column<int>(type: "integer", nullable: true),
                    DistrictId = table.Column<int>(type: "integer", nullable: true),
                    Village = table.Column<string>(type: "text", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("CompanyOwnerAddressHistory_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "CompanyOwnerAddressHistory_CompanyOwnerId_fkey",
                        column: x => x.CompanyOwnerId,
                        principalSchema: "org",
                        principalTable: "CompanyOwner",
                        principalColumn: "Id");
                });

            // LicenseDetails
            migrationBuilder.CreateTable(
                name: "LicenseDetails",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LicenseNumber = table.Column<string>(type: "text", nullable: true),
                    LicenseType = table.Column<string>(type: "text", nullable: true),
                    LicenseCategory = table.Column<string>(type: "text", nullable: true),
                    LicenseDate = table.Column<DateOnly>(type: "date", nullable: true),
                    LicenseExpireDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CompanyId = table.Column<int>(type: "integer", nullable: true),
                    AreaId = table.Column<int>(type: "integer", nullable: true),
                    TaxPaymentAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    CompanyCommission = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("LicenseDetails_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "LicenseDetails_CompanyId_fkey",
                        column: x => x.CompanyId,
                        principalSchema: "org",
                        principalTable: "CompanyDetails",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "LicenseDetails_AreaId_fkey",
                        column: x => x.AreaId,
                        principalSchema: "look",
                        principalTable: "Area",
                        principalColumn: "Id");
                });

            // Gaurantee
            migrationBuilder.CreateTable(
                name: "Gaurantee",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GuaranteeTypeId = table.Column<int>(type: "integer", nullable: true),
                    GuaranteeNumber = table.Column<long>(type: "bigint", nullable: true),
                    GuaranteeDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CompanyId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Gaurantee_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "Gaurantee_CompanyId_fkey",
                        column: x => x.CompanyId,
                        principalSchema: "org",
                        principalTable: "CompanyDetails",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "Gaurantee_GuaranteeTypeId_fkey",
                        column: x => x.GuaranteeTypeId,
                        principalSchema: "look",
                        principalTable: "GuaranteeType",
                        principalColumn: "Id");
                });

            // Guarantors
            migrationBuilder.CreateTable(
                name: "Guarantors",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    FatherName = table.Column<string>(type: "text", nullable: true),
                    GrandFatherName = table.Column<string>(type: "text", nullable: true),
                    TazkiraNumber = table.Column<string>(type: "text", nullable: true),
                    TazkiraPage = table.Column<string>(type: "text", nullable: true),
                    TazkiraVolume = table.Column<string>(type: "text", nullable: true),
                    TazkiraRegNumber = table.Column<string>(type: "text", nullable: true),
                    NationalIdNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: true),
                    Photo = table.Column<string>(type: "text", nullable: true),
                    CompanyId = table.Column<int>(type: "integer", nullable: true),
                    IdentityCardTypeId = table.Column<int>(type: "integer", nullable: true),
                    GuaranteeTypeId = table.Column<int>(type: "integer", nullable: true),
                    PaddressProvinceId = table.Column<int>(type: "integer", nullable: true),
                    PaddressDistrictId = table.Column<int>(type: "integer", nullable: true),
                    PaddressVillage = table.Column<string>(type: "text", nullable: true),
                    TaddressProvinceId = table.Column<int>(type: "integer", nullable: true),
                    TaddressDistrictId = table.Column<int>(type: "integer", nullable: true),
                    TaddressVillage = table.Column<string>(type: "text", nullable: true),
                    GuaranteeDistrictId = table.Column<int>(type: "integer", nullable: true),
                    CourtName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CollateralNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    SetSerialNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BankName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    DepositNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Guarantors_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "Guarantors_CompanyId_fkey",
                        column: x => x.CompanyId,
                        principalSchema: "org",
                        principalTable: "CompanyDetails",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "Guarantors_IdentityCardTypeId_fkey",
                        column: x => x.IdentityCardTypeId,
                        principalSchema: "look",
                        principalTable: "IdentityCardType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "Guarantors_GuaranteeTypeId_fkey",
                        column: x => x.GuaranteeTypeId,
                        principalSchema: "look",
                        principalTable: "GuaranteeType",
                        principalColumn: "Id");
                });

            // Haqulemtyaz
            migrationBuilder.CreateTable(
                name: "Haqulemtyaz",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HaqulemtyazNumber = table.Column<int>(type: "integer", nullable: true),
                    HaqulemtyazDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CompanyId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Haqulemtyaz_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "Haqulemtyaz_CompanyId_fkey",
                        column: x => x.CompanyId,
                        principalSchema: "org",
                        principalTable: "CompanyDetails",
                        principalColumn: "Id");
                });

            // PeriodicForm
            migrationBuilder.CreateTable(
                name: "PeriodicForm",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReferenceId = table.Column<int>(type: "integer", nullable: true),
                    FormNumber = table.Column<int>(type: "integer", nullable: true),
                    FormDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CompanyId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PeriodicForm_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "PeriodicForm_CompanyId_fkey",
                        column: x => x.CompanyId,
                        principalSchema: "org",
                        principalTable: "CompanyDetails",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "PeriodicForm_ReferenceId_fkey",
                        column: x => x.ReferenceId,
                        principalSchema: "look",
                        principalTable: "FormsReference",
                        principalColumn: "Id");
                });

            // Seta
            migrationBuilder.CreateTable(
                name: "Seta",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TransactionTypeId = table.Column<int>(type: "integer", nullable: true),
                    InquiryNumber = table.Column<int>(type: "integer", nullable: true),
                    InquiryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    SetaSerialNumber = table.Column<int>(type: "integer", nullable: true),
                    SetaStampedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CompanyId = table.Column<int>(type: "integer", nullable: true),
                    DocPath = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Seta_pkey", x => x.Id);
                });

            // CompanyAccountInfo
            migrationBuilder.CreateTable(
                name: "CompanyAccountInfo",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    SettlementInfo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TaxPaymentAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    CompanyCommission = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("CompanyAccountInfo_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyAccountInfo_CompanyDetails",
                        column: x => x.CompanyId,
                        principalSchema: "org",
                        principalTable: "CompanyDetails",
                        principalColumn: "Id");
                });

            // CompanyCancellationInfo
            migrationBuilder.CreateTable(
                name: "CompanyCancellationInfo",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    LicenseCancellationLetterNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LicenseCancellationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    RevenueCancellationLetterNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RevenueCancellationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Remarks = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("CompanyCancellationInfo_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyCancellationInfo_CompanyDetails",
                        column: x => x.CompanyId,
                        principalSchema: "org",
                        principalTable: "CompanyDetails",
                        principalColumn: "Id");
                });

            // Create indexes
            migrationBuilder.CreateIndex(name: "IX_CompanyOwner_CompanyId", schema: "org", table: "CompanyOwner", column: "CompanyId");
            migrationBuilder.CreateIndex(name: "IX_CompanyOwner_EducationLevelId", schema: "org", table: "CompanyOwner", column: "EducationLevelId");
            migrationBuilder.CreateIndex(name: "IX_CompanyOwnerAddress_CompanyOwnerId", schema: "org", table: "CompanyOwnerAddress", column: "CompanyOwnerId");
            migrationBuilder.CreateIndex(name: "IX_LicenseDetails_CompanyId", schema: "org", table: "LicenseDetails", column: "CompanyId");
            migrationBuilder.CreateIndex(name: "IX_LicenseDetails_AreaId", schema: "org", table: "LicenseDetails", column: "AreaId");
            migrationBuilder.CreateIndex(name: "IX_Gaurantee_CompanyId", schema: "org", table: "Gaurantee", column: "CompanyId");
            migrationBuilder.CreateIndex(name: "IX_Guarantors_CompanyId", schema: "org", table: "Guarantors", column: "CompanyId");
            migrationBuilder.CreateIndex(name: "IX_Haqulemtyaz_CompanyId", schema: "org", table: "Haqulemtyaz", column: "CompanyId");
            migrationBuilder.CreateIndex(name: "IX_PeriodicForm_CompanyId", schema: "org", table: "PeriodicForm", column: "CompanyId");
            migrationBuilder.CreateIndex(name: "IX_CompanyAccountInfo_CompanyId", schema: "org", table: "CompanyAccountInfo", column: "CompanyId");
            migrationBuilder.CreateIndex(name: "IX_CompanyCancellationInfo_CompanyId", schema: "org", table: "CompanyCancellationInfo", column: "CompanyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "CompanyCancellationInfo", schema: "org");
            migrationBuilder.DropTable(name: "CompanyAccountInfo", schema: "org");
            migrationBuilder.DropTable(name: "Seta", schema: "org");
            migrationBuilder.DropTable(name: "PeriodicForm", schema: "org");
            migrationBuilder.DropTable(name: "Haqulemtyaz", schema: "org");
            migrationBuilder.DropTable(name: "Guarantors", schema: "org");
            migrationBuilder.DropTable(name: "Gaurantee", schema: "org");
            migrationBuilder.DropTable(name: "LicenseDetails", schema: "org");
            migrationBuilder.DropTable(name: "CompanyOwnerAddressHistory", schema: "org");
            migrationBuilder.DropTable(name: "CompanyOwnerAddress", schema: "org");
            migrationBuilder.DropTable(name: "CompanyOwner", schema: "org");
            migrationBuilder.DropTable(name: "CompanyDetails", schema: "org");
        }
    }
}
