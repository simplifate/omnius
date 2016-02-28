﻿using FSS.Omnius.Modules.Entitron;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSS.Omnius.Controllers.EPK
{
    [PersonaAuthorize(AppId = 26)]
    public class OrdersController : Controller
    {
        public ActionResult Index()
        {
            return View("/Views/App/26/Page/15.cshtml");
        }
    }
}