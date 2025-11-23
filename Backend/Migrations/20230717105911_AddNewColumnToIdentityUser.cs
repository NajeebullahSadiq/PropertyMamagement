using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddNewColumnToIdentityUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "look");

            migrationBuilder.EnsureSchema(
                name: "tr");

            migrationBuilder.EnsureSchema(
                name: "org");

            migrationBuilder.EnsureSchema(
                name: "log");

            // Create sequences before tables that reference them
            migrationBuilder.Sql("CREATE SEQUENCE IF NOT EXISTS look.educationlevel_id_seq;");
            migrationBuilder.Sql("CREATE SEQUENCE IF NOT EXISTS look.location_id_seq;");
            migrationBuilder.Sql("CREATE SEQUENCE IF NOT EXISTS tr.\"WitnessDetails_Id_seq\";");

            // Add CompanyId column to AspNetUsers table (other custom columns already exist from first migration)
            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AddressType",
                schema: "look",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Des = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("AddressType_pkey", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Area",
                schema: "look",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Des = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Area_pkey", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyDetails",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: true),
                    DatabaseNumber = table.Column<int>(type: "integer", nullable: true),
                    PetitionDate = table.Column<DateOnly>(type: "date", nullable: true),
                    PetitionNumber = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: true),
                    DocPath = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("CompanyDetails_pkey", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EducationLevel",
                schema: "look",
                columns: table => new
                {
                    ID = table.Column<short>(type: "smallint", nullable: false, defaultValueSql: "nextval('look.educationlevel_id_seq'::regclass)"),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    parentid = table.Column<short>(type: "smallint", nullable: true),
                    Sorter = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_educationlevel", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "FormsReference",
                schema: "look",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Des = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("FormsReference_pkey", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GuaranteeType",
                schema: "look",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Des = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("GuaranteeType_pkey", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IdentityCardType",
                schema: "look",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Des = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("IdentityCardType_pkey", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Location",
                schema: "look",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('look.location_id_seq'::regclass)"),
                    Dari = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    IsActive = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: true),
                    Path = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PathDari = table.Column<string>(name: "Path_Dari", type: "character varying(255)", maxLength: 255, nullable: true),
                    ParentID = table.Column<int>(type: "integer", nullable: true),
                    TypeID = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_location", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "LostdocumentsType",
                schema: "look",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Des = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("LostdocumentsType_pkey", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PropertyType",
                schema: "look",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Des = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PropertyType_pkey", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PUnitType",
                schema: "look",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Des = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PUnitType_pkey", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "TransactionType",
                schema: "look",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Des = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("TransactionType_pkey", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ViolationType",
                schema: "look",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Des = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ViolationType_pkey", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "companydetailsaudit",
                schema: "log",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    OldValue = table.Column<string>(type: "text", nullable: true),
                    NewValue = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PropertyName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("companydetailsaudit_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "companydetailsaudit_CompanyId_fkey",
                        column: x => x.CompanyId,
                        principalSchema: "org",
                        principalTable: "CompanyDetails",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Haqulemtyaz",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FormNumber = table.Column<int>(type: "integer", nullable: true),
                    FormDate = table.Column<DateOnly>(type: "date", nullable: true),
                    SubmissionFormNumber = table.Column<int>(type: "integer", nullable: true),
                    SubmissionFormDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CompanyId = table.Column<int>(type: "integer", nullable: true),
                    DocPath = table.Column<string>(type: "text", nullable: true),
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

            migrationBuilder.CreateTable(
                name: "LicenseDetails",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LicenseNumber = table.Column<int>(type: "integer", nullable: false),
                    IssueDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ExpireDate = table.Column<DateOnly>(type: "date", nullable: true),
                    AreaId = table.Column<int>(type: "integer", nullable: true),
                    OfficeAddress = table.Column<string>(type: "text", nullable: true),
                    CompanyId = table.Column<int>(type: "integer", nullable: true),
                    DocPath = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("LicenseDetails_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "LicenseDetails_AreaId_fkey",
                        column: x => x.AreaId,
                        principalSchema: "look",
                        principalTable: "Area",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "LicenseDetails_CompanyId_fkey",
                        column: x => x.CompanyId,
                        principalSchema: "org",
                        principalTable: "CompanyDetails",
                        principalColumn: "Id");
                });

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
                    SubmissionDate = table.Column<DateOnly>(type: "date", nullable: true),
                    MaktobNumber = table.Column<string>(type: "text", nullable: true),
                    MaktobDate = table.Column<DateOnly>(type: "date", nullable: true),
                    DiagnosisNumber = table.Column<int>(type: "integer", nullable: true),
                    Details = table.Column<string>(type: "text", nullable: true),
                    DocPath = table.Column<string>(type: "text", nullable: true),
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

            migrationBuilder.CreateTable(
                name: "Gaurantee",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GuaranteeTypeId = table.Column<int>(type: "integer", nullable: true),
                    PropertyDocumentNumber = table.Column<int>(type: "integer", nullable: true),
                    PropertyDocumentDate = table.Column<DateOnly>(type: "date", nullable: true),
                    SenderMaktobNumber = table.Column<string>(type: "text", nullable: true),
                    SenderMaktobDate = table.Column<DateOnly>(type: "date", nullable: true),
                    AnswerdMaktobNumber = table.Column<int>(type: "integer", nullable: true),
                    AnswerdMaktobDate = table.Column<DateOnly>(type: "date", nullable: true),
                    DateofGuarantee = table.Column<DateOnly>(type: "date", nullable: true),
                    GuaranteeDocNumber = table.Column<int>(type: "integer", nullable: true),
                    GuaranteeDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CompanyId = table.Column<int>(type: "integer", nullable: true),
                    DocPath = table.Column<string>(type: "text", nullable: true),
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

            migrationBuilder.CreateTable(
                name: "CompanyOwner",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    FatherName = table.Column<string>(type: "text", nullable: false),
                    GrandFatherName = table.Column<string>(type: "text", nullable: true),
                    EducationLevelId = table.Column<short>(type: "smallint", nullable: true),
                    DateofBirth = table.Column<DateOnly>(type: "date", nullable: true),
                    IdentityCardTypeId = table.Column<int>(type: "integer", nullable: true),
                    IndentityCardNumber = table.Column<int>(type: "integer", nullable: true),
                    Jild = table.Column<string>(type: "text", nullable: true),
                    Safha = table.Column<string>(type: "text", nullable: true),
                    CompanyId = table.Column<int>(type: "integer", nullable: true),
                    SabtNumber = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: true),
                    PothoPath = table.Column<string>(type: "text", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "Guarantors",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    FatherName = table.Column<string>(type: "text", nullable: false),
                    IdentityCardTypeId = table.Column<int>(type: "integer", nullable: true),
                    CompanyId = table.Column<int>(type: "integer", nullable: true),
                    IndentityCardNumber = table.Column<int>(type: "integer", nullable: true),
                    Jild = table.Column<int>(type: "integer", nullable: true),
                    Safha = table.Column<int>(type: "integer", nullable: true),
                    SabtNumber = table.Column<int>(type: "integer", nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: true),
                    PothoPath = table.Column<string>(type: "text", nullable: true),
                    PaddressProvinceId = table.Column<int>(type: "integer", nullable: true),
                    PaddressDistrictId = table.Column<int>(type: "integer", nullable: true),
                    PaddressVillage = table.Column<string>(type: "text", nullable: true),
                    TaddressProvinceId = table.Column<int>(type: "integer", nullable: true),
                    TaddressDistrictId = table.Column<int>(type: "integer", nullable: true),
                    TaddressVillage = table.Column<string>(type: "text", nullable: true)
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
                        name: "Guarantors_PaddressDistrictId_fkey",
                        column: x => x.PaddressDistrictId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "Guarantors_PaddressProvinceId_fkey",
                        column: x => x.PaddressProvinceId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "Guarantors_TaddressDistrictId_fkey",
                        column: x => x.TaddressDistrictId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "Guarantors_TaddressProvinceId_fkey",
                        column: x => x.TaddressProvinceId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "PropertyDetails",
                schema: "tr",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PNumber = table.Column<int>(type: "integer", nullable: false),
                    PArea = table.Column<int>(type: "integer", nullable: false),
                    PUnitTypeId = table.Column<int>(type: "integer", nullable: true),
                    NumofFloor = table.Column<int>(type: "integer", nullable: true),
                    NumofRooms = table.Column<int>(type: "integer", nullable: true),
                    PropertyTypeId = table.Column<int>(type: "integer", nullable: true),
                    Price = table.Column<double>(type: "double precision", nullable: true),
                    PriceText = table.Column<string>(type: "text", nullable: true),
                    RoyaltyAmount = table.Column<double>(type: "double precision", nullable: true),
                    TransactionTypeId = table.Column<int>(type: "integer", nullable: true),
                    Des = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    FilePath = table.Column<string>(type: "text", nullable: true),
                    iscomplete = table.Column<bool>(type: "boolean", nullable: true, defaultValueSql: "false"),
                    iseditable = table.Column<bool>(type: "boolean", nullable: true, defaultValueSql: "false"),
                    west = table.Column<string>(type: "text", nullable: true),
                    south = table.Column<string>(type: "text", nullable: true),
                    east = table.Column<string>(type: "text", nullable: true),
                    north = table.Column<string>(type: "text", nullable: true),
                    doctype = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PropertyDetails_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "PropertyDetails_PUnitTypeId_fkey",
                        column: x => x.PUnitTypeId,
                        principalSchema: "look",
                        principalTable: "PUnitType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "PropertyDetails_PropertyTypeId_fkey",
                        column: x => x.PropertyTypeId,
                        principalSchema: "look",
                        principalTable: "PropertyType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "PropertyDetails_TransactionTypeId_fkey",
                        column: x => x.TransactionTypeId,
                        principalSchema: "look",
                        principalTable: "TransactionType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VehiclesPropertyDetails",
                schema: "tr",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PermitNo = table.Column<int>(type: "integer", nullable: false),
                    PilateNo = table.Column<int>(type: "integer", nullable: false),
                    TypeOfVehicle = table.Column<string>(type: "text", nullable: true),
                    Model = table.Column<string>(type: "text", nullable: true),
                    EnginNo = table.Column<int>(type: "integer", nullable: true),
                    ShasiNo = table.Column<int>(type: "integer", nullable: true),
                    Color = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<double>(type: "double precision", nullable: true),
                    PriceText = table.Column<string>(type: "text", nullable: true),
                    RoyaltyAmount = table.Column<double>(type: "double precision", nullable: true),
                    PropertyTypeId = table.Column<int>(type: "integer", nullable: true),
                    TransactionTypeId = table.Column<int>(type: "integer", nullable: true),
                    Des = table.Column<string>(type: "text", nullable: true),
                    FilePath = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    iscomplete = table.Column<bool>(type: "boolean", nullable: true),
                    iseditable = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("VehiclesPropertyDetails_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "VehiclesPropertyDetails_PropertyTypeId_fkey",
                        column: x => x.PropertyTypeId,
                        principalSchema: "look",
                        principalTable: "PropertyType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "VehiclesPropertyDetails_TransactionTypeId_fkey",
                        column: x => x.TransactionTypeId,
                        principalSchema: "look",
                        principalTable: "TransactionType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Violation",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ViolationTypeId = table.Column<int>(type: "integer", nullable: false),
                    ViolationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    NotifyDate = table.Column<DateOnly>(type: "date", nullable: true),
                    NumberOfViolation = table.Column<int>(type: "integer", nullable: true),
                    DateOfsummons = table.Column<DateOnly>(type: "date", nullable: true),
                    PresentedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Violation_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "Violation_ViolationTypeId_fkey",
                        column: x => x.ViolationTypeId,
                        principalSchema: "look",
                        principalTable: "ViolationType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "licenseaudit",
                schema: "log",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    LicenseId = table.Column<int>(type: "integer", nullable: false),
                    OldValue = table.Column<string>(type: "text", nullable: true),
                    NewValue = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PropertyName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("licenseaudit_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "licenseaudit_LicenseId_fkey",
                        column: x => x.LicenseId,
                        principalSchema: "org",
                        principalTable: "LicenseDetails",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "graunteeaudit",
                schema: "log",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GauranteeId = table.Column<int>(type: "integer", nullable: false),
                    OldValue = table.Column<string>(type: "text", nullable: true),
                    NewValue = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PropertyName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("graunteeaudit_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "graunteeaudit_GauranteeId_fkey",
                        column: x => x.GauranteeId,
                        principalSchema: "org",
                        principalTable: "Gaurantee",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CompanyOwnerAddress",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AddressTypeId = table.Column<int>(type: "integer", nullable: true),
                    ProvinceId = table.Column<int>(type: "integer", nullable: true),
                    DistrictId = table.Column<int>(type: "integer", nullable: true),
                    CompanyOwnerId = table.Column<int>(type: "integer", nullable: true),
                    Village = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("CompanyOwnerAddress_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "CompanyOwnerAddress_AddressTypeId_fkey",
                        column: x => x.AddressTypeId,
                        principalSchema: "look",
                        principalTable: "AddressType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "CompanyOwnerAddress_CompanyOwnerId_fkey",
                        column: x => x.CompanyOwnerId,
                        principalSchema: "org",
                        principalTable: "CompanyOwner",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "CompanyOwnerAddress_DistrictId_fkey",
                        column: x => x.DistrictId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "CompanyOwnerAddress_ProvinceId_fkey",
                        column: x => x.ProvinceId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "companyowneraudit",
                schema: "log",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    OldValue = table.Column<string>(type: "text", nullable: true),
                    NewValue = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PropertyName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("companyowneraudit_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "companyowneraudit_OwnerId_fkey",
                        column: x => x.OwnerId,
                        principalSchema: "org",
                        principalTable: "CompanyOwner",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "guarantorsaudit",
                schema: "log",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GuarantorsId = table.Column<int>(type: "integer", nullable: false),
                    OldValue = table.Column<string>(type: "text", nullable: true),
                    NewValue = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PropertyName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("guarantorsaudit_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "guarantorsaudit_GuarantorsId_fkey",
                        column: x => x.GuarantorsId,
                        principalSchema: "org",
                        principalTable: "Guarantors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BuyerDetails",
                schema: "tr",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    FatherName = table.Column<string>(type: "text", nullable: false),
                    GrandFather = table.Column<string>(type: "text", nullable: false),
                    IndentityCardNumber = table.Column<int>(type: "integer", nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    PaddressProvinceId = table.Column<int>(type: "integer", nullable: true),
                    PaddressDistrictId = table.Column<int>(type: "integer", nullable: true),
                    PaddressVillage = table.Column<string>(type: "text", nullable: true),
                    TaddressProvinceId = table.Column<int>(type: "integer", nullable: true),
                    TaddressDistrictId = table.Column<int>(type: "integer", nullable: true),
                    TaddressVillage = table.Column<string>(type: "text", nullable: true),
                    PropertyDetailsId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    photo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("BuyerDetails_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "BuyerDetails_PaddressDistrictId_fkey",
                        column: x => x.PaddressDistrictId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "BuyerDetails_PaddressProvinceId_fkey",
                        column: x => x.PaddressProvinceId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "BuyerDetails_PropertyDetailsId_fkey",
                        column: x => x.PropertyDetailsId,
                        principalSchema: "tr",
                        principalTable: "PropertyDetails",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "BuyerDetails_TaddressDistrictId_fkey",
                        column: x => x.TaddressDistrictId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "BuyerDetails_TaddressProvinceId_fkey",
                        column: x => x.TaddressProvinceId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "PropertyAddress",
                schema: "tr",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProvinceId = table.Column<int>(type: "integer", nullable: true),
                    DistrictId = table.Column<int>(type: "integer", nullable: true),
                    PropertyDetailsId = table.Column<int>(type: "integer", nullable: true),
                    Village = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PropertyAddress_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "PropertyAddress_PropertyDetailsId_fkey",
                        column: x => x.PropertyDetailsId,
                        principalSchema: "tr",
                        principalTable: "PropertyDetails",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "propertyaudit",
                schema: "log",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PropertyId = table.Column<int>(type: "integer", nullable: false),
                    OldValue = table.Column<string>(type: "text", nullable: true),
                    NewValue = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PropertyName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("propertyaudit_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "propertyaudit_PropertyId_fkey",
                        column: x => x.PropertyId,
                        principalSchema: "tr",
                        principalTable: "PropertyDetails",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SellerDetails",
                schema: "tr",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    FatherName = table.Column<string>(type: "text", nullable: false),
                    GrandFather = table.Column<string>(type: "text", nullable: false),
                    IndentityCardNumber = table.Column<int>(type: "integer", nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    PaddressProvinceId = table.Column<int>(type: "integer", nullable: true),
                    PaddressDistrictId = table.Column<int>(type: "integer", nullable: true),
                    PaddressVillage = table.Column<string>(type: "text", nullable: true),
                    TaddressProvinceId = table.Column<int>(type: "integer", nullable: true),
                    TaddressDistrictId = table.Column<int>(type: "integer", nullable: true),
                    TaddressVillage = table.Column<string>(type: "text", nullable: true),
                    PropertyDetailsId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    photo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SellerDetails_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "SellerDetails_PaddressDistrictId_fkey",
                        column: x => x.PaddressDistrictId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "SellerDetails_PaddressProvinceId_fkey",
                        column: x => x.PaddressProvinceId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "SellerDetails_PropertyDetailsId_fkey",
                        column: x => x.PropertyDetailsId,
                        principalSchema: "tr",
                        principalTable: "PropertyDetails",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "SellerDetails_TaddressDistrictId_fkey",
                        column: x => x.TaddressDistrictId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "SellerDetails_TaddressProvinceId_fkey",
                        column: x => x.TaddressProvinceId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "WitnessDetails",
                schema: "tr",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    FatherName = table.Column<string>(type: "text", nullable: false),
                    IndentityCardNumber = table.Column<int>(type: "integer", nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    PropertyDetailsId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("WitnessDetails_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "WitnessDetails_PropertyDetailsId_fkey",
                        column: x => x.PropertyDetailsId,
                        principalSchema: "tr",
                        principalTable: "PropertyDetails",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "vehicleaudit",
                schema: "log",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VehicleId = table.Column<int>(type: "integer", nullable: false),
                    OldValue = table.Column<string>(type: "text", nullable: true),
                    NewValue = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ColumnName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("vehicleaudit_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "vehicleaudit_VehicleId_fkey",
                        column: x => x.VehicleId,
                        principalSchema: "tr",
                        principalTable: "VehiclesPropertyDetails",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VehiclesBuyerDetails",
                schema: "tr",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    FatherName = table.Column<string>(type: "text", nullable: false),
                    GrandFather = table.Column<string>(type: "text", nullable: false),
                    IndentityCardNumber = table.Column<int>(type: "integer", nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    PaddressProvinceId = table.Column<int>(type: "integer", nullable: true),
                    PaddressDistrictId = table.Column<int>(type: "integer", nullable: true),
                    PaddressVillage = table.Column<string>(type: "text", nullable: true),
                    TaddressProvinceId = table.Column<int>(type: "integer", nullable: true),
                    TaddressDistrictId = table.Column<int>(type: "integer", nullable: true),
                    TaddressVillage = table.Column<string>(type: "text", nullable: true),
                    PropertyDetailsId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    photo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("VehiclesBuyerDetails_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "VehiclesBuyerDetails_PaddressDistrictId_fkey",
                        column: x => x.PaddressDistrictId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "VehiclesBuyerDetails_PaddressProvinceId_fkey",
                        column: x => x.PaddressProvinceId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "VehiclesBuyerDetails_PropertyDetailsId_fkey",
                        column: x => x.PropertyDetailsId,
                        principalSchema: "tr",
                        principalTable: "VehiclesPropertyDetails",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "VehiclesBuyerDetails_TaddressDistrictId_fkey",
                        column: x => x.TaddressDistrictId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "VehiclesBuyerDetails_TaddressProvinceId_fkey",
                        column: x => x.TaddressProvinceId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "VehiclesSellerDetails",
                schema: "tr",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    FatherName = table.Column<string>(type: "text", nullable: false),
                    GrandFather = table.Column<string>(type: "text", nullable: false),
                    IndentityCardNumber = table.Column<int>(type: "integer", nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    PaddressProvinceId = table.Column<int>(type: "integer", nullable: true),
                    PaddressDistrictId = table.Column<int>(type: "integer", nullable: true),
                    PaddressVillage = table.Column<string>(type: "text", nullable: true),
                    TaddressProvinceId = table.Column<int>(type: "integer", nullable: true),
                    TaddressDistrictId = table.Column<int>(type: "integer", nullable: true),
                    TaddressVillage = table.Column<string>(type: "text", nullable: true),
                    PropertyDetailsId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    photo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("VehiclesSellerDetails_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "VehiclesSellerDetails_PaddressDistrictId_fkey",
                        column: x => x.PaddressDistrictId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "VehiclesSellerDetails_PaddressProvinceId_fkey",
                        column: x => x.PaddressProvinceId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "VehiclesSellerDetails_PropertyDetailsId_fkey",
                        column: x => x.PropertyDetailsId,
                        principalSchema: "tr",
                        principalTable: "VehiclesPropertyDetails",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "VehiclesSellerDetails_TaddressDistrictId_fkey",
                        column: x => x.TaddressDistrictId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "VehiclesSellerDetails_TaddressProvinceId_fkey",
                        column: x => x.TaddressProvinceId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "VehiclesWitnessDetails",
                schema: "tr",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('tr.\"WitnessDetails_Id_seq\"'::regclass)"),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    FatherName = table.Column<string>(type: "text", nullable: false),
                    IndentityCardNumber = table.Column<int>(type: "integer", nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    PropertyDetailsId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("VehiclesWitnessDetails_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "VehiclesWitnessDetails_PropertyDetailsId_fkey",
                        column: x => x.PropertyDetailsId,
                        principalSchema: "tr",
                        principalTable: "VehiclesPropertyDetails",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "propertybuyeraudit",
                schema: "log",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BuyerId = table.Column<int>(type: "integer", nullable: false),
                    OldValue = table.Column<string>(type: "text", nullable: true),
                    NewValue = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ColumnName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("propertybuyeraudit_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "propertybuyeraudit_BuyerId_fkey",
                        column: x => x.BuyerId,
                        principalSchema: "tr",
                        principalTable: "BuyerDetails",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "propertyselleraudit",
                schema: "log",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SellerId = table.Column<int>(type: "integer", nullable: false),
                    OldValue = table.Column<string>(type: "text", nullable: true),
                    NewValue = table.Column<string>(type: "text", nullable: true),
                    ColumnName = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("propertyselleraudit_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "propertyselleraudit_SellerId_fkey",
                        column: x => x.SellerId,
                        principalSchema: "tr",
                        principalTable: "SellerDetails",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "vehiclebuyeraudit",
                schema: "log",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VehicleBuyerId = table.Column<int>(type: "integer", nullable: false),
                    OldValue = table.Column<string>(type: "text", nullable: true),
                    NewValue = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ColumnName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("vehiclebuyeraudit_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "vehiclebuyeraudit_VehicleBuyerId_fkey",
                        column: x => x.VehicleBuyerId,
                        principalSchema: "tr",
                        principalTable: "VehiclesBuyerDetails",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "vehicleselleraudit",
                schema: "log",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VehicleSellerId = table.Column<int>(type: "integer", nullable: false),
                    OldValue = table.Column<string>(type: "text", nullable: true),
                    NewValue = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ColumnName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("vehicleselleraudit_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "vehicleselleraudit_VehicleSellerId_fkey",
                        column: x => x.VehicleSellerId,
                        principalSchema: "tr",
                        principalTable: "VehiclesSellerDetails",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BuyerDetails_PaddressDistrictId",
                schema: "tr",
                table: "BuyerDetails",
                column: "PaddressDistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerDetails_PaddressProvinceId",
                schema: "tr",
                table: "BuyerDetails",
                column: "PaddressProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerDetails_PropertyDetailsId",
                schema: "tr",
                table: "BuyerDetails",
                column: "PropertyDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerDetails_TaddressDistrictId",
                schema: "tr",
                table: "BuyerDetails",
                column: "TaddressDistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerDetails_TaddressProvinceId",
                schema: "tr",
                table: "BuyerDetails",
                column: "TaddressProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_companydetailsaudit_CompanyId",
                schema: "log",
                table: "companydetailsaudit",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyOwner_CompanyId",
                schema: "org",
                table: "CompanyOwner",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyOwner_EducationLevelId",
                schema: "org",
                table: "CompanyOwner",
                column: "EducationLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyOwner_IdentityCardTypeId",
                schema: "org",
                table: "CompanyOwner",
                column: "IdentityCardTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyOwnerAddress_AddressTypeId",
                schema: "org",
                table: "CompanyOwnerAddress",
                column: "AddressTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyOwnerAddress_CompanyOwnerId",
                schema: "org",
                table: "CompanyOwnerAddress",
                column: "CompanyOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyOwnerAddress_DistrictId",
                schema: "org",
                table: "CompanyOwnerAddress",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyOwnerAddress_ProvinceId",
                schema: "org",
                table: "CompanyOwnerAddress",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_companyowneraudit_OwnerId",
                schema: "log",
                table: "companyowneraudit",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Gaurantee_CompanyId",
                schema: "org",
                table: "Gaurantee",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Gaurantee_GuaranteeTypeId",
                schema: "org",
                table: "Gaurantee",
                column: "GuaranteeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_graunteeaudit_GauranteeId",
                schema: "log",
                table: "graunteeaudit",
                column: "GauranteeId");

            migrationBuilder.CreateIndex(
                name: "IX_Guarantors_CompanyId",
                schema: "org",
                table: "Guarantors",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Guarantors_IdentityCardTypeId",
                schema: "org",
                table: "Guarantors",
                column: "IdentityCardTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Guarantors_PaddressDistrictId",
                schema: "org",
                table: "Guarantors",
                column: "PaddressDistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_Guarantors_PaddressProvinceId",
                schema: "org",
                table: "Guarantors",
                column: "PaddressProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Guarantors_TaddressDistrictId",
                schema: "org",
                table: "Guarantors",
                column: "TaddressDistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_Guarantors_TaddressProvinceId",
                schema: "org",
                table: "Guarantors",
                column: "TaddressProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_guarantorsaudit_GuarantorsId",
                schema: "log",
                table: "guarantorsaudit",
                column: "GuarantorsId");

            migrationBuilder.CreateIndex(
                name: "IX_Haqulemtyaz_CompanyId",
                schema: "org",
                table: "Haqulemtyaz",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_licenseaudit_LicenseId",
                schema: "log",
                table: "licenseaudit",
                column: "LicenseId");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseDetails_AreaId",
                schema: "org",
                table: "LicenseDetails",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseDetails_CompanyId",
                schema: "org",
                table: "LicenseDetails",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodicForm_CompanyId",
                schema: "org",
                table: "PeriodicForm",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodicForm_ReferenceId",
                schema: "org",
                table: "PeriodicForm",
                column: "ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyAddress_PropertyDetailsId",
                schema: "tr",
                table: "PropertyAddress",
                column: "PropertyDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_propertyaudit_PropertyId",
                schema: "log",
                table: "propertyaudit",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_propertybuyeraudit_BuyerId",
                schema: "log",
                table: "propertybuyeraudit",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyDetails_PropertyTypeId",
                schema: "tr",
                table: "PropertyDetails",
                column: "PropertyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyDetails_PUnitTypeId",
                schema: "tr",
                table: "PropertyDetails",
                column: "PUnitTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyDetails_TransactionTypeId",
                schema: "tr",
                table: "PropertyDetails",
                column: "TransactionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_propertyselleraudit_SellerId",
                schema: "log",
                table: "propertyselleraudit",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerDetails_PaddressDistrictId",
                schema: "tr",
                table: "SellerDetails",
                column: "PaddressDistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerDetails_PaddressProvinceId",
                schema: "tr",
                table: "SellerDetails",
                column: "PaddressProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerDetails_PropertyDetailsId",
                schema: "tr",
                table: "SellerDetails",
                column: "PropertyDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerDetails_TaddressDistrictId",
                schema: "tr",
                table: "SellerDetails",
                column: "TaddressDistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerDetails_TaddressProvinceId",
                schema: "tr",
                table: "SellerDetails",
                column: "TaddressProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_vehicleaudit_VehicleId",
                schema: "log",
                table: "vehicleaudit",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_vehiclebuyeraudit_VehicleBuyerId",
                schema: "log",
                table: "vehiclebuyeraudit",
                column: "VehicleBuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_VehiclesBuyerDetails_PaddressDistrictId",
                schema: "tr",
                table: "VehiclesBuyerDetails",
                column: "PaddressDistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_VehiclesBuyerDetails_PaddressProvinceId",
                schema: "tr",
                table: "VehiclesBuyerDetails",
                column: "PaddressProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_VehiclesBuyerDetails_PropertyDetailsId",
                schema: "tr",
                table: "VehiclesBuyerDetails",
                column: "PropertyDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_VehiclesBuyerDetails_TaddressDistrictId",
                schema: "tr",
                table: "VehiclesBuyerDetails",
                column: "TaddressDistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_VehiclesBuyerDetails_TaddressProvinceId",
                schema: "tr",
                table: "VehiclesBuyerDetails",
                column: "TaddressProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_vehicleselleraudit_VehicleSellerId",
                schema: "log",
                table: "vehicleselleraudit",
                column: "VehicleSellerId");

            migrationBuilder.CreateIndex(
                name: "IX_VehiclesPropertyDetails_PropertyTypeId",
                schema: "tr",
                table: "VehiclesPropertyDetails",
                column: "PropertyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_VehiclesPropertyDetails_TransactionTypeId",
                schema: "tr",
                table: "VehiclesPropertyDetails",
                column: "TransactionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_VehiclesSellerDetails_PaddressDistrictId",
                schema: "tr",
                table: "VehiclesSellerDetails",
                column: "PaddressDistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_VehiclesSellerDetails_PaddressProvinceId",
                schema: "tr",
                table: "VehiclesSellerDetails",
                column: "PaddressProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_VehiclesSellerDetails_PropertyDetailsId",
                schema: "tr",
                table: "VehiclesSellerDetails",
                column: "PropertyDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_VehiclesSellerDetails_TaddressDistrictId",
                schema: "tr",
                table: "VehiclesSellerDetails",
                column: "TaddressDistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_VehiclesSellerDetails_TaddressProvinceId",
                schema: "tr",
                table: "VehiclesSellerDetails",
                column: "TaddressProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_VehiclesWitnessDetails_PropertyDetailsId",
                schema: "tr",
                table: "VehiclesWitnessDetails",
                column: "PropertyDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_Violation_ViolationTypeId",
                schema: "org",
                table: "Violation",
                column: "ViolationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_WitnessDetails_PropertyDetailsId",
                schema: "tr",
                table: "WitnessDetails",
                column: "PropertyDetailsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "companydetailsaudit",
                schema: "log");

            migrationBuilder.DropTable(
                name: "CompanyOwnerAddress",
                schema: "org");

            migrationBuilder.DropTable(
                name: "companyowneraudit",
                schema: "log");

            migrationBuilder.DropTable(
                name: "graunteeaudit",
                schema: "log");

            migrationBuilder.DropTable(
                name: "guarantorsaudit",
                schema: "log");

            migrationBuilder.DropTable(
                name: "Haqulemtyaz",
                schema: "org");

            migrationBuilder.DropTable(
                name: "licenseaudit",
                schema: "log");

            migrationBuilder.DropTable(
                name: "LostdocumentsType",
                schema: "look");

            migrationBuilder.DropTable(
                name: "PeriodicForm",
                schema: "org");

            migrationBuilder.DropTable(
                name: "PropertyAddress",
                schema: "tr");

            migrationBuilder.DropTable(
                name: "propertyaudit",
                schema: "log");

            migrationBuilder.DropTable(
                name: "propertybuyeraudit",
                schema: "log");

            migrationBuilder.DropTable(
                name: "propertyselleraudit",
                schema: "log");

            migrationBuilder.DropTable(
                name: "Seta",
                schema: "org");

            migrationBuilder.DropTable(
                name: "vehicleaudit",
                schema: "log");

            migrationBuilder.DropTable(
                name: "vehiclebuyeraudit",
                schema: "log");

            migrationBuilder.DropTable(
                name: "vehicleselleraudit",
                schema: "log");

            migrationBuilder.DropTable(
                name: "VehiclesWitnessDetails",
                schema: "tr");

            migrationBuilder.DropTable(
                name: "Violation",
                schema: "org");

            migrationBuilder.DropTable(
                name: "WitnessDetails",
                schema: "tr");

            migrationBuilder.DropTable(
                name: "AddressType",
                schema: "look");

            migrationBuilder.DropTable(
                name: "CompanyOwner",
                schema: "org");

            migrationBuilder.DropTable(
                name: "Gaurantee",
                schema: "org");

            migrationBuilder.DropTable(
                name: "Guarantors",
                schema: "org");

            migrationBuilder.DropTable(
                name: "LicenseDetails",
                schema: "org");

            migrationBuilder.DropTable(
                name: "FormsReference",
                schema: "look");

            migrationBuilder.DropTable(
                name: "BuyerDetails",
                schema: "tr");

            migrationBuilder.DropTable(
                name: "SellerDetails",
                schema: "tr");

            migrationBuilder.DropTable(
                name: "VehiclesBuyerDetails",
                schema: "tr");

            migrationBuilder.DropTable(
                name: "VehiclesSellerDetails",
                schema: "tr");

            migrationBuilder.DropTable(
                name: "ViolationType",
                schema: "look");

            migrationBuilder.DropTable(
                name: "EducationLevel",
                schema: "look");

            migrationBuilder.DropTable(
                name: "GuaranteeType",
                schema: "look");

            migrationBuilder.DropTable(
                name: "IdentityCardType",
                schema: "look");

            migrationBuilder.DropTable(
                name: "Area",
                schema: "look");

            migrationBuilder.DropTable(
                name: "CompanyDetails",
                schema: "org");

            migrationBuilder.DropTable(
                name: "PropertyDetails",
                schema: "tr");

            migrationBuilder.DropTable(
                name: "Location",
                schema: "look");

            migrationBuilder.DropTable(
                name: "VehiclesPropertyDetails",
                schema: "tr");

            migrationBuilder.DropTable(
                name: "PUnitType",
                schema: "look");

            migrationBuilder.DropTable(
                name: "PropertyType",
                schema: "look");

            migrationBuilder.DropTable(
                name: "TransactionType",
                schema: "look");

            // Drop CompanyId column from AspNetUsers table
            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "AspNetUsers");

            // Drop sequences
            migrationBuilder.Sql("DROP SEQUENCE IF EXISTS look.educationlevel_id_seq;");
            migrationBuilder.Sql("DROP SEQUENCE IF EXISTS look.location_id_seq;");
            migrationBuilder.Sql("DROP SEQUENCE IF EXISTS tr.\"WitnessDetails_Id_seq\";");
        }
    }
}
