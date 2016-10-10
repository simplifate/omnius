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
        public static string groupADServer = "rwe-cz";
        
        public static void RemoveDuplicated<T>(List<T> oldData, List<T> newData) where T : IComparable
        {
            RemoveDuplicated(oldData, newData, (a, b) => a.CompareTo(b) == 0);
        }
        public static void RemoveDuplicated<T>(List<T> oldData, List<T> newData, Func<T, T, bool> compare)
        {
            for (int iOld = 0; iOld < oldData.Count; iOld++)
            {
                T oldItem = oldData[iOld];
                int iNew = newData.IndexOf(item => compare(oldItem, item));
                if (iNew >= 0)
                {
                    oldData.RemoveAt(iOld);
                    newData.RemoveAt(iNew);
                }
            }
        }

        public static void RefreshFromAD(Modules.CORE.CORE core)
        {
            // refresh all users
            DBEntities context = core.Entitron.GetStaticTables();
            //foreach(User user in context.Users.ToList())
            //{
            //    core.Persona.RefreshUser(user);
            //}

            NexusLdapService ldap = new NexusLdapService();
            ldap.UseServer(groupADServer);

            // get ADgroup_User from AD
            List<ADgroup_User> rightsLdap = new List<ADgroup_User>();
            foreach (ADgroup group in context.ADgroups.ToList())
            {
                var ADapps = ldap.GetGroups(group.Name);
                if (ADapps.Count() == 0)
                    continue;
                
                foreach (JToken ADapp in ADapps) // should be only 1
                {
                    foreach (JToken member in ADapp["member"])
                    {
                        // save user with groups
                        core.Persona.GetUser(identify: (string)member);
                    }
                }
            }
            
            context.SaveChanges();
        }

        public override string ToString()
        {
            return Application != null ? Application.DisplayName : Name;
        }
    }
}
