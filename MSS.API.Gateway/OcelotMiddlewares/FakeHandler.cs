using CSRedis;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Http;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using Serilog;
using Serilog.Events;

namespace MSS.API.Gateway.OcelotMiddlewares
{
    public class FakeHandler : DelegatingHandler
    {
        public IConfiguration _configuration { get; }
        public FakeHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private static Dictionary<string, string> dic = new Dictionary<string, string>()
        {   { "actiongroup", "权限组管理" },
            { "action","权限管理" },
            { "role","角色管理" },
            { "user","人员管理" },
            { "code","代码管理" },
            { "organization","组织架构" },
            { "eqptype","设备类型定义" },
            { "area2","位置配置" },
            { "equipment","设备定义" },
            { "area1","站区配置" },
            { "warnsetting","预警设置" },
             { "expert","专家库查询" },
            { "login","登录模块" }
        };

        private static Dictionary<string, string> dic2 = new Dictionary<string, string>()
        {   { "post", "新增" },
            { "put","修改" },
            { "delete","删除" },
        };

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //do stuff and optionally call the base handler..

            var response = await base.SendAsync(request, cancellationToken);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                //.WriteTo.Console()
                .WriteTo.File("Logs/handler.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("fake start");

            if (response.StatusCode == HttpStatusCode.OK && request.Method.ToString().ToUpper() != "GET")
            {
                string url = request.RequestUri.AbsolutePath;
                Log.Information(url);
                if (url.IndexOf("/v1") >= 0)
                {
                    url = url.Replace("/v1", "");
                }
                string[] urlarr = url.Split("/");
                if (urlarr.Length >= 4)
                {
                    Log.Information("444");
                    string controllername = urlarr[2];
                    string actionname = urlarr[3];
                    string methodname = request.Method.ToString();
                    string header = request.Headers.Authorization.ToString();
                    if (!string.IsNullOrEmpty(header))
                    {
                        Log.Information("header:" + header);
                        if (header.IndexOf("Bearer") >= 0)
                        {
                            var token = header.Replace("Bearer", "").Trim();
                            Log.Information("token:" + token);
                            if (!string.IsNullOrEmpty(token))
                            {
                                try
                                {
                                    using (var redis = new CSRedisClient(_configuration["redis:ConnectionString"]))
                                    {
                                        Log.Information("redis start:" + token);
                                        var userid = redis.Get(token);
                                        Log.Information("userid:" + userid);
                                        //var userid = "3";
                                        var userobj = JsonConvert.DeserializeObject<UserTokenResponse>(redis.Get(userid));
                                        using (HttpClient client = new HttpClient())
                                        {
                                            Log.Information("httpclient start");
                                            client.DefaultRequestHeaders.Accept.Clear();
                                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                                            string httpurl = _configuration["operlog:posturl"];

                                            HttpContextAccessor context = new HttpContextAccessor();
                                            var ip = context.HttpContext?.Connection.RemoteIpAddress.ToString();
                                            var macaddr = LocalMacAddress;

                                            controllername = dic[controllername.ToLower()];
                                            methodname = dic2[methodname.ToLower()];
                                            UserOperationLog parmobj = new UserOperationLog()
                                            {
                                                controller_name = controllername,
                                                action_name = actionname,
                                                method_name = methodname,
                                                acc_name = userobj.acc_name,
                                                user_name = userobj.user_name,
                                                ip = ip,
                                                mac_add = macaddr
                                            };
                                            var content = new StringContent(JsonConvert.SerializeObject(parmobj), Encoding.UTF8, "application/json");

                                            await client.PostAsync(httpurl, content);
                                            Log.Information("httpclient end ");
                                        }

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(ex.Message + " " + ex.StackTrace);
                                }
                            }
                        }

                    }

                }


            }

            Log.Information("fake end");
            return response;
        }




        public class UserTokenResponse
        {
            public string acc_name { get; set; }
            public string user_name { get; set; }

            public string ip { get; set; }
            public string mac { get; set; }
        }
        public class UserOperationLog
        {
            public string controller_name { get; set; }
            public string action_name { get; set; }

            public string method_name { get; set; }

            public string acc_name { get; set; }

            public string user_name { get; set; }

            public string ip { get; set; }

            public string mac_add { get; set; }
            public int id { get; set; }
            public DateTime created_time { get; set; }
            public int created_by { get; set; }
            public DateTime updated_time { get; set; }
            public int updated_by { get; set; }
            public int is_del { get; set; }

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
