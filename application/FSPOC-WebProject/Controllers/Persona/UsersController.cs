using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FSPOC_WebProject.Models;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using Microsoft.AspNet.Identity;

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

        public ActionResult Detail(int id)
        {
            DBEntities e = new DBEntities();
            return View(e.Users.SingleOrDefault(x => x.Id == id));
        }

        public ActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Create(SetPasswordViewModel model)
        {
            DBEntities e=new DBEntities();

            //unikátní username, TODO lepší řešení vymyslet přes javascript
            if (e.Users.Where(x => x.UserName == model.user.UserName).ToList().Count > 0)
            {
                TempData["error"] = "Uživatelské jméno již existuje. Změňte prosím své uživatelské jméno.";
                return RedirectToAction("Create");
            }

            //user je lokalní, není řešeno přes AD
            model.user.isLocalUser = true;

            //datumy se nastavují protože datetime v databázi a v c# mají rozdílné min.hodnoty
            model.user.LastLogin = DateTime.Now;
            model.user.localExpiresAt = DateTime.Now;
            model.user.CurrentLogin = DateTime.Now;

            e.Users.Add(model.user);
            e.SaveChanges();
            
            return RedirectToRoute("Persona",new {@action = "SetPassword", @controller= "Account", @password=model.NewPassword, @userId=model.user.Id});
        }

        public ActionResult Update(int id)
        {
            DBEntities e = new DBEntities();
            User u = e.Users.SingleOrDefault(x => x.Id == id);

            return View(u);
        }

        [HttpPost]
        public ActionResult Edit(User model, string password, string confPassword)
        {
            DBEntities e = new DBEntities();

            FSS.Omnius.Modules.Entitron.Entity.Persona.User u = e.Users.SingleOrDefault(x => x.Id == model.Id);
            e.Users.AddOrUpdate(u, model);
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