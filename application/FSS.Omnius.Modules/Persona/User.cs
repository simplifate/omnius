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
        public void UpdateAppRightFromAd(IEnumerable<string> newADgroupNames, DBEntities context)
        {
            // AD groups
            List<ADgroup_User> newADgroups = context.ADgroups.Where(ad => newADgroupNames.Contains(ad.Name)).Select(ad => new ADgroup_User { ADgroup = ad, User = this }).ToList();

            // DB groups
            List<ADgroup_User> oldADgroups = context.ADgroup_Users.Where(adu => adu.UserId == Id).ToList();

            // update
            ADgroup.RemoveDuplicated(oldADgroups, newADgroups, (a, b) => a.ADgroupId == b.ADgroupId && a.UserId == b.UserId);
            context.ADgroup_Users.RemoveRange(oldADgroups);
            context.ADgroup_Users.AddRange(newADgroups);
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
