using Consul;
using Microsoft.Extensions.Configuration;
using MSS.Platform.ProcessApp.Data;
using MSS.Platform.ProcessApp.Model;
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
        private readonly IConsulRepo<ConsulServiceEntity> _repo;
        public ConsulService(IConfiguration configuration, IConsulRepo<ConsulServiceEntity> repo)
        {
            _configuration = configuration;
            _repo = repo;
        }


        public async Task<ApiResult> GetPageByParm(ConsulServiceEntityParm parm)
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
                List<ConsulObj> consulhealthlist = new List<ConsulObj>();
                foreach (var c in consulhealthresponse)
                {
                    consulhealthlist.Add(new ConsulObj() { ID = c.Value.ID, Service = c.Value.Service });
                }

                //var registedservice = _configuration.GetSection("ConsulServiceDB").Get<List<ConsulServiceEntity>>();
                ConsulServiceEntityView data = new ConsulServiceEntityView();
                var registedservice = await _repo.GetPageList(parm);
                data.total = registedservice.total;
                var dbdatalist = registedservice.rows;
                var query = from c in dbdatalist
                            join o in consulhealthlist on c.ServiceName equals o.Service
                             into g
                             from tt in g.DefaultIfEmpty()
                             select new ConsulServiceEntity { ServiceName = c.ServiceName,ServiceAddr = c.ServiceAddr,ServicePort = c.ServicePort
                             , HealthStatus = tt != null ? ConsulServiceStatus.Running:ConsulServiceStatus.Closed };

                data.rows = query.Cast<ConsulServiceEntity>().ToList<ConsulServiceEntity>();
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
        Task<ApiResult> GetPageByParm(ConsulServiceEntityParm parm);
    }


}
