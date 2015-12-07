using System.Linq;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Nexus;
using System.DirectoryServices;
using System;
using System.Text;
using System.Collections;
using System.Reflection;
using FSS.Omnius.Modules.Nexus.Service;
using System.Collections.Generic;

namespace FSS.Omnius.Controllers.Nexus
{
    public class LDAPController : Controller
    {
        // GET: LDAP
        public ActionResult Index()
        {
            DBEntities e = new DBEntities();
            return View(e.Ldaps);
        }

        #region configuration methods

        public ActionResult Create()
        {
            return View("~/Views/Nexus/LDAP/Form.cshtml");
        }

        [HttpPost]
        public ActionResult Save(Ldap model)
        {
            DBEntities e = new DBEntities();
            if (ModelState.IsValid)
            {
                // Záznam již existuje - pouze upravujeme
                if (!model.Id.Equals(null))
                {
                    Ldap row = e.Ldaps.Single(m => m.Id == model.Id);
                    row.Domain_Ntlm = model.Domain_Ntlm;
                    row.Domain_Kerberos = model.Domain_Kerberos;
                    row.Domain_Server = model.Domain_Server;
                    row.Bind_User = model.Bind_User;
                    row.Bind_Password = model.Bind_Password.Length > 0 ? model.Bind_Password : row.Bind_Password;
                    row.Use_SSL = model.Use_SSL;
                    row.Active = model.Active;

                    e.SaveChanges();
                }
                else
                {
                    e.Ldaps.Add(model);
                    e.SaveChanges();
                }
                return RedirectToRoute("Nexus", new { @action = "Index" });
            }
            else
            {
                return View("~/Views/Nexus/LDAP/Form.cshtml", model);
            }
        }

        public ActionResult Detail(int id)
        {
            DBEntities e = new DBEntities();
            return View("~/Views/Nexus/LDAP/Detail.cshtml", e.Ldaps.Single(m => m.Id == id));
        }

        public ActionResult Edit(int id)
        {
            DBEntities e = new DBEntities();
            return View("~/Views/Nexus/LDAP/Form.cshtml", e.Ldaps.Single(m => m.Id == id));
        }

        public ActionResult Delete(int id)
        {
            DBEntities e = new DBEntities();
            Ldap row = e.Ldaps.Single(m => m.Id == id);

            e.Ldaps.Remove(row);
            e.SaveChanges(); 

            return RedirectToRoute("Nexus", new { @action = "Index" });
        }

        #endregion

        #region tools

        public ActionResult Search()
        {
            ViewBag.Result = String.Empty;
            if(Request.HttpMethod == "POST")
            {
                string query = Request.Form["query"];
                NexusLdapService service = new NexusLdapService();

                SearchResult user = service.SearchByLogin(query);
                ViewBag.Result = var_dump(user, 0);
            }
            
            return View("~/Views/Nexus/LDAP/Search.cshtml");
        }

        public ActionResult Groups()
        {
            SearchResultCollection groups;
            List<string> groupList = new List<string>();
            NexusLdapService service = new NexusLdapService();

            if(Request.HttpMethod == "POST")
            {
                string baseDN = Request.Form["query"];
                groups = service.GetGroups(baseDN);
            }
            else
            {
                groups = service.GetGroups();
            }
            
            foreach (SearchResult group in groups) {
                groupList.Add(var_dump(group, 0));
            }

            ViewBag.Result = groupList;

            return View("~/Views/Nexus/LDAP/GroupList.cshtml");

        }
        
        #endregion

        #region helper methods

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