using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WebAPIBackend.Configuration;
using WebAPIBackend.Models.Audit;

namespace WebAPIBackend.Services
{
    /// <summary>
    /// Service for comprehensive audit logging across the application
    /// </summary>
    public interface IComprehensiveAuditService
    {
        Task LogAsync(
            string actionType,
            string module,
            string description,
            string? descriptionDari = null,
            string? entityType = null,
            string? entityId = null,
            object? oldValues = null,
            object? newValues = null,
            string status = "Success",
            string? errorMessage = null,
            object? metadata = null,
            long? durationMs = null);

        Task LogLoginAsync(string userId, string? userName, string? userRole, bool success, string? errorMessage = null);
        Task LogLogoutAsync(string userId, string? userName);
        Task LogCreateAsync(string module, string entityType, string entityId, object? newValues = null, string? descriptionDari = null);
        Task LogUpdateAsync(string module, string entityType, string entityId, object? oldValues = null, object? newValues = null, string? descriptionDari = null);
        Task LogDeleteAsync(string module, string entityType, string entityId, object? oldValues = null, string? descriptionDari = null);
        Task LogViewAsync(string module, string entityType, string? entityId = null, string? descriptionDari = null);
        Task LogPrintAsync(string module, string entityType, string entityId, string? descriptionDari = null);
        Task LogExportAsync(string module, string description, string? descriptionDari = null);
        Task LogPasswordChangeAsync(string userId, bool success);
        Task LogPasswordResetAsync(string targetUserId, string performedByUserId);
        Task LogUserLockAsync(string targetUserId, string performedByUserId);
        Task LogUserUnlockAsync(string targetUserId, string performedByUserId);
        Task LogPermissionChangeAsync(string entityType, string entityId, string description, string? descriptionDari = null);
        Task LogErrorAsync(string module, string description, string errorMessage, string? entityType = null, string? entityId = null);
    }

    public class ComprehensiveAuditService : IComprehensiveAuditService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JsonSerializerOptions _jsonOptions;

        public ComprehensiveAuditService(
            AppDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task LogAsync(
            string actionType,
            string module,
            string description,
            string? descriptionDari = null,
            string? entityType = null,
            string? entityId = null,
            object? oldValues = null,
            object? newValues = null,
            string status = "Success",
            string? errorMessage = null,
            object? metadata = null,
            long? durationMs = null)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var user = httpContext?.User;

            var auditLog = new ComprehensiveAuditLog
            {
                UserId = user?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System",
                UserName = user?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? user?.Identity?.Name,
                UserRole = user?.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value,
                ActionType = actionType,
                Module = module,
                EntityType = entityType,
                EntityId = entityId,
                Description = description,
                DescriptionDari = descriptionDari,
                OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues, _jsonOptions) : null,
                NewValues = newValues != null ? JsonSerializer.Serialize(newValues, _jsonOptions) : null,
                IpAddress = GetClientIpAddress(httpContext),
                UserAgent = httpContext?.Request.Headers["User-Agent"].ToString(),
                RequestUrl = httpContext?.Request.Path.Value,
                HttpMethod = httpContext?.Request.Method,
                Status = status,
                ErrorMessage = errorMessage,
                Metadata = metadata != null ? JsonSerializer.Serialize(metadata, _jsonOptions) : null,
                UserProvince = user?.FindFirst("ProvinceId")?.Value,
                Timestamp = DateTime.UtcNow,
                DurationMs = durationMs
            };

            _context.Set<ComprehensiveAuditLog>().Add(auditLog);
            await _context.SaveChangesAsync();
        }

        public async Task LogLoginAsync(string userId, string? userName, string? userRole, bool success, string? errorMessage = null)
        {
            await LogAsync(
                success ? AuditActionTypes.Login : AuditActionTypes.FailedLogin,
                AuditModules.Authentication,
                success ? $"User {userName} logged in successfully" : $"Failed login attempt for user {userName}",
                success ? $"کاربر {userName} با موفقیت وارد سیستم شد" : $"تلاش ناموفش برای ورود کاربر {userName}",
                "User",
                userId,
                status: success ? "Success" : "Failed",
                errorMessage: errorMessage);
        }

        public async Task LogLogoutAsync(string userId, string? userName)
        {
            await LogAsync(
                AuditActionTypes.Logout,
                AuditModules.Authentication,
                $"User {userName} logged out",
                $"کاربر {userName} از سیستم خارج شد",
                "User",
                userId);
        }

        public async Task LogCreateAsync(string module, string entityType, string entityId, object? newValues = null, string? descriptionDari = null)
        {
            await LogAsync(
                AuditActionTypes.Create,
                module,
                $"Created {entityType} with ID {entityId}",
                descriptionDari ?? $"ثبت {entityType} با شماره {entityId}",
                entityType,
                entityId,
                newValues: newValues);
        }

        public async Task LogUpdateAsync(string module, string entityType, string entityId, object? oldValues = null, object? newValues = null, string? descriptionDari = null)
        {
            await LogAsync(
                AuditActionTypes.Update,
                module,
                $"Updated {entityType} with ID {entityId}",
                descriptionDari ?? $"تغییر {entityType} با شماره {entityId}",
                entityType,
                entityId,
                oldValues: oldValues,
                newValues: newValues);
        }

        public async Task LogDeleteAsync(string module, string entityType, string entityId, object? oldValues = null, string? descriptionDari = null)
        {
            await LogAsync(
                AuditActionTypes.Delete,
                module,
                $"Deleted {entityType} with ID {entityId}",
                descriptionDari ?? $"حذف {entityType} با شماره {entityId}",
                entityType,
                entityId,
                oldValues: oldValues);
        }

        public async Task LogViewAsync(string module, string entityType, string? entityId = null, string? descriptionDari = null)
        {
            await LogAsync(
                AuditActionTypes.View,
                module,
                entityId != null ? $"Viewed {entityType} with ID {entityId}" : $"Viewed {entityType} list",
                descriptionDari ?? (entityId != null ? $"مشاهده {entityType} با شماره {entityId}" : $"مشاهده لیست {entityType}"),
                entityType,
                entityId);
        }

        public async Task LogPrintAsync(string module, string entityType, string entityId, string? descriptionDari = null)
        {
            await LogAsync(
                AuditActionTypes.Print,
                module,
                $"Printed {entityType} with ID {entityId}",
                descriptionDari ?? $"چاپ {entityType} با شماره {entityId}",
                entityType,
                entityId);
        }

        public async Task LogExportAsync(string module, string description, string? descriptionDari = null)
        {
            await LogAsync(
                AuditActionTypes.Export,
                module,
                description,
                descriptionDari ?? description);
        }

        public async Task LogPasswordChangeAsync(string userId, bool success)
        {
            await LogAsync(
                AuditActionTypes.PasswordChange,
                AuditModules.UserManagement,
                success ? "Password changed successfully" : "Failed to change password",
                success ? "پسورد با موفقیت تغییر یافت" : "تغییر پسورد ناموفق بود",
                "User",
                userId,
                status: success ? "Success" : "Failed");
        }

        public async Task LogPasswordResetAsync(string targetUserId, string performedByUserId)
        {
            await LogAsync(
                AuditActionTypes.PasswordReset,
                AuditModules.UserManagement,
                $"Password reset for user {targetUserId} by admin",
                $"بازنشانی پسورد کاربر {targetUserId} توسط مدیر",
                "User",
                targetUserId,
                metadata: new { PerformedBy = performedByUserId });
        }

        public async Task LogUserLockAsync(string targetUserId, string performedByUserId)
        {
            await LogAsync(
                AuditActionTypes.UserLock,
                AuditModules.UserManagement,
                $"User {targetUserId} locked by admin",
                $"کاربر {targetUserId} توسط مدیر قفل شد",
                "User",
                targetUserId,
                metadata: new { PerformedBy = performedByUserId });
        }

        public async Task LogUserUnlockAsync(string targetUserId, string performedByUserId)
        {
            await LogAsync(
                AuditActionTypes.UserUnlock,
                AuditModules.UserManagement,
                $"User {targetUserId} unlocked by admin",
                $"کاربر {targetUserId} توسط مدیر باز شد",
                "User",
                targetUserId,
                metadata: new { PerformedBy = performedByUserId });
        }

        public async Task LogPermissionChangeAsync(string entityType, string entityId, string description, string? descriptionDari = null)
        {
            await LogAsync(
                AuditActionTypes.Assign,
                AuditModules.PermissionManagement,
                description,
                descriptionDari ?? description,
                entityType,
                entityId);
        }

        public async Task LogErrorAsync(string module, string description, string errorMessage, string? entityType = null, string? entityId = null)
        {
            await LogAsync(
                AuditActionTypes.Error,
                module,
                description,
                status: "Error",
                errorMessage: errorMessage,
                entityType: entityType,
                entityId: entityId);
        }

        private string? GetClientIpAddress(HttpContext? httpContext)
        {
            if (httpContext == null) return null;

            var ipAddress = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(ipAddress))
            {
                var addresses = ipAddress.Split(',', StringSplitOptions.TrimEntries);
                if (addresses.Length > 0)
                {
                    return addresses[0];
                }
            }

            return httpContext.Connection.RemoteIpAddress?.ToString();
        }
    }
}
