using Microsoft.AspNetCore.Mvc;
using MSS.API.Common;
using MSS.Platform.Monitor.Model;
using MSS.Platform.Monitor.Service;
using StackExchange.Opserver.Views.Dashboard;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSS.Platform.Monitor.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OpController : ControllerBase
    {
        private readonly IOpServerService _service;
        private readonly IConsulDeviceService _serviceDevice;
        public OpController(IOpServerService service, IConsulDeviceService serviceDevice)
        {
            _service = service;
            _serviceDevice = serviceDevice;
        }


        [HttpGet("Dashboard")]
        public ActionResult<List<ServerInfo>> Dashboard(string q)
        {
            //ApiResult ret = new ApiResult { code = Code.Failure };
            //ret.data = _service.GetDashboard(q);
            var ret = _service.GetDashboard(q);
            return ret;
        }

        [HttpGet("Dashboard2")]
        public async Task<ActionResult<ApiResult>> Dashboard2()
        {
            ConsulDeviceParm parm = new ConsulDeviceParm();
            parm.page = 1;
            parm.rows = 100;
            parm.order = "asc";
            parm.sort = "id";
            var ret = await _serviceDevice.GetPageList(parm);
            return ret;
        }

        [HttpGet("MonitorServer")]
        public ActionResult<ApiResult> MonitorServer()
        {
            ApiResult ret = new ApiResult { code = Code.Failure };
            ret.data = _service.GetMonitorServer();
            ret.code = Code.Success;
            return ret;
        }


    }
}
