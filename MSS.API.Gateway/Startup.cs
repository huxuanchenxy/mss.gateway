using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MSS.API.Gateway.OcelotMiddlewares;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Cache.Middleware;
using Ocelot.Configuration.Creator;
using Ocelot.Configuration.Repository;
using Ocelot.DownstreamRouteFinder.Middleware;
using Ocelot.DownstreamUrlCreator.Middleware;
using Ocelot.Errors.Middleware;
using Ocelot.Headers.Middleware;
using Ocelot.LoadBalancer.Middleware;
using Ocelot.Middleware.Pipeline;
using Ocelot.Request.Middleware;
using Ocelot.Requester.Middleware;
using Ocelot.RequestId.Middleware;
using Ocelot.Responder.Middleware;
using Serilog;
using Serilog.Events;

namespace MSS.API.Gateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            //var builder = new ConfigurationBuilder()
            //    .SetBasePath(env.ContentRootPath)
            //    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            //    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            //    // .AddJsonFile("hosting.json", optional: true)
            //    .AddJsonFile("configuration.json")
            //    .AddEnvironmentVariables();

            //Configuration = builder.Build();
            Configuration = configuration;
            var logger = new LoggerConfiguration()

.MinimumLevel.Debug()

        //.WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information).WriteTo.RollingFile(@"Logs\Info-{Date}.log"))
        .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Debug).WriteTo.RollingFile(@"Logs/Debug-{Date}.log"))

        .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error).WriteTo.RollingFile(@"Logs/Error-{Date}.log"))
//  .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Fatal).WriteTo.RollingFile(@"Logs\Fatal-{Date}.log"))

.CreateLogger();
            Log.Logger = logger;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            Action<IdentityServerAuthenticationOptions> isaOptMss = option =>
            {
                option.Authority = Configuration["IdentityService:Uri"];
                option.ApiName = "MssService";
                option.RequireHttpsMetadata = Convert.ToBoolean(Configuration["IdentityService:UseHttps"]);
                option.SupportedTokens = SupportedTokens.Both;
                //option.ApiSecret = Configuration["IdentityService:ApiSecrets:MssService"];
                option.JwtValidationClockSkew = TimeSpan.FromSeconds(0);//ÉèÖÃÊ±¼äÆ«ÒÆ

            };

            services.AddAuthentication("Bearer")
            .AddIdentityServerAuthentication("MssServiceKey", isaOptMss)
            ;

            services.AddOcelot(Configuration);

            //¿çÓò Cors
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            //app.UseStaticFiles();
            //app.UseCookiePolicy();
            app.UseCors("AllowAll");
            app.UseOcelot();

            //app.Map(new PathString("/ocelot/admin/configuration"), appBuilder =>
            //{
            //    appBuilder.Use(async (context, next) =>
            //    {
            //        // WARN: this api should be protected with permissions
            //        var configurationRepo =
            //            context.RequestServices.GetRequiredService<IFileConfigurationRepository>();
            //        var ocelotConfiguration = await configurationRepo.Get();
            //        var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("OcelotConfiguration");
            //        if (!ocelotConfiguration.IsError)
            //        {
            //            var internalConfigurationRepo = context.RequestServices.GetRequiredService<IInternalConfigurationRepository>();
            //            var internalConfigurationCreator =
            //                context.RequestServices.GetRequiredService<IInternalConfigurationCreator>();
            //            var internalConfiguration = await internalConfigurationCreator.Create(ocelotConfiguration.Data);
            //            if (!internalConfiguration.IsError)
            //            {
            //                internalConfigurationRepo.AddOrReplace(internalConfiguration.Data);
            //                context.Response.StatusCode = 200;
            //                return;
            //            }
            //            else
            //            {
            //                logger.LogError($"update ocelot configuration error, error in create ocelot internal configuration, error messages:{string.Join(", ", ocelotConfiguration.Errors)}");
            //            }
            //        }
            //        else
            //        {
            //            logger.LogError($"update ocelot configuration error, error in get ocelot configuration from configurationRepo, error messages:{string.Join(", ", ocelotConfiguration.Errors)}");
            //        }
            //        context.Response.StatusCode = 500;
            //    });
            //});
            //app.UseOcelotWhenRouteMatch((ocelotBuilder, pipelineConfiguration) =>
            //{
            //    // This is registered to catch any global exceptions that are not handled
            //    // It also sets the Request Id if anything is set globally
            //    //ocelotBuilder.UseExceptionHandlerMiddleware();
            //    //// This is registered first so it can catch any errors and issue an appropriate response
            //    //ocelotBuilder.UseResponderMiddleware();
            //    //ocelotBuilder.UseDownstreamRouteFinderMiddleware();
            //    //ocelotBuilder.UseDownstreamRequestInitialiser();
            //    //ocelotBuilder.UseRequestIdMiddleware();
            //    ////ocelotBuilder.UseMiddleware<ClaimsToHeadersMiddleware>();
            //    ////ocelotBuilder.UseLoadBalancingMiddleware();
            //    //ocelotBuilder.UseDownstreamUrlCreatorMiddleware();
            //    //ocelotBuilder.UseOutputCacheMiddleware();
            //    //ocelotBuilder.UseMiddleware<HttpRequesterMiddleware>();
            //    // cors headers
            //    //ocelotBuilder.UseMiddleware<CorsMiddleware>();
            //    ocelotBuilder.UseMiddleware<HttpResponderMiddleware>();
            //});
            //app.UseMvc();
            loggerFactory.AddSerilog();
        }
    }
}
