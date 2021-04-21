using Microsoft.Extensions.DependencyInjection;
using Plexo.Config;
using System;

namespace Plexo.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPlexoClient(this IServiceCollection services, Action<PlexoClientSettings> configureOptions)
        {
            services.Configure<PlexoClientSettings>(configureOptions);

            services.AddSingleton<IPlexoClient, PlexoClient>();

            return services;
        }
    }
}
