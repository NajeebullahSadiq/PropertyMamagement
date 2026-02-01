namespace WebAPIBackend.Services
{
    /// <summary>
    /// Service for logging security-related events
    /// </summary>
    public interface ISecurityAuditLogger
    {
        void LogProvinceAccessViolation(
            string userId,
            string userProvince,
            string attemptedProvince,
            string resourceType,
            string resourceId,
            string operation);
    }

    public class SecurityAuditLogger : ISecurityAuditLogger
    {
        private readonly ILogger<SecurityAuditLogger> _logger;

        public SecurityAuditLogger(ILogger<SecurityAuditLogger> logger)
        {
            _logger = logger;
        }

        public void LogProvinceAccessViolation(
            string userId,
            string userProvince,
            string attemptedProvince,
            string resourceType,
            string resourceId,
            string operation)
        {
            _logger.LogWarning(
                "Province access violation: User {UserId} (Province: {UserProvince}) " +
                "attempted {Operation} on {ResourceType} {ResourceId} (Province: {AttemptedProvince})",
                userId, userProvince, operation, resourceType, resourceId, attemptedProvince);
        }
    }
}
