# Securities Entry Manager Edit Permission Fix

## Problem
User with role `SECURITIES_ENTRY_MANAGER` has `securities.edit` permission in their JWT token, but gets 403 Forbidden when trying to edit securities distribution records.

## Root Cause Analysis

### JWT Token Shows Correct Permissions:
```json
{
  "userRole": "SECURITIES_ENTRY_MANAGER",
  "permission": [
    "securities.view",
    "securities.create",
    "dashboard.view",
    "securities.edit"  ← Permission exists!
  ]
}
```

### But Backend Authorization Blocks:
```csharp
[HttpPut("{id}")]
[Authorize(Roles = "ADMIN,COMPANY_REGISTRAR,PROPERTY_OPERATOR,SECURITIES_MANAGER")]
// ❌ SECURITIES_ENTRY_MANAGER is missing!
```

### The Issue:
The controller uses **role-based authorization** (`[Authorize(Roles = "...")]`) instead of **permission-based authorization**. Even though the user has the `securities.edit` permission, they're blocked because their role isn't in the allowed roles list.

## The Fix

### File: `Backend/Controllers/Securities/SecuritiesDistributionController.cs`

**Line 252 - Changed:**
```csharp
[Authorize(Roles = "ADMIN,COMPANY_REGISTRAR,PROPERTY_OPERATOR,SECURITIES_MANAGER")]
```

**To:**
```csharp
[Authorize(Roles = "ADMIN,COMPANY_REGISTRAR,PROPERTY_OPERATOR,SECURITIES_MANAGER,SECURITIES_ENTRY_MANAGER")]
```

### File: `Backend/Controllers/Securities/PetitionWriterSecuritiesController.cs`

**Line 229 - Already Fixed:**
```csharp
[Authorize(Roles = "ADMIN,COMPANY_REGISTRAR,PROPERTY_OPERATOR,SECURITIES_MANAGER,PETITION_WRITER_SECURITIES_ENTRY_MANAGER")]
```

## Why This Happened

The system has **two authorization mechanisms**:

1. **Role-based**: `[Authorize(Roles = "ROLE1,ROLE2")]` - Checks user's role
2. **Permission-based**: `[Authorize(Policy = "PolicyName")]` - Checks user's permissions

The securities controllers use role-based authorization, which is more restrictive. When you added `securities.edit` permission to `SECURITIES_ENTRY_MANAGER` role, the permission was granted but the role wasn't added to the controller's allowed roles list.

## Proper Solution (Long-term)

### Option 1: Switch to Permission-Based Authorization (Recommended)

Instead of checking roles, check permissions:

```csharp
[HttpPut("{id}")]
[Authorize] // Just require authentication
public async Task<IActionResult> Update(int id, [FromBody] SecuritiesDistributionData data)
{
    // Check permission manually
    var hasPermission = User.Claims.Any(c => 
        c.Type == "permission" && c.Value == "securities.edit");
    
    if (!hasPermission)
    {
        return Forbid();
    }
    
    // ... rest of the code
}
```

Or create a policy:

**In Program.cs:**
```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanEditSecurities", policy => 
        policy.RequireClaim("permission", "securities.edit"));
});
```

**In Controller:**
```csharp
[HttpPut("{id}")]
[Authorize(Policy = "CanEditSecurities")]
public async Task<IActionResult> Update(int id, [FromBody] SecuritiesDistributionData data)
```

### Option 2: Keep Role-Based (Current Fix)

Continue using role-based authorization but ensure all relevant roles are included. This is what we did.

## Testing

### Test Case 1: SECURITIES_ENTRY_MANAGER Can Edit
1. Login as user with `SECURITIES_ENTRY_MANAGER` role
2. Go to securities list
3. Click edit on a record
4. Make changes
5. Click save
6. ✅ Should succeed (no 403 error)

### Test Case 2: SECURITIES_ENTRY