using System;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Tapestry2.Actions.App
{
    public partial class Grid : ActionManager
    {
        [Action(300011, "Join JArray", "Result")]
        public static JArray JArrayJoin(COREobject core, JArray InputArray1 = null, JArray InputArray2 = null)
        {
            if (InputArray1 == null)
                InputArray1 = new JArray();
            if (InputArray2 == null)
                InputArray2 = new JArray();
            
            return new JArray(InputArray1.Union(InputArray2));
        }
    }
}
