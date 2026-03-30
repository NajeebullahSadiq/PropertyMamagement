using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebAPIBackend.Configuration;
using WebAPIBackend.Models.Verification;

namespace WebAPIBackend.Services.Verification
{
    /// <summary>
    /// Service for managing document verification codes and verification operations
    /// </summary>
    public class VerificationService : IVerificationService
    {
        private readonly AppDbContext _context;
        private readonly IVerificationCodeGenerator _codeGenerator;
        private readonly ISignatureService _signatureService;
        private readonly string _verificationBaseUrl;

        // Document type prefixes for verification codes
        private static readonly Dictionary<string, string> DocumentTypePrefixes = new()
        {
            { "RealEstateLicense", "LIC" },
            { "PetitionWriterLicense", "PWL" },
            { "Securities", "SEC" },
            { "PetitionWriterSecurities", "PWS" },
            { "PropertyDocument", "PRO" },
            { "VehicleDocument", "VEH" }
        };

        public VerificationService(
            AppDbContext context,
            IVerificationCodeGenerator codeGenerator,
            ISignatureService signatureService,
            IConfiguration configuration)
        {
            _context = context;
            _codeGenerator = codeGenerator;
            _signatureService = signatureService;
            _verificationBaseUrl = configuration["Verification:BaseUrl"] ?? "https://prmis.gov.af/verify";
        }

        /// <summary>
        /// Gets existing verification code or creates a new one for a document
        /// </summary>
        public async Task<VerificationResult> GetOrCreateVerificationAsync(int documentId, string documentType, string userId)
        {
            try
            {
                // Check if verification already exists
                var existing = await _context.DocumentVerifications
                    .FirstOrDefaultAsync(v => v.DocumentId == documentId && 
                                              v.DocumentType == documentType && 
                                              !v.IsRevoked);

                if (existing != null)
                {
                    return new VerificationResult
                    {
                        VerificationCode = existing.VerificationCode,
                        VerificationUrl = $"{_verificationBaseUrl}/{existing.VerificationCode}",
                        IsNew = false,
                        Success = true
                    };
                }

                // Get document data for signature
                var documentData = await GetDocumentDataAsync(documentId, documentType);
                if (documentData == null)
                {
                    return new VerificationResult
                    {
                        Success = false,
                        ErrorMessage = "Document not found"
                    };
                }

                // Generate new verification code
                var prefix = DocumentTypePrefixes.GetValueOrDefault(documentType, "DOC");
                string verificationCode;
                int attempts = 0;
                const int maxAttempts = 10;

                do
                {
                    verificationCode = _codeGenerator.GenerateCode(prefix);
                    attempts++;
                    
                    var exists = await _context.DocumentVerifications
                        .AnyAsync(v => v.VerificationCode == verificationCode);
                    
                    if (!exists) break;
                    
                } while (attempts < maxAttempts);

                if (attempts >= maxAttempts)
                {
                    return new VerificationResult
                    {
                        Success = false,
                        ErrorMessage = "Failed to generate unique verification code"
                    };
                }

                // Generate digital signature
                var signatureData = new DocumentSignatureData
                {
                    DocumentId = documentId,
                    DocumentType = documentType,
                    LicenseNumber = documentData.LicenseNumber,
                    HolderName = documentData.HolderName,
                    IssueDate = documentData.IssueDate,
                    ExpiryDate = documentData.ExpiryDate
                };
                var signature = _signatureService.GenerateSignature(signatureData);

                // Create verification record
                var verification = new DocumentVerification
                {
                    VerificationCode = verificationCode,
                    DocumentId = documentId,
                    DocumentType = documentType,
                    DigitalSignature = signature,
                    DocumentSnapshot = JsonSerializer.Serialize(documentData),
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId
                };

                _context.DocumentVerifications.Add(verification);
                await _context.SaveChangesAsync();

                return new VerificationResult
                {
                    VerificationCode = verificationCode,
                    VerificationUrl = $"{_verificationBaseUrl}/{verificationCode}",
                    IsNew = true,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new VerificationResult
                {
                    Success = false,
                    ErrorMessage = $"Error creating verification: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Verifies a document using its verification code
        /// </summary>
        public async Task<DocumentVerificationDto> VerifyDocumentAsync(string verificationCode, string ipAddress)
        {
            var result = new DocumentVerificationDto
            {
                VerificationCode = verificationCode,
                VerifiedAt = DateTime.UtcNow
            };

            try
            {
                // Find verification record
                var verification = await _context.DocumentVerifications
                    .FirstOrDefaultAsync(v => v.VerificationCode == verificationCode);

                if (verification == null)
                {
                    await LogVerificationAttempt(verificationCode, ipAddress, false, "NotFound");
                    result.IsValid = false;
                    result.Status = "Invalid";
                    return result;
                }

                // Check if revoked
                if (verification.IsRevoked)
                {
                    await LogVerificationAttempt(verificationCode, ipAddress, false, "Revoked");
                    result.IsValid = false;
                    result.Status = "Revoked";
                    result.RevokedReason = verification.RevokedReason;
                    result.DocumentType = verification.DocumentType;
                    return result;
                }

                // Get current document data
                var currentData = await GetDocumentDataAsync(verification.DocumentId, verification.DocumentType);
                if (currentData == null)
                {
                    await LogVerificationAttempt(verificationCode, ipAddress, false, "DocumentDeleted");
                    result.IsValid = false;
                    result.Status = "Invalid";
                    return result;
                }

                // Verify signature
                var signatureData = new DocumentSignatureData
                {
                    DocumentId = verification.DocumentId,
                    DocumentType = verification.DocumentType,
                    LicenseNumber = currentData.LicenseNumber,
                    HolderName = currentData.HolderName,
                    IssueDate = currentData.IssueDate,
                    ExpiryDate = currentData.ExpiryDate
                };

                if (!_signatureService.VerifySignature(signatureData, verification.DigitalSignature))
                {
                    await LogVerificationAttempt(verificationCode, ipAddress, false, "SignatureMismatch");
                    result.IsValid = false;
                    result.Status = "Invalid";
                    return result;
                }

                // Check expiry
                if (currentData.ExpiryDate.HasValue && currentData.ExpiryDate.Value < DateTime.UtcNow)
                {
                    await LogVerificationAttempt(verificationCode, ipAddress, true, null);
                    result.IsValid = true;
                    result.Status = "Expired";
                }
                else
                {
                    await LogVerificationAttempt(verificationCode, ipAddress, true, null);
                    result.IsValid = true;
                    result.Status = "Valid";
                }

                // Populate result with document data
                result.DocumentType = verification.DocumentType;
                result.LicenseNumber = currentData.LicenseNumber;
                result.HolderName = currentData.HolderName;
                result.HolderPhoto = currentData.HolderPhoto;
                result.IssueDate = currentData.IssueDate;
                result.ExpiryDate = currentData.ExpiryDate;
                result.CompanyTitle = currentData.CompanyTitle;
                result.OfficeAddress = currentData.OfficeAddress;
                
                // Populate property details
                result.SerialNumber = currentData.SerialNumber;
                result.CustomDocumentType = currentData.CustomDocumentType;
                result.PropertyType = currentData.PropertyType;
                result.PropertyTypeName = currentData.PropertyTypeName;
                result.PropertyTypeDari = currentData.PropertyTypeDari;
                result.Area = currentData.Area;
                result.UnitType = currentData.UnitType;
                result.UnitTypeDari = currentData.UnitTypeDari;
                result.Province = currentData.Province;
                result.ProvinceDari = currentData.ProvinceDari;
                result.District = currentData.District;
                result.DistrictDari = currentData.DistrictDari;
                result.Village = currentData.Village;
                
                // Populate boundaries
                result.North = currentData.North;
                result.South = currentData.South;
                result.East = currentData.East;
                result.West = currentData.West;
                
                // Populate price info
                result.Price = currentData.Price;
                result.PriceText = currentData.PriceText;
                result.RoyaltyAmount = currentData.RoyaltyAmount;
                result.HalfPrice = currentData.HalfPrice;
                
                // Populate witnesses
                if (currentData.WitnessOne != null)
                {
                    result.WitnessOne = new WitnessInfoDto
                    {
                        FirstName = currentData.WitnessOne.FirstName,
                        FatherName = currentData.WitnessOne.FatherName,
                        ElectronicNationalIdNumber = currentData.WitnessOne.ElectronicNationalIdNumber
                    };
                }
                
                if (currentData.WitnessTwo != null)
                {
                    result.WitnessTwo = new WitnessInfoDto
                    {
                        FirstName = currentData.WitnessTwo.FirstName,
                        FatherName = currentData.WitnessTwo.FatherName,
                        ElectronicNationalIdNumber = currentData.WitnessTwo.ElectronicNationalIdNumber
                    };
                }
                
                // Populate seller and buyer information (for Property and Vehicle documents)
                if (currentData.Seller != null)
                {
                    result.SellerInfo = new SellerInfoDto
                    {
                        FirstName = currentData.Seller.FirstName,
                        FatherName = currentData.Seller.FatherName,
                        GrandFatherName = currentData.Seller.GrandFatherName,
                        ElectronicNationalIdNumber = currentData.Seller.ElectronicNationalIdNumber,
                        PhoneNumber = currentData.Seller.PhoneNumber,
                        Photo = currentData.Seller.Photo,
                        Province = currentData.Seller.Province,
                        District = currentData.Seller.District,
                        Village = currentData.Seller.Village
                    };
                }
                
                if (currentData.Buyer != null)
                {
                    result.BuyerInfo = new BuyerInfoDto
                    {
                        FirstName = currentData.Buyer.FirstName,
                        FatherName = currentData.Buyer.FatherName,
                        GrandFatherName = currentData.Buyer.GrandFatherName,
                        ElectronicNationalIdNumber = currentData.Buyer.ElectronicNationalIdNumber,
                        PhoneNumber = currentData.Buyer.PhoneNumber,
                        Photo = currentData.Buyer.Photo,
                        Province = currentData.Buyer.Province,
                        District = currentData.Buyer.District,
                        Village = currentData.Buyer.Village
                    };
                }
                
                // Populate petition writer information (for PetitionWriterLicense documents)
                if (currentData.PetitionWriterData != null)
                {
                    result.PetitionWriterInfo = new PetitionWriterInfoDto
                    {
                        ApplicantFatherName = currentData.PetitionWriterData.ApplicantFatherName,
                        ApplicantGrandFatherName = currentData.PetitionWriterData.ApplicantGrandFatherName,
                        ElectronicNationalIdNumber = currentData.PetitionWriterData.ElectronicNationalIdNumber,
                        MobileNumber = currentData.PetitionWriterData.MobileNumber,
                        Competency = currentData.PetitionWriterData.Competency,
                        District = currentData.PetitionWriterData.District,
                        LicenseType = currentData.PetitionWriterData.LicenseType,
                        LicensePrice = currentData.PetitionWriterData.LicensePrice,
                        PermanentProvinceName = currentData.PetitionWriterData.PermanentProvinceName,
                        PermanentDistrictName = currentData.PetitionWriterData.PermanentDistrictName,
                        PermanentVillage = currentData.PetitionWriterData.PermanentVillage,
                        CurrentProvinceName = currentData.PetitionWriterData.CurrentProvinceName,
                        CurrentDistrictName = currentData.PetitionWriterData.CurrentDistrictName,
                        CurrentVillage = currentData.PetitionWriterData.CurrentVillage,
                        DetailedAddress = currentData.PetitionWriterData.DetailedAddress,
                        LatestRelocation = currentData.PetitionWriterData.LatestRelocation
                    };
                }

                // Populate vehicle details (for VehicleDocument)
                result.PlateNumber = currentData.PlateNumber;
                result.VehicleType = currentData.VehicleType;
                result.VehicleModel = currentData.VehicleModel;
                result.EngineNumber = currentData.EngineNumber;
                result.ChassisNumber = currentData.ChassisNumber;
                result.VehicleColor = currentData.VehicleColor;
                result.Description = currentData.Description;

                return result;
            }
            catch (Exception)
            {
                await LogVerificationAttempt(verificationCode, ipAddress, false, "Error");
                result.IsValid = false;
                result.Status = "Invalid";
                return result;
            }
        }

        /// <summary>
        /// Revokes a verification code
        /// </summary>
        public async Task<bool> RevokeVerificationAsync(string verificationCode, string reason, string userId)
        {
            var verification = await _context.DocumentVerifications
                .FirstOrDefaultAsync(v => v.VerificationCode == verificationCode);

            if (verification == null || verification.IsRevoked)
            {
                return false;
            }

            verification.IsRevoked = true;
            verification.RevokedAt = DateTime.UtcNow;
            verification.RevokedBy = userId;
            verification.RevokedReason = reason;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Gets verification statistics for a document
        /// </summary>
        public async Task<VerificationStatsDto> GetVerificationStatsAsync(string verificationCode)
        {
            var verification = await _context.DocumentVerifications
                .FirstOrDefaultAsync(v => v.VerificationCode == verificationCode);

            if (verification == null)
            {
                return new VerificationStatsDto { VerificationCode = verificationCode };
            }

            var logs = await _context.VerificationLogs
                .Where(l => l.VerificationCode == verificationCode)
                .ToListAsync();

            return new VerificationStatsDto
            {
                VerificationCode = verificationCode,
                TotalAttempts = logs.Count,
                SuccessfulAttempts = logs.Count(l => l.WasSuccessful),
                FailedAttempts = logs.Count(l => !l.WasSuccessful),
                LastVerifiedAt = logs.OrderByDescending(l => l.AttemptedAt).FirstOrDefault()?.AttemptedAt,
                CreatedAt = verification.CreatedAt,
                IsRevoked = verification.IsRevoked
            };
        }

        /// <summary>
        /// Logs a verification attempt
        /// </summary>
        private async Task LogVerificationAttempt(string verificationCode, string ipAddress, bool wasSuccessful, string? failureReason)
        {
            var log = new VerificationLog
            {
                VerificationCode = verificationCode,
                AttemptedAt = DateTime.UtcNow,
                IpAddress = ipAddress,
                WasSuccessful = wasSuccessful,
                FailureReason = failureReason
            };

            _context.VerificationLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Gets document data for verification
        /// </summary>
        private async Task<DocumentDataDto?> GetDocumentDataAsync(int documentId, string documentType)
        {
            return documentType switch
            {
                "RealEstateLicense" => await GetRealEstateLicenseDataAsync(documentId),
                "CompanyLicense" => await GetCompanyLicenseDataAsync(documentId),
                "PetitionWriterLicense" => await GetPetitionWriterLicenseDataAsync(documentId),
                "PropertyDocument" => await GetPropertyDocumentDataAsync(documentId),
                "VehicleDocument" => await GetVehicleDocumentDataAsync(documentId),
                _ => null
            };
        }

        private async Task<DocumentDataDto?> GetCompanyLicenseDataAsync(int licenseId)
        {
            var license = await _context.LicenseDetails
                .Include(l => l.Company)
                    .ThenInclude(c => c!.CompanyOwners)
                .FirstOrDefaultAsync(l => l.Id == licenseId);

            if (license == null) return null;

            var owner = license.Company?.CompanyOwners?.FirstOrDefault();

            return new DocumentDataDto
            {
                LicenseNumber = license.LicenseNumber?.ToString() ?? "",
                HolderName = owner?.FirstName ?? license.Company?.Title ?? "",
                HolderPhoto = owner?.PothoPath,
                IssueDate = license.IssueDate?.ToDateTime(TimeOnly.MinValue),
                ExpiryDate = license.ExpireDate?.ToDateTime(TimeOnly.MinValue),
                CompanyTitle = license.Company?.Title,
                OfficeAddress = license.OfficeAddress
            };
        }

        private async Task<DocumentDataDto?> GetRealEstateLicenseDataAsync(int licenseId)
        {
            var license = await _context.LicenseDetails
                .Include(l => l.Company)
                    .ThenInclude(c => c!.CompanyOwners)
                .FirstOrDefaultAsync(l => l.Id == licenseId);

            if (license == null) return null;

            var owner = license.Company?.CompanyOwners?.FirstOrDefault();

            return new DocumentDataDto
            {
                LicenseNumber = license.LicenseNumber?.ToString() ?? "",
                HolderName = owner?.FirstName ?? license.Company?.Title ?? "",
                HolderPhoto = owner?.PothoPath,
                IssueDate = license.IssueDate?.ToDateTime(TimeOnly.MinValue),
                ExpiryDate = license.ExpireDate?.ToDateTime(TimeOnly.MinValue),
                CompanyTitle = license.Company?.Title,
                OfficeAddress = license.OfficeAddress
            };
        }

        private async Task<DocumentDataDto?> GetPetitionWriterLicenseDataAsync(int licenseId)
        {
            var license = await _context.PetitionWriterLicenses
                .Include(l => l.PermanentProvince)
                .Include(l => l.PermanentDistrict)
                .Include(l => l.CurrentProvince)
                .Include(l => l.CurrentDistrict)
                .Include(l => l.Relocations)
                .FirstOrDefaultAsync(l => l.Id == licenseId);

            if (license == null) return null;

            // Get province and district names
            string permanentProvinceName = license.PermanentProvince?.Dari ?? license.PermanentProvince?.Name ?? "";
            string permanentDistrictName = license.PermanentDistrict?.Dari ?? license.PermanentDistrict?.Name ?? "";
            string currentProvinceName = license.CurrentProvince?.Dari ?? license.CurrentProvince?.Name ?? "";
            string currentDistrictName = license.CurrentDistrict?.Dari ?? license.CurrentDistrict?.Name ?? "";

            // Get latest relocation
            var latestRelocation = license.Relocations
                .OrderByDescending(r => r.RelocationDate)
                .FirstOrDefault();

            return new DocumentDataDto
            {
                LicenseNumber = license.LicenseNumber ?? "",
                HolderName = license.ApplicantName,
                HolderPhoto = license.PicturePath,
                IssueDate = license.LicenseIssueDate?.ToDateTime(TimeOnly.MinValue),
                ExpiryDate = license.LicenseExpiryDate?.ToDateTime(TimeOnly.MinValue),
                CompanyTitle = null,
                OfficeAddress = license.DetailedAddress,
                
                // Petition Writer specific fields
                PetitionWriterData = new PetitionWriterData
                {
                    ApplicantFatherName = license.ApplicantFatherName,
                    ApplicantGrandFatherName = license.ApplicantGrandFatherName,
                    ElectronicNationalIdNumber = license.ElectronicNationalIdNumber,
                    MobileNumber = license.MobileNumber,
                    Competency = license.Competency,
                    District = license.District,
                    LicenseType = license.LicenseType,
                    LicensePrice = license.LicensePrice,
                    PermanentProvinceName = permanentProvinceName,
                    PermanentDistrictName = permanentDistrictName,
                    PermanentVillage = license.PermanentVillage,
                    CurrentProvinceName = currentProvinceName,
                    CurrentDistrictName = currentDistrictName,
                    CurrentVillage = license.CurrentVillage,
                    DetailedAddress = license.DetailedAddress,
                    LatestRelocation = latestRelocation?.NewActivityLocation
                }
            };
        }

        private async Task<DocumentDataDto?> GetPropertyDocumentDataAsync(int propertyId)
        {
            // Use direct query instead of GetPrintType view
            var property = await _context.PropertyDetails
                .Include(p => p.SellerDetails)
                .Include(p => p.BuyerDetails)
                .Include(p => p.WitnessDetails)
                .Include(p => p.PropertyAddresses)
                .Include(p => p.PropertyType)
                .Include(p => p.PunitType)
                .FirstOrDefaultAsync(p => p.Id == propertyId);

            if (property == null) return null;

            var seller = property.SellerDetails.FirstOrDefault();
            var buyer = property.BuyerDetails.FirstOrDefault();
            var address = property.PropertyAddresses.FirstOrDefault();
            var witnessOne = property.WitnessDetails.FirstOrDefault();
            var witnessTwo = property.WitnessDetails.Skip(1).FirstOrDefault();

            // Get province and district names from Location table
            string province = "";
            string provinceDari = "";
            string district = "";
            string districtDari = "";
            
            if (address != null)
            {
                if (address.ProvinceId.HasValue)
                {
                    var provinceLocation = await _context.Locations.FindAsync(address.ProvinceId.Value);
                    province = provinceLocation?.Name ?? "";
                    provinceDari = provinceLocation?.Dari ?? "";
                }
                
                if (address.DistrictId.HasValue)
                {
                    var districtLocation = await _context.Locations.FindAsync(address.DistrictId.Value);
                    district = districtLocation?.Name ?? "";
                    districtDari = districtLocation?.Dari ?? "";
                }
            }

            // Prepare seller data
            SellerData? sellerData = null;
            if (seller != null)
            {
                string sellerProvince = "";
                string sellerDistrict = "";
                
                if (seller.PaddressProvinceId.HasValue)
                {
                    var prov = await _context.Locations.FindAsync(seller.PaddressProvinceId.Value);
                    sellerProvince = prov?.Dari ?? prov?.Name ?? "";
                }
                
                if (seller.PaddressDistrictId.HasValue)
                {
                    var dist = await _context.Locations.FindAsync(seller.PaddressDistrictId.Value);
                    sellerDistrict = dist?.Dari ?? dist?.Name ?? "";
                }
                
                sellerData = new SellerData
                {
                    FirstName = seller.FirstName,
                    FatherName = seller.FatherName,
                    GrandFatherName = seller.GrandFather,
                    ElectronicNationalIdNumber = seller.ElectronicNationalIdNumber,
                    PhoneNumber = seller.PhoneNumber,
                    Photo = seller.Photo,
                    Province = sellerProvince,
                    District = sellerDistrict,
                    Village = seller.PaddressVillage
                };
            }

            // Prepare buyer data
            BuyerData? buyerData = null;
            if (buyer != null)
            {
                string buyerProvince = "";
                string buyerDistrict = "";
                
                if (buyer.PaddressProvinceId.HasValue)
                {
                    var prov = await _context.Locations.FindAsync(buyer.PaddressProvinceId.Value);
                    buyerProvince = prov?.Dari ?? prov?.Name ?? "";
                }
                
                if (buyer.PaddressDistrictId.HasValue)
                {
                    var dist = await _context.Locations.FindAsync(buyer.PaddressDistrictId.Value);
                    buyerDistrict = dist?.Dari ?? dist?.Name ?? "";
                }
                
                buyerData = new BuyerData
                {
                    FirstName = buyer.FirstName,
                    FatherName = buyer.FatherName,
                    GrandFatherName = buyer.GrandFather,
                    ElectronicNationalIdNumber = buyer.ElectronicNationalIdNumber,
                    PhoneNumber = buyer.PhoneNumber,
                    Photo = buyer.Photo,
                    Province = buyerProvince,
                    District = buyerDistrict,
                    Village = buyer.PaddressVillage
                };
            }

            // Prepare witness data
            WitnessData? witnessOneData = null;
            if (witnessOne != null)
            {
                witnessOneData = new WitnessData
                {
                    FirstName = witnessOne.FirstName,
                    FatherName = witnessOne.FatherName,
                    ElectronicNationalIdNumber = witnessOne.ElectronicNationalIdNumber
                };
            }
            
            WitnessData? witnessTwoData = null;
            if (witnessTwo != null)
            {
                witnessTwoData = new WitnessData
                {
                    FirstName = witnessTwo.FirstName,
                    FatherName = witnessTwo.FatherName,
                    ElectronicNationalIdNumber = witnessTwo.ElectronicNationalIdNumber
                };
            }

            // Parse area to decimal
            decimal? areaValue = null;
            if (!string.IsNullOrEmpty(property.Parea) && decimal.TryParse(property.Parea, out var parsedArea))
            {
                areaValue = parsedArea;
            }

            // Parse price values
            decimal? priceValue = null;
            if (buyer?.Price != null && decimal.TryParse(buyer.Price, out var parsedPrice))
            {
                priceValue = parsedPrice;
            }
            else if (property.Price != null && decimal.TryParse(property.Price, out parsedPrice))
            {
                priceValue = parsedPrice;
            }

            decimal? royaltyValue = null;
            if (buyer?.RoyaltyAmount != null && decimal.TryParse(buyer.RoyaltyAmount, out var parsedRoyalty))
            {
                royaltyValue = parsedRoyalty;
            }
            else if (property.RoyaltyAmount != null && decimal.TryParse(property.RoyaltyAmount, out parsedRoyalty))
            {
                royaltyValue = parsedRoyalty;
            }

            decimal? halfPriceValue = null;
            if (buyer?.HalfPrice != null && decimal.TryParse(buyer.HalfPrice, out var parsedHalfPrice))
            {
                halfPriceValue = parsedHalfPrice;
            }

            return new DocumentDataDto
            {
                LicenseNumber = property.IssuanceNumber ?? property.Pnumber ?? "",
                HolderName = $"{seller?.FirstName ?? ""} - {buyer?.FirstName ?? ""}",
                HolderPhoto = seller?.Photo,
                IssueDate = property.CreatedAt,
                ExpiryDate = null, // Property documents don't expire
                CompanyTitle = null,
                OfficeAddress = $"{province}, {district}",
                
                // Property details
                SerialNumber = property.SerialNumber,
                CustomDocumentType = property.CustomDocumentType,
                PropertyType = property.CustomPropertyType,
                PropertyTypeName = property.PropertyType?.Name,
                PropertyTypeDari = property.PropertyType?.Dari,
                Area = areaValue,
                UnitType = property.PunitType?.Name,
                UnitTypeDari = property.PunitType?.Dari,
                Province = province,
                ProvinceDari = provinceDari,
                District = district,
                DistrictDari = districtDari,
                Village = address?.Village,
                
                // Boundaries (from PropertyDetail, not PropertyAddress)
                North = property.North,
                South = property.South,
                East = property.East,
                West = property.West,
                
                // Price info (from buyer)
                Price = priceValue,
                PriceText = buyer?.PriceText ?? property.PriceText,
                RoyaltyAmount = royaltyValue,
                HalfPrice = halfPriceValue,
                
                // Witnesses
                WitnessOne = witnessOneData,
                WitnessTwo = witnessTwoData,
                
                Seller = sellerData,
                Buyer = buyerData
            };
        }

        private async Task<DocumentDataDto?> GetVehicleDocumentDataAsync(int vehicleId)
        {
            // Query vehicle details with seller and buyer information
            var vehicle = await _context.VehiclesPropertyDetails
                .Include(v => v.VehiclesSellerDetails)
                .Include(v => v.VehiclesBuyerDetails)
                .FirstOrDefaultAsync(v => v.Id == vehicleId);

            if (vehicle == null) return null;

            var seller = vehicle.VehiclesSellerDetails.FirstOrDefault();
            var buyer = vehicle.VehiclesBuyerDetails.FirstOrDefault();

            // Get province and district names from Location table for seller
            string sellerProvince = "";
            string sellerDistrict = "";
            
            if (seller != null)
            {
                if (seller.PaddressProvinceId.HasValue)
                {
                    var provinceLocation = await _context.Locations.FindAsync(seller.PaddressProvinceId.Value);
                    sellerProvince = provinceLocation?.Dari ?? provinceLocation?.Name ?? "";
                }
                
                if (seller.PaddressDistrictId.HasValue)
                {
                    var districtLocation = await _context.Locations.FindAsync(seller.PaddressDistrictId.Value);
                    sellerDistrict = districtLocation?.Dari ?? districtLocation?.Name ?? "";
                }
            }

            // Prepare seller data
            SellerData? sellerData = null;
            if (seller != null)
            {
                sellerData = new SellerData
                {
                    FirstName = seller.FirstName,
                    FatherName = seller.FatherName,
                    GrandFatherName = seller.GrandFather,
                    ElectronicNationalIdNumber = seller.ElectronicNationalIdNumber,
                    PhoneNumber = seller.PhoneNumber,
                    Photo = seller.Photo,
                    Province = sellerProvince,
                    District = sellerDistrict,
                    Village = seller.PaddressVillage
                };
            }

            // Prepare buyer data
            BuyerData? buyerData = null;
            if (buyer != null)
            {
                string buyerProvince = "";
                string buyerDistrict = "";
                
                if (buyer.PaddressProvinceId.HasValue)
                {
                    var prov = await _context.Locations.FindAsync(buyer.PaddressProvinceId.Value);
                    buyerProvince = prov?.Dari ?? prov?.Name ?? "";
                }
                
                if (buyer.PaddressDistrictId.HasValue)
                {
                    var dist = await _context.Locations.FindAsync(buyer.PaddressDistrictId.Value);
                    buyerDistrict = dist?.Dari ?? dist?.Name ?? "";
                }
                
                buyerData = new BuyerData
                {
                    FirstName = buyer.FirstName,
                    FatherName = buyer.FatherName,
                    GrandFatherName = buyer.GrandFather,
                    ElectronicNationalIdNumber = buyer.ElectronicNationalIdNumber,
                    PhoneNumber = buyer.PhoneNumber,
                    Photo = buyer.Photo,
                    Province = buyerProvince,
                    District = buyerDistrict,
                    Village = buyer.PaddressVillage
                };
            }

            return new DocumentDataDto
            {
                LicenseNumber = vehicle.PermitNo ?? vehicle.PilateNo ?? "",
                HolderName = $"{seller?.FirstName ?? ""} - {buyer?.FirstName ?? ""}",
                HolderPhoto = seller?.Photo,
                IssueDate = vehicle.CreatedAt,
                ExpiryDate = null, // Vehicle documents don't expire
                CompanyTitle = null,
                OfficeAddress = $"{sellerProvince}, {sellerDistrict}",
                
                // Vehicle details
                PlateNumber = vehicle.PilateNo,
                VehicleType = vehicle.TypeOfVehicle,
                VehicleModel = vehicle.Model,
                EngineNumber = vehicle.EnginNo,
                ChassisNumber = vehicle.ShasiNo,
                VehicleColor = vehicle.Color,
                Description = vehicle.Des,
                
                // Price info
                Price = decimal.TryParse(vehicle.Price, out var parsedPrice) ? parsedPrice : null,
                PriceText = vehicle.PriceText,
                RoyaltyAmount = decimal.TryParse(vehicle.RoyaltyAmount, out var parsedRoyalty) ? parsedRoyalty : null,
                HalfPrice = decimal.TryParse(vehicle.HalfPrice, out var parsedHalfPrice) ? parsedHalfPrice : null,
                
                Seller = sellerData,
                Buyer = buyerData
            };
        }
    }

    /// <summary>
    /// Internal DTO for document data
    /// </summary>
    internal class DocumentDataDto
    {
        public string LicenseNumber { get; set; } = string.Empty;
        public string HolderName { get; set; } = string.Empty;
        public string? HolderPhoto { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? CompanyTitle { get; set; }
        public string? OfficeAddress { get; set; }
        
        // Property details
        public string? SerialNumber { get; set; }
        public string? CustomDocumentType { get; set; }
        public string? PropertyType { get; set; }
        public string? PropertyTypeName { get; set; }
        public string? PropertyTypeDari { get; set; }
        public decimal? Area { get; set; }
        public string? UnitType { get; set; }
        public string? UnitTypeDari { get; set; }
        public string? Province { get; set; }
        public string? ProvinceDari { get; set; }
        public string? District { get; set; }
        public string? DistrictDari { get; set; }
        public string? Village { get; set; }
        
        // Boundaries
        public string? North { get; set; }
        public string? South { get; set; }
        public string? East { get; set; }
        public string? West { get; set; }
        
        // Price info
        public decimal? Price { get; set; }
        public string? PriceText { get; set; }
        public decimal? RoyaltyAmount { get; set; }
        public decimal? HalfPrice { get; set; }
        
        // Witnesses
        public WitnessData? WitnessOne { get; set; }
        public WitnessData? WitnessTwo { get; set; }
        
        // Seller information (for Property and Vehicle documents)
        public SellerData? Seller { get; set; }
        
        // Buyer information (for Property and Vehicle documents)
        public BuyerData? Buyer { get; set; }
        
        // Petition Writer specific information
        public PetitionWriterData? PetitionWriterData { get; set; }
        
        // Vehicle details
        public string? PlateNumber { get; set; }
        public string? VehicleType { get; set; }
        public string? VehicleModel { get; set; }
        public string? EngineNumber { get; set; }
        public string? ChassisNumber { get; set; }
        public string? VehicleColor { get; set; }
        public string? Description { get; set; }
    }

    /// <summary>
    /// Internal witness data
    /// </summary>
    internal class WitnessData
    {
        public string? FirstName { get; set; }
        public string? FatherName { get; set; }
        public string? ElectronicNationalIdNumber { get; set; }
    }

    /// <summary>
    /// Internal seller data
    /// </summary>
    internal class SellerData
    {
        public string? FirstName { get; set; }
        public string? FatherName { get; set; }
        public string? GrandFatherName { get; set; }
        public string? ElectronicNationalIdNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Photo { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Village { get; set; }
    }

    /// <summary>
    /// Internal buyer data
    /// </summary>
    internal class BuyerData
    {
        public string? FirstName { get; set; }
        public string? FatherName { get; set; }
        public string? GrandFatherName { get; set; }
        public string? ElectronicNationalIdNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Photo { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Village { get; set; }
    }

    /// <summary>
    /// Internal petition writer data
    /// </summary>
    internal class PetitionWriterData
    {
        public string? ApplicantFatherName { get; set; }
        public string? ApplicantGrandFatherName { get; set; }
        public string? ElectronicNationalIdNumber { get; set; }
        public string? MobileNumber { get; set; }
        public string? Competency { get; set; }
        public string? District { get; set; }
        public string? LicenseType { get; set; }
        public decimal? LicensePrice { get; set; }
        public string? PermanentProvinceName { get; set; }
        public string? PermanentDistrictName { get; set; }
        public string? PermanentVillage { get; set; }
        public string? CurrentProvinceName { get; set; }
        public string? CurrentDistrictName { get; set; }
        public string? CurrentVillage { get; set; }
        public string? DetailedAddress { get; set; }
        public string? LatestRelocation { get; set; }
    }
}
