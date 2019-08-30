using StackExchange.Opserver.Data.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Opserver.Views;
using StackExchange.Opserver.Views.Dashboard;
using StackExchange.Opserver;
using StackExchange.Profiling;

namespace MSS.Platform.Monitor.Service
{
    public interface IOpServerService
    {
        Object GetDashboard(string q);
    }

    public class OpServerService : IOpServerService
    {
        public static DateTime DefaultStart => DateTime.UtcNow.AddDays(-1);
        public static DateTime DefaultEnd => DateTime.UtcNow;
        public Object GetDashboard(string q)
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

            foreach (var g in categories)
            {
                var c = g.Key;
                using (MiniProfiler.Current.Step("Category: " + c.Name))
                {
                    foreach (var n in g.OrderBy(n => n.PrettyName))
                    {
                        var tmp = n.CPULoad;
                    }
                }
                    
            }
            var ret = CPUData(vd.Nodes[0]);
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
