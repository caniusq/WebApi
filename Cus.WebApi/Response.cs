using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace Cus.WebApi
{
    /// <summary>
    /// 默认响应
    /// </summary>
    class Response
    {
        /// <summary>
        /// 默认响应的构造函数
        /// </summary>
        public Response()
        {
            code = 200;
            reason = "成功的返回";
        }
        /// <summary>
        /// 返回码
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 返回说明
        /// </summary>
        public string reason { get; set; }
        /// <summary>
        /// 错误堆栈
        /// </summary>
        public string stacktrace { get; set; }
        /// <summary>
        /// 用户身份信息
        /// </summary>
        public Identity User { get; set; }
    }

    /// <summary>
    /// 默认响应
    /// </summary>
    class Response<T> : Response
    {
        /// <summary>
        /// 返回的业务数据
        /// </summary>
        public T result { get; set; }
    }

    /// <summary>
    /// 用户身份
    /// </summary>
    public class Identity
    {
        private readonly string _name;
        private readonly string _authType;
        private readonly bool _isAnonymous;
        private readonly bool _isAuthenticated;

        /// <summary>
        /// 构造一个匿名用户身份
        /// </summary>
        internal Identity()
        {
            _isAnonymous = true;
            _authType = "anonymous";
        }

        /// <summary>
        /// 构造一个已通过验证的用户身份
        /// </summary>
        /// <param name="name">当前用户的名称</param>
        /// <param name="authType">身份验证类型</param>
        public Identity(string name,string authType)
        {
            _name = name;
            _authType = authType;
            _isAuthenticated = true;
        }
        /// <summary>
        /// 身份验证类型
        /// </summary>
        public string AuthenticationType
        {
            get
            {
                return _authType;
            }
        }
        /// <summary>
        /// 是否匿名
        /// </summary>
        internal bool IsAnonymous
        {
            get
            {
                return _isAnonymous;
            }
        }
        /// <summary>
        /// 是否验证了用户
        /// </summary>
        public bool IsAuthenticated
        {
            get
            {
                return _isAuthenticated;
            }
        }
        /// <summary>
        /// 当前用户的名称
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }
    }
}
