using System;
using System.Collections.Generic;
using System.Text;

namespace MSS.API.Model.Data
{
    public abstract class BaseEntity
    {
        public int id { get; set; }
        public DateTime created_time { get; set; }
        public int created_by { get; set; }
        public DateTime updated_time { get; set; }
        public int updated_by { get; set; }
    }
}
