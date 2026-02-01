using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using WebAPI.Models;
using WebAPIBackend.Configuration;

namespace WebAPIBackend.Helpers
{
    public static class RbacHelper
    {
        /// <summary>
        /// Check if user has any of the specified roles
        /// </summary>
        public static bool HasAnyRole(IList<string> userRoles, params string[] allowedRoles)
        {
            return userRoles.Any(r => allowedRoles.Contains(r));
        }

        /// <summary>
        /// Check if user can view all records (Admin, Authority, or specific module viewers)
        /// </summary>
        public static bool CanViewAllRecords(IList<string> userRoles, string module)
        {
            // Admin and Authority can view all
            if (HasAnyRole(userRoles, UserRoles.Admin, UserRoles.Authority))
                return true;

            return module.ToLower() switch
            {
                "property" => HasAnyRole(userRoles, UserRoles.CompanyRegistrar), // Company registrar can view all property records
                "vehicle" => HasAnyRole(userRoles, UserRoles.CompanyRegistrar),  // Company registrar can view all vehicle records
                "company" => HasAnyRole(userRoles, UserRoles.CompanyRegistrar, UserRoles.LicenseReviewer),
                _ => false
            };
        }

        /// <summary>
        /// Check if user should filter by company (PropertyOperator/VehicleOperator)
        /// </summary>
        public static bool ShouldFilterByCompany(IList<string> userRoles, string module)
        {
            // Admin and Authority see all records
            if (HasAnyRole(userRoles, UserRoles.Admin, UserRoles.Authority))
                return false;

            return module.ToLower() switch
            {
                "property" => HasAnyRole(userRoles, UserRoles.PropertyOperator),
                "vehicle" => HasAnyRole(userRoles, UserRoles.VehicleOperator),
                _ => false
            };
        }

        /// <summary>
        /// Check if user can create records in the specified module
        /// </summary>
        public static bool CanCreateRecords(IList<string> userRoles, string module)
        {
            // View-only roles cannot create
            if (HasAnyRole(userRoles, UserRoles.Authority, UserRoles.LicenseReviewer))
                return false;

            // Admin can create anything
            if (HasAnyRole(userRoles, UserRoles.Admin))
                return true;

            return module.ToLower() switch
            {
                "property" => HasAnyRole(userRoles, UserRoles.PropertyOperator),
                "vehicle" => HasAnyRole(userRoles, UserRoles.VehicleOperator),
                "company" => HasAnyRole(userRoles, UserRoles.CompanyRegistrar),
                _ => false
            };
        }

        /// <summary>
        /// Check if user can edit a specific record
        /// </summary>
        public static bool CanEditRecord(IList<string> userRoles, string module, string recordCreatedBy, string currentUserId)
        {
            // View-only roles cannot edit
            if (HasAnyRole(userRoles, UserRoles.Authority, UserRoles.LicenseReviewer))
                return false;

            // Admin can edit anything
            if (HasAnyRole(userRoles, UserRoles.Admin))
                return true;

            // Company registrar can edit company records
            if (module.ToLower() == "company" && HasAnyRole(userRoles, UserRoles.CompanyRegistrar))
                return true;

            // Property/Vehicle operators can only edit their own records
            if (module.ToLower() == "property" && HasAnyRole(userRoles, UserRoles.PropertyOperator))
                return recordCreatedBy == currentUserId;

            if (module.ToLower() == "vehicle" && HasAnyRole(userRoles, UserRoles.VehicleOperator))
                return recordCreatedBy == currentUserId;

            return false;
        }

        /// <summary>
        /// Check if user can delete records
        /// </summary>
        public static bool CanDeleteRecords(IList<string> userRoles, string module)
        {
            // Only Admin can delete
            return HasAnyRole(userRoles, UserRoles.Admin);
        }

        /// <summary>
        /// Check if user can access the specified module
        /// </summary>
        public static bool CanAccessModule(IList<string> userRoles, string? licenseType, string module)
        {
            // Admin and Authority can access all modules
            if (HasAnyRole(userRoles, UserRoles.Admin, UserRoles.Authority))
                return true;

            return module.ToLower() switch
            {
                "company" => HasAnyRole(userRoles, UserRoles.CompanyRegistrar, UserRoles.LicenseReviewer),
                "property" => HasAnyRole(userRoles, UserRoles.PropertyOperator, UserRoles.CompanyRegistrar) || licenseType == "realEstate",
                "vehicle" => HasAnyRole(userRoles, UserRoles.VehicleOperator, UserRoles.CompanyRegistrar) || licenseType == "carSale",
                "reports" => !HasAnyRole(userRoles, UserRoles.LicenseReviewer),
                "dashboard" => !HasAnyRole(userRoles, UserRoles.LicenseReviewer),
                "users" => HasAnyRole(userRoles, UserRoles.Admin),
                _ => false
            };
        }

        /// <summary>
        /// Get user ID from claims
        /// </summary>
        public static string? GetUserId(ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
        }

        /// <summary>
        /// Check if user is view-only
        /// </summary>
        public static bool IsViewOnly(IList<string> userRoles)
        {
            return HasAnyRole(userRoles, UserRoles.Authority, UserRoles.LicenseReviewer);
        }
    }
}
