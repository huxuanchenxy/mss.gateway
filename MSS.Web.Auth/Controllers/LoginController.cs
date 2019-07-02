using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MSS.API.Model.DTO;
using MSS.Web.Auth.Provider;

namespace MSS.Web.Auth.Controllers
{
    [Route("idsapi/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {

        private readonly IAPITokenDataProvider _APITokenDataProvider;
        public LoginController(
            IAPITokenDataProvider APITokenDataProvider)
        {
            _APITokenDataProvider = APITokenDataProvider;
        }

        [HttpPost, Route("GetToken")]
        public async Task<IActionResult> GetToken([FromBody]TokenRequest req)
        {
            
            var token = await _APITokenDataProvider.GetApiTokenAsync(req);
            //string api_key = Constants.Redis_API_Key;
            //var token = await _cache.GetStringAsync(api_key);
            return new JsonResult(token);
            //return View();
        }

        [HttpPost, Route("GetNewToken")]
        public async Task<IActionResult> GetNewToken([FromBody]TokenRequest req)
        {

            var token = await _APITokenDataProvider.GetApiNewTokenAsync(req);
            //string api_key = Constants.Redis_API_Key;
            //var token = await _cache.GetStringAsync(api_key);
            return new JsonResult(token);
            //return View();
        }

    }

}