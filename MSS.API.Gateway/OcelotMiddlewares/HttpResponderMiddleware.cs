using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Ocelot.Logging;
using Ocelot.Middleware;

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
                if (context.HttpContext.Response.StatusCode == StatusCodes.Status200OK)
                {
                    RedisMSSHelper.Init(this._configuration["redis:ConnectionString"]);
                    string userid = RedisMSSHelper.Get("3");
                    //response = JsonConvert.DeserializeObject<UserTokenResponse>(RedisMSSHelper.Get(userid.ToString()));
                }
            }

            await _next(context);
        }
    }
}
