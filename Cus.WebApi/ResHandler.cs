using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Cus.WebApi
{
    class ResHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            ResManager.ProcessRes(context, (string)context.Items["res"]);
        }
    }
}
