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
    }
}
