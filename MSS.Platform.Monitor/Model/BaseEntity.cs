using System;
using System.ComponentModel;

namespace MSS.Platform.Monitor.Model
{
    public abstract class BaseEntity
    {
        public int ID { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedTime { get; set; }
        public bool IsDel { get; set; }
    }

    public abstract class BaseQueryParm
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int page { get; set; }
        /// <summary>
        /// 每页显示行数
        /// </summary>
        public int rows { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        public string sort { get; set; }
        /// <summary>
        /// asc/desc:顺序/降序
        /// </summary>
        public string order { get; set; }
    }

}