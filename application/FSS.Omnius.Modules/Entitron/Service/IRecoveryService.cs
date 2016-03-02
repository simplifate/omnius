using FSS.Omnius.Modules.Entitron.Entity.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Service
{
    public interface IRecoveryService
    {
        Application RecoverApplication(string jsonInput);
    }
}
