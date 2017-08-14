using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Entitron.Service
{
    public class RecoveryService : IRecoveryService
    {
        //This method will take a json String and return Application object
        public Application RecoverApplication(string jsonInput)
        {
            JToken json = JToken.Parse(jsonInput);
            return json.ToObject<Application>();
        }
    }
}
