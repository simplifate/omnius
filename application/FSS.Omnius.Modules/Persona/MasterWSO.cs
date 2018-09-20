using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.Nexus.Service;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Persona
{
    public class MasterWSO : MasterAuth, IMasterAuth
    {
        public override int Id => 3;
        public override string Name => "WSO";
        public override bool AllowLogout => false;

        public override IPersonaAuth CreateAuth(User user)
        {
            return new AuthWSO(user);
        }

        public override void Refresh()
        {
            var users = GetUserList();
            UpdateUsers(users);
        }

        public override void RedirectToLogin(HttpContext context)
        {
            context.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
        }

        private List<User> GetUserList()
        {
            /// INIT
            COREobject core = COREobject.i;
            DBEntities context = core.Context;

            //
            NexusWSService webService = new NexusWSService();
            object[] parameters = new[] { "Auction_User" };
            JToken results = webService.CallWebService("RWE_WSO_SOAP", "getUserListOfRole", parameters);
            var x = results.Children().First().Value<String>();

            //get the list of users names and add it to the list.
            IEnumerable<String> usersNames = results.Children().Values().Select(y => y.Value<String>());

            List<User> listUsers = new List<User>();

            //iterate list of usersNames and make USerObject
            foreach (string userName in usersNames)
            {
                object[] param = new[] { userName, null };
                JToken userClaim = webService.CallWebService("RWE_WSO_SOAP", "getUserClaimValues", param);
                User newUser = new User();
                newUser.AuthTypeId = Id;
                newUser.localExpiresAt = DateTime.Today;//for test
                newUser.LastLogin = DateTime.Today;
                newUser.CurrentLogin = DateTime.Today;
                newUser.EmailConfirmed = false;
                newUser.PhoneNumberConfirmed = false;
                newUser.TwoFactorEnabled = false;
                newUser.LockoutEnabled = false;
                newUser.AccessFailedCount = 0;
                newUser.isActive = true;
                foreach (JToken property in userClaim.Children())
                {
                    var a = (property.Children().Single(c => (c as JProperty).Name == "claimUri") as JProperty).Value.ToString();

                    switch (a)
                    {
                        case "email":
                            var email = (property.Children().Single(c => (c as JProperty).Name == "value") as JProperty).Value.ToString();
                            newUser.Email = email;
                            newUser.UserName = email;
                            break;

                        case "http://wso2.org/claims/mobile":
                            var mobile = (property.Children().Single(c => (c as JProperty).Name == "value") as JProperty).Value.ToString();
                            newUser.MobilPhone = mobile;
                            break;

                        case "http://wso2.org/claims/organization":
                            var organization = (property.Children().Single(c => (c as JProperty).Name == "value") as JProperty).Value.ToString();
                            newUser.Company = organization;
                            break;

                        case "fullname":
                            var fullname = (property.Children().Single(c => (c as JProperty).Name == "value") as JProperty).Value.ToString();
                            newUser.DisplayName = fullname;
                            break;
                        //SET ROLES FOR this newly created USER
                        case "http://wso2.org/claims/role":
                            var roles = (property.Children().Single(c => (c as JProperty).Name == "value") as JProperty).Value.ToString().Split(',').Where(r => r.Substring(0, 8) == "Auction_").Select(e => e.Remove(0, 8));
                            foreach (string role in roles)
                            {
                                PersonaAppRole approle = context.AppRoles.SingleOrDefault(r => r.Name == role && r.ApplicationId == core.Application.Id);
                                if (approle == null)
                                {
                                    context.AppRoles.Add(new PersonaAppRole() { Name = role, Application = core.Application, Priority = 0 });
                                    context.SaveChanges();
                                }
                                //User_Role userRole = newUser.Roles.SingleOrDefault(ur => ur.AppRole == approle && ur.User == newUser);
                                if (approle != null && !newUser.Users_Roles.Contains(new User_Role { RoleName = approle.Name, Application = approle.Application, User = newUser }))
                                {
                                    newUser.Users_Roles.Add(new User_Role { RoleName = approle.Name, Application = approle.Application, User = newUser });
                                }
                            }
                            break;
                    }
                }
                listUsers.Add(newUser);
            }

            return listUsers;
        }
        private void UpdateUsers(List<User> users)
        {
            var db = COREobject.i.Context;
            //iterate all users from WSO
            foreach (User user in users)
            {
                //if theres already this user. we will update the db
                var databaseUser = db.Users.SingleOrDefault(u => u.AuthTypeId == Id && u.UserName == user.UserName);
                if (databaseUser != null)
                {
                    databaseUser.UserName = user.UserName;
                    databaseUser.Company = user.Company;
                    databaseUser.MobilPhone = user.MobilPhone;
                    databaseUser.DisplayName = user.DisplayName;
                    databaseUser.Email = user.Email;
                    //Refresh ROLES
                    databaseUser.Roles.Clear();
                    foreach (User_Role role in user.Users_Roles)
                    {
                        databaseUser.Users_Roles.Add(new User_Role { RoleName = role.RoleName, Application = role.Application, User = databaseUser });
                    }
                    //end refresh roles
                    databaseUser.isActive = true;
                    db.SaveChanges();
                }

                //if the user is not in DB, we will add this user to DB
                else
                {
                    db.Users.Add(user);
                }
            }

            //iterate the users in the DB
            foreach (User user in db.Users)
            {
                //if this user is in db but not in the  WSO, set inactive
                if (!users.Any(u => u.UserName == user.UserName))
                {
                    user.isActive = false;
                }
            }
            db.SaveChanges();
        }
    }
}
