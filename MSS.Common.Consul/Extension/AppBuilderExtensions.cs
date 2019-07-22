using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using System;

namespace MSS.Common.Consul
{
    public static class AppBuilderExtensions
    {
        public static IApplicationBuilder RegisterConsul(this IApplicationBuilder app, IApplicationLifetime lifetime,IOptions<ConsulServiceEntity>  serviceEntity)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                //.WriteTo.Console()
                .WriteTo.File("ConsolLogs/Regist.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("Consul Start registration");
            var consulClient = new ConsulClient(x => x.Address = new Uri($"http://{serviceEntity.Value.ConsulIP}:{serviceEntity.Value.ConsulPort}"));
            Log.Information("Consul Init ConsulCient :" + JsonConvert.SerializeObject(consulClient));
            var httpCheck = new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                Interval = TimeSpan.FromSeconds(10),
                HTTP = $"http://{serviceEntity.Value.IP}:{serviceEntity.Value.Port}/health",
                Timeout = TimeSpan.FromSeconds(5),
                
             
            };

            // Register service with consul
            var registration = new AgentServiceRegistration()
            {
                Checks = new[] { httpCheck },
                //ID = Guid.NewGuid().ToString(),
                ID = serviceEntity.Value.IP + ":" + serviceEntity.Value.Port,
                Name = serviceEntity.Value.ServiceName,
                Address = serviceEntity.Value.IP,
                Port = serviceEntity.Value.Port,
                
                Tags = new[] { $"urlprefix-/{serviceEntity.Value.ServiceName}" }
            };

            consulClient.Agent.ServiceRegister(registration).Wait();

            Log.Information("Consul Agent ServiceRegister Wait Finish ");
            lifetime.ApplicationStopping.Register(() =>
            {
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
                Log.Information("Consul Agent ServiceRegister Quit Finish ");
            });

            app.Map("/health", HealthMap);

            Log.Information("Consul Add HealthCheck ");
            return app;
        }

        private static void HealthMap(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("OK");
            });
        }
    }
}
