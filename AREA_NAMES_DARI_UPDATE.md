# Area Names Update to Dari/Persian

## Problem
The "ناحیه مربوطه" (Related Area) dropdown in the company license form was showing English labels instead of Dari/Persian.

## Solution

### 1. Database Update (Run This First)
Execute the SQL script to update existing data:

```bash
psql -U postgres -d your_database_name -f Backend/Scripts/update_area_names_to_dari.sql
```

This will update all Area names from English to Dari/Persian:
- Construction → ساختمان سازی
- Real Estate → املاک و مستغلات
- Import/Export → واردات/صادرات
- Manufacturing → تولیدی
- Retail Trade → تجارت خرده فروشی
- Wholesale Trade → تجارت عمده فروشی
- Transportation → ترانسپورت
- Agriculture → زراعت
- Mining → معدن
- Tourism → توریزم
- Healthcare → صحت
- Education → تعلیم و تربیه
- Financial Services → خدمات مالی
- Technology → تکنالوژی
- Consulting → مشاوره

### 2. Code Update (Already Done)
Updated `Backend/Configuration/DatabaseSeeder.cs` to use Dari/Persian names for new installations.

### 3. Verify
1. Restart your backend application
2. Open the company license form
3. Check the "ناحیه مربوطه" dropdown - it should now show Dari/Persian labels

## Files Modified
- `Backend/Configuration/DatabaseSeeder.cs` - Updated seed data
- `Backend/Scripts/update_area_names_to_dari.sql` - Database update script
