# Verify Resources folder setup for the backend

Write-Host "=== Property Module Resources Verification ===" -ForegroundColor Cyan
Write-Host ""

# Get the backend root directory (parent of Scripts)
$backendRoot = Split-Path -Parent $PSScriptRoot
$projectRoot = Split-Path -Parent $backendRoot

# Check source directory (Backend/Resources)
$sourceDir = Join-Path $backendRoot "Resources"
Write-Host "1. Source Directory: $sourceDir" -ForegroundColor Yellow
if (Test-Path $sourceDir) {
    Write-Host "   [OK] Exists" -ForegroundColor Green
    $sourceFolders = Get-ChildItem -Path $sourceDir -Directory -Recurse | Select-Object -ExpandProperty FullName
    Write-Host "   Folders: $($sourceFolders.Count)" -ForegroundColor White
} else {
    Write-Host "   [MISSING] Does not exist" -ForegroundColor Red
}
Write-Host ""

# Check bin directory (where backend runs from)
$binDir = Join-Path $backendRoot "bin\Debug\net9.0\Resources"
Write-Host "2. Runtime Directory: $binDir" -ForegroundColor Yellow
if (Test-Path $binDir) {
    Write-Host "   [OK] Exists" -ForegroundColor Green
    
    # Check specific folders
    $folders = @(
        "Documents\Profile",
        "Documents\Identity",
        "Documents\Property",
        "Documents\Vehicle",
        "Documents\Company",
        "Images"
    )
    
    foreach ($folder in $folders) {
        $fullPath = Join-Path $binDir $folder
        if (Test-Path $fullPath) {
            $fileCount = (Get-ChildItem -Path $fullPath -File -ErrorAction SilentlyContinue).Count
            Write-Host "   [OK] $folder ($fileCount files)" -ForegroundColor Green
        } else {
            Write-Host "   [MISSING] $folder" -ForegroundColor Red
        }
    }
} else {
    Write-Host "   [MISSING] Does not exist" -ForegroundColor Red
    Write-Host "   Note: This is normal if backend hasn't been run yet" -ForegroundColor Yellow
}
Write-Host ""

# Check for profile photos
Write-Host "3. Profile Photos:" -ForegroundColor Yellow
$profilePath = Join-Path $binDir "Documents\Profile"
if (Test-Path $profilePath) {
    $photos = Get-ChildItem -Path $profilePath -File -Include *.jpg,*.jpeg,*.png
    if ($photos.Count -gt 0) {
        Write-Host "   Found $($photos.Count) photos:" -ForegroundColor Green
        $photos | Select-Object -First 5 | ForEach-Object {
            Write-Host "   - $($_.Name) ($([math]::Round($_.Length/1KB, 2)) KB)" -ForegroundColor White
        }
        if ($photos.Count -gt 5) {
            Write-Host "   ... and $($photos.Count - 5) more" -ForegroundColor White
        }
    } else {
        Write-Host "   [INFO] No photos found (users need to upload)" -ForegroundColor Yellow
    }
} else {
    Write-Host "   [INFO] Profile folder doesn't exist yet" -ForegroundColor Yellow
}
Write-Host ""

# Check Program.cs configuration
Write-Host "4. Backend Configuration:" -ForegroundColor Yellow
$programCs = Join-Path $backendRoot "Program.cs"
if (Test-Path $programCs) {
    $content = Get-Content $programCs -Raw
    
    if ($content -match 'UseStaticFiles.*RequestPath.*"/api/Resources"') {
        Write-Host "   [OK] Static file serving configured" -ForegroundColor Green
    } else {
        Write-Host "   [WARNING] Static file serving may not be configured" -ForegroundColor Red
    }
    
    if ($content -match 'Directory\.CreateDirectory.*Profile') {
        Write-Host "   [OK] Auto-creates Profile folder" -ForegroundColor Green
    } else {
        Write-Host "   [WARNING] May not auto-create Profile folder" -ForegroundColor Red
    }
} else {
    Write-Host "   [ERROR] Program.cs not found at: $programCs" -ForegroundColor Red
}
Write-Host ""

# Summary
Write-Host "=== Summary ===" -ForegroundColor Cyan
Write-Host "The backend is configured to:" -ForegroundColor White
Write-Host "  1. Auto-create Resources folder structure on startup" -ForegroundColor White
Write-Host "  2. Serve static files from /api/Resources/" -ForegroundColor White
Write-Host "  3. Save uploads to bin/Debug/net9.0/Resources/" -ForegroundColor White
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "  1. Restart the backend server" -ForegroundColor White
Write-Host "  2. Upload new photos in Property module" -ForegroundColor White
Write-Host "  3. Verify photos display correctly" -ForegroundColor White
Write-Host ""
