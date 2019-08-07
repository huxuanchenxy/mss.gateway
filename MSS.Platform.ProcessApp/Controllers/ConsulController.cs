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


    }
}
