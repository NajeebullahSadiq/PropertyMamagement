# PowerShell script to replace DateTime.Now with DateTime.UtcNow in all controller files

$controllersPath = "Controllers"
$files = Get-ChildItem -Path $controllersPath -Filter "*.cs" -Recurse

$totalReplacements = 0

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    $originalContent = $content
    
    # Replace DateTime.Now with DateTime.UtcNow (but not DateTime.UtcNow itself)
    $content = $content -replace 'DateTime\.Now(?!U)', 'DateTime.UtcNow'
    
    if ($content -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
        $replacements = ([regex]::Matches($originalContent, 'DateTime\.Now(?!U)')).Count
        $totalReplacements += $replacements
        Write-Host "Fixed $replacements occurrences in $($file.Name)" -ForegroundColor Green
    }
}

Write-Host "`nTotal replacements: $totalReplacements" -ForegroundColor Cyan
Write-Host "All DateTime.Now instances have been replaced with DateTime.UtcNow" -ForegroundColor Green
