using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Persona;

namespace FSPOC_WebProject.Controllers.Persona
{
    public class UsersController : Controller
    {
        // GET: Users
        public ActionResult Index()
        {
            DBEntities e= new DBEntities();
            return View(e.Users);
        }

        public ActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Create(User model)
        {
            DBEntities e=new DBEntities();
            e.Users.Add(model);
            e.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Update(int id)
        {
            DBEntities e = new DBEntities();
            FSS.Omnius.Modules.Entitron.Entity.Persona.User u = e.Users.SingleOrDefault(x => x.Id == id);

            return View(u);
        }

        [HttpPost]
        public ActionResult Edit(User model)
        {
            DBEntities e=new DBEntities();
            FSS.Omnius.Modules.Entitron.Entity.Persona.User u = e.Users.SingleOrDefault(x => x.Id == model.Id);
            e.Users.AddOrUpdate(model,u);
            e.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            DBEntities e= new DBEntities();
            e.Users.Remove(e.Users.SingleOrDefault(x => x.Id == id));
            e.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}