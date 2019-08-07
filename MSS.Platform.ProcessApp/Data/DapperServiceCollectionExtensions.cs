using Dapper.FluentMap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MSS.Platform.ProcessApp.Model;
using System;

namespace MSS.Platform.ProcessApp.Data
{
    public static class DapperServiceCollectionExtensions
    {
        public static IServiceCollection AddDapper(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            var optionsSection = configuration.GetSection("Dapper");
            var options = new DapperOptions();
            optionsSection.Bind(options);
            services.AddSingleton<DapperOptions>(options);

            services.AddTransient<IConsulRepo<ConsulServiceEntity>, ConsulRepo>();

            // 配置列名映射
            FluentMapper.Initialize(config =>
            {
                //config.AddMap(new BaseEntityMap());
                config.AddMap(new ConsulServiceEntityMap());
            });
            return services;
        }
    }
}
