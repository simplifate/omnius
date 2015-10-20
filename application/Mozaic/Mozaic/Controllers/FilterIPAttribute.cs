using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mozaic.Controllers
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method)]
    internal class FilterIPAttribute : AuthorizeAttribute
    {
        public string allowedIp { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return (allowedIp.Split(';').Contains(httpContext.Request.UserHostAddress));
        }
    }
}