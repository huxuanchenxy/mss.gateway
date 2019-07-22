using Microsoft.AspNetCore.Mvc;
using MSS.Web.Auth.Provider;
using System.Threading.Tasks;

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