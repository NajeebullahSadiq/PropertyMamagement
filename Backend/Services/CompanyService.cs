using Microsoft.EntityFrameworkCore;
using WebAPIBackend.Configuration;
using WebAPIBackend.Models;
using WebAPIBackend.Models.Common;

namespace WebAPIBackend.Services
{
    public interface ICompanyService
    {
        Task<List<CompanyDetail>> GetAllCompaniesAsync();
        Task<CompanyDetail> GetCompanyByIdAsync(int id);
        Task<CompanyDetail> CreateCompanyAsync(CompanyDetail company);
        Task<CompanyDetail> UpdateCompanyAsync(int id, CompanyDetail company);
        Task DeleteCompanyAsync(int id);
        Task UpdateLicenseCompletionStatusAsync(int companyId);
    }

    public class CompanyService : ICompanyService
    {
        private readonly AppDbContext _context;
        private readonly IProvinceFilterService _provinceFilter;
        private readonly ILicenseNumberGenerator _licenseNumberGenerator;

        public CompanyService(
            AppDbContext context, 
            IProvinceFilterService provinceFilter,
            ILicenseNumberGenerator licenseNumberGenerator)
        {
            _context = context;
            _provinceFilter = provinceFilter;
            _licenseNumberGenerator = licenseNumberGenerator;
        }

        public async Task<List<CompanyDetail>> GetAllCompaniesAsync()
        {
            var query = _context.CompanyDetails.AsQueryable();
            query = _provinceFilter.ApplyProvinceFilter(query);
            return await query.ToListAsync();
        }

        public async Task<CompanyDetail> GetCompanyByIdAsync(int id)
        {
            var company = await _context.CompanyDetails.FindAsync(id);
            if (company == null)
            {
                throw new NotFoundException("Company not found");
            }

            _provinceFilter.ValidateProvinceAccess(company.ProvinceId);
            return company;
        }

        public async Task<CompanyDetail> CreateCompanyAsync(CompanyDetail company)
        {
            // Auto-populate province for COMPANY_REGISTRAR
            if (!_provinceFilter.IsAdministrator())
            {
                company.ProvinceId = _provinceFilter.GetUserProvinceId();
            }

            // Validate province access
            _provinceFilter.ValidateProvinceAccess(company.ProvinceId);

            _context.CompanyDetails.Add(company);
            await _context.SaveChangesAsync();
            return company;
        }

        public async Task<CompanyDetail> UpdateCompanyAsync(int id, CompanyDetail updatedCompany)
        {
            var company = await _context.CompanyDetails.FindAsync(id);
            if (company == null)
            {
                throw new NotFoundException("Company not found");
            }

            // Validate province access
            _provinceFilter.ValidateProvinceAccess(company.ProvinceId);

            // Preserve original province ID (immutable)
            var originalProvinceId = company.ProvinceId;

            // Update properties
            company.Title = updatedCompany.Title;
            company.Status = updatedCompany.Status;
            company.DocPath = updatedCompany.DocPath;
            company.Tin = updatedCompany.Tin;

            // Ensure province hasn't changed
            company.ProvinceId = originalProvinceId;

            await _context.SaveChangesAsync();
            return company;
        }

        public async Task DeleteCompanyAsync(int id)
        {
            var company = await _context.CompanyDetails.FindAsync(id);
            if (company == null)
            {
                throw new NotFoundException("Company not found");
            }

            _provinceFilter.ValidateProvinceAccess(company.ProvinceId);

            _context.CompanyDetails.Remove(company);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates the IsComplete status of all licenses for a company based on required field validation
        /// Also generates license number when the license becomes complete for the first time
        /// </summary>
        public async Task UpdateLicenseCompletionStatusAsync(int companyId)
        {
            var company = await _context.CompanyDetails
                .Include(c => c.CompanyOwners)
                .Include(c => c.Guarantors)
                .FirstOrDefaultAsync(c => c.Id == companyId);

            if (company == null)
            {
                return;
            }

            // Get license details separately to avoid AreaId column issue
            var licenseDetails = await _context.LicenseDetails
                .Where(l => l.CompanyId == companyId)
                .ToListAsync();

            // Check if all required fields are filled
            bool isComplete = true;

            // 1. Check if company has a title
            if (string.IsNullOrWhiteSpace(company.Title))
            {
                isComplete = false;
            }

            // 2. Check if company has at least one owner with required fields
            if (!company.CompanyOwners.Any())
            {
                isComplete = false;
            }
            else
            {
                var owner = company.CompanyOwners.First();
                if (string.IsNullOrWhiteSpace(owner.FirstName) ||
                    string.IsNullOrWhiteSpace(owner.FatherName) ||
                    string.IsNullOrWhiteSpace(owner.ElectronicNationalIdNumber))
                {
                    isComplete = false;
                }
            }

            // 3. Check if company has at least one license with required fields (except LicenseNumber which will be auto-generated)
            if (!licenseDetails.Any())
            {
                isComplete = false;
            }
            else
            {
                var license = licenseDetails.First();
                // Don't check LicenseNumber here - it will be generated when complete
                if (!license.IssueDate.HasValue ||
                    !license.ExpireDate.HasValue ||
                    string.IsNullOrWhiteSpace(license.OfficeAddress))
                {
                    isComplete = false;
                }
            }

            // 4. Check if company has at least one guarantor with required fields
            if (company.Guarantors == null || !company.Guarantors.Any())
            {
                isComplete = false;
            }
            else
            {
                var guarantor = company.Guarantors.First();
                if (string.IsNullOrWhiteSpace(guarantor.FirstName) ||
                    string.IsNullOrWhiteSpace(guarantor.FatherName) ||
                    string.IsNullOrWhiteSpace(guarantor.ElectronicNationalIdNumber))
                {
                    isComplete = false;
                }
            }

            // Update the IsComplete status for all licenses of this company
            // AND generate license number if becoming complete for the first time
            if (licenseDetails.Any())
            {
                foreach (var license in licenseDetails)
                {
                    var wasIncomplete = !license.IsComplete;
                    license.IsComplete = isComplete;

                    // Generate license number when transitioning from incomplete to complete
                    // This ensures license numbers are only assigned to completed licenses
                    if (isComplete && wasIncomplete && string.IsNullOrWhiteSpace(license.LicenseNumber))
                    {
                        if (license.ProvinceId.HasValue)
                        {
                            try
                            {
                                license.LicenseNumber = await _licenseNumberGenerator.GenerateNextLicenseNumber(license.ProvinceId.Value);
                            }
                            catch (ArgumentException)
                            {
                                // If license number generation fails, keep the license incomplete
                                license.IsComplete = false;
                            }
                        }
                    }
                }
                await _context.SaveChangesAsync();
            }
        }
    }
}
