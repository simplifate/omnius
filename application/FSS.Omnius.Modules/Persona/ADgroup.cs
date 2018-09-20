using FSS.Omnius.Modules.CORE;
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
                int iNew = newData.IndexOf(item => compare(oldItem, item));
                if (iNew >= 0)
                {
                    oldData.RemoveAt(iOld);
                    newData.RemoveAt(iNew);
                }
            }
        }

        public override string ToString()
        {
            return Application != null ? Application.DisplayName : Name;
        }
    }
}
