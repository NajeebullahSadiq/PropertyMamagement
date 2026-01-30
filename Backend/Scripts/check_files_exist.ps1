# PowerShell script to check if Resource files exist on disk

Write-Host "Checking Resource files..." -ForegroundColor Cyan

# Get the backend directory
$scriptDir = $PSScriptRoot
$backendDir = Split-Path -Parent $scriptDir
$resourcesDir = Join-Path $backendDir "Resources"

Write-Host ""
Write-Host "Resources directory: $resourcesDir" -ForegroundColor Yellow

# Check if Resources directory exists
if (Test-Path $resourcesDir) {
    Write-Host "[OK] Resources directory exists" -ForegroundColor Green
    
    # Check subdirectories
    $subdirs = @("Documents", "Documents/Profile", "Documents/Property", "Documents/Identity", "Images")
    foreach ($subdir in $subdirs) {
        $path = Join-Path $resourcesDir $subdir
        if (Test-Path $path) {
            $fileCount = (Get-ChildItem $path -File -ErrorAction SilentlyContinue).Count
            Write-Host "  [OK] $subdir exists ($fileCount files)" -ForegroundColor Green
        } else {
            Write-Host "  [MISSING] $subdir does NOT exist" -ForegroundColor Red
        }
    }
    
    # List some sample files
    Write-Host ""
    Write-Host "Sample Profile photos:" -ForegroundColor Cyan
    $profileDir = Join-Path $resourcesDir "Documents/Profile"
    if (Test-Path $profileDir) {
        Get-ChildItem $profileDir -File | Select-Object -First 5 | ForEach-Object {
            Write-Host "  - $($_.Name)" -ForegroundColor White
        }
    }
    
    Write-Host ""
    Write-Host "Sample Property images:" -ForegroundColor Cyan
    $propertyDir = Join-Path $resourcesDir "Documents/Property"
    if (Test-Path $propertyDir) {
        Get-ChildItem $propertyDir -File | Select-Object -First 5 | ForEach-Object {
            Write-Host "  - $($_.Name)" -ForegroundColor White
        }
    }
    
    # Check specific files from the error
    Write-Host ""
    Write-Host "Checking specific files from error logs:" -ForegroundColor Cyan
    $testFiles = @(
        "Documents/Profile/profile_20251227_092027_197.jpg",
        "Documents/Profile/profile_20251227_055335_033.jpg"
    )
    
    foreach ($file in $testFiles) {
        $fullPath = Join-Path $resourcesDir $file
        if (Test-Path $fullPath) {
            $fileInfo = Get-Item $fullPath
            Write-Host "  [OK] $file exists ($($fileInfo.Length) bytes)" -ForegroundColor Green
        } else {
            Write-Host "  [MISSING] $file does NOT exist" -ForegroundColor Red
        }
    }
    
} else {
    Write-Host "[ERROR] Resources directory does NOT exist at: $resourcesDir" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please create the Resources directory structure:" -ForegroundColor Yellow
    Write-Host "  mkdir Resources" -ForegroundColor White
    Write-Host "  mkdir Resources/Documents" -ForegroundColor White
    Write-Host "  mkdir Resources/Documents/Profile" -ForegroundColor White
    Write-Host "  mkdir Resources/Documents/Property" -ForegroundColor White
    Write-Host "  mkdir Resources/Images" -ForegroundColor White
}

Write-Host ""
Write-Host "Done!" -ForegroundColor Cyan
