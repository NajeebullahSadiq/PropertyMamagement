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

        public CompanyService(AppDbContext context, IProvinceFilterService provinceFilter)
        {
            _context = context;
            _provinceFilter = provinceFilter;
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
        /// </summary>
        public async Task UpdateLicenseCompletionStatusAsync(int companyId)
        {
            var company = await _context.CompanyDetails
                .Include(c => c.CompanyOwners)
                .Include(c => c.LicenseDetails)
                .Include(c => c.Guarantors)
                .FirstOrDefaultAsync(c => c.Id == companyId);

            if (company == null)
            {
                return;
            }

            // Check if all required fields are filled
            bool isComplete = true;

            // 1. Check if company has a title
            if (string.IsNullOrWhiteSpace(company.Title))
            {
                isComplete = false;
            }

            // 2. Check if company has at least one owner with required fields
            if (company.CompanyOwners == null || !company.CompanyOwners.Any())
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

            // 3. Check if company has at least one license with required fields
            if (company.LicenseDetails == null || !company.LicenseDetails.Any())
            {
                isComplete = false;
            }
            else
            {
                var license = company.LicenseDetails.First();
                if (string.IsNullOrWhiteSpace(license.LicenseNumber) ||
                    !license.IssueDate.HasValue ||
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
            if (company.LicenseDetails != null && company.LicenseDetails.Any())
            {
                foreach (var license in company.LicenseDetails)
                {
                    license.IsComplete = isComplete;
                }
                await _context.SaveChangesAsync();
            }
        }
    }
}
