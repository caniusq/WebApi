using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cus.WebApi
{
    /// <summary>
    /// 接口异常
    /// </summary>
    [Serializable]
    public class ApiException : Exception
    {
        /// <summary>
        /// 200:成功的返回
        /// </summary>
        public const int CODE_SUCCESS = 200;
        /// <summary>
        /// 201:服务异常
        /// </summary>
        public const int CODE_ERROR = 201;
        /// <summary>
        /// 202:方法不存在
        /// </summary>
        public const int CODE_MISS_METHOD = 202;
        /// <summary>
        /// 203:参数错误
        /// </summary>
        public const int CODE_ARG_ERROR = 203;
        /// <summary>
        /// 204:没有权限
        /// </summary>
        public const int CODE_UNAUTH = 204;

        /// <summary>
        /// 初始化接口异常
        /// </summary>
        /// <param name="message">信息</param>
        /// <param name="code">返回码</param>
        public ApiException(string message, int code)
            : base(message)
        {
            Code = code;
        }

        /// <summary>
        /// 获取或设置返回码
        /// </summary>
        public int Code { get; set; }
    }
}
