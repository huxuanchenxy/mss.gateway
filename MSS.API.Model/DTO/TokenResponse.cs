using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSS.API.Model.DTO
{
    public class TokenResponse
    {
        public int code { get; set; }
        public string error { get; set; }
        public string error_description { get; set; }
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }
        public string username { get; set; }
        public int userid { get; set; }
    }

}
