using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPIBackend.Models.Audit
{
    /// <summary>
    /// Comprehensive audit log for tracking all system activities
    /// </summary>
    [Table("ComprehensiveAuditLogs", Schema = "audit")]
    public class ComprehensiveAuditLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// User who performed the action
        /// </summary>
        [Required]
        [MaxLength(450)]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// User's display name at time of action
        /// </summary>
        [MaxLength(200)]
        public string? UserName { get; set; }

        /// <summary>
        /// User's role at time of action
        /// </summary>
        [MaxLength(100)]
        public string? UserRole { get; set; }

        /// <summary>
        /// Type of action performed (Create, Update, Delete, Login, Logout, etc.)
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string ActionType { get; set; } = string.Empty;

        /// <summary>
        /// Module/Area of the system (Property, Vehicle, User, License, etc.)
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Module { get; set; } = string.Empty;

        /// <summary>
        /// Specific entity type affected
        /// </summary>
        [MaxLength(100)]
        public string? EntityType { get; set; }

        /// <summary>
        /// ID of the affected entity
        /// </summary>
        [MaxLength(100)]
        public string? EntityId { get; set; }

        /// <summary>
        /// Human-readable description of the action
        /// </summary>
        [Required]
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Detailed information in Dari language
        /// </summary>
        [MaxLength(1000)]
        public string? DescriptionDari { get; set; }

        /// <summary>
        /// Previous values before change (JSON)
        /// </summary>
        public string? OldValues { get; set; }

        /// <summary>
        /// New values after change (JSON)
        /// </summary>
        public string? NewValues { get; set; }

        /// <summary>
        /// IP address of the user
        /// </summary>
        [MaxLength(45)]
        public string? IpAddress { get; set; }

        /// <summary>
        /// User agent / Browser information
        /// </summary>
        [MaxLength(500)]
        public string? UserAgent { get; set; }

        /// <summary>
        /// Request URL that triggered the action
        /// </summary>
        [MaxLength(500)]
        public string? RequestUrl { get; set; }

        /// <summary>
        /// HTTP method used
        /// </summary>
        [MaxLength(10)]
        public string? HttpMethod { get; set; }

        /// <summary>
        /// Status of the action (Success, Failed, Unauthorized)
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Success";

        /// <summary>
        /// Error message if action failed
        /// </summary>
        [MaxLength(1000)]
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Additional metadata (JSON)
        /// </summary>
        public string? Metadata { get; set; }

        /// <summary>
        /// Province the user was assigned to
        /// </summary>
        [MaxLength(100)]
        public string? UserProvince { get; set; }

        /// <summary>
        /// Timestamp of the action
        /// </summary>
        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Duration of the operation in milliseconds
        /// </summary>
        public long? DurationMs { get; set; }
    }

    /// <summary>
    /// Action types for audit logging
    /// </summary>
    public static class AuditActionTypes
    {
        public const string Create = "Create";
        public const string Update = "Update";
        public const string Delete = "Delete";
        public const string Login = "Login";
        public const string Logout = "Logout";
        public const string View = "View";
        public const string Export = "Export";
        public const string Print = "Print";
        public const string Verify = "Verify";
        public const string Approve = "Approve";
        public const string Reject = "Reject";
        public const string Assign = "Assign";
        public const string Revoke = "Revoke";
        public const string PasswordChange = "PasswordChange";
        public const string PasswordReset = "PasswordReset";
        public const string UserLock = "UserLock";
        public const string UserUnlock = "UserUnlock";
        public const string FailedLogin = "FailedLogin";
        public const string Unauthorized = "Unauthorized";
        public const string Error = "Error";
    }

    /// <summary>
    /// Modules for audit logging
    /// </summary>
    public static class AuditModules
    {
        public const string Authentication = "Authentication";
        public const string UserManagement = "UserManagement";
        public const string RoleManagement = "RoleManagement";
        public const string PermissionManagement = "PermissionManagement";
        public const string Property = "Property";
        public const string Vehicle = "Vehicle";
        public const string Company = "Company";
        public const string License = "License";
        public const string Securities = "Securities";
        public const string PetitionWriter = "PetitionWriter";
        public const string ActivityMonitoring = "ActivityMonitoring";
        public const string DistrictManagement = "DistrictManagement";
        public const string Report = "Report";
        public const string Verification = "Verification";
        public const string System = "System";
    }
}
