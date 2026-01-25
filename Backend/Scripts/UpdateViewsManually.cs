// Quick C# script to update database views
// Run this with: dotnet script UpdateViewsManually.cs
// Or copy the SQL and run it in pgAdmin

using System;
using Npgsql;

var connectionString = "Server=localhost;Database=PRMIS;Username=postgres;Password=Khan@223344";

var sql = @"
-- Drop and recreate GetPrintType view
DROP VIEW IF EXISTS ""GetPrintType"";

CREATE VIEW ""GetPrintType"" AS
SELECT 
    pd.""Id"",
    pd.""DocumentType"" as ""Doctype"",
    pd.""PNumber"" as ""Pnumber"",
    pd.""PArea"",
    pd.""NumofRooms"",
    pd.""north"",
    pd.""south"",
    pd.""west"",
    pd.""east"",
    pd.""Price"",
    pd.""PriceText"",
    pd.""RoyaltyAmount"",
    pt.""Name"" as ""PropertypeType"",
    pd.""CreatedAt"",
    pd.""TransactionDate"" as ""DeedDate"",
    pd.""FilePath"",
    pd.""PreviousDocumentsPath"",
    pd.""ExistingDocumentsPath"",
    pd.""DocumentType"",
    pd.""IssuanceNumber"",
    pd.""IssuanceDate"",
    pd.""SerialNumber"",
    pd.""TransactionDate"",
    pd.""PNumber"",
    pa_prov.""Name"" as ""Province"",
    pa_dist.""Name"" as ""District"",
    pa_prov.""Dari"" as ""ProvinceDari"",
    pa_dist.""Dari"" as ""DistrictDari"",
    pa.""Village"",
    sd.""FirstName"" as ""SellerFirstName"",
    sd.""FatherName"" as ""SellerFatherName"",
    sd.""ElectronicNationalIdNumber"" as ""SellerElectronicNationalIdNumber"",
    sd.""PaddressVillage"" as ""SellerVillage"",
    sd.""TaddressVillage"" as ""TSellerVillage"",
    sd.""photo"" as ""SellerPhoto"",
    s_perm_prov.""Name"" as ""SellerProvince"",
    s_perm_dist.""Name"" as ""SellerDistrict"",
    s_perm_prov.""Dari"" as 