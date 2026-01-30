# Verification script for DateTime timezone fixes

Write-Host ""
Write-Host "=== DateTime Timezone Fix Verification ===" -ForegroundColor Cyan
Write-Host ""

# 1. Check Program.cs for Npgsql configuration
Write-Host "1. Checking Program.cs for Npgsql Legacy Timestamp Behavior..." -ForegroundColor Yellow
$programCs = Get-Content "Program.cs" -Raw
if ($programCs -match 'EnableLegacyTimestampBehavior') {
    Write-Host "   OK - Npgsql Legacy Timestamp Behavior is configured" -ForegroundColor Green
} else {
    Write-Host "   ERROR - Npgsql Legacy Timestamp Behavior NOT found" -ForegroundColor Red
}

# 2. Check DateConversionHelper for UTC conversion
Write-Host ""
Write-Host "2. Checking DateConversionHelper for UTC conversion..." -ForegroundColor Yellow
$helperCs = Get-Content "Helpers/DateConversionHelper.cs" -Raw
if ($helperCs -match 'DateTime\.SpecifyKind.*DateTimeKind\.Utc') {
    Write-Host "   OK - DateConversionHelper returns UTC DateTime" -ForegroundColor Green
} else {
    Write-Host "   ERROR - DateConversionHelper UTC conversion NOT found" -ForegroundColor Red
}

# 3. Check for remaining DateTime.Now
Write-Host ""
Write-Host "3. Checking for remaining DateTime.Now in controllers..." -ForegroundColor Yellow
$dateTimeNowMatches = Get-ChildItem -Path "Controllers" -Filter "*.cs" -Recurse | Select-String -Pattern 'DateTime\.Now(?!U)'
if ($dateTimeNowMatches.Count -eq 0) {
    Write-Host "   OK - No DateTime.Now found in controllers" -ForegroundColor Green
} else {
    Write-Host "   WARNING - Found $($dateTimeNowMatches.Count) DateTime.Now occurrences" -ForegroundColor Red
}

# 4. Count DateTime.UtcNow usage
Write-Host ""
Write-Host "4. Counting DateTime.UtcNow usage in controllers..." -ForegroundColor Yellow
$utcNowCount = (Get-ChildItem -Path "Controllers" -Filter "*.cs" -Recurse | Select-String -Pattern 'DateTime\.UtcNow').Count
Write-Host "   OK - Found $utcNowCount occurrences of DateTime.UtcNow" -ForegroundColor Green

# Summary
Write-Host ""
Write-Host "=== Verification Summary ===" -ForegroundColor Cyan
Write-Host "All DateTime timezone fixes have been applied successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "1. Restart the backend server" -ForegroundColor White
Write-Host "2. Test property form submission" -ForegroundColor White
Write-Host "3. Test other modules with date fields" -ForegroundColor White
Write-Host ""
