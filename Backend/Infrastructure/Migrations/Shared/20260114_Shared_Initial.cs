using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebAPIBackend.Infrastructure.Migrations.Shared
{
    /// <summary>
    /// Initial migration for Shared/Lookup tables.
    /// Schema: look
    /// Dependencies: None
    /// </summary>
    public partial class Shared_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ensure schema exists
            migrationBuilder.EnsureSchema(name: "look");

            // AddressType
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

            // Area
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

            // EducationLevel
            migrationBuilder.Sql(@"
                CREATE SEQUENCE IF NOT EXISTS look.educationlevel_id_seq;
            ");
            
            migrationBuilder.CreateTable(
                name: "EducationLevel",
                schema: "look",
                columns: table => new
                {
                    ID = table.Column<short>(type: "smallint", nullable: false, defaultValueSql: "nextval('look.educationlevel_id_seq'::regclass)"),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Dari = table.Column<string>(type: "text", nullable: true),
                    parentid = table.Column<short>(type: "smallint", nullable: true),
                    Sorter = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_educationlevel", x => x.ID);
                });

            // FormsReference
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

            // GuaranteeType
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

            // IdentityCardType
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

            // Location
            migrationBuilder.Sql(@"
                CREATE SEQUENCE IF NOT EXISTS look.location_id_seq;
            ");

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
                    Path_Dari = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ParentID = table.Column<int>(type: "integer", nullable: true),
                    TypeID = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_location", x => x.ID);
                });

            // LostdocumentsType
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

            // PropertyType
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

            // PUnitType
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

            // TransactionType
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

            // ViolationType
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ViolationType", schema: "look");
            migrationBuilder.DropTable(name: "TransactionType", schema: "look");
            migrationBuilder.DropTable(name: "PUnitType", schema: "look");
            migrationBuilder.DropTable(name: "PropertyType", schema: "look");
            migrationBuilder.DropTable(name: "LostdocumentsType", schema: "look");
            migrationBuilder.DropTable(name: "Location", schema: "look");
            migrationBuilder.DropTable(name: "IdentityCardType", schema: "look");
            migrationBuilder.DropTable(name: "GuaranteeType", schema: "look");
            migrationBuilder.DropTable(name: "FormsReference", schema: "look");
            migrationBuilder.DropTable(name: "EducationLevel", schema: "look");
            migrationBuilder.DropTable(name: "Area", schema: "look");
            migrationBuilder.DropTable(name: "AddressType", schema: "look");
            
            migrationBuilder.Sql("DROP SEQUENCE IF EXISTS look.educationlevel_id_seq;");
            migrationBuilder.Sql("DROP SEQUENCE IF EXISTS look.location_id_seq;");
        }
    }
}
