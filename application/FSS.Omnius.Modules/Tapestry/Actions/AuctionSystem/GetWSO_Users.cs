using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Nexus.Service;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.Entitron.Entity.Persona;

namespace FSS.Omnius.Modules.Tapestry.Actions.AuctionSystem
{
    public class GetWSO_Users : Action
    {
        public override int Id
        {
            get
            {
                return 5000;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[0];
            }
        }

        public override string Name
        {
            get
            {
                return "GetWsoUsers";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[0];
            }
        }

        public override int? ReverseActionId
        {
            get
            {
                return null;
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
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
                newUser.isLocalUser = false;
                newUser.localExpiresAt = DateTime.Today;//for test
                newUser.LastLogin = DateTime.Today;
                newUser.CurrentLogin = DateTime.Today;
                newUser.EmailConfirmed = false;
                newUser.PhoneNumberConfirmed = false;
                newUser.TwoFactorEnabled = false;
                newUser.LockoutEnabled = false;
                newUser.AccessFailedCount = 0;
                foreach (JToken property in userClaim.Children())
                {
                    var xx = property.Children();
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
                            var roles  = (property.Children().Single(c => (c as JProperty).Name == "value") as JProperty).Value.ToString().Split(',').Where(r => r.Substring(0,8) == "Auction_").Select(e => e.Remove(0,8));
                            foreach (string role in roles) {
                                var db = core.Entitron.GetStaticTables();
                                    PersonaAppRole approle = db.Roles.SingleOrDefault(r => r.Name == role && r.ApplicationId == core.Entitron.AppId);
                                    if (approle == null) {
                                        db.Roles.Add(new PersonaAppRole() { Name = role,Application = core.Entitron.Application,Priority = 0 });
                                        db.SaveChanges();
                                    }
                                    User_Role userRole = newUser.Roles.SingleOrDefault(ur => ur.AppRole == approle && ur.User == newUser);
                                    if (approle != null && !newUser.Roles.Contains(new User_Role { AppRole = approle, User = newUser }))
                                    {
                                        newUser.Roles.Add(new User_Role { AppRole = approle, User = newUser });

                                    }
                            }
                            break;

                    }

                }
                

                listUsers.Add(newUser);

            }

            //Now we can cal the resfresh method from persona

            Persona.Persona.RefreshUsersFromWSO(listUsers,core);

        }




    }
}
