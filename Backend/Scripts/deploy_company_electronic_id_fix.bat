@echo off
REM Deploy Company Electronic ID Column Type Fix
REM This script fixes the ElectronicNationalIdNumber column type in Company module tables

echo ==========================================
echo Company Electronic ID Column Type Fix
echo ==========================================
echo.

REM Database connection details
set DB_NAME=prmis
set DB_USER=postgres
set DB_HOST=localhost

echo This script will:
echo 1. Convert ElectronicNationalIdNumber from double precision to VARCHAR(50) in CompanyOwner table
echo 2. Convert ElectronicNationalIdNumber from double precision to VARCHAR(50) in Guarantor table
echo.
echo Database: %DB_NAME%
echo User: %DB_USER%
echo Host: %DB_HOST%
echo.
set /p confirm="Do you want to continue? (yes/no): "

if /i not "%confirm%"=="yes" (
    echo Deployment cancelled.
    exit /b 0
)

echo.
echo Applying fix...
psql -h %DB_HOST% -U %DB_USER% -d %DB_NAME% -f fix_company_electronic_id_columns.sql

if %errorlevel% equ 0 (
    echo.
    echo ==========================================
    echo Fix applied successfully!
    echo ==========================================
    echo.
    echo Please restart the backend service if running on Linux:
    echo   sudo systemctl restart prmis-backend
    echo.
) else (
    echo.
    echo ==========================================
    echo ERROR: Fix failed!
    echo ==========================================
    echo Please check the error messages above.
    exit /b 1
)
