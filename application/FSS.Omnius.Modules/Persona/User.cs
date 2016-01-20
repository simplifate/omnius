using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;

namespace FSS.Omnius.Modules.Entitron.Entity.Persona
{
    public partial class User
    {
        /// <summary>
        /// without saving...
        /// </summary>
        /// <param name="groupNames"></param>
        public void UpdateAppRightFromAd(IEnumerable<string> newAppNames, DBEntities context)
        {
            List<AppRight> rightsDB = ApplicationRights.ToList();

            // in NEW
            foreach (string newAppName in newAppNames)
            {
                AppRight dbRight = rightsDB.SingleOrDefault(r => r.UserId == Id && r.Application.Name == newAppName);
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
                        Application = context.Applications.Single(a => a.Name == newAppName),
                        User = this,
                        hasAccess = true
                    };
                    context.ApplicationRights.Add(dbRight);
                }
            }

            // not NEW but DB
            foreach (AppRight rightDB in rightsDB)
            {
                // Uncomment on Production
                // rightDB.hasAccess = false;
            }
        }

        public bool isAdmin()
        {
            // TODO
            return true;
            //return Groups.Any(g => g.Name == "Admin");
        }
        public bool HasRole(string roleName, DBEntities context)
        {
            PersonaAppRole role = context.PersonaAppRoles.Single(ar => ar.RoleName == roleName);
            return role.MembersList.Split(',').Contains(Id.ToString());
        }

        public bool canUseAction(int actionId, DBEntities context)
        {
            // TODO
            return true;
        }
    }
}
