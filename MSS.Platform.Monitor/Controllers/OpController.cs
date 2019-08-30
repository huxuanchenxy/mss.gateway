using Microsoft.AspNetCore.Mvc;
using MSS.API.Common;
using MSS.Platform.Monitor.Service;

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
        public ActionResult<ApiResult> Dashboard(string q)
        {
            ApiResult ret = new ApiResult { code = Code.Failure };
            ret.data = _service.GetDashboard(q);
            return ret;
        }


    }
}
