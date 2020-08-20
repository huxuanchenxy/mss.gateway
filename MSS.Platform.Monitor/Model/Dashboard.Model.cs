using System.Collections.Generic;
using StackExchange.Opserver.Data.Dashboard;
using static StackExchange.Opserver.Data.Dashboard.HardwareSummary;

namespace StackExchange.Opserver.Views.Dashboard
{
    public class DashboardModel
    {
        public string Filter { get; set; }
        public List<string> ErrorMessages { get; set; }
        public List<Node> Nodes { get; set; }
        public bool IsStartingUp { get; set; }
    }

    public class ServerInfo
    {
        public string PrettyName { get; set; }
        public string CPULoad { get; set; }
        public string PrettyMemoryUsed { get; set; }
        public string PrettyTotalMemory { get; set; }
        public string PercentMemoryUsed { get; set; }
        public string PrettyTotalNetwork { get; set; }
        public string PrettyTotalVolumePerformance { get; set; }
        public string DiskText { get; set; }
        public string IP { get; set; }
        public List<TemperatureInfo> temps { get; set; }

    }
}