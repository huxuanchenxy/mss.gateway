using Dapper.FluentMap.Mapping;
using MSS.Platform.ProcessApp.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MSS.Platform.ProcessApp.Model
{
    public class ConsulModel
    {
    }
    public class ConsulServiceEntity: BaseEntity
    {
        public string ServiceName { get; set; }
        public string ServiceAddr { get; set; }
        public int ServicePort { get; set; }
        public int ServicePID { get; set; }
        
        public ConsulServiceStatus HealthStatus { get; set; }
    }

    public class ConsulObj
    {
        public string ID { get; set; }
        public string Service { get; set; }
        
    }

    public enum ConsulServiceStatus
    {
        Running = 1,
        Closed = 0
    }

    public class ConsulServiceEntityMap : EntityMap<ConsulServiceEntity>
    {
        public ConsulServiceEntityMap()
        {
            Map(o => o.ServiceName).ToColumn("service_name");
            Map(o => o.ServiceAddr).ToColumn("service_addr");
            Map(o => o.ServicePort).ToColumn("service_port");
            Map(o => o.ServicePID).ToColumn("service_pid");
            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.UpdatedBy).ToColumn("updated_by");
            Map(o => o.UpdatedTime).ToColumn("updated_time");
            Map(o => o.IsDel).ToColumn("is_del");
        }
    }

    public class ConsulServiceEntityParm : BaseQueryParm
    {
        public string ServiceName { get; set; }
    }

    public class ConsulServiceEntityView
    {
        public List<ConsulServiceEntity> rows { get; set; }
        public int total { get; set; }
    }


}
