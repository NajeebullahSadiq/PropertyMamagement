using Microsoft.Extensions.DependencyInjection;
using WebAPIBackend.Application.Common.Interfaces;
using WebAPIBackend.Infrastructure.Services;

namespace WebAPIBackend.Infrastructure
{
    /// <summary>
    /// Dependency injection configuration for Infrastructure layer
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // Register services
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IAppAuthorizationService, AppAuthorizationService>();

            return services;
        }
    }
}
