using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MSS.API.Dao.Interface;
using MSS.API.Model;
using MSS.API.Model.Data;
using MSS.API.Model.DTO;
using MSS.Common.Consul;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MSS.Web.Auth.Provider
{
    public class APITokenDataProvider : IAPITokenDataProvider
    {
        private static readonly HttpClient client;

        public IConfiguration Configuration { get; }
        //private readonly ILogger<APITokenDataProvider> _logger;

        private readonly IUserRepo<User> _userRepo;

        private readonly IDistributedCache _cache;
        private readonly IServiceDiscoveryProvider _consulclient;

        static APITokenDataProvider()
        {
            client = new HttpClient();
        }
        public APITokenDataProvider(IConfiguration configuration, IUserRepo<User> userRepo, IDistributedCache cache, IServiceDiscoveryProvider consulclient)
        {
            //_logger = logger;
            Configuration = configuration;
            _userRepo = userRepo;
            _cache = cache;
            _consulclient = consulclient;
        }


        public static string Encode(string data)
        {
            byte[] encodedBytes = System.Text.Encoding.UTF8.GetBytes(data);
            string encodedTxt = Convert.ToBase64String(encodedBytes);
            return encodedTxt;
        }
        public async Task<TokenResponse> GetApiTokenAsync(TokenRequest req)
        {


            TokenResponse ret = new TokenResponse();
            var url = Configuration["Ids:url"];
            var grant_type = "password";
            var client_id = Configuration["Ids:client_id"];
            var client_secret = Configuration["Ids:client_secret"];
            var scope = Configuration["Ids:scope"];


            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("client_id", client_id);
            dic.Add("client_secret", client_secret);
            dic.Add("grant_type", grant_type);
            dic.Add("username", req.username);
            dic.Add("password", req.password);
            dic.Add("scope", scope);

            FormUrlEncodedContent data = new FormUrlEncodedContent(dic);

            //var tempkey = await csredis.GetAsync("test1");

            try
            {
                //User user = await _userRepo.IsValid(new User() { acc_name = req.username });
                //if (user == null)
                //{
                //    throw new Exception("No this user");
                //}
                //if (user != null)
                //{
                //    Encrypt en = new Encrypt();
                //    var md5 = en.DoEncrypt(req.password, user.random_num);
                //    if (md5 != user.password)
                //    {
                //        throw new Exception("Wrong password");
                //    }
                //}

                string userurl = await _consulclient.GetServiceAsync("AuthService");
                userurl = userurl + "/api/v1/User/Login/" + req.username + "/" + req.password;
                var userresponse = await client.GetAsync(userurl);
                var userresponseString = await userresponse.Content.ReadAsStringAsync();
                var userrretobj = JsonConvert.DeserializeObject<ApiRet>(userresponseString);
                if (userrretobj != null && userrretobj.code != 0)
                {
                    throw new Exception("用户名或者密码错误");
                }
                var retuserid = userrretobj.data;
                //var retuserid = "1";
                var response = await client.PostAsync(url, data);
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                ret = JsonConvert.DeserializeObject<TokenResponse>(responseString);
                
                if (string.IsNullOrEmpty(ret.error))
                {
                    ret.code = 0;
                    //ret.username = req.username;
                    //await _cache.SetStringAsync(ret.access_token,user.id.ToString());
                    //await _cache.SetStringAsync(user.id.ToString(),JsonConvert.SerializeObject(redisobj));
                    //using (var csredis = new CSRedis.CSRedisClient(Configuration["redis:ConnectionString"]))
                    //{
                    await _cache.SetStringAsync(ret.access_token, retuserid, new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(int.Parse(Configuration["redis:ttl"])) });
                    //await _cache.SetStringAsync(user.id.ToString(), JsonConvert.SerializeObject(redisobj),null);
                    //}

                }
                else
                {
                    ret.code = -1;
                }


            }
            catch (Exception ex)
            {
                ret.code = -2;
                ret.error_description = ex.Message.ToString();

            }
            return ret;
        }

        public class ApiRet
        {
            public int code { get; set; }
            public string data { get; set; }
        }

        public async Task<TokenResponse> GetApiNewTokenAsync(TokenRequest req)
        {

            TokenResponse ret = new TokenResponse();
            var url = Configuration["Ids:url"];
            var grant_type = "refresh_token";
            var client_id = Configuration["Ids:client_id"];
            var client_secret = Configuration["Ids:client_secret"];


            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("client_id", client_id);
            dic.Add("client_secret", client_secret);
            dic.Add("grant_type", grant_type);
            dic.Add("refresh_token", req.refresh_token);

            FormUrlEncodedContent data = new FormUrlEncodedContent(dic);



            try
            {
                var redisret = await _cache.GetStringAsync(req.username);
                var redisobj = JsonConvert.DeserializeObject<TokenResponse>(redisret);
                var response = await client.PostAsync(url, data);
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                ret = JsonConvert.DeserializeObject<TokenResponse>(responseString);
                if (string.IsNullOrEmpty(ret.error))
                {
                    ret.code = 0;
                    ret.username = redisobj.username;
                    await _cache.SetStringAsync(req.username, JsonConvert.SerializeObject(ret));
                }
                else
                {
                    ret.code = -1;
                }


            }
            catch (Exception ex)
            {
                ret.code = -2;
                ret.error_description = ex.Message.ToString();

            }
            return ret;
        }
    }


}
