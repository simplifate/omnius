using System.Collections.Generic;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.Entitron.Entity.Master;

namespace FSS.Omnius.Modules.CORE
{
    public class CORE : IModule
    {
        private Dictionary<string, IModule> _modules = new Dictionary<string, IModule>();
        
        public User User { get; set; }
        public Dictionary<string, object> CrossBlockRegistry { get; set; }

        public CORE()
        {            
            _modules["Entitron"] = new Entitron.Entitron(this);
        }
        
        public Entitron.Entitron Entitron
        {
            get
            {
                //if (!isModuleEnabled("Entitron"))
                //    throw new ModuleNotFoundOrEnabledException("Entitron");

                if (!_modules.ContainsKey("Entitron"))
                    _modules["Entitron"] = new Entitron.Entitron(this);

                return (Entitron.Entitron)_modules["Entitron"];
            }
        }
        public Mozaic.Mozaic Mozaic
        {
            get
            {
                //if (!isModuleEnabled("Mozaic"))
                //    throw new ModuleNotFoundOrEnabledException("Mozaic");

                if (!_modules.ContainsKey("Mozaic"))
                    _modules["Mozaic"] = new Mozaic.Mozaic(this);

                return (Mozaic.Mozaic)_modules["Mozaic"];
            }
        }
        public Tapestry.Tapestry Tapestry
        {
            get
            {
                //if (!isModuleEnabled("Tapestry"))
                //    throw new ModuleNotFoundOrEnabledException("Tapestry");

                if (!_modules.ContainsKey("Tapestry"))
                    _modules["Tapestry"] = new Tapestry.Tapestry(this);

                return (Tapestry.Tapestry)_modules["Tapestry"];
            }
        }
        public Persona.Persona Persona
        {
            get
            {
                //if (!isModuleEnabled("Persona"))
                //    throw new ModuleNotFoundOrEnabledException("Persona");

                if (!_modules.ContainsKey("Persona"))
                    _modules["Persona"] = new Persona.Persona(this);

                return (Persona.Persona)_modules["Persona"];
            }
        }

        public Application Application => Modules.Entitron.Entitron.i.Application;

        //private bool isModuleEnabled(string moduleName)
        //{
        //    if (_enabledModules == null)
        //        _enabledModules = (_modules["Entitron"] as Entitron.Entitron).GetStaticTables().Modules.Where(m => m.IsEnabled);

        //    return _enabledModules.Any(m => m.Name == moduleName);
        //}
    }
}
