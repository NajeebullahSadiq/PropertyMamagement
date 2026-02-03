# Quick Fix: Property Verification Code Invalid

## Problem
`PRO-2026-D3XU8P` returns invalid with no seller/buyer info

## ⚠️ CRITICAL: Check Tables Exist First!

The verification tables might not exist in your database. Run this first:

```bash
psql -U your_username -d your_database -f Backend/Scripts/setup_and_check_verification.sql
```

This will:
- Create verification tables if missing
- Check if the verification code exists
- Provide diagnostic information

See `VERIFICATION_TABLES_MISSING.md` for details.

## Quick Fix (Choose One)

### Option A: Regenerate from UI (Easiest)
1. Login → Property List
2. Find property → Click "Print"
3. New verification code generated automatically
4. Done!

### Option B: Run Diagnostic
```bash
psql -U postgres -d your_database -f Backend/Scripts/diagnose_verification_PRO-2026-D3XU8P.sql
```
Follow the recommendations in the output.

### Option C: Generate via API
```bash
# Replace PROPERTY_ID and TOKEN
curl -X POST http://localhost:5143/api/verification/generate \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{"documentId": PROPERTY_ID, "documentType": "PropertyDocument"}'
```

## Test the Fix
```bash
curl http://localhost:5143/api/verification/verify/PRO-2026-D3XU8P
```

Should return `"isValid": true` with seller/buyer info.

## Most Common Causes
1. ❌ Verification code never generated (print page closed too early)
2. ❌ Property has no seller/buyer data
3. ❌ Property was modified after verification generated

## Files to Help You
- `VERIFICATION_ISSUE_SUMMARY.md` - Complete guide
- `PROPERTY_VERIFICATION_DEBUG.md` - Detailed debugging
- `Backend/Scripts/diagnose_verification_PRO-2026-D3XU8P.sql` - Diagnostic script
