using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using FSS.Omnius.Modules.Tapestry.Actions.Nexus;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Tapestry.Actions.Entitron;

namespace FSS.Omnius.Modules.Tapestry.Actions.Apps.Grid
{
    class DeleteCacheDataAction : Action
    {

        public override int Id
        {
            get
            {
                return 3120117;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[]
                {
                };
            }
        }

        public override string Name
        {
            get
            {
                return "Grid:Delete Cache Data";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { };
            }
        }

        public override int? ReverseActionId
        {
            get
            {
                return null;
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            ReadCachedDataAction.dataCache.Clear();
            CallRestAction.Cache.Clear();
        }
    }
}
