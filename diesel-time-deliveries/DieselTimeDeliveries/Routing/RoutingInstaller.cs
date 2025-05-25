using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using RegistR.Attributes.Extensions;
using Routing.Services;
using Wolverine.Attributes;

[assembly: WolverineModule]

namespace Routing;

public static class RoutingInstaller
{
    public static IServiceCollection RoutingInstall(this IServiceCollection services, string googleApiKey)
    {
        services.InstallRegisterAttribute(Assembly.GetExecutingAssembly());

        services.AddSingleton<IApiKeyProvider, ApiKeyProvider>(e => new ApiKeyProvider(googleApiKey));
        return services;
    }
}