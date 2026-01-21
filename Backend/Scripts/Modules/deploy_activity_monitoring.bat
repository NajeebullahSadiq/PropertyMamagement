@echo off
REM =====================================================
REM Activity Monitoring Module Deployment Script (Windows)
REM =====================================================
REM This script deploys only the Activity Monitoring module
REM Safe for existing databases (uses IF NOT EXISTS)
REM =====================================================

setlocal enabledelayedexpansion

REM Database connection parameters (set these or use environment variables)
if "%DB_HOST%"=="" set DB_HOST=localhost
if "%DB_PORT%"=="" set DB_PORT=5432
if "%DB_NAME%"=="" set DB_NAME=prmis_db
if "%DB_USER%"=="" set DB_USER=postgres

echo ==========================================
echo Activity Monitoring Module Deployment
echo ==========================================
echo.
echo Database: %DB_NAME%
echo Host: %DB_HOST%:%DB_PORT%
echo User: %DB_USER%
echo.

REM Check if psql is available
where psql >nul 2>nul
if %ERRORLEVEL% neq 0 (
    echo Error: psql command not found. Please install PostgreSQL client.
    echo Add PostgreSQL bin directory to your PATH.
    pause
    exit /b 1
)

REM Prompt for password if not set
if "%PGPASSWORD%"=="" (
    set /p PGPASSWORD="Enter database password: "
)

REM Test database connection
echo Testing database connection...
psql -h %DB_HOST% -p %DB_PORT% -U %DB_USER% -d %DB_NAME% -c "SELECT 1;" >nul 2>nul
if %ERRORLEVEL% neq 0 (
    echo Error: Cannot connect to database.
    echo Please check your connection parameters and credentials.
    pause
    exit /b 1
)
echo [OK] Database connection successful
echo.

REM Deploy Activity Monitoring module
echo Deploying Activity Monitoring module...
psql -h %DB_HOST% -p %DB_PORT% -U %DB_USER% -d %DB_NAME% -f "%~dp011_ActivityMonitoring_Initial.sql"
if %ERRORLEVEL% neq 0 (
    echo [ERROR] Error deploying Activity Monitoring module
    pause
    exit /b 1
)
echo [OK] Activity Monitoring module deployed successfully
echo.

echo ==========================================
echo Deployment Summary
echo ==========================================
echo [OK] ActivityMonitoringRecords table created
echo [OK] ActivityMonitoringComplaints table created
echo [OK] ActivityMonitoringRealEstateViolations table created
echo [OK] ActivityMonitoringPetitionWriterViolations table created
echo [OK] All indexes and foreign keys created
echo.
echo Deployment completed successfully!
echo.

REM Verify tables were created
echo Verifying table creation...
psql -h %DB_HOST% -p %DB_PORT% -U %DB_USER% -d %DB_NAME% -t -c "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'org' AND table_name IN ('ActivityMonitoringRecords','ActivityMonitoringComplaints','ActivityMonitoringRealEstateViolations','ActivityMonitoringPetitionWriterViolations');" > temp_count.txt
set /p TABLE_COUNT=<temp_count.txt
del temp_count.txt

set TABLE_COUNT=%TABLE_COUNT: =%
if "%TABLE_COUNT%"=="4" (
    echo [OK] All 4 tables verified in database
) else (
    echo [WARNING] Expected 4 tables, found %TABLE_COUNT%
)

echo.
echo Done!
pause
