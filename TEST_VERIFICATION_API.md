# Test Verification API

## Current Status

Database check shows:
- ✓ Verification code `PRO-2026-D3XU8P` EXISTS
- ✓ Property ID 1 EXISTS
- ✓ Seller data EXISTS (1 seller)
- ✓ Buyer data EXISTS (1 buyer)
- ✓ Verification is ACTIVE (not revoked)

But API returns:
```json
{
  "isValid": false,
  "status": "Invalid",
  "sellerInfo": null,
  "buyerInfo": null
}
```

## The Problem

The VerificationService has a try-catch block that swallows exceptions:

```csharp
catch (Exception)
{
    await LogVerificationAttempt(verificationCode, ipAddress, false, "Error");
    result.IsValid = false;
    result.Status = "Invalid";
    return result;
}
```

This means if ANY exception occurs, it returns "Invalid" without telling us what went wrong.

## Possible Causes

1. **Exception in GetPropertyDocumentDataAsync**
   - Location lookup failing
   - Include() not loading related data
   - Database connection issue

2. **Signature Verification Failing**
   - Property data changed since verification was created
   - Signature algorithm mismatch

3. **DbContext Configuration Issue**
   - Locations DbSet not properly configured
   - Schema mismatch

## Solution: Add Logging

We need to see what exception is being thrown. Let's check the backend logs or add temporary logging.

### Check Backend Logs

Look for errors around the time you called the verification API:

```bash
# If using console logging
# Check the terminal where backend is running

# If using file logging
tail -f Backend/logs/app.log
```

### Temporary Fix: Add Logging to VerificationService

In `Backend/Services/Verification/VerificationService.cs`, change the catch block:

```csharp
catch (Exception ex)
{
    // Log the actual exception
    Console.WriteLine($"[VERIFICATION ERROR] Code: {verificationCode}, Error: {ex.Message}");
    Console.WriteLine($"[VERIFICATION ERROR] Stack: {ex.StackTrace}");
    
    await LogVerificationAttempt(verificationCode, ipAddress, false, "Error");
    result.IsValid = false;
    result.Status = "Invalid";
    return result;
}
```

Then restart the backend and try the verification again.

## Test the API

```bash
curl -v http://localhost:5143/api/verification/verify/PRO-2026-D3XU8P
```

Watch the backend console for the error message.

## Most Likely Issue

Based on the code, the most likely issue is:

**The `_context.Locations.FindAsync()` is failing because the Location table uses uppercase `ID` instead of `Id`.**

The VerificationService code does:
```csharp
var provinceLocation = await _context.Locations.FindAsync(address.ProvinceId.Value);
```

But if the Location entity has `ID` (uppercase) as the primary key, and the code is looking for `Id` (PascalCase), this could cause issues.

## Quick Fix

Check the Location model:

```bash
# Look at Backend/Models/Lookup/Location.cs
```

If it uses `ID` instead of `Id`, the Entity Framework mapping might be incorrect.

## Alternative: Regenerate Verification

Since the verification was created on Jan 16 and it's now Feb 3, the property data might have changed. Try:

1. Open Property ID 1 in the UI
2. Click "Print"
3. This will generate a NEW verification code
4. Test the new code

The new verification will have the current property data and should work.
