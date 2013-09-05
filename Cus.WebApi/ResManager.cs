using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Cus.WebApi
{
    class ResManager
    {
        private static Dictionary<string, string> _etags = new Dictionary<string, string>();

        public static string GetResourceId(string res)
        {
            var ss = typeof(ApiManager).Assembly.GetManifestResourceNames();
            return "Cus.WebApi.Resource." + res;
        }

        public static void ProcessRes(HttpContext context, string res)
        {
            string etag;
            string id = GetResourceId(res);

            lock (_etags)
            {
                if (!_etags.TryGetValue(id, out etag))
                {
                    using (var fs = typeof(ApiManager).Assembly.GetManifestResourceStream(id))
                    {
                        if (fs == null)
                        {
                            context.Response.StatusCode = 404;
                            return;
                        }
                        using (SHA1 sha1 = new SHA1Managed())
                        {
                            etag = Convert.ToBase64String(sha1.ComputeHash(fs));
                            _etags.Add(id, etag);
                        }
                    }
                }
            }
            string match = context.Request.Headers.Get("If-None-Match");
            if (!string.IsNullOrEmpty(match) && match == etag)
            {
                context.Response.StatusCode = 304;
                return;
            }

            using (var fs = typeof(ApiManager).Assembly.GetManifestResourceStream(id))
            {
                if (fs == null)
                {
                    context.Response.StatusCode = 404;
                    return;
                }
                context.Response.ContentType = MimeTypeByExt(Path.GetExtension(res));
                var buffer = new byte[4096];
                while (true)
                {
                    int len = fs.Read(buffer, 0, buffer.Length);
                    if (len == 0) break;
                    context.Response.OutputStream.Write(buffer, 0, len);
                }
                context.Response.OutputStream.Flush();
                context.Response.AddHeader("Etag", etag);
            }
        }

        private static string MimeTypeByExt(string ext)
        {
            switch (ext)
            {
                case ".css":
                    return "text/css";
                case ".js":
                    return "text/javascript";
                case ".png":
                    return "image/png";
                case ".gif":
                    return "image/gif";
                default:
                    return "text/html";
            }
        }
    }
}
