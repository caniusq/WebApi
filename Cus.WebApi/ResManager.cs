using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Cus.WebApi
{
    class ResManager
    {
        static ResManager()
        {
            var info = new System.IO.FileInfo(typeof(ApiManager).Assembly.Location);
            DateTime t = info.LastWriteTimeUtc;
            _lastModified = new DateTime(t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second);
            _cinfo = CultureInfo.CreateSpecificCulture("en-US");
            _lastModifiedString = _lastModified.ToString("ddd, dd MMM yyyy HH':'mm':'ss 'GMT'", _cinfo);
        }

        private static CultureInfo _cinfo;
        private static DateTime _lastModified;
        private static string _lastModifiedString;
        private static Dictionary<string, string> _etags = new Dictionary<string, string>();

        public static string GetResourceId(string res)
        {
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

            context.Response.AddHeader("Last-Modified", _lastModifiedString);
            context.Response.AddHeader("Etag", etag);

            string imsString = context.Request.Headers["If-Modified-Since"];
            if (!string.IsNullOrEmpty(imsString))
            {
                DateTime ifModifiedSince = DateTime.Parse(imsString, _cinfo, DateTimeStyles.AdjustToUniversal);
                if (_lastModified <= ifModifiedSince)
                {
                    context.Response.StatusCode = 304;
                    return;
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
                case ".swf":
                    return "application/x-shockwave-flash";
                default:
                    return "text/html";
            }
        }
    }
}
