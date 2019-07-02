using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSS.API.Model.DTO
{


    public class TokenRequest
    {
        public string username { get; set; }
        public string password { get; set; }
        public string refresh_token { get; set; }
    }
}
