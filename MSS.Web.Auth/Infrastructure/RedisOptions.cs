using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSS.Web.Auth.Infrastructure
{
    public class RedisOptions
    {
        public string ConnectionString { get; set; }
        public string Instance { get; set; }
    }
}
