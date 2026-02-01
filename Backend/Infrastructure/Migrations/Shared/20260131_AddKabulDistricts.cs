using FluentMigrator;

namespace WebAPIBackend.Infrastructure.Migrations.Shared
{
    /// <summary>
    /// Migration to add 22 administrative districts (ناحیه) for Kabul province
    /// </summary>
    [Migration(20260131_01)]
    public class AddKabulDistricts : Migration
    {
        public override void Up()
        {
            // SQL script to add 22 districts for Kabul
            Execute.Sql(@"
                DO $$
                DECLARE
                    kabul_province_id INTEGER;
                    district_names TEXT[] := ARRAY[
                        'ناحیه اول',
                        'ناحیه دوم', 
                        'ناحیه سوم',
                        'ناحیه چهارم',
                        'ناحیه پنجم',
                        'ناحیه ششم',
                        'ناحیه هفتم',
                        'ناحیه هشتم',
                        'ناحیه نهم',
                        'ناحیه دهم',
                        'ناحیه یازدهم',
                        'ناحیه دوازدهم',
                        'ناحیه سیزدهم',
                        'ناحیه چهاردهم',
                        'ناحیه پانزدهم',
                        'ناحیه شانزدهم',
                        'ناحیه هفدهم',
                        'ناحیه هجدهم',
                        'ناحیه نوزدهم',
                        'ناحیه بیستم',
                        'ناحیه بیست و یکم',
                        'ناحیه بیست و دوم'
                    ];
                    district_name TEXT;
                    counter INTEGER := 1;
                BEGIN
                    -- Find Kabul province ID
                    SELECT ""ID"" INTO kabul_province_id
                    FROM look.""Location""
                    WHERE ""Dari"" LIKE '%کابل%' 
                      AND ""TypeID"" = 2
                    LIMIT 1;

                    -- Check if Kabul was found
                    IF kabul_province_id IS NULL THEN
                        RAISE EXCEPTION 'Kabul province not found in Location table';
                    END IF;

                    -- Insert 22 districts for Kabul
                    FOREACH district_name IN ARRAY district_names
                    LOOP
                        -- Check if district already exists
                        IF NOT EXISTS (
                            SELECT 1 FROM look.""Location""
                            WHERE ""ParentID"" = kabul_province_id
                              AND ""Dari"" = district_name
                              AND ""TypeID"" = 3
                        ) THEN
                            INSERT INTO look.""Location"" (
                                ""Dari"",
                                ""IsActive"",
                                ""ParentID"",
                                ""TypeID"",
                                ""Name"",
                                ""Path_Dari""
                            ) VALUES (
                                district_name,
                                1,
                                kabul_province_id,
                                3,
                                'District ' || counter,
                                'کابل/' || district_name
                            );
                        END IF;
                        
                        counter := counter + 1;
                    END LOOP;
                END $$;
            ");
        }

        public override void Down()
        {
            // Remove the 22 Kabul districts
            Execute.Sql(@"
                DELETE FROM look.""Location""
                WHERE ""ParentID"" IN (
                    SELECT ""ID"" FROM look.""Location"" 
                    WHERE ""Dari"" LIKE '%کابل%' AND ""TypeID"" = 2
                )
                AND ""TypeID"" = 3
                AND ""Dari"" IN (
                    'ناحیه اول', 'ناحیه دوم', 'ناحیه سوم', 'ناحیه چهارم', 'ناحیه پنجم',
                    'ناحیه ششم', 'ناحیه هفتم', 'ناحیه هشتم', 'ناحیه نهم', 'ناحیه دهم',
                    'ناحیه یازدهم', 'ناحیه دوازدهم', 'ناحیه سیزدهم', 'ناحیه چهاردهم', 'ناحیه پانزدهم',
                    'ناحیه شانزدهم', 'ناحیه هفدهم', 'ناحیه هجدهم', 'ناحیه نوزدهم', 'ناحیه بیستم',
                    'ناحیه بیست و یکم', 'ناحیه بیست و دوم'
                );
            ");
        }
    }
}
