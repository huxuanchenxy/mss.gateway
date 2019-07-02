using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Ocelot.Logging;
using Ocelot.Middleware;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;

namespace MSS.API.Gateway.OcelotMiddlewares
{
    public class HttpResponderMiddleware : OcelotMiddleware
    {
        private readonly GatewayOptions _gatewayOptions;
        private readonly OcelotRequestDelegate _next;
        private IConfiguration _configuration { get; }

        public HttpResponderMiddleware(OcelotRequestDelegate next, IOptions<GatewayOptions> options, IOcelotLoggerFactory loggerFactory, IConfiguration configuration)
            : base(loggerFactory.CreateLogger<CorsMiddleware>())
        {
            _next = next;

            _gatewayOptions = options.Value;
            _configuration = configuration;
        }

        public async Task Invoke(DownstreamContext context)
        {
            string methodname = context.HttpContext.Request.Method.ToString();
            string header = context.HttpContext.Request.Headers["Authorization"].ToString();
            string url = context.HttpContext.Request.Path;
            string[] urlarr = url.Split("/");
            if (urlarr.Length >= 4)
            {
                string controllername = urlarr[2];
                string actionname = urlarr[3];
                if (context.HttpContext.Response.StatusCode == StatusCodes.Status200OK )
                {
                    
                    string token = string.Empty;
                    if (header.IndexOf("Bearer") >= 0)
                    {
                        token = header.Replace("Bearer", "").Trim();
                        //RedisHelper.Initialization(new CSRedis.CSRedisClient(_configuration["redis:connectionString"]));
                        RedisHelper.Initialization(new CSRedis.CSRedisClient("10.89.36.204:6379,password=Test01supersecret,defaultDatabase=7"));
                        string userid = RedisHelper.Get(token);
                        userid = "3";
                        if (!string.IsNullOrEmpty(userid))
                        {
                            UserTokenResponse userobj = JsonConvert.DeserializeObject<UserTokenResponse>(RedisHelper.Get(userid));

                            using (HttpClient client = new HttpClient())
                            {
                                client.DefaultRequestHeaders.Accept.Clear();
                                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                Dictionary<string, string> dic = new Dictionary<string, string>();
                                dic.Add("controller_name", controllername);
                                dic.Add("action_name", actionname);
                                dic.Add("method_name", methodname);
                                dic.Add("acc_name", userobj.acc_name);
                                dic.Add("user_name", userobj.user_name);
                                dic.Add("ip", LocalIPAddress);
                                dic.Add("mac_add", LocalMacAddress);

                                FormUrlEncodedContent data = new FormUrlEncodedContent(dic);
                                //string httpurl = this._configuration["operlog:posturl"];
                                string httpurl = "http://10.89.36.204:8003/api/v1/UserOperationLog/Add";
                                var repes = await client.PostAsync(httpurl, data);
                            }
                        }
                        
                           
                    }
                    
                    

                }
            }

            await _next(context);
        }

        public class UserTokenResponse
        {
            public string acc_name { get; set; }
            public string user_name { get; set; }

            public string ip { get; set; }
            public string mac { get; set; }
        }
        public static string LocalIPAddress
        {
            get
            {
                UnicastIPAddressInformation mostSuitableIp = null;
                var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

                foreach (var network in networkInterfaces)
                {
                    if (network.OperationalStatus != OperationalStatus.Up)
                        continue;
                    var properties = network.GetIPProperties();
                    if (properties.GatewayAddresses.Count == 0)
                        continue;

                    foreach (var address in properties.UnicastAddresses)
                    {
                        if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                            continue;
                        if (IPAddress.IsLoopback(address.Address))
                            continue;
                        return address.Address.ToString();
                    }
                }

                return mostSuitableIp != null
                    ? mostSuitableIp.Address.ToString()
                    : "";
            }
        }


        public static string LocalMacAddress
        {
            get
            {
                string macadd = string.Empty;

                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in nics)
                {
                    PhysicalAddress address = adapter.GetPhysicalAddress();
                    byte[] bytes = address.GetAddressBytes();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        // Display the physical address in hexadecimal.
                        //Console.Write("{0}", bytes[i].ToString("X2"));
                        macadd += bytes[i].ToString("X2");
                        // Insert a hyphen after each byte, unless we are at the end of the 
                        // address.
                        if (i != bytes.Length - 1)
                        {
                            //Console.Write("-");
                            macadd += "-";
                        }
                    }
                    break;//TODO
                }
                return macadd;
            }
        }
    }
}
