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
    public partial class ADgroup
    {
        
        public static void RemoveDuplicated<T>(List<T> oldData, List<T> newData) where T : IComparable
        {
            RemoveDuplicated(oldData, newData, (a, b) => a.CompareTo(b) == 0);
        }
        public static void RemoveDuplicated<T>(List<T> oldData, List<T> newData, Func<T, T, bool> compare)
        {
            for (int iOld = 0; iOld < oldData.Count; iOld++)
            {
                T oldItem = oldData[iOld];
                int iNew = newData.IndexOf(item => compare(item, oldItem));
                if (iNew >= 0)
                {
                    oldData.RemoveAt(iOld);
                    newData.RemoveAt(iNew);
                }
            }
        }

        public static void RefreshFromAD(Modules.CORE.CORE core)
        {
            DBEntities context = core.Entitron.GetStaticTables();
            NexusLdapService ldap = new NexusLdapService();

            // get ADgroup_User from AD
            List<ADgroup_User> rightsLdap = new List<ADgroup_User>();
            foreach (ADgroup group in context.ADgroups)
            {
                var ADapps = ldap.GetGroups(group.Name);
                if (ADapps.Count() == 0)
                    continue;

                foreach (JToken member in ADapps["member"])
                {
                    string userName = (string)member;
                    int startI = userName.IndexOf("CN=") + 3;
                    int EndI = userName.IndexOf(',', startI);

                    User user = core.Persona.getUserWithoutGroups(userName);

                    rightsLdap.Add(new ADgroup_User { ADgroup = group, User = user });
                }
            }

            // update
            List<ADgroup_User> rightsDB = context.ADgroup_Users.ToList();
            RemoveDuplicated(rightsDB, rightsLdap, (a, b) => a.ADgroupId == b.ADgroupId && a.UserId == b.UserId);
            context.ADgroup_Users.RemoveRange(rightsDB);
            context.ADgroup_Users.AddRange(rightsLdap);
            context.SaveChanges();
        }

        public override string ToString()
        {
            return Application != null ? Application.DisplayName : Name;
        }
    }
}
