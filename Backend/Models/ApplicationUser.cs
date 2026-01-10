using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhotoPath { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsLocked { get; set; }
        
        /// <summary>
        /// Associated company ID (0 for admin/system users)
        /// </summary>
        public int CompanyId { get; set; }

        /// <summary>
        /// License type from company (realEstate or carSale) - determines module access
        /// </summary>
        public string? LicenseType { get; set; }

        /// <summary>
        /// User's primary role for quick access
        /// </summary>
        public string? UserRole { get; set; }

        /// <summary>
        /// Date when user was created
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// User who created this account
        /// </summary>
        public string? CreatedBy { get; set; }
    }
}
