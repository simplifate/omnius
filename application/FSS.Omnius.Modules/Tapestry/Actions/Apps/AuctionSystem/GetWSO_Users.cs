using System;
using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Nexus.Service;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.Entitron.Entity;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.Persona;

namespace FSS.Omnius.Modules.Tapestry.Actions.AuctionSystem
{
    [Obsolete]
    public class GetWSO_Users : Action
    {
        public override int Id => 5000;

        public override string[] InputVar => new string[0];

        public override string Name => "GetWsoUsers";

        public override string[] OutputVar => new string[0];

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            //COREobject core = COREobject.i;
            //DBEntities db = core.AppContext;

            //NexusWSService webService = new NexusWSService();
            //object[] parameters = new[] { "Auction_User" };
            //JToken results = webService.CallWebService("RWE_WSO_SOAP", "getUserListOfRole", parameters);
            //var x = results.Children().First().Value<String>();

            ////get the list of users names and add it to the list.
            //IEnumerable<String> usersNames = results.Children().Values().Select(y => y.Value<String>());

            //List<User> listUsers = new List<User>();

            ////iterate list of usersNames and make USerObject
            //foreach (string userName in usersNames)
            //{
            //    object[] param = new[] { userName, null };
            //    JToken userClaim = webService.CallWebService("RWE_WSO_SOAP", "getUserClaimValues", param);
            //    User newUser = new User();
            //    newUser.AuthTypeId = new MasterWSO().Id;
            //    newUser.localExpiresAt = DateTime.Today;//for test
            //    newUser.LastLogin = DateTime.Today;
            //    newUser.CurrentLogin = DateTime.Today;
            //    newUser.EmailConfirmed = false;
            //    newUser.PhoneNumberConfirmed = false;
            //    newUser.TwoFactorEnabled = false;
            //    newUser.LockoutEnabled = false;
            //    newUser.AccessFailedCount = 0;
            //    newUser.isActive = true;
            //    foreach (JToken property in userClaim.Children())
            //    {
            //        var a = (property.Children().Single(c => (c as JProperty).Name == "claimUri") as JProperty).Value.ToString();

            //        switch (a)
            //        {
            //            case "email":
            //                var email = (property.Children().Single(c => (c as JProperty).Name == "value") as JProperty).Value.ToString();
            //                newUser.Email = email;
            //                newUser.UserName = email;
            //                break;

            //            case "http://wso2.org/claims/mobile":
            //                var mobile = (property.Children().Single(c => (c as JProperty).Name == "value") as JProperty).Value.ToString();
            //                newUser.MobilPhone = mobile;
            //                break;

            //            case "http://wso2.org/claims/organization":
            //                var organization = (property.Children().Single(c => (c as JProperty).Name == "value") as JProperty).Value.ToString();
            //                newUser.Company = organization;
            //                break;

            //            case "fullname":
            //                var fullname = (property.Children().Single(c => (c as JProperty).Name == "value") as JProperty).Value.ToString();
            //                newUser.DisplayName = fullname;
            //                break; 
            //                //SET ROLES FOR this newly created USER
            //            case "http://wso2.org/claims/role":
            //                var roles  = (property.Children().Single(c => (c as JProperty).Name == "value") as JProperty).Value.ToString().Split(',').Where(r => r.Substring(0,8) == "Auction_").Select(e => e.Remove(0,8));
            //                foreach (string role in roles)
            //                {
            //                        PersonaAppRole approle = db.AppRoles.SingleOrDefault(r => r.Name == role && r.ApplicationId == core.Application.Id);
            //                        if (approle == null)
            //                        {
            //                            db.AppRoles.Add(new PersonaAppRole() { Name = role,Application = core.Application,Priority = 0 });
            //                            db.SaveChanges();
            //                        }
            //                        //User_Role userRole = newUser.Roles.SingleOrDefault(ur => ur.AppRole == approle && ur.User == newUser);
            //                        if (approle != null && !newUser.Users_Roles.Contains(new User_Role { RoleName = approle.Name, User = newUser }))
            //                        {
            //                            newUser.Users_Roles.Add(new User_Role { RoleName = approle.Name, User = newUser });
            //                        }
            //                }
            //                break;
            //        }
            //    }              
            //    listUsers.Add(newUser);
            //}

            ////Now we can cal the resfresh method from persona
            //Modules.Persona.Persona.RefreshUsersFromWSO(listUsers, core);
        }
    }
}
