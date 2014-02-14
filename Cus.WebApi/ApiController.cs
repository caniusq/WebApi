using Cus.WebApi.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace Cus.WebApi
{
    /// <summary>
    /// 接口控制器
    /// </summary>
    [ApiCode(ApiException.CODE_SUCCESS, "成功的返回")]
    [ApiCode(ApiException.CODE_ERROR, "服务异常")]
    [ApiCode(ApiException.CODE_MISS_METHOD, "方法不存在")]
    [ApiCode(ApiException.CODE_ARG_ERROR, "参数错误")]
    [ApiCode(ApiException.CODE_UNAUTH, "没有权限")]
    public abstract class ApiController
    {
        private static object _locker = new object();
        private static IAuthetication _authetication;

        internal IAuthetication Authetication
        {
            get
            {
                if (_authetication == null)
                {
                    lock (_locker)
                    {
                        if (_authetication == null)
                        {
                            _authetication = ConfigManager.CreateAuthetication();
                            if (_authetication == null) _authetication = new DefaultAutheticationImpl();
                        }
                    }
                }
                return _authetication;
            }
        }

        internal void InvokeUnhandledException(HttpContext context, string method, string input, Exception ex)
        {
            this.OnUnhandledException(context, new UnhandledApiExceptionEventArgs(method, input, ex));
        }

        /// <summary>
        /// 表示发生了除ApiException以外的异常
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="arg">异常事件参数</param>
        protected virtual void OnUnhandledException(HttpContext context, UnhandledApiExceptionEventArgs arg)
        {
        }

        internal Identity TempUser { get; set; }

        internal Identity InternalUser { get { return User; } }

        /// <summary>
        /// 获取或设置用户身份信息
        /// </summary>
        protected Identity User
        {
            get
            {
                if (TempUser != null) return TempUser;
                return Authetication.GetUser(HttpContext.Current);
            }
            set
            {
                Authetication.SaveUser(HttpContext.Current, value);
                TempUser = value;
                if (TempUser == null) TempUser = new Identity();
            }
        }
    }
}
