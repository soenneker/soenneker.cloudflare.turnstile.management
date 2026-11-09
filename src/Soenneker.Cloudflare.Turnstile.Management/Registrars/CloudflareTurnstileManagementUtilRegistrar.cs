using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Cloudflare.Turnstile.Management.Abstract;
using Soenneker.Cloudflare.Utils.Client.Registrars;

namespace Soenneker.Cloudflare.Turnstile.Management.Registrars;

/// <summary>
/// Registers Turnstile management and the shared Cloudflare client.
/// The shared client requires Cloudflare:ApiKey configuration when it is resolved.
/// </summary>
public static class CloudflareTurnstileManagementUtilRegistrar
{
    /// <summary>
    /// Adds <see cref="ICloudflareTurnstileManagementUtil"/> as a singleton service. <para/>
    /// </summary>
    public static IServiceCollection AddCloudflareTurnstileManagementUtilAsSingleton(this IServiceCollection services)
    {
        services.AddCloudflareClientUtilAsSingleton().TryAddSingleton<ICloudflareTurnstileManagementUtil, CloudflareTurnstileManagementUtil>();

        return services;
    }

    /// <summary>
    /// Adds <see cref="ICloudflareTurnstileManagementUtil"/> as a scoped service. <para/>
    /// </summary>
    public static IServiceCollection AddCloudflareTurnstileManagementUtilAsScoped(this IServiceCollection services)
    {
        services.AddCloudflareClientUtilAsSingleton().TryAddScoped<ICloudflareTurnstileManagementUtil, CloudflareTurnstileManagementUtil>();

        return services;
    }
}


