using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace StackExchange.Opserver.Data.Dashboard.Providers
{
    internal partial class WmiDataProvider : DashboardDataProvider<WMISettings>, IServiceControlProvider
    {
        private readonly IEnumerable<WMISettings> _config;
        private readonly List<WmiNode> _wmiNodes = new List<WmiNode>();
        private readonly Dictionary<string, WmiNode> _wmiNodeLookup;

        public WmiDataProvider(IEnumerable<WMISettings> settings) : base(settings)
        {
            _config = settings;

            _wmiNodes = InitNodeList(_config).OrderBy(x => x.Endpoint).ToList();
            // Do this ref cast list once
            AllNodes.AddRange(_wmiNodes.Cast<Node>().ToList());
            // For fast lookups
            _wmiNodeLookup = new Dictionary<string, WmiNode>(_wmiNodes.Count);
            foreach (var n in _wmiNodes)
            {
                _wmiNodeLookup[n.Id] = n;
            }
        }

        /// <summary>
        /// Make list of nodes as per configuration.
        /// When adding, a node's IP address is resolved via DNS.
        /// </summary>
        /// <param name="nodeNames">The names of the server nodes to monitor.</param>
        private IEnumerable<WmiNode> InitNodeList(IEnumerable<WMISettings> settings)
        {
            var nodesList = new List<WmiNode>();
            var exclude = Current.Settings.Dashboard.ExcludePatternRegex;

            foreach (var setting in settings)
            {
                foreach (var nodeName in setting.Nodes)
                {
                    if (exclude?.IsMatch(nodeName) ?? false) continue;
                    var node = new WmiNode(nodeName)
                    {
                        Config = _config.FirstOrDefault(a => a.Nodes.Contains(nodeName)),
                        DataProvider = this
                    };

                    try
                    {
                        var hostEntry = Dns.GetHostEntry(node.Name);
                        if (hostEntry.AddressList.Any())
                        {
                            node.Ip = hostEntry.AddressList[0].ToString();
                            node.Status = NodeStatus.Active;
                        }
                        else
                        {
                            node.Status = NodeStatus.Unreachable;
                        }
                    }
                    catch (Exception)
                    {
                        node.Status = NodeStatus.Unreachable;
                    }

                    node.Caches.Add(ProviderCache(
                    () => node.PollNodeInfoAsync(),
                    node.Config.StaticDataTimeoutSeconds.Seconds(),
                    memberName: node.Name + "-Static"));

                    node.Caches.Add(ProviderCache(
                    () => node.PollStats(),
                    node.Config.DynamicDataTimeoutSeconds.Seconds(),
                    memberName: node.Name + "-Dynamic"));
                    nodesList.Add(node);
                }
            }
            return nodesList;
        }

        private WmiNode GetWmiNodeById(string id) =>
            _wmiNodeLookup.TryGetValue(id, out WmiNode n) ? n : null;

        public override int MinSecondsBetweenPolls => 10;

        public override string NodeType => "WMI";

        public override IEnumerable<Cache> DataPollers => _wmiNodes.SelectMany(x => x.Caches);

        protected override IEnumerable<MonitorStatus> GetMonitorStatus()
        {
            yield break;
        }

        protected override string GetMonitorStatusReason() => null;

        public override bool HasData => DataPollers.Any(x => x.ContainsData);

        public override List<Node> AllNodes { get; } = new List<Node>();
    }
}
