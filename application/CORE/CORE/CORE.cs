using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE
{
    public class CORE
    {
        private IEnumerable<Entitron.Entity.Module> _enabledModules = null;

        private Entitron.Entitron _Entitron;
        private Mozaic.Mozaic _Mozaic;

        public CORE()
        {
            _Entitron = new Entitron.Entitron(this);
        }

        public Entitron.Entitron Entitron()
        {
            if (isModuleEnabled("Entitron"))
                return _Entitron;
            else
                return null;
        }

        public Mozaic.Mozaic Mozaic()
        {
            if (isModuleEnabled("Mozaic"))
            {
                if (_Mozaic == null)
                    _Mozaic = new Mozaic.Mozaic(this);

                return _Mozaic;
            }
            else
                return null;
        }
        
        private bool isModuleEnabled(string moduleName)
        {
            if (_enabledModules == null)
                _enabledModules = _Entitron.GetStaticTables().Modules.Where(m => m.IsEnabled);

            return _enabledModules.FirstOrDefault(m => m.Name == moduleName) != null;
        }
    }
}
