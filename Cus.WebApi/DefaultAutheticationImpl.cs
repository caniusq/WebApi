using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cus.WebApi
{
    class DefaultAutheticationImpl : IAuthetication
    {
        const string SESSION_USER_KEY = "cus.webapi.user";

        public virtual Identity VerifyUser(System.Web.HttpContext context)
        {
            return null;
        }

        public virtual Identity GetUser(System.Web.HttpContext context)
        {
            return (Identity)context.Session[SESSION_USER_KEY];
        }

        public virtual void SaveUser(System.Web.HttpContext context, Identity user)
        {
            context.Session.Add(SESSION_USER_KEY, user);
        }
    }
}
