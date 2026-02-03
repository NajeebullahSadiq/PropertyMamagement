# CRITICAL: Verification Tables Missing

## Problem Identified
The verification system tables (`DocumentVerifications` and `VerificationLogs`) **do not exist** in your database!

This is why verification code `PRO-2026-D3XU8P` returns invalid - the entire verification system hasn't been set up yet.

## Quick Fix

Run this script to create the verification tables and check the code:

```bash
psql -U your_username -d your_database -f Backend/Scripts/setup_and_check_verification.sql
```

Or run the full verification module script:

```bash
psql -U your_username -d your_database -f Backend/Scripts/Modules/10_Verification_Initial.sql
```

## What This Creates

### 1. DocumentVerifications Table
Stores verification codes for all documents:
- Verification code (e.g., PRO-2026-D3XU8P)
- Document ID and type
- Digital signature
- Creation timestamp
- Revocation status

### 2. VerificationLogs Table
Audit trail for all verification attempts:
- Who verified
- When verified
- Success/failure status
- IP address

## After Creating Tables

Once the tables are created, you need to **generate verification codes** for existing documents:

### For Properties
1. Open each property in the UI
2. Click "Print"
3. Verification code will be generated automatically

### For Vehicles
1. Open each vehicle in the UI
2. Click "Print"
3. Verification code will be generated automatically

### For Licenses
1. Open each license in the UI
2. Click "Print"
3. Verification code will be generated automatically

## Why This Happened

The verification system was added to the codebase but the database migration was never run. This can happen when:

1. Migration scripts exist but weren't executed
2. Database was restored from an old backup
3. Development environment wasn't fully synced

## Verification System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Document Creation                         │
│  (Property, Vehicle, License)                               │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                    Print Document                            │
│  User clicks "Print" button                                 │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│              Generate Verification Code                      │
│  POST /api/verification/generate                            │
│  - Creates record in DocumentVerifications table            │
│  - Generates unique code (PRO-2026-XXXXXX)                  │
│  - Creates digital signature                                │
│  - Returns QR code URL                                      │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                  Display QR Code                             │
│  Shows on printed document                                  │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                  User Scans QR Code                          │
│  Opens: https://prmis.gov.af/verify/PRO-2026-XXXXXX        │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                Verify Document                               │
│  GET /api/verification/verify/PRO-2026-XXXXXX              │
│  - Looks up code in DocumentVerifications table            │
│  - Retrieves document data                                 │
│  - Verifies digital signature                              │
│  - Returns document details + seller/buyer info            │
│  - Logs attempt in VerificationLogs table                  │
└─────────────────────────────────────────────────────────────┘
```

## Testing After Setup

### 1. Verify Tables Exist
```sql
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'org' 
AND table_name IN ('DocumentVerifications', 'VerificationLogs');
```

Expected: 2 rows

### 2. Create Test Property
1. Create a new property with seller and buyer
2. Mark it as complete
3. Click "Print"

### 3. Check Verification Code Generated
```sql
SELECT * FROM org."DocumentVerifications" 
ORDER BY "CreatedAt" DESC 
LIMIT 1;
```

Should show the newly generated verification code.

### 4. Test Verification
```bash
# Use the verification code from step 3
curl http://localhost:5143/api/verification/verify/PRO-2026-XXXXXX
```

Should return valid status with seller/buyer info.

## Migration Order

If you're setting up a fresh database, run migrations in this order:

1. `01_Shared_Initial.sql` - Shared lookup tables
2. `02_UserManagement_Initial.sql` - Users and roles
3. `03_Company_Initial.sql` - Company management
4. `04_Property_Initial.sql` - Property module
5. `05_Vehicle_Initial.sql` - Vehicle module
6. `06_Securities_Initial.sql` - Securities module
7. `07_Audit_Initial.sql` - Audit trails
8. `08_LicenseApplication_Initial.sql` - License applications
9. `09_PetitionWriterLicense_Initial.sql` - Petition writer licenses
10. **`10_Verification_Initial.sql`** ← YOU ARE HERE
11. `11_ActivityMonitoring_Initial.sql` - Activity monitoring

## Common Issues After Setup

### Issue 1: "Verification code not found"
**Cause**: Code was never generated
**Fix**: Open document and click "Print"

### Issue 2: "Invalid signature"
**Cause**: Document data changed after verification
**Fix**: Regenerate verification code

### Issue 3: "No seller/buyer info"
**Cause**: Document missing seller/buyer data
**Fix**: Add seller/buyer, then regenerate verification

## Files Created

1. **Backend/Scripts/setup_and_check_verification.sql**
   - Creates verification tables
   - Checks if code exists
   - Provides diagnostic info

2. **Backend/Scripts/Modules/10_Verification_Initial.sql**
   - Full verification module setup
   - Includes comments and documentation

3. **VERIFICATION_TABLES_MISSING.md** (this file)
   - Explains the missing tables issue
   - Provides setup instructions

## Next Steps

1. ✅ Run `setup_and_check_verification.sql` to create tables
2. ✅ Check if `PRO-2026-D3XU8P` exists in database
3. ✅ If not, find the property and regenerate verification
4. ✅ Test verification endpoint
5. ✅ Generate verification codes for all existing documents

## Prevention

To prevent this in production:

1. **Always run all migrations** when deploying
2. **Check migration status** before deployment
3. **Test verification system** after deployment
4. **Document required migrations** in deployment guide
5. **Add health check** endpoint that verifies tables exist
