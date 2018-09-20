using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry2.Actions.App
{
    public partial class Grid : ActionManager
    {
        [Action(3120117, "Grid: Delete Cache Data")]
        public static void DeleteCacheData(COREobject core)
        {
            Grid.dataCache.Clear();
            Nexus.cache.Clear();
        }
    }
}
