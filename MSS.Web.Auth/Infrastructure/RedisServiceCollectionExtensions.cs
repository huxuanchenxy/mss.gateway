using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSS.Web.Auth.Infrastructure
{
    public static class RedisServiceCollectionExtensions
    {
        public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            var optionsSection = configuration.GetSection("redis");
            var options = new RedisOptions();
            optionsSection.Bind(options);
            services.Configure<RedisOptions>(optionsSection);

            services.AddDistributedRedisCache(x =>
            {
                x.Configuration = options.ConnectionString;
                x.InstanceName = options.Instance;
                //x.ConfigurationOptions.SyncTimeout = 15000;
            });
            return services;
        }
    }
}
