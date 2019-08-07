using System;
using System.ComponentModel;

namespace MSS.Platform.ProcessApp.Data
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


    public enum Code
    {
        [Description("接口调用成功")]
        Success = 0,
        [Description("接口调用失败")]
        Failure = 1,
        [Description("数据已存在")]
        DataIsExist = 2,
        [Description("数据不存在")]
        DataIsnotExist = 3,
        // 向不可添加子节点的节点添加节点
        [Description("数据校验失败")]
        CheckDataRulesFail = 4,
        [Description("绑定用户存在冲突")]
        BindUserConflict = 5
    }
    public class ApiResult
    {
        public Code code { get; set; }
        public string msg { get; set; }
        public object data { get; set; }
    }
}