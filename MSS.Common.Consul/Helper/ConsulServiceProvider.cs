using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

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
            string consulurl = "http://" + Configuration["ConsulServiceEntity:ConsulIP"] + ":" + Configuration["ConsulServiceEntity:ConsulPort"];
            var consuleClient = new ConsulClient(consulConfig =>
            {
                consulConfig.Address = new Uri(consulurl);
            });
            string ret = string.Empty;
            var data = await consuleClient.Agent.Services();
                
            var services = data.Response.Values.Where(s => s.Service.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
            if (!services.Any())
            {
                //Console.WriteLine("找不到服务的实例");
                throw new ApplicationException("找不到服务的实例");
            }
            else
            {
                var service = services.ElementAt(Environment.TickCount % services.Count());
                Console.WriteLine($"{service.Address}:{service.Port}");
                ret = "http://" + service.Address + ":" + service.Port;
            }

            return ret;
        }


    }
}
