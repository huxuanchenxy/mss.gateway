using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSS.Common.Consul
{
    public interface IServiceDiscoveryProvider
    {
        Task<List<string>> GetServicesAsync(string serviceName);
        Task<string> GetServiceAsync(string serviceName);
    }
}