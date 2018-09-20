using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FSS.Omnius.FrontEnd.Controllers.Persona;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using reCAPTCHA.MVC;

namespace FSS.Omnius.FrontEnd.Controllers
{
    public class CustomCaptchaValidator : CaptchaValidatorAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if(filterContext.Controller is AccountController) {
                if(AccountController.watchBadLogins) {
                    string userIp = filterContext.HttpContext.Request.UserHostAddress;
                    BadLoginCount record = COREobject.i.Context.BadLoginCounts.SingleOrDefault(a => a.IP == userIp);

                    if(record != null && record.AttemptsCount >= AccountController.captchaLimit) {
                        base.OnActionExecuting(filterContext);
                    }
                }
            }
        }
    }
}