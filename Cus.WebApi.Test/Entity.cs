using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cus.WebApi.Test
{
    /// <summary>
    /// 测试数据的说明
    /// </summary>
    public class TestData
    {
        /// <summary>
        /// test1的说明
        /// </summary>
        public List<string> test1 { get; set; }
        /// <summary>
        /// test2的说明
        /// </summary>
        public int? test2 { get; set; }
        /// <summary>
        /// test3的说明
        /// </summary>
        public int? test3 { get; set; }
        /// <summary>
        /// test4的说明
        /// </summary>
        public List<TestData> test4 { get; set; }
        /// <summary>
        /// test5的说明
        /// </summary>
        public TestSubData test5 { get; set; }
        /// <summary>
        /// test6的说明
        /// </summary>
        public List<TestSubData> test6 { get; set; }
    }

    /// <summary>
    /// TestSubData的说明
    /// </summary>
    public class TestSubData
    {
        /// <summary>
        /// sub1的说明
        /// </summary>
        public DateTime sub1 { get; set; }
        /// <summary>
        /// sub2的说明
        /// </summary>
        public string sub2 { get; set; }
        /// <summary>
        /// sub3的说明
        /// </summary>
        public byte[] sub3 { get; set; }
    }
}