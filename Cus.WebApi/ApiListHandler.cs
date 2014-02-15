using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace Cus.WebApi
{
    class ApiListHandler : IHttpHandler
    {
        private static Encoding _encoding = new UTF8Encoding(false);
        private readonly List<Type> _types;
        public ApiListHandler(List<Type> types)
        {
            _types = types;
        }

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentEncoding = _encoding;
            context.Response.ContentType = "text/html";

            if (!ApiManager.IsDebug)
            {
                context.Response.StatusCode = 401;
                return;
            }

            if ("GET".Equals(context.Request.HttpMethod, StringComparison.CurrentCultureIgnoreCase))
            {
                string path = context.Request.Path.TrimEnd('/');
                var sb = new StringBuilder();
                sb.Append("<html>");
                sb.Append("<head>");
                sb.Append("<title>WebApi beta 1.0</title>");
                sb.AppendFormat("<link type='text/css' rel='stylesheet' href='{0}/special-res?res=main.css' />", path);
                sb.Append("</head>");
                sb.Append("<body>");
                sb.Append("<ul>");

                foreach (Type type in _types)
                {
                    sb.Append("<li>");
                    var apiManager = ApiManager.GetOrCreate(type);
                    apiManager.FetchClassDocumentation();
                    var api = apiManager.ApiDescriptor;
                    sb.AppendFormat("<div class='doc'>{0}</div>", api.Documentation);
                    sb.AppendFormat("<div><a href='{0}' target='_blank'>{0}</a></div>", path + "/" + api.Name);
                    sb.Append("</li>");
                }

                sb.Append("</ul>");
                sb.Append("</body>");
                sb.Append("</html>");
                context.Response.Write(sb.ToString());
            }
        }
    }
}
