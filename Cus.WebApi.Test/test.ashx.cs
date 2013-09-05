using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cus.WebApi.Test
{
    /// <summary>
    /// test 的摘要说明
    /// </summary>
    [Documentation("~/App_Data/Cus.WebApi.Test.XML")]
    [ApiCode(500, "通用错误")]
    public class test : ApiHandler
    {
        /// <summary>
        /// method1 的摘要说明
        /// </summary>
        /// <param name="p1">p1 111的摘要说明</param>
        /// <param name="p2">p2 的摘要说明</param>
        /// <param name="p3">p3 的摘要说明</param>
        /// <param name="p4">p4 的摘要说明</param>
        /// <returns>method1测试返回数据</returns>
        [ApiCode(501, "用户名或密码错误")]
        [ApiAuth(false)]
        public TestData SignIn(string p1, Dictionary<string, string> p2, int p3, DateTime? p4)
        {
            base.User = new Identity("chuyiyang", "test");
            return null;
        }

        /// <summary>
        /// method2 的摘要说明
        /// </summary>
        /// <param name="p1">p1 的摘要说明</param>
        /// <returns>method2测试返回数据</returns>
        [ApiAuth(false)]
        public string SignIn2(int p1)
        {
            base.User = null;
            return null;
        }
        
        /// <summary>
        /// method3 的摘要说明
        /// </summary>
        /// <param name="p1">p1 的摘要说明</param>
        [ApiAuth(true)]
        public void method3(IDictionary<string,string> p1)
        {

        }

        public TestData method4(string p1, float p2, bool p3, DateTime? p4)
        {
            return null;
        }
        public TestData method5(string p1, float p2, bool p3, DateTime? p4)
        {
            return null;
        }
        public TestData method6(string p1, float p2, bool p3, DateTime? p4)
        {
            return null;
        }
        public TestData method7(string p1, float p2, bool p3, DateTime? p4)
        {
            return null;
        }
        public TestData method8(string p1, float p2, bool p3, DateTime? p4)
        {
            return null;
        }
        public TestData method15(string p1, float p2, bool p3, DateTime? p4)
        {
            return null;
        }
        public TestData method16(string p1, float p2, bool p3, DateTime? p4)
        {
            return null;
        }
        public TestData methodmethodmethodmethodmethodmethodmethodmethodmethodmethodmethodmethodmethod17(string p1, float p2, bool p3, DateTime? p4)
        {
            return null;
        }
        public TestData method18(string p1, float p2, bool p3, DateTime? p4)
        {
            return null;
        }
        protected override void OnUnhandledException(HttpContext context, UnhandledApiExceptionEventArgs arg)
        {

        }
    }
}