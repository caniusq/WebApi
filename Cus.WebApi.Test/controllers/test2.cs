using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cus.WebApi.Test.api
{
    /// <summary>
    /// test2 的摘要说明
    /// </summary>
    [Documentation("~/App_Data/Cus.WebApi.Test.XML")]
    [ApiAuth(false)]
    public class test2 : ApiController
    {
        /// <summary>
        /// Foo 的摘要说明
        /// </summary>
        /// <param name="a">a 的摘要说明</param>
        /// <returns>return value</returns>
        public string Foo(string a)
        {
            return "bar";
        }
    }
}