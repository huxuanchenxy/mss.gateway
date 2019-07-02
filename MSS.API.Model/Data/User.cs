using System;
using System.Collections.Generic;
using System.Text;

namespace MSS.API.Model.Data
{
    public class User:BaseEntity
    {
        public string acc_name { get; set; }
        public string user_name { get; set; }
        public string password { get; set; }
        public int random_num { get; set; }
        public string job_number { get; set; }
        public string position { get; set; }
        public string id_card { get; set; }
        public DateTime birth { get; set; }
        public bool sex { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
        public string id_photo { get; set; }
        /// <summary>
        /// 存在没有任何权限，但下拉能选中的人员
        /// </summary>
        public int? role_id { get; set; }
        public bool is_del { get; set; }
        public bool is_super { get; set; }
    }
}
