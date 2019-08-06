using Consul;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MSS.Platform.ProcessApp.Service
{
    public class ConsulService : IConsulService
    {
        public IConfiguration _configuration { get; }
        public ConsulService(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public async Task<ApiResult> ListServiceAll()
        {
            ApiResult ret = new ApiResult();
            try
            {
                string consulurl = "http://" + _configuration["ConsulServiceEntity:ConsulIP"] + ":" + _configuration["ConsulServiceEntity:ConsulPort"];


                var consuleClient = new ConsulClient(consulConfig =>
                {
                    consulConfig.Address = new Uri(consulurl);
                });

                var consulhealthdata = await consuleClient.Agent.Services();
                var consulhealthresponse = consulhealthdata.Response;
                List<ConsulServiceEntity> consulhealthlist = new List<ConsulServiceEntity>();
                foreach (var c in consulhealthresponse)
                {
                    consulhealthlist.Add(new ConsulServiceEntity() { ID = c.Value.ID, Service = c.Value.Service });
                }

                var registedservice = _configuration.GetSection("ConsulServiceDB").Get<List<ConsulServiceEntity>>();

                var query = from c in registedservice
                             join o in consulhealthlist on c.ID equals o.ID
                             into g
                             from tt in g.DefaultIfEmpty()
                             select new ConsulServiceEntity { ID = c.ID,Service = c.Service, HealthStatus = tt != null ? ConsulServiceStatus.Running:ConsulServiceStatus.Closed };

                List<ConsulServiceEntity> data = query.Cast<ConsulServiceEntity>().ToList<ConsulServiceEntity>();
                ret.code = Code.Success;
                ret.data = data;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

    }

    public interface IConsulService
    {
        Task<ApiResult> ListServiceAll();
    }

    public class ConsulServiceEntity
    { 
        public string ID { get; set; }
        public string Service { get; set; }
        public ConsulServiceStatus HealthStatus { get; set; }
    }

    public enum ConsulServiceStatus
    {
        Running = 1,
        Closed = 0
    }

    public enum Code
    {
        [Description("接口调用成功")]
        Success = 0,
        [Description("接口调用失败")]
        Failure = 1,
        [Description("数据已存在")]
        DataIsExist = 2,
        [Description("数据不存在")]
        DataIsnotExist = 3,
        // 向不可添加子节点的节点添加节点
        [Description("数据校验失败")]
        CheckDataRulesFail = 4,
        [Description("绑定用户存在冲突")]
        BindUserConflict = 5
    }
    public class ApiResult
    {
        public Code code { get; set; }
        public string msg { get; set; }
        public object data { get; set; }
    }
}
