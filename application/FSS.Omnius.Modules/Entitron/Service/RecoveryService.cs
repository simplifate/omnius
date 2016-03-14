using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using Newtonsoft.Json;

namespace FSS.Omnius.Modules.Entitron.Service
{
    public class RecoveryService : IRecoveryService
    {
        //This method will take a json String and return Application object
        public Application RecoverApplication(string jsonInput)
        {
            return JsonConvert.DeserializeObject<Application>(jsonInput);
        }
    }
}
