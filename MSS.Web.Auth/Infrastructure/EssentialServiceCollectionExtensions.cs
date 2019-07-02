
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MSS.Web.Auth.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSS.Web.Auth.Infrastructure
{
    public static class EssentialServiceCollectionExtensions
    {
        public static IServiceCollection AddEssentialService(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            services.AddTransient<IAPITokenDataProvider, APITokenDataProvider>();
            return services;
        }
    }
}
