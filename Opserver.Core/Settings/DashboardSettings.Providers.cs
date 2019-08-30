using System.Collections.Generic;
using System.Linq;

namespace StackExchange.Opserver
{
    public class ProvidersSettings
    {
        public List<WMISettings> WMI { get; set; }

        public bool Any() => All.Any(p => p != null);

        public IEnumerable<IEnumerable<IProviderSettings>> All
        {
            get
            {
                yield return WMI;
            }
        }
    }

    public interface IProviderSettings
    {
        bool Enabled { get; }
        string Name { get; }

        void Normalize();
    }
}
