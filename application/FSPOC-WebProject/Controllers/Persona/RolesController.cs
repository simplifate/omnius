﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using System.Data.Entity.Validation;

namespace FSPOC_WebProject.Controllers.Persona
{
    [PersonaAuthorize(NeedsAdmin = true, Module = "Persona")]
    public class RolesController : Controller
    {

        // GET: Roles
        public ActionResult App(int? Id)
        {
            ViewBag.Saved = false;

            using (var context = new DBEntities())
            {
                #region Getting app
                Application app;
                if (Id != null)
                {
                    app = context.Applications.Find(Id);
                }
                else
                {
                    app = context.Applications.First();
                }
                #endregion

                #region Basic variables declaration
                List<ColumnHeaderAppRolesForTable> colHeaders = new List<ColumnHeaderAppRolesForTable>();
                List<RowHeaderAppRolesForTable> rowHeaders = new List<RowHeaderAppRolesForTable>();
                List<bool[]> data = new List<bool[]>(); //bool[] jsou sloupce, List jsou radky 
                #endregion

                #region Rows headers
               
                   //find ad_group =>
                   ADgroup adg = context.ADgroups.SingleOrDefault(i => i.ApplicationId == Id);
                    foreach (ADgroup_User adgu in adg.ADgroup_Users) {
                        rowHeaders.Add(new RowHeaderAppRolesForTable(adgu.User.Id, adgu.User.DisplayName));

                   }
                #endregion

                #region Column headers + data
                var roles = context.Roles.Where(c => c.ADgroup.ApplicationId == app.Id);

                int x = 0;
                foreach (var role in roles)
                {
                    #region Data column prepare
                    //Creating a column length of rowHeaders.Count
                    bool[] boolColumn = new bool[rowHeaders.Count];
                    
                    data.Add(boolColumn);
                    #endregion

                    colHeaders.Add(new ColumnHeaderAppRolesForTable(role.Id, role.Name,role.Priority));

                    #region Data
                    List<int> MemberList = role.Users.Select(u => u.UserId).ToList();

                    for (int y = 0; y < MemberList.Count; y++)
                    {
                        int currID = MemberList[y];

                        int index = 0;
                        for (; index < rowHeaders.Count; index++)
                        {
                            if (currID == rowHeaders[index].Id)
                                break;

                            if (index == rowHeaders.Count + 1)
                            {
                                throw new IndexOutOfRangeException("There is no user with this ID");
                            }
                        }

                        data[x][index] = true;
                    }
                    #endregion

                    x++;
                }
                #endregion

                AjaxPersonaAppRolesForTable model = new AjaxPersonaAppRolesForTable(colHeaders, rowHeaders, data);
                model.AppName = app.DisplayName;
                model.AppID = app.Id;

                return View("App", model);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult App(AjaxPersonaAppRolesForTable model, string submitButton)
        {
            switch (submitButton)
            {
                case "addRole": return addColumn(model);

                case "save": return saveModel(model);

                default:
                    #region Remove column
                    if (submitButton.StartsWith("removeColumn"))
                    {
                        int colIndex = Convert.ToInt32(submitButton.Substring("removeColumn".Length));

                        model.ColHeaders[colIndex].IsDeleted = true;


                        if (model.DeletedCols == null)
                            model.DeletedCols = new List<int>();

                        model.DeletedCols.Add(colIndex);
                    } 
                    #endregion
                    break;
            }
            return View("App", model);
        }

        private ActionResult saveModel(AjaxPersonaAppRolesForTable model)
        {
            #region Column headers validation
            foreach (ColumnHeaderAppRolesForTable colHeader in model.ColHeaders)
            {
                colHeader.Name = colHeader.Name.Trim();
                if (String.IsNullOrWhiteSpace(colHeader.Name))
                {
                    ViewBag.BadNameRole = true;
                    ViewBag.Saved = false;
                    return View("App", model);
                }
              
                
            }

            for (int i = 0; i < model.ColHeaders.Count; i++)
            {
                ColumnHeaderAppRolesForTable currHeader = model.ColHeaders[i];
                for (int j = i + 1; j < model.ColHeaders.Count; j++)
                {
                    if (model.DeletedCols == null || (!model.DeletedCols.Contains(i) && !model.DeletedCols.Contains(j)))
                        if (currHeader.Name == model.ColHeaders[j].Name)
                        {
                            ViewBag.RolesAreEqual = true;
                            ViewBag.Saved = false;
                            return View("App", model);
                        }
                  
                }
                if (String.IsNullOrEmpty(currHeader.Priority.ToString()) || currHeader.Priority == 0)
                {
                    ViewBag.BadPriorityRole = true;
                    ViewBag.Saved = false;
                    return View("App", model);
                }
            }
            #endregion

            #region Save model
            using (var context = new DBEntities())
            {
                Application app = context.Applications.Find(model.AppID);

                #region Column headers + data
                var roles = context.Roles.Where(c => c.ADgroup.ApplicationId == app.Id);

                #region Save columns
                int x = 0;
                foreach (ColumnHeaderAppRolesForTable colHeader in model.ColHeaders)
                {
                    if (model.DeletedCols == null || !model.DeletedCols.Contains(x))
                    {
                        #region Add or update column
                        #region New users IDs - Data for this column
                        List<int> NewUsersIDsList = new List<int>();
                        for (int y = 0; y < model.RowHeaders.Count; y++)
                        {
                            if (model.Data[x][y] == true)
                            {
                                NewUsersIDsList.Add(model.RowHeaders[y].Id);
                            }
                        }
                        #endregion

                        if (colHeader.Id != -1)
                        {
                            #region Update role
                            PersonaAppRole realRole = roles.FirstOrDefault(a => a.Id == colHeader.Id);

                            if (realRole != null)
                            {
                                #region Role name
                                if (realRole.Name != colHeader.Name)
                                {
                                    realRole.Name = colHeader.Name;
                                }
                                if (realRole.Priority != colHeader.Priority)
                                {
                                    realRole.Priority = colHeader.Priority;
                                }
                                #endregion

                                realRole.Users.Clear();

                                foreach (int id in NewUsersIDsList)
                                {
                                    realRole.Users.Add(new User_Role() { AppRole = realRole, RoleId = realRole.Id, UserId = id, User = context.Users.FirstOrDefault(a => a.Id == id) });
                                }

                                context.SaveChanges();
                            }
                            #endregion
                        }
                        else
                        {
                            #region New role
                            PersonaAppRole realRole = new PersonaAppRole();

                            realRole.ADgroup = app.ADgroups.First();
                            realRole.Name = colHeader.Name;
                            realRole.Priority = colHeader.Priority;
                            if (realRole.Name == "Nová role")
                            {

                            }

                            #region Fill realRole with users
                            foreach (int id in NewUsersIDsList)
                            {
                                realRole.Users.Add(new User_Role() { AppRole = realRole, UserId = id, User = context.Users.FirstOrDefault(a => a.Id == id) });
                            }
                            #endregion

                            realRole = context.Roles.Add(realRole);

                            try
                            {
                                context.SaveChanges();
                            }
                            catch (DbEntityValidationException e)
                            {
                                foreach (var eve in e.EntityValidationErrors)
                                {
                                    string text = "Entity of type \"" + eve.Entry.Entity.GetType().Name + "\" in state \"" + eve.Entry.State + "\" has the following validation errors:";
                                    foreach (var ve in eve.ValidationErrors)
                                    {
                                        string message = "- Property: \"" + ve.PropertyName + "\", Error: \"" + ve.ErrorMessage + "\"";
                                    }
                                }
                                throw;
                            }
                            #endregion

                            colHeader.Id = realRole.Id;
                            #endregion
                        } 
                        #endregion
                    }
                    else
                    {
                        #region Delete column (role)
                        if (colHeader.Id != -1)
                        {
                            PersonaAppRole role = context.Roles.First(a => a.Id == colHeader.Id);

                            context.Roles.Remove(role);
                            context.SaveChanges();
                        }
                        #endregion
                    }

                    x++;
                }
                #endregion

                try
                {
                    context.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        string text = "Entity of type \""+ eve.Entry.Entity.GetType().Name + "\" in state \""+ eve.Entry.State + "\" has the following validation errors:";
                        foreach (var ve in eve.ValidationErrors)
                        {
                            string message = "- Property: \"" + ve.PropertyName + "\", Error: \"" + ve.ErrorMessage + "\"";
                        }
                    }
                    throw;
                }
                #endregion

                ViewBag.Saved = true;
            }

            return RedirectToAction("App", "Roles", new { @Id = model.AppID });//App(model.AppID);
        }

        private ActionResult addColumn(AjaxPersonaAppRolesForTable model)
        {
            #region ColHeader
            ColumnHeaderAppRolesForTable newColHeader = new ColumnHeaderAppRolesForTable(-1, "Nová role",0);

            if (model.ColHeaders == null)
            {
                model.ColHeaders = new List<ColumnHeaderAppRolesForTable>();
            }

            model.ColHeaders.Add(newColHeader);
            #endregion

            #region Data
            if (model.Data == null)
            {
                model.Data = new List<bool[]>();
            }

            model.Data.Add(new bool[model.RowHeaders.Count]); 
            #endregion

            return View("App", model);
        }

        private ActionResult removeColumn(AjaxPersonaAppRolesForTable model, int columnIndex)
        {
            model.ColHeaders[columnIndex].IsDeleted = true;

            return View("App", model);
        }
    }
}