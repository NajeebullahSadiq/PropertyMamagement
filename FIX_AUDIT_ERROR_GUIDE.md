# Quick Fix Guide: License Update Audit Error

## The Error You're Seeing
```
23502: null value in column "Id" of relation "licenseaudit" violates not-null constraint
```

## Quick Fix (2 Steps)

### Step 1: Run the Database Fix Script
Open your PostgreSQL client (pgAdmin, psql, or any tool) and run:

```bash
# Using psql command line:
psql -U postgres -d your_database_name -f Backend/Scripts/fix_licenseaudit_id.sql

# Or copy/paste the script content directly in pgAdmin Query Tool
```

**Important:** Replace `your_database_name` with your actual database name.

### Step 2: Rebuild and Restart Your Backend
```bash
cd Backend
dotnet build
dotnet run
```

## What This Fixes
The script automatically configures all 11 audit tables to generate IDs automatically when new audit records are created. This prevents the "null value" error.

## Verify the Fix
1. Try updating a company license again
2. The update should succeed without errors
3. Check the `log.licenseaudit` table - you should see new audit records with auto-generated IDs

## If You Still Have Issues
- Make sure you're connected to the correct database
- Check that your database user has permission to create sequences
- Verify the `log` schema exists in your database

## Technical Details
See `AUDIT_TABLE_ID_FIX_SUMMARY.md` for complete technical explanation.
