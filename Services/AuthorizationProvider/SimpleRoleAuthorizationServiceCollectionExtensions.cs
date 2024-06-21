using Microsoft.AspNetCore.Authentication;

namespace WebRegistry.Services.AuthorizationProvider
{
    public static class SimpleRoleAuthorizationServiceCollectionExtensions
    {
        public static void AddSimpleRoleAuthorization<TRoleProvider>(this IServiceCollection services)
           where TRoleProvider : class, ISimpleRoleProvider
        {
            services.AddScoped<ISimpleRoleProvider, TRoleProvider>();
            services.AddScoped<IClaimsTransformation, SimpleRoleAuthorizationTransform>();
        }
    }
}
