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
        public void UpdateGroupsFromAd(IEnumerable<string> groupNames, DBEntities e)
        {
            var groups = Groups.Where(g => g.IsFromAD == true).ToList();

            // added groups
            foreach (string groupName in groupNames)
            {
                // if not in original, but in new
                Group newGroup = groups.SingleOrDefault(g => g.Name == groupName);
                if (newGroup == null)
                {
                    newGroup = e.Groups.SingleOrDefault(g => g.Name == groupName && g.IsFromAD == true);
                    // skupina není v DB
                    if (newGroup == null)
                        newGroup = new Group { Name = groupName, IsFromAD = true };
                    Groups.Add(newGroup);
                }
                else
                    groups.Remove(newGroup);
            }

            // removed groups
            foreach(Group group in groups)
            {
                Groups.Remove(group);
            }
        }

        public bool isInGroup(string groupName)
        {
            return Groups.Any(g => g.Name == groupName);
        }

        public bool canUseAction(int actionId, DBEntities e)
        {
            var right = e.ActionRuleRights.FirstOrDefault(ar => ar.ActionRuleId == actionId && Groups.Contains(ar.Group));
            return right.Executable;
        }
    }
}
