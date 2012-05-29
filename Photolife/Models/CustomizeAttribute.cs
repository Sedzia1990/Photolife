using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Photolife.Models
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            bool isAuthorized = AuthorizeCore(filterContext.HttpContext);

            if (!isAuthorized)
            {
                var result = new ViewResult();
                result.ViewName = "_Unauthorized";
                result.MasterName = "_Layout";
                filterContext.Result = result;
            }
        }
    }
}