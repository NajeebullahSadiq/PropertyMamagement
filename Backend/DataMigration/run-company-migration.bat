@echo off
echo ========================================
echo Company/License Data Migration
echo ========================================
echo.
echo This will migrate company data from:
echo   - mainform_full_records.json
echo   - guarantee_records.json
echo.
echo To PostgreSQL database tables:
echo   - org.CompanyDetails
echo   - org.CompanyOwner
echo   - org.LicenseDetails
echo   - org.Guarantors
echo.
pause
echo.
echo Starting migration...
echo.
dotnet run company
echo.
echo ========================================
echo Migration Complete!
echo ========================================
pause
