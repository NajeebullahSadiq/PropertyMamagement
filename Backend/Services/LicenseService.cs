using Microsoft.EntityFrameworkCore;
using WebAPIBackend.Configuration;
using WebAPIBackend.Models;
using WebAPIBackend.Models.Common;

namespace WebAPIBackend.Services
{
    public interface ILicenseService
    {
        Task<List<LicenseDetail>> GetAllLicensesAsync();
        Task<LicenseDetail> GetLicenseByIdAsync(int id);
        Task<LicenseDetail> CreateLicenseAsync(LicenseDetail license);
        Task<LicenseDetail> UpdateLicenseAsync(int id, LicenseDetail license);
        Task DeleteLicenseAsync(int id);
    }

    public class LicenseService : ILicenseService
    {
        private readonly AppDbContext _context;
        private readonly IProvinceFilterService _provinceFilter;

        public LicenseService(AppDbContext context, IProvinceFilterService provinceFilter)
        {
            _context = context;
            _provinceFilter = provinceFilter;
        }

        public async Task<List<LicenseDetail>> GetAllLicensesAsync()
        {
            var query = _context.LicenseDetails
                .FromSqlRaw(@"SELECT ""Id"", ""LicenseNumber"", ""ProvinceId"", ""IssueDate"", ""ExpireDate"", 
                              ""TransferLocation"", ""OfficeAddress"", ""CompanyId"", ""DocPath"", ""LicenseType"", 
                              ""LicenseCategory"", ""RenewalRound"", ""RoyaltyAmount"", ""RoyaltyDate"", ""TariffNumber"", 
                              ""PenaltyAmount"", ""PenaltyDate"", ""HrLetter"", ""HrLetterDate"", ""CreatedAt"", 
                              ""CreatedBy"", ""Status"", ""IsComplete""
                              FROM org.""LicenseDetails""")
                .AsQueryable();
            query = _provinceFilter.ApplyProvinceFilter(query);
            return await query.ToListAsync();
        }

        public async Task<LicenseDetail> GetLicenseByIdAsync(int id)
        {
            var license = await _context.LicenseDetails
                .FromSqlRaw(@"SELECT ""Id"", ""LicenseNumber"", ""ProvinceId"", ""IssueDate"", ""ExpireDate"", 
                              ""TransferLocation"", ""OfficeAddress"", ""CompanyId"", ""DocPath"", ""LicenseType"", 
                              ""LicenseCategory"", ""RenewalRound"", ""RoyaltyAmount"", ""RoyaltyDate"", ""TariffNumber"", 
                              ""PenaltyAmount"", ""PenaltyDate"", ""HrLetter"", ""HrLetterDate"", ""CreatedAt"", 
                              ""CreatedBy"", ""Status"", ""IsComplete""
                              FROM org.""LicenseDetails"" WHERE ""Id"" = {0}", id)
                .FirstOrDefaultAsync();
                
            if (license == null)
            {
                throw new NotFoundException("License not found");
            }

            _provinceFilter.ValidateProvinceAccess(license.ProvinceId);
            return license;
        }

        public async Task<LicenseDetail> CreateLicenseAsync(LicenseDetail license)
        {
            // Get parent company to inherit province
            var company = await _context.CompanyDetails.FindAsync(license.CompanyId);
            if (company == null)
            {
                throw new NotFoundException("Company not found");
            }

            // Validate province access to parent company
            _provinceFilter.ValidateProvinceAccess(company.ProvinceId);

            // Inherit province from company
            license.ProvinceId = company.ProvinceId;

            _context.LicenseDetails.Add(license);
            await _context.SaveChangesAsync();
            return license;
        }

        public async Task<LicenseDetail> UpdateLicenseAsync(int id, LicenseDetail updatedLicense)
        {
            var license = await _context.LicenseDetails
                .FromSqlRaw(@"SELECT ""Id"", ""LicenseNumber"", ""ProvinceId"", ""IssueDate"", ""ExpireDate"", 
                              ""TransferLocation"", ""OfficeAddress"", ""CompanyId"", ""DocPath"", ""LicenseType"", 
                              ""LicenseCategory"", ""RenewalRound"", ""RoyaltyAmount"", ""RoyaltyDate"", ""TariffNumber"", 
                              ""PenaltyAmount"", ""PenaltyDate"", ""HrLetter"", ""HrLetterDate"", ""CreatedAt"", 
                              ""CreatedBy"", ""Status"", ""IsComplete""
                              FROM org.""LicenseDetails"" WHERE ""Id"" = {0}", id)
                .FirstOrDefaultAsync();
                
            if (license == null)
            {
                throw new NotFoundException("License not found");
            }

            // Validate province access
            _provinceFilter.ValidateProvinceAccess(license.ProvinceId);

            // Preserve original province ID (immutable)
            var originalProvinceId = license.ProvinceId;

            // Update properties
            license.LicenseNumber = updatedLicense.LicenseNumber;
            license.IssueDate = updatedLicense.IssueDate;
            license.ExpireDate = updatedLicense.ExpireDate;
            license.TransferLocation = updatedLicense.TransferLocation;
            license.OfficeAddress = updatedLicense.OfficeAddress;
            license.DocPath = updatedLicense.DocPath;
            license.LicenseType = updatedLicense.LicenseType;
            license.LicenseCategory = updatedLicense.LicenseCategory;
            license.RenewalRound = updatedLicense.RenewalRound;
            license.RoyaltyAmount = updatedLicense.RoyaltyAmount;
            license.RoyaltyDate = updatedLicense.RoyaltyDate;
            license.TariffNumber = updatedLicense.TariffNumber;
            license.PenaltyAmount = updatedLicense.PenaltyAmount;
            license.PenaltyDate = updatedLicense.PenaltyDate;
            license.HrLetter = updatedLicense.HrLetter;
            license.HrLetterDate = updatedLicense.HrLetterDate;
            license.Status = updatedLicense.Status;

            // Ensure province hasn't changed
            license.ProvinceId = originalProvinceId;

            await _context.SaveChangesAsync();
            return license;
        }

        public async Task DeleteLicenseAsync(int id)
        {
            var license = await _context.LicenseDetails
                .FromSqlRaw(@"SELECT ""Id"", ""LicenseNumber"", ""ProvinceId"", ""IssueDate"", ""ExpireDate"", 
                              ""TransferLocation"", ""OfficeAddress"", ""CompanyId"", ""DocPath"", ""LicenseType"", 
                              ""LicenseCategory"", ""RenewalRound"", ""RoyaltyAmount"", ""RoyaltyDate"", ""TariffNumber"", 
                              ""PenaltyAmount"", ""PenaltyDate"", ""HrLetter"", ""HrLetterDate"", ""CreatedAt"", 
                              ""CreatedBy"", ""Status"", ""IsComplete""
                              FROM org.""LicenseDetails"" WHERE ""Id"" = {0}", id)
                .FirstOrDefaultAsync();
                
            if (license == null)
            {
                throw new NotFoundException("License not found");
            }

            _provinceFilter.ValidateProvinceAccess(license.ProvinceId);

            _context.LicenseDetails.Remove(license);
            await _context.SaveChangesAsync();
        }
    }
}
