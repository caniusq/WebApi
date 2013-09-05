using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cus.WebApi
{
    /// <summary>
    /// 标记文档
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public sealed class DocumentationAttribute : Attribute
    {
        private readonly string _path;

        /// <summary>
        /// 文档特性
        /// </summary>
        /// <param name="path">文档路径</param>
        public DocumentationAttribute(string path)
        {
            _path = path;
        }

        /// <summary>
        /// 文档路径
        /// </summary>
        public string Path
        {
            get
            {
                return _path;
            }
        }
    }
}
