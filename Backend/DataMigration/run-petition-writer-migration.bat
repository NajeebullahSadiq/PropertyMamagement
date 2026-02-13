@echo off
echo ================================================================
echo Petition Writer License Migration
echo ================================================================
echo.

REM Check if JSON files exist
if not exist "petition_1403_records.json" (
    echo ERROR: petition_1403_records.json not found!
    echo Please ensure the file is in the current directory.
    pause
    exit /b 1
)

if not exist "petition_1404_records.json" (
    echo ERROR: petition_1404_records.json not found!
    echo Please ensure the file is in the current directory.
    pause
    exit /b 1
)

echo JSON files found:
echo - petition_1403_records.json
echo - petition_1404_records.json
echo.

REM Set connection string (already set in code)
echo Using connection string from PetitionWriterMigration.cs
echo.
echo Starting PETITION WRITER migration...
echo.

REM Run the migration with petitionwriter argument
dotnet run petitionwriter

echo.
echo ================================================================
echo Migration completed!
echo ================================================================
pause
