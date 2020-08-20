
using Dapper.FluentMap.Mapping;
using System.Collections.Generic;

// Coded by admin 2020/8/20 10:33:33
namespace MSS.Platform.Monitor.Model
{
    public class ConsulDeviceParm : BaseQueryParm
    {

    }
    public class ConsulDevicePageView
    {
        public List<ConsulDevice> rows { get; set; }
        public int total { get; set; }
    }

    public class ConsulDevice : BaseEntity
    {
        public int Id { get; set; }
        public string Ip { get; set; }
        public string PrettyName { get; set; }
        public decimal CpuLoad { get; set; }
        public decimal CpuUp { get; set; }
        public string PrettyMemoryUsed { get; set; }
        public string PrettyTotalMemory { get; set; }
        public decimal PercentMemoryUsed { get; set; }
        public decimal MemoryUp { get; set; }
        public string PrettyTotalNetwork { get; set; }
        public int DeviceTypeId { get; set; }
        public decimal Temperature { get; set; }
        public decimal TemperatureUp { get; set; }
        public decimal DiskUsed { get; set; }
        public decimal DiskUp { get; set; }
    }

    public class ConsulDeviceMap : EntityMap<ConsulDevice>
    {
        public ConsulDeviceMap()
        {
            Map(o => o.Id).ToColumn("id");
            Map(o => o.Ip).ToColumn("ip");
            Map(o => o.PrettyName).ToColumn("pretty_name");
            Map(o => o.CpuLoad).ToColumn("cpu_load");
            Map(o => o.CpuUp).ToColumn("cpu_up");
            Map(o => o.PrettyMemoryUsed).ToColumn("pretty_memory_used");
            Map(o => o.PrettyTotalMemory).ToColumn("pretty_total_memory");
            Map(o => o.PercentMemoryUsed).ToColumn("percent_memory_used");
            Map(o => o.MemoryUp).ToColumn("memory_up");
            Map(o => o.PrettyTotalNetwork).ToColumn("pretty_total_network");
            Map(o => o.DeviceTypeId).ToColumn("device_type_id");
            Map(o => o.Temperature).ToColumn("temperature");
            Map(o => o.TemperatureUp).ToColumn("temperature_up");
            Map(o => o.DiskUsed).ToColumn("disk_used");
            Map(o => o.DiskUp).ToColumn("disk_up");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.UpdatedTime).ToColumn("updated_time");
            Map(o => o.UpdatedBy).ToColumn("updated_by");
        }
    }

}