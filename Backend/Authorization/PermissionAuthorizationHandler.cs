using Microsoft.AspNetCore.Authorization;

namespace WebAPIBackend.Authorization
{
    /// <summary>
    /// Handler for permission-based authorization
    /// Checks if user has the required permission in their JWT claims
    /// </summary>
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            // Check if user has the required permission claim
            var hasPermission = context.User.Claims.Any(c => 
                c.Type == "permission" && 
                c.Value == requirement.Permission);

            if (hasPermission)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
