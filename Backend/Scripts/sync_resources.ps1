# Sync Resources between source and bin directories
# This script copies uploaded files from bin/Debug/net9.0/Resources to Backend/Resources

$sourceDir = "Backend\bin\Debug\net9.0\Resources"
$destDir = "Backend\Resources"

Write-Host "Syncing Resources from bin to source directory..." -ForegroundColor Cyan

if (-not (Test-Path $sourceDir)) {
    Write-Host "[ERROR] Source directory does not exist: $sourceDir" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path $destDir)) {
    Write-Host "[INFO] Creating destination directory: $destDir" -ForegroundColor Yellow
    New-Item -ItemType Directory -Path $destDir -Force | Out-Null
}

# Copy all files recursively, preserving structure
Write-Host "Copying files..." -ForegroundColor Yellow
Copy-Item -Path "$sourceDir\*" -Destination $destDir -Recurse -Force

Write-Host "[SUCCESS] Resources synced successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "Files copied to: $destDir" -ForegroundColor Cyan

# Show summary
$profileCount = (Get-ChildItem -Path "$destDir\Documents\Profile" -File -ErrorAction SilentlyContinue).Count
$propertyCount = (Get-ChildItem -Path "$destDir\Documents\Property" -File -ErrorAction SilentlyContinue).Count
$companyCount = (Get-ChildItem -Path "$destDir\Documents\Company" -File -ErrorAction SilentlyContinue).Count

Write-Host ""
Write-Host "Summary:" -ForegroundColor Cyan
Write-Host "  Profile photos: $profileCount" -ForegroundColor White
Write-Host "  Property images: $propertyCount" -ForegroundColor White
Write-Host "  Company photos: $companyCount" -ForegroundColor White
