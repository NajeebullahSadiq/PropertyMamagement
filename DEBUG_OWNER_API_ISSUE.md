# Debug Owner API Issue

## Debug Code Added

Added debug logging to see if the query finds any owners:

```csharp
var count = await _context.CompanyOwners.CountAsync(x => x.CompanyId == id);
Console.WriteLine($"DEBUG: Found {count} owners for CompanyId {id}");
```

## Steps to Test

### 1. Rebuild Backend
```bash
cd Backend
dotnet build
```

### 2. Restart Backend
```bash
dotnet run
```

### 3. Call the API
```bash
curl http://localhost:5143/api/CompanyOwner/2
```

### 4. Check Console Output

Look at the backend console for the debug message:

**Expected Output:**
```
DEBUG: Found 1 owners for CompanyId 2
```

**If you see:**
- `Found 0 owners` → The WHERE clause isn't matching (CompanyId issue)
- `Found 1 owners` but still returns `[]` → The Select projection is failing
- No debug message → The endpoint isn't being hit

---

## Possible Issues

### Issue 1: CompanyId Type Mismatch
The `CompanyId` in the database might be stored differently than expected.

**Test Query:**
```sql
SELECT 
    "Id",
    "CompanyId",
    pg_typeof("CompanyId") as type,
    "FirstName"
FROM org."CompanyOwners"
WHERE "Id" = 5;
```

### Issue 2: Authorization Blocking
The `[Authorize(Roles = "ADMIN,COMPANY_REGISTRAR")]` attribute might be blocking the request.

**Test:** Try calling the API while logged in as ADMIN

### Issue 3: Select Projection Failing
The `.Select()` might be throwing an exception that's being caught silently.

**Solution:** Check the catch block or remove the Select temporarily

---

## Next Steps Based on Debug Output

### If "Found 0 owners":
The WHERE clause isn't working. Try:
```csharp
.Where(x => x.CompanyId == id)  // Use == instead of .Equals()
```

### If "Found 1 owners" but returns []:
The Select projection is failing. Check:
1. Navigation properties are NULL
2. Exception in Select
3. Data type conversion issues

### If no debug message:
1. Check if you're logged in
2. Check authorization
3. Check if the endpoint is being hit

---

## Test Commands

```bash
# Rebuild
cd Backend
dotnet build

# Restart
dotnet run

# Test (watch the console output)
curl http://localhost:5143/api/CompanyOwner/2
```

Look for the "DEBUG: Found X owners" message in the console!
