using FSS.Omnius.Modules.CORE;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry2.Actions
{
    public partial class Other : ActionManager
    {
        [Action(7253, "Get JObject Property", "JValue")]
        public static JToken GetJObjectProperty(COREobject core, JObject JObject, string PropertyName)
        {
            return JObject[PropertyName];
        }
    }
}
