using Microsoft.AspNetCore.Mvc;
using MSS.API.Common;
using MSS.Platform.Monitor.Service;
using StackExchange.Opserver.Views.Dashboard;
using System.Collections.Generic;

namespace MSS.Platform.Monitor.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OpController : ControllerBase
    {
        private readonly IOpServerService _service;
        public OpController(IOpServerService service)
        {
            _service = service;
        }


        [HttpGet("Dashboard")]
        public ActionResult<List<ServerInfo>> Dashboard(string q)
        {
            //ApiResult ret = new ApiResult { code = Code.Failure };
            //ret.data = _service.GetDashboard(q);
            var ret = _service.GetDashboard(q);
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
