using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MSS.API.Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            int port = int.Parse(args[0]);
            return WebHost.CreateDefaultBuilder(args)
                            .UseStartup<Startup>()
                                          .UseKestrel(options =>
                                          {
                                              options.Limits.MaxRequestBodySize = 524288000;
                                              options.Listen(IPAddress.Any, port);
                                              //options.Listen(IPAddress.Any, 443, listenOptions =>
                                              //{
                                              //    listenOptions.UseHttps("server.pfx", "password");
                                              //});
                                          })
                            .ConfigureAppConfiguration((hostingContext, config) =>
                            {
                                config
                                    .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                                    .AddJsonFile("appsettings.json", true, true)
                                    //.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                                    //.AddJsonFile("ocelot.json", true, true)
                                    .AddJsonFile($"configuration.json")
                                    .AddEnvironmentVariables()

                                    ;
                            })
                            .Build();
        }
    }
}
