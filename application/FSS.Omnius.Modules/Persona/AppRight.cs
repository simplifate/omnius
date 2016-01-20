using FSS.Omnius.Modules.Entitron.Entity.Master;
using FSS.Omnius.Modules.Nexus.Service;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Entity.Persona
{
    public partial class AppRight
    {
        public static void RefreshFromAD(DBEntities context)
        {
            NexusLdapService ldap = new NexusLdapService();

            // get appRights from AD
            List<AppRight> rights = new List<AppRight>();
            foreach (Application app in context.Applications)
            {
                var ADapps = ldap.GetGroups(app.Name);
                if (ADapps.Count() == 0)
                    continue;

                foreach (JToken member in ADapps["member"])
                {
                    string userName = (string)member;
                    int startI = userName.IndexOf("CN=") + 3;
                    int EndI = userName.IndexOf(',', startI);

                    rights.Add(new AppRight
                    {
                        Application = app,
                        User = new User { DisplayName = userName }
                    });
                }
            }

            // refresh
            Refresh(rights, context);
        }
        private static void Refresh(List<AppRight> mapping, DBEntities context)
        {
            List<AppRight> rightsDB = context.ApplicationRights.ToList();

            // in NEW
            foreach (AppRight newRight in mapping)
            {
                AppRight dbRight = rightsDB.SingleOrDefault(r => r.User.DisplayName == newRight.User.DisplayName && r.ApplicationId == newRight.ApplicationId);
                // in DB
                if (dbRight != null)
                {
                    dbRight.hasAccess = true;
                    rightsDB.Remove(dbRight);
                }
                // not DB - create
                else
                {
                    dbRight = new AppRight
                    {
                        Application = newRight.Application,
                        User = context.Users.Single(u => u.DisplayName == newRight.User.DisplayName),
                        hasAccess = true
                    };
                    context.ApplicationRights.Add(dbRight);
                }
            }

            // not NEW but DB
            foreach (AppRight rightDB in rightsDB)
            {
                // Uncomment on production
                //rightDB.hasAccess = false;
            }
        }
    }
}
