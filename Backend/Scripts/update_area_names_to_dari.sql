-- Update Area names from English to Dari/Persian
-- This script updates the existing Area records to use Dari/Persian labels

UPDATE look."Area"
SET 
    "Name" = CASE "Name"
        WHEN 'Construction' THEN 'ساختمان سازی'
        WHEN 'Real Estate' THEN 'املاک و مستغلات'
        WHEN 'Import/Export' THEN 'واردات/صادرات'
        WHEN 'Manufacturing' THEN 'تولیدی'
        WHEN 'Retail Trade' THEN 'تجارت خرده فروشی'
        WHEN 'Wholesale Trade' THEN 'تجارت عمده فروشی'
        WHEN 'Transportation' THEN 'ترانسپورت'
        WHEN 'Agriculture' THEN 'زراعت'
        WHEN 'Mining' THEN 'معدن'
        WHEN 'Tourism' THEN 'توریزم'
        WHEN 'Healthcare' THEN 'صحت'
        WHEN 'Education' THEN 'تعلیم و تربیه'
        WHEN 'Financial Services' THEN 'خدمات مالی'
        WHEN 'Technology' THEN 'تکنالوژی'
        WHEN 'Consulting' THEN 'مشاوره'
        ELSE "Name"
    END,
    "Des" = CASE "Name"
        WHEN 'Construction' THEN 'خدمات ساختمان سازی و تعمیرات'
        WHEN 'Real Estate' THEN 'خدمات املاک و معاملات ملکی'
        WHEN 'Import/Export' THEN 'تجارت واردات و صادرات'
        WHEN 'Manufacturing' THEN 'تولید و ساخت'
        WHEN 'Retail Trade' THEN 'خرده فروشی و فروش'
        WHEN 'Wholesale Trade' THEN 'عمده فروشی'
        WHEN 'Transportation' THEN 'خدمات حمل و نقل'
        WHEN 'Agriculture' THEN 'فعالیت های زراعتی'
        WHEN 'Mining' THEN 'استخراج معادن'
        WHEN 'Tourism' THEN 'توریزم و مهمان نوازی'
        WHEN 'Healthcare' THEN 'خدمات صحی'
        WHEN 'Education' THEN 'خدمات آموزشی'
        WHEN 'Financial Services' THEN 'بانکداری و خدمات مالی'
        WHEN 'Technology' THEN 'خدمات تکنالوژی معلوماتی'
        WHEN 'Consulting' THEN 'خدمات مشاوره حرفوی'
        ELSE "Des"
    END
WHERE "Name" IN (
    'Construction', 'Real Estate', 'Import/Export', 'Manufacturing',
    'Retail Trade', 'Wholesale Trade', 'Transportation', 'Agriculture',
    'Mining', 'Tourism', 'Healthcare', 'Education', 'Financial Services',
    'Technology', 'Consulting'
);

-- Verify the update
SELECT "Id", "Name", "Des" FROM look."Area" ORDER BY "Id";
