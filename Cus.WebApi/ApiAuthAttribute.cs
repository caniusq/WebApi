using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cus.WebApi
{
    /// <summary>
    /// 标记是否需要身份验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ApiAuthAttribute : Attribute
    {
        private readonly bool _needAuth;

        /// <summary>
        /// 身份验证特性
        /// </summary>
        /// <param name="needAuth">是否需要身份验证</param>
        public ApiAuthAttribute(bool needAuth)
        {
            _needAuth = needAuth;
        }

        /// <summary>
        /// 是否需要身份验证
        /// </summary>
        public bool NeedAuth
        {
            get
            {
                return _needAuth;
            }
        }
    }
}
