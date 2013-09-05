using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cus.WebApi
{
    /// <summary>
    /// 表示除ApiException以外的异常
    /// </summary>
    [Serializable]
    public class UnhandledApiExceptionEventArgs : EventArgs
    {
        private readonly string _method;
        private readonly string _input;
        private readonly Exception _ex;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="method">方法名称</param>
        /// <param name="input">输入数据</param>
        /// <param name="ex">异常</param>
        public UnhandledApiExceptionEventArgs(string method, string input, Exception ex)
        {
            _method = method;
            _input = input;
            _ex = ex;
        }

        /// <summary>
        /// 获取方法名称
        /// </summary>
        public string Method
        {
            get
            {
                return _method;
            }
        }

        /// <summary>
        /// 获取输入数据
        /// </summary>
        public string Input
        {
            get
            {
                return _input;
            }
        }

        /// <summary>
        /// 获取异常
        /// </summary>
        public Exception Exception
        {
            get
            {
                return _ex;
            }
        }
    }
}
