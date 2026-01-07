using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEducationLevelDariTranslations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update Dari column with proper Dari/Pashto translations
            migrationBuilder.Sql(@"
                UPDATE look.""EducationLevel"" SET ""Dari"" = 'بی سواد' WHERE ""Name"" = 'Illiterate';
                UPDATE look.""EducationLevel"" SET ""Dari"" = 'مکتب ابتدایی' WHERE ""Name"" = 'Primary School';
                UPDATE look.""EducationLevel"" SET ""Dari"" = 'مکتب متوسطه' WHERE ""Name"" = 'Secondary School';
                UPDATE look.""EducationLevel"" SET ""Dari"" = 'لیسه' WHERE ""Name"" = 'High School';
                UPDATE look.""EducationLevel"" SET ""Dari"" = 'دیپلوم' WHERE ""Name"" = 'Diploma';
                UPDATE look.""EducationLevel"" SET ""Dari"" = 'لیسانس' WHERE ""Name"" = 'Bachelor''s Degree';
                UPDATE look.""EducationLevel"" SET ""Dari"" = 'ماستر' WHERE ""Name"" = 'Master''s Degree';
                UPDATE look.""EducationLevel"" SET ""Dari"" = 'دکتورا' WHERE ""Name"" = 'PhD/Doctorate';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reset Dari column to NULL
            migrationBuilder.Sql(@"
                UPDATE look.""EducationLevel"" SET ""Dari"" = NULL;
            ");
        }
    }
}
