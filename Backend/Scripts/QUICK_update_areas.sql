-- Quick update for Area names to Dari/Persian
-- Copy and paste this entire script into pgAdmin or psql

UPDATE look."Area" SET "Name" = 'ساختمان سازی', "Des" = 'خدمات ساختمان سازی و تعمیرات' WHERE "Name" = 'Construction';
UPDATE look."Area" SET "Name" = 'املاک و مستغلات', "Des" = 'خدمات املاک و معاملات ملکی' WHERE "Name" = 'Real Estate';
UPDATE look."Area" SET "Name" = 'واردات/صادرات', "Des" = 'تجارت واردات و صادرات' WHERE "Name" = 'Import/Export';
UPDATE look."Area" SET "Name" = 'تولیدی', "Des" = 'تولید و ساخت' WHERE "Name" = 'Manufacturing';
UPDATE look."Area" SET "Name" = 'تجارت خرده فروشی', "Des" = 'خرده فروشی و فروش' WHERE "Name" = 'Retail Trade';
UPDATE look."Area" SET "Name" = 'تجارت عمده فروشی', "Des" = 'عمده فروشی' WHERE "Name" = 'Wholesale Trade';
UPDATE look."Area" SET "Name" = 'ترانسپورت', "Des" = 'خدمات حمل و نقل' WHERE "Name" = 'Transportation';
UPDATE look."Area" SET "Name" = 'زراعت', "Des" = 'فعالیت های زراعتی' WHERE "Name" = 'Agriculture';
UPDATE look."Area" SET "Name" = 'معدن', "Des" = 'استخراج معادن' WHERE "Name" = 'Mining';
UPDATE look."Area" SET "Name" = 'توریزم', "Des" = 'توریزم و مهمان نوازی' WHERE "Name" = 'Tourism';
UPDATE look."Area" SET "Name" = 'صحت', "Des" = 'خدمات صحی' WHERE "Name" = 'Healthcare';
UPDATE look."Area" SET "Name" = 'تعلیم و تربیه', "Des" = 'خدمات آموزشی' WHERE "Name" = 'Education';
UPDATE look."Area" SET "Name" = 'خدمات مالی', "Des" = 'بانکداری و خدمات مالی' WHERE "Name" = 'Financial Services';
UPDATE look."Area" SET "Name" = 'تکنالوژی', "Des" = 'خدمات تکنالوژی معلوماتی' WHERE "Name" = 'Technology';
UPDATE look."Area" SET "Name" = 'مشاوره', "Des" = 'خدمات مشاوره حرفوی' WHERE "Name" = 'Consulting';

-- Check the results
SELECT "Id", "Name", "Des" FROM look."Area" ORDER BY "Id";
