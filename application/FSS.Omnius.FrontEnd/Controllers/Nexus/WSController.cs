using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Nexus;
using FSS.Omnius.Modules.Nexus.Service;
using System.Text;
using System.Reflection;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Controllers.Nexus
{
    [PersonaAuthorize(NeedsAdmin = true, Module = "Nexus")]
    public class WSController : Controller
    {
        // GET: WS
        public ActionResult Index()
        {
            DBEntities e = COREobject.i.Context;
            ViewData["LdapServersCount"] = e.Ldaps.Count();
            ViewData["WebServicesCount"] = e.WSs.Count();
            ViewData["ExtDatabasesCount"] = e.ExtDBs.Count();
            ViewData["WebDavServersCount"] = e.WebDavServers.Count();
            ViewData["APICount"] = e.APIs.Count();
            ViewData["TCPSocketCount"] = e.TCPListeners.Count();
            ViewData["RabbitMQCount"] = e.RabbitMQs.Count();

            return View(e.WSs);
        }

        #region Configuration Methods

        public ActionResult Create()
        {
            return View("~/Views/Nexus/WS/Form.cshtml");
        }

        [HttpPost]
        public ActionResult Save(WS model, HttpPostedFileBase upload)
        {
            DBEntities e = COREobject.i.Context;
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    using (var reader = new System.IO.BinaryReader(upload.InputStream)) {
                        model.WSDL_File = reader.ReadBytes(upload.ContentLength);
                    }
                }


                // Záznam již existuje - pouze upravujeme
                if (!model.Id.Equals(null))
                {
                    WS row = e.WSs.Single(m => m.Id == model.Id);
                    row.Name = model.Name;
                    row.WSDL_Url = model.WSDL_Url;
                    row.Auth_User = model.Auth_User;
                    row.Auth_Password = model.Auth_Password;
                    row.SOAP_Endpoint = model.SOAP_Endpoint;
                    row.SOAP_XML_NS = model.SOAP_XML_NS;
                    row.REST_Base_Url = model.REST_Base_Url;
                    row.Type = model.Type;
                    

                    if (model.WSDL_File != null) {
                        row.WSDL_File = model.WSDL_File;
                    }
                    else {
                        model.WSDL_File = row.WSDL_File;
                    }

                    e.SaveChanges();
                }
                else
                {
                    e.WSs.Add(model);
                    e.SaveChanges();
                }

                // Pokud se jedná o SOAP WS, vygenerujeme proxy class
                if (model.Type == WSType.SOAP)
                {
                    NexusWSService service = new NexusWSService();
                    bool result = service.CreateProxyForWS(model);
                }
                
                return RedirectToRoute("Nexus", new { @action = "Index" });
            }
            else
            {
                return View("~/Views/Nexus/WS/Form.cshtml", model);
            }
        }

        public ActionResult Detail(int id)
        {
            DBEntities e = COREobject.i.Context;
            return View("~/Views/Nexus/WS/Detail.cshtml", e.WSs.Single(m => m.Id == id));
        }

        public ActionResult Edit(int id)
        {
            DBEntities e = COREobject.i.Context;
            return View("~/Views/Nexus/WS/Form.cshtml", e.WSs.Single(m => m.Id == id));
        }

        public ActionResult Delete(int id)
        {
            DBEntities e = COREobject.i.Context;
            WS row = e.WSs.Single(m => m.Id == id);

            e.WSs.Remove(row);
            e.SaveChanges();

            return RedirectToRoute("Nexus", new { @action = "Index" });
        }

        #endregion

        #region Tools

        public ActionResult Test()
        {
            NexusWSService service = new NexusWSService();

            object[] parameters = new[] { "Praha / Ruzyne", "Czech Republic" };
            JToken result = service.CallWebService("Global Weather", "GetWeather", parameters);

            ViewBag.result = result.ToString();

            return View("~/Views/Nexus/WS/Test.cshtml");
        }

        public ActionResult TestRest()
        {
            NexusWSService service = new NexusWSService();

            NameValueCollection queryParams = new NameValueCollection();
            queryParams.Add("text", "republic");

            // zavolá http://services.groupkt.com/country/search?text=republic
            JToken result = service.CallRestService("Country Search", "search", queryParams);

            ViewBag.result = result.ToString();

            return View("~/Views/Nexus/WS/Test.cshtml");
        }
        
        #endregion
    }
}