# Fix Audit Table ID Auto-Generation

## Issue
When updating migrated company/license data, getting error:
```
null value in column "Id" of relation "licenseaudit" violates not-null constraint
```

## Root Cause
The audit tables (Licenseaudit, Companydetailsaudit, etc.) were created without proper SERIAL configuration for the Id column, so PostgreSQL is not auto-generating IDs.

## Solution Applied

### 1. Updated EF Core Configuration
Added `.UseIdentityColumn()` to all audit entity configurations in `AppDbContext.cs`:
- Licenseaudit
- Guarantorsaudit
- Graunteeaudit
- Companyowneraudit
- Companydetailsaudit

### 2. Created Database Fix Script
Created `Backend/Scripts/fix_audit_table_sequences.sql` to:
- Check all audit tables for missing SERIAL configuration
- Create sequences if they don't exist
- Set sequences to start from max existing Id + 1
- Alter columns to use the sequences
- Verify all audit tables have proper SERIAL configuration

## Deployment Steps

### On Production Server:

#### 1. Run the Database Fix Script
```bash
cd ~/PropertyMamagement/Backend
psql -h 127.0.0.1 -U prmis_user -d PRMIS -f Scripts/fix_audit_table_sequences.sql
```

**Expected Output:**
```
NOTICE:  Fixing licenseaudit table - Id column is not SERIAL
NOTICE:  Fixed licenseaudit table successfully
NOTICE:  Fixing Companydetailsaudit table - Id column is not SERIAL
NOTICE:  Fixed Companydetailsaudit table successfully
...

 table_name            | column_name | column_default                        | status
-----------------------+-------------+---------------------------------------+-----------
 Companydetailsaudit   | Id          | nextval('..._Id_seq'::regclass)      | ✓ SERIAL
 Companyowneraudit     | Id          | nextval('..._Id_seq'::regclass)      | ✓ SERIAL
 Graunteeaudit         | Id          | nextval('..._Id_seq'::regclass)      | ✓ SERIAL
 Guarantorsaudit       | Id          | nextval('..._Id_seq'::regclass)      | ✓ SERIAL
 Licenseaudit          | Id          | nextval('..._Id_seq'::regclass)      | ✓ SERIAL
```

#### 2. Rebuild Backend
```bash
cd ~/PropertyMamagement/Backend
dotnet build
```

#### 3. Restart Backend Service
```bash
sudo systemctl restart prmis-backend
sudo systemctl status prmis-backend
```

#### 4. Test License Update
Try updating a migrated license record through the UI. The audit logging should now work without errors.

## Verification

### Test Audit Logging:
```bash
psql -h 127.0.0.1 -U prmis_user -d PRMIS
```

```sql
-- Check if audit records are being created
SELECT * FROM log."Licenseaudit" ORDER BY "Id" DESC LIMIT 5;

-- Check sequence values
SELECT 
    schemaname,
    sequencename,
    last_value
FROM pg_sequences
WHERE schemaname = 'log'
ORDER BY sequencename;
```

### Test in UI:
1. Open a migrated company record
2. Go to License tab
3. Edit any field (e.g., change office address)
4. Save changes
5. Should save successfully without "null value in column Id" error

## Files Modified

### Backend Code:
- `Backend/Configuration/AppDbContext.cs` - Added `.UseIdentityColumn()` to all audit entities

### Database Scripts:
- `Backend/Scripts/fix_audit_table_sequences.sql` - New script to fix audit table sequences

## Technical Details

### What Changed in AppDbContext.cs:

**Before:**
```csharp
entity.Property(e => e.Id)
    .ValueGeneratedOnAdd();
```

**After:**
```csharp
entity.Property(e => e.Id)
    .ValueGeneratedOnAdd()
    .UseIdentityColumn();
```

### What the SQL Script Does:

1. **Checks each audit table** to see if Id column has a default value (SERIAL)
2. **Creates sequence** if it doesn't exist: `log."TableName_Id_seq"`
3. **Sets sequence value** to max existing Id + 1
4. **Alters column** to use the sequence as default
5. **Verifies** all tables are properly configured

### Why This Happened:

The audit tables were likely created manually or through a migration that didn't properly set up SERIAL columns. When EF Core tries to insert audit records, it expects PostgreSQL to auto-generate the Id, but without SERIAL configuration, PostgreSQL tries to insert NULL, causing the constraint violation.

## Success Criteria

✅ All audit tables have SERIAL Id columns  
✅ Sequences are created and set to correct values  
✅ Backend builds without errors  
✅ License updates work without audit errors  
✅ Audit records are created with auto-generated IDs  

## Troubleshooting

### If Script Fails:

**Check table names (case-sensitive):**
```sql
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'log' 
AND LOWER(table_name) LIKE '%audit';
```

**Manually fix a specific table:**
```sql
-- Example for licenseaudit
CREATE SEQUENCE IF NOT EXISTS log."licenseaudit_Id_seq";
SELECT setval('log."licenseaudit_Id_seq"', COALESCE((SELECT MAX("Id") FROM log."licenseaudit"), 0) + 1, false);
ALTER TABLE log."licenseaudit" ALTER COLUMN "Id" SET DEFAULT nextval('log."licenseaudit_Id_seq"'::regclass);
```

### If Still Getting Errors:

1. Check if the table name case matches exactly
2. Verify the sequence was created
3. Check if the column default is set
4. Restart the backend service

## Related Issues

This same fix may be needed for other audit tables:
- Property audit tables (Propertyaudit, Propertybuyeraudit, Propertyselleraudit)
- Vehicle audit tables (Vehicleaudit, Vehiclebuyeraudit, Vehicleselleraudit)

If you encounter similar errors with those tables, run the same fix script - it handles all audit tables automatically.
