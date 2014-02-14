using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cus.WebApi.Test.api
{
    [Documentation("~/App_Data/Cus.WebApi.Test.XML")]
    [ApiCode(500, "通用错误")]
    public class test : ApiController
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
        /// Logout 的摘要说明
        /// </summary>
        /// <param name="p1">p1 的摘要说明</param>
        /// <returns>method2测试返回数据</returns>
        [ApiAuth(true)]
        public string Logout(int p1)
        {
            base.User = null;
            return null;
        }
    }
}