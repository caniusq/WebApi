using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cus.WebApi
{
    /// <summary>
    /// 标记返回码
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public sealed class ApiCodeAttribute : Attribute
    {
        readonly int _code;
        readonly string _description;
        /// <summary>
        /// 返回码特性
        /// </summary>
        /// <param name="code">返回码</param>
        /// <param name="description">说明</param>
        public ApiCodeAttribute(int code, string description)
        {
            _code = code;
            _description = description;
        }

        /// <summary>
        /// 返回码
        /// </summary>
        public int Code
        {
            get { return _code; }
        }

        /// <summary>
        /// 说明
        /// </summary>
        public string Description
        {
            get { return _description; }
        }

        /// <summary>
        /// 分类 0:类自定义 1:类默认 2:方法自定义
        /// </summary>
        public int Category { get; internal set; }
    }
}
