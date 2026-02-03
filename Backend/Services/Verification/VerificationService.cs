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
                "PetitionWriterLicense" => await GetPetitionWriterLicenseDataAsync(documentId),
                "PropertyDocument" => await GetPropertyDocumentDataAsync(documentId),
                "VehicleDocument" => await GetVehicleDocumentDataAsync(documentId),
                _ => null
            };
        }

        private async Task<DocumentDataDto?> GetRealEstateLicenseDataAsync(int licenseId)
        {
            var license = await _context.LicenseDetails
                .Include(l => l.Company)
                    .ThenInclude(c => c!.CompanyOwners)
                .Include(l => l.Area)
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
                .FirstOrDefaultAsync(l => l.Id == licenseId);

            if (license == null) return null;

            return new DocumentDataDto
            {
                LicenseNumber = license.LicenseNumber ?? "",
                HolderName = license.ApplicantName,
                HolderPhoto = null, // PetitionWriterLicense doesn't have a photo field
                IssueDate = license.LicenseIssueDate?.ToDateTime(TimeOnly.MinValue),
                ExpiryDate = license.LicenseExpiryDate?.ToDateTime(TimeOnly.MinValue),
                CompanyTitle = null,
                OfficeAddress = license.ActivityLocation
            };
        }

        private async Task<DocumentDataDto?> GetPropertyDocumentDataAsync(int propertyId)
        {
            // Use direct query instead of GetPrintType view
            var property = await _context.PropertyDetails
                .Include(p => p.SellerDetails)
                .Include(p => p.BuyerDetails)
                .Include(p => p.PropertyAddresses)
                .FirstOrDefaultAsync(p => p.Id == propertyId);

            if (property == null) return null;

            var seller = property.SellerDetails.FirstOrDefault();
            var buyer = property.BuyerDetails.FirstOrDefault();
            var address = property.PropertyAddresses.FirstOrDefault();

            // Get province and district names from Location table
            string province = "";
            string district = "";
            
            if (address != null)
            {
                if (address.ProvinceId.HasValue)
                {
                    var provinceLocation = await _context.Locations.FindAsync(address.ProvinceId.Value);
                    province = provinceLocation?.Dari ?? provinceLocation?.Name ?? "";
                }
                
                if (address.DistrictId.HasValue)
                {
                    var districtLocation = await _context.Locations.FindAsync(address.DistrictId.Value);
                    district = districtLocation?.Dari ?? districtLocation?.Name ?? "";
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

            return new DocumentDataDto
            {
                LicenseNumber = property.IssuanceNumber ?? property.Pnumber ?? "",
                HolderName = $"{seller?.FirstName ?? ""} - {buyer?.FirstName ?? ""}",
                HolderPhoto = seller?.Photo,
                IssueDate = property.CreatedAt,
                ExpiryDate = null, // Property documents don't expire
                CompanyTitle = null,
                OfficeAddress = $"{province}, {district}",
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
        
        // Seller information (for Property and Vehicle documents)
        public SellerData? Seller { get; set; }
        
        // Buyer information (for Property and Vehicle documents)
        public BuyerData? Buyer { get; set; }
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
}
