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

namespace FSPOC_WebProject.Controllers.Nexus
{
    public class WSController : Controller
    {
        // GET: WS
        public ActionResult Index()
        {
            DBEntities e = new DBEntities();
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
            DBEntities e = new DBEntities();
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
            DBEntities e = new DBEntities();
            return View("~/Views/Nexus/WS/Detail.cshtml", e.WSs.Single(m => m.Id == id));
        }

        public ActionResult Edit(int id)
        {
            DBEntities e = new DBEntities();
            return View("~/Views/Nexus/WS/Form.cshtml", e.WSs.Single(m => m.Id == id));
        }

        public ActionResult Delete(int id)
        {
            DBEntities e = new DBEntities();
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
            JObject result = service.CallWebService("Global Weather", "GetWeather", parameters);

            ViewBag.result = var_dump(result.SelectToken("CurrentWeather"), 0);

            return View("~/Views/Nexus/WS/Test.cshtml");
        }

        public ActionResult TestRest()
        {
            NexusWSService service = new NexusWSService();

            return View("~/Views/Nexus/WS/Test.cshtml");
        }

        private string var_dump(object obj, int recursion)
        {
            StringBuilder result = new StringBuilder();
            if (recursion < 5)
            {
                Type t = obj.GetType();
                PropertyInfo[] properties = t.GetProperties();

                foreach (PropertyInfo property in properties)
                {
                    try
                    {
                        object value = property.GetValue(obj, null);

                        string indent = String.Empty;
                        string spaces = "|   ";
                        string trail = "|...";

                        if (recursion > 0)
                        {
                            indent = new StringBuilder(trail).Insert(0, spaces, recursion - 1).ToString();
                        }

                        if (value != null)
                        {
                            string displayValue = value.ToString();
                            if (value is string) displayValue = String.Concat('"', displayValue, '"');

                            result.AppendFormat("{0}{1} = {2}\n", indent, property.Name, displayValue);

                            try
                            {
                                if (!(value is ICollection))
                                {
                                    result.Append(var_dump(value, recursion + 1));
                                }
                                else
                                {
                                    int elementCount = 0;
                                    foreach (object element in ((ICollection)value))
                                    {
                                        string elementName = String.Format("{0}[{1}]", property.Name, elementCount);
                                        indent = new StringBuilder(trail).Insert(0, spaces, recursion).ToString();

                                        result.AppendFormat("{0}{1} = {2}\n", indent, elementName, element.ToString());
                                        result.Append(var_dump(element, recursion + 2));
                                        elementCount++;
                                    }

                                    result.Append(var_dump(value, recursion + 1));
                                }
                            }
                            catch { }
                        }
                        else
                        {
                            result.AppendFormat("{0}{1} = {2}\n", indent, property.Name, "null");
                        }
                    }
                    catch
                    {

                    }
                }
            }
            return result.ToString();
        }

        #endregion
    }
}