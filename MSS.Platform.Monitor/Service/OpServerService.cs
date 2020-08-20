using StackExchange.Opserver.Data.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Opserver.Views;
using StackExchange.Opserver.Views.Dashboard;
using StackExchange.Opserver;
using StackExchange.Profiling;
using StackExchange.Opserver.Models;
using Microsoft.Extensions.Configuration;
using MSS.API.Common.Utility;
using Newtonsoft.Json;

namespace MSS.Platform.Monitor.Service
{
    public interface IOpServerService
    {
        List<ServerInfo> GetDashboard(string q);
        List<ServerInfo> GetMonitorServer();
    }
    
    public class OpServerService : IOpServerService
    {
        public IConfiguration _configuration { get; }
        public OpServerService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public static DateTime DefaultStart => DateTime.Now.AddDays(-1);
        public static DateTime DefaultEnd => DateTime.Now;
        public List<ServerInfo> GetDashboard(string q)
        {
            var vd = new DashboardModel
            {
                Nodes = GetNodes(q),
                ErrorMessages = DashboardModule.ProviderExceptions.ToList(),
                Filter = q,
                IsStartingUp = DashboardModule.AnyDoingFirstPoll
            };
            var categories = vd.Nodes
                          .GroupBy(n => n.Category)
                          .Where(g => g.Any() && (g.Key != DashboardCategory.Unknown || true))
                          .OrderBy(g => g.Key.Index);
            List<ServerInfo> ret = new List<ServerInfo>();
            foreach (var g in categories)
            {
                var c = g.Key;

                foreach (var n in g.OrderBy(n => n.PrettyName))
                {
                    var tmp = n.CPULoad;
                    ServerInfo obj = new ServerInfo()
                    {
                        PrettyName = n.PrettyName,
                        CPULoad = n.CPULoad.ToString(),
                        PrettyMemoryUsed = n.MemoryUsed?.ToSize() ?? "",
                        PrettyTotalMemory = n.TotalMemory?.ToSize() ?? "",
                        PercentMemoryUsed = n.PercentMemoryUsed?.ToString("n2"),
                        PrettyTotalNetwork = n.TotalPrimaryNetworkbps < 0 ? null: n.TotalPrimaryNetworkbps.ToSpeed(),
                        PrettyTotalVolumePerformance = n.TotalVolumePerformancebps < 0 ? null : n.TotalVolumePerformancebps.ToSpeed(),
                        DiskText = n.Volumes?.Where(v => v?.PercentUsed.HasValue ?? false).DefaultIfEmpty().Max(v => v?.PercentUsed)?.ToString("n2"),
                        //temps = n.Hardware.Temps
                    };
                    ret.Add(obj);
                }


            }

            return ret;
        }

        public List<ServerInfo> GetMonitorServer()
        {
            List<ServerInfo> ret = new List<ServerInfo>();
            var monitorsArr = _configuration["MonitorServer"].Split(",");
            foreach (var m in monitorsArr)
            {
                string url = "http://" + m + "/api/v1/op/Dashboard";
                var resp = HttpClientHelper.GetResponse(url);
                List<ServerInfo> tmp = JsonConvert.DeserializeObject<List<ServerInfo>>(resp);
                tmp[0].IP = m.Split(":")[0];
                ret.Add(tmp[0]);
            }
            return ret;
        }


        private List<Node> GetNodes(string search) =>
    search.HasValue()
    ? DashboardModule.AllNodes.Where(n => n.SearchString?.IndexOf(search, StringComparison.InvariantCultureIgnoreCase) > -1).ToList()
    : DashboardModule.AllNodes.ToList();

        //public async Task<ActionResult> CPUJson(string id, long? start = null, long? end = null, bool? summary = false)
        //{
        //    var node = DashboardModule.GetNodeById(id);
        //    if (node == null) return JsonNotFound();
        //    var data = await CPUData(node, start, end, summary).ConfigureAwait(false);
        //    if (data == null) return JsonNotFound();

        //    return Json(data);
        //}

        public static async Task<object> CPUData(Node node, long? start = null, long? end = null, bool? summary = false)
        {
            var points = await node.GetCPUUtilization(start?.ToDateTime() ?? DefaultStart, end?.ToDateTime() ?? DefaultEnd, 1000).ConfigureAwait(false);
            if (points == null) return null;
            return new
            {
                points = points.Select(p => new
                {
                    date = p.DateEpoch,
                    value = p.Value ?? 0
                }),
                summary = summary.GetValueOrDefault(false) ? (await node.GetCPUUtilization(null, null, 2000).ConfigureAwait(false)).Select(p => new
                {
                    date = p.DateEpoch,
                    value = p.Value ?? 0
                }) : null
            };
        }
    }


}
