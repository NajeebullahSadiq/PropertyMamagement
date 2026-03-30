using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace WebAPIBackend.Authorization
{
    /// <summary>
    /// Custom policy provider that creates policies dynamically based on permission names
    /// Allows using [Authorize(Policy = "permission.name")] without pre-registering policies
    /// </summary>
    public class PermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return _fallbackPolicyProvider.GetDefaultPolicyAsync();
        }

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        {
            return _fallbackPolicyProvider.GetFallbackPolicyAsync();
        }

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            // If policy name looks like a permission (contains a dot), create a dynamic policy
            if (policyName.Contains('.'))
            {
                var policy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new PermissionRequirement(policyName))
                    .Build();

                return Task.FromResult<AuthorizationPolicy?>(policy);
            }

            // Otherwise, fall back to default policy provider
            return _fallbackPolicyProvider.GetPolicyAsync(policyName);
        }
    }
}
