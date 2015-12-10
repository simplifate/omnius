using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Nexus;
using FSS.Omnius.Modules.Nexus.Service;
using System;
using System.Collections.Generic;
using DbExtensions;

namespace FSPOC_WebProject.Controllers.Nexus
{
    public class ExtDBController : Controller
    {
        // GET: ExtDB list
        public ActionResult Index()
        {
            DBEntities e = new DBEntities();
            return View(e.ExtDBs);
        }

        #region Configuration Methods

        public ActionResult Create()
        {
            return View("~/Views/Nexus/ExtDB/Form.cshtml");
        }

        [HttpPost]
        public ActionResult Save(ExtDB model)
        {
            DBEntities e = new DBEntities();
            if (ModelState.IsValid)
            {
                // Záznam již existuje - pouze upravujeme
                if (!model.Id.Equals(null))
                {
                    ExtDB row = e.ExtDBs.Single(m => m.Id == model.Id);
                    row.DB_Type = model.DB_Type;
                    row.DB_Server = model.DB_Server;
                    row.DB_Port = model.DB_Port;
                    row.DB_Name = model.DB_Name;
                    row.DB_User = model.DB_User;
                    row.DB_Password = model.DB_Password;

                    e.SaveChanges();
                }
                else
                {
                    e.ExtDBs.Add(model);
                    e.SaveChanges();
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
            return View("~/Views/Nexus/ExtDB/Detail.cshtml", e.ExtDBs.Single(m => m.Id == id));
        }

        public ActionResult Edit(int id)
        {
            DBEntities e = new DBEntities();
            return View("~/Views/Nexus/ExtDB/Form.cshtml", e.ExtDBs.Single(m => m.Id == id));
        }

        public ActionResult Delete(int id)
        {
            DBEntities e = new DBEntities();
            ExtDB row = e.ExtDBs.Single(m => m.Id == id);

            e.ExtDBs.Remove(row);
            e.SaveChanges();

            return RedirectToRoute("Nexus", new { @action = "Index" });
        }

        #endregion

        #region Tools

        public ActionResult Test()
        {
            string result = "";

            NexusExtDBService service = new NexusExtDBService("localhost", "fss_rating");

            // Multiple rows
            /*JToken rows = service.NewQuery()
                .Select("*")
                .From("fss_employee")
                .OrderBy("lastname, firstname")
                .FetchAll();

            result += "<p>" + service.sql + "</p>";
            result += "<pre>" + rows.ToString() + "</pre>";*/

            // Single row
            /*JToken row = service.NewQuery()
                .Select("*")
                .From("fss_employee")
                .Where()
                    ._("firstname like {0}", "Martin")
                    ._("lastname like {0}", "Novák")
                .FetchOne();

            result += "<p>" + service.sql + "</p>";
            result += "<pre>" + row.ToString() + "</pre>";*/

            // Single cell
            /*object value = service.NewQuery()
                .Select("email")
                .From("fss_employee")
                .Limit(1)
                .Offset(5)
                .FetchCell("email");

            result += "<p>" + service.sql + "</p>";
            result += "<pre>" + value.ToString() + "</pre>";*/

            // Array of values
            /*int[] ids = new int[] { 1, 2, 3, 4, 5 };

            List<Object> arr = service.NewQuery()
                .Select("email")
                .From("fss_employee")
                .Where("id in ({0})", ids)
                .OrderBy("email")
                .FetchArray("email");

            result += "<p>" + service.sql + "</p>";
            result += "<pre>" + string.Join(", ", arr) + "</pre>";*/

            // hash
            JToken hash = service.NewQuery()
                .Select("CONCAT_WS(' ', lastname, firstname) as name, email")
                .From("fss_employee")
                .OrderBy("lastname desc, firstname desc")
                .FetchHash("name", "email");

            result += "<p>" + service.sql + "</p>";
            result += "<pre>" + hash.ToString() + "</pre>";

            // řádky jako hash
            JToken rowsAsHash = service.NewQuery()
                .Select("*")
                .From("fss_employee")
                .OrderBy("lastname, firstname")
                .FetchAllAsHash("email");

            result += "<p>" + service.sql + "</p>";
            result += "<pre>" + rowsAsHash.ToString() + "</pre>";

            // hash řádků se subquery
            JToken rowsAsHashArray = service.NewQuery()
                .Select()
                    ._("({0}) as email", service.NewSubquery()
                        .Select("email")
                        .From("fss_employee")
                        .Where("id = r.fss_employee_id")
                        .sql
                    )
                    ._("r.role")
                .From("fss_employee_role r")
                .OrderBy("email")
                .FetchAllAsHashArray("email");

            result += "<p>" + service.sql + "</p>";
            result += "<pre>" + rowsAsHashArray.ToString() + "</pre>";
            
            ViewBag.result = result;

            return View("~/Views/Nexus/ExtDB/Test.cshtml");
        }

        #endregion
    }
}