using Microsoft.AspNetCore.Mvc;
using MSS.Platform.ProcessApp.Data;
using MSS.Platform.ProcessApp.Model;
using MSS.Platform.ProcessApp.Service;
using System.Threading.Tasks;

namespace MSS.Platform.ProcessApp.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ConsulController : ControllerBase
    {
        private readonly IConsulService _consulService;
        public ConsulController(IConsulService consulService)
        {
            _consulService = consulService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult>> GetPageByParm([FromQuery] ConsulServiceEntityParm parm)
        {
            ApiResult ret = new ApiResult { code = Code.Failure };
            ret = await _consulService.GetPageByParm(parm);
            return ret;
        }

        [HttpGet("start/{id}")]
        public ActionResult<ApiResult> StartProcess(int id)
        {
            ApiResult ret = new ApiResult { code = Code.Failure };
            ret.data = _consulService.StartProcess(id);
            return ret;
        }

        [HttpGet("stop/{id}")]
        public ActionResult<ApiResult> StopProcess(int id)
        {
            ApiResult ret = new ApiResult { code = Code.Failure };
            _consulService.StopProcess(id);
            return ret;
        }

    }
}
