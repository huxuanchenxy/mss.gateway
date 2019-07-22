using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;

namespace MSS.Common.Consul
{
    public class ConsulServiceProvider : IServiceDiscoveryProvider
    {
        public IConfiguration Configuration { get; }
        public ConsulServiceProvider(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public async Task<List<string>> GetServicesAsync(string serviceName)
        {
            string consulurl =  "http://" + Configuration["ConsulServiceEntity:ConsulIP"] + ":" + Configuration["ConsulServiceEntity:ConsulPort"];
            var consuleClient = new ConsulClient(consulConfig =>
            {
                consulConfig.Address = new Uri(consulurl);
            });

            var queryResult = await consuleClient.Health.Service(serviceName, string.Empty, true);

            //while (queryResult.Response.Length == 0)
            //{
            //    Console.WriteLine("No services found, wait 1s....");
            //    await Task.Delay(1000);
            //    queryResult = await consuleClient.Health.Service("ServiceA", string.Empty, true);
            //}
            var result = new List<string>();
            foreach (var serviceEntry in queryResult.Response)
            {
                result.Add(serviceEntry.Service.Address + ":" + serviceEntry.Service.Port);
            }

            //var kvlist = await consuleClient.KV.Get("user");
            //result.Add(JsonConvert.SerializeObject(kvlist.Response.Value));



            return result;
        }

        public async Task<string> GetServiceAsync(string serviceName)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                //.WriteTo.Console()
                .WriteTo.File("ConsolLogs/GetService.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            string consulurl = "http://" + Configuration["ConsulServiceEntity:ConsulIP"] + ":" + Configuration["ConsulServiceEntity:ConsulPort"];

            Log.Information("Start Find Consul Address");
            var consuleClient = new ConsulClient(consulConfig =>
            {
                consulConfig.Address = new Uri(consulurl);
            });
            string ret = string.Empty;
            var data = await consuleClient.Agent.Services();

            Log.Information("consuleClient return data:" + JsonConvert.SerializeObject(data));

            var services = data.Response.Values.Where(s => s.Service.Equals(serviceName, StringComparison.OrdinalIgnoreCase));

            Log.Information("consuleClient return services:" + JsonConvert.SerializeObject(services));
            if (!services.Any())
            {
                Log.Information("consuleClient ApplicationException: 找不到服务的实例");
                //Console.WriteLine("找不到服务的实例");
                throw new ApplicationException("找不到服务的实例");
            }
            else
            {
                var service = services.ElementAt(Environment.TickCount % services.Count());
                //Console.WriteLine($"{service.Address}:{service.Port}");
                ret = "http://" + service.Address + ":" + service.Port;
                Log.Information("consuleClient have find ip port : " + ret);
            }

            return ret;
        }


    }
}
