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
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string url = request.RequestUri.AbsolutePath;
                string[] urlarr = url.Split("/");
                if (urlarr.Length >= 4)
                {
                    string controllername = urlarr[2];
                    string actionname = urlarr[3];
                    string methodname = request.Method.ToString();
                    string header = request.Headers.Authorization.ToString();
                    if (!string.IsNullOrEmpty(header))
                    {
                        if (header.IndexOf("Bearer") >= 0)
                        {
                            var token = header.Replace("Bearer", "").Trim();
                            if (!string.IsNullOrEmpty(token))
                            {
                                try
                                {
                                    using (var redis = new CSRedisClient(_configuration["redis:ConnectionString"]))
                                    {
                                        var userid = redis.Get(token);
                                        //var userid = "3";
                                        var userobj = JsonConvert.DeserializeObject<UserTokenResponse>(redis.Get(userid));
                                        using (HttpClient client = new HttpClient())
                                        {
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
                                        }

                                    }
                                }
                                catch (Exception ex)
                                {
                                    
                                }
                            }
                        }
                            
                    }
                    
                }

                
            }
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
