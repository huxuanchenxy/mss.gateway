using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MSS.Common.Consul.Controller
{
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        [HttpGet, Route("Health")]
        public IActionResult Get() => Ok("ok");
    }
}