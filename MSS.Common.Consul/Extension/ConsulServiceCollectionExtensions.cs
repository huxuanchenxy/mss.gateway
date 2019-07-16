using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace MSS.Common.Consul
{
    public static class ConsulServiceCollectionExtensions
    {
        public static IServiceCollection AddConsulService(this IServiceCollection services, IConfiguration _configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddOptions();
            services.Configure<ConsulServiceEntity>(_configuration.GetSection(nameof(ConsulServiceEntity)));
            services.AddTransient<IServiceDiscoveryProvider, ConsulServiceProvider>();
            return services;
        }
    }
}
