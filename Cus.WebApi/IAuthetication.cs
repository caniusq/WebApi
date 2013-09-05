using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Cus.WebApi
{
    /// <summary>
    /// 认证接口
    /// </summary>
    public interface IAuthetication
    {
        /// <summary>
        /// 验证用户
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns>用户</returns>
        Identity VerifyUser(HttpContext context);
        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns>用户</returns>
        Identity GetUser(HttpContext context);
        /// <summary>
        /// 存储用户
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="user">用户</param>
        void SaveUser(HttpContext context, Identity user);
    }
}
