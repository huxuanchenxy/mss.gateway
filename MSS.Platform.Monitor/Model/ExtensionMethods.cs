using EnumsNET;
using StackExchange.Opserver.Data;
using StackExchange.Opserver.Data.Dashboard;
using StackExchange.Opserver.Data.SQL;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Xsl;

namespace StackExchange.Opserver.Models
{
    public static class ExtensionMethods
    {
        public static string ToSpeed(this float bps, string unit = "b") =>
            bps < 1 ? "0 b/s" : $"{bps.ToSize(unit)}/s";
    }

    public static class VolumeExtensionMethods
    {
        public static string PercentFreeSpace(this Volume vol) => (100 - vol.PercentUsed)?.ToString("n0") + "% Free";

        public static string PrettyRead(this Volume i) => i.ReadBps?.ToSpeed();

        public static string PrettyWrite(this Volume i) => i.WriteBps?.ToSpeed();
    }

    public static class InterfaceExtensionMethods
    {
        public static string PrettyIn(this Interface i) => i.InBps?.ToSpeed();

        public static string PrettyOut(this Interface i) => i.OutBps?.ToSpeed();
    }

    public static class NodeExtensionMethods
    {
        public static string PrettyTotalMemory(this Node info) => info.TotalMemory?.ToSize() ?? "";

        public static string PrettyMemoryUsed(this Node info) => info.MemoryUsed?.ToSize() ?? "";

        public static MonitorStatus MemoryMonitorStatus(this Node info)
        {
            if (!info.PercentMemoryUsed.HasValue) return MonitorStatus.Unknown;
            if (info.MemoryCriticalPercent > 0 && info.PercentMemoryUsed > (float) info.MemoryCriticalPercent) return MonitorStatus.Critical;
            if (info.MemoryWarningPercent > 0 && info.PercentMemoryUsed > (float) info.MemoryWarningPercent) return MonitorStatus.Warning;
            return MonitorStatus.Good;
        }


        public static MonitorStatus CPUMonitorStatus(this Node info)
        {
            if (!info.CPULoad.HasValue) return MonitorStatus.Unknown;
            if (info.CPUCriticalPercent > 0 && info.CPULoad > info.CPUCriticalPercent) return MonitorStatus.Critical;
            if (info.CPUWarningPercent > 0 && info.CPULoad > info.CPUWarningPercent) return MonitorStatus.Warning;
            return MonitorStatus.Good;
        }



        public static string PrettyTotalNetwork(this Node info) =>
            info.TotalPrimaryNetworkbps < 0
                ? null
                : info.TotalPrimaryNetworkbps.ToSpeed();

        public static string PrettyTotalVolumePerformance(this Node info) =>
            info.TotalVolumePerformancebps < 0
                ? null
                : info.TotalVolumePerformancebps.ToSpeed();
    }

}