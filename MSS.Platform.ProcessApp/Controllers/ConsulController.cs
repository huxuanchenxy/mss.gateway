﻿using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<ApiResult>> GetServices()
        {
            ApiResult ret = new ApiResult { code = Code.Failure };
            ret = await _consulService.ListServiceAll();
            return ret;
        }
    }
}
