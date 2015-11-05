using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE
{
    public class CORE : Module
    {
        private IQueryable<Entitron.Entity.Module> _enabledModules = null;

        private Dictionary<string, Module> _modules = new Dictionary<string, Module>();

        private RunableModule _activeModule;
        public Entitron.Entity.User ActiveUser { get; set; }
        public CORE() : base("CORE")
        {
            _modules["CORE"] = this;
            _modules["Entitron"] = new Entitron.Entitron(this);
        }

        public string html
        {
            get { return _activeModule.GetHtmlOutput(); }
        }
        public string json
        {
            get { return _activeModule.GetJsonOutput(); }
        }
        public string mail
        {
            get { return _activeModule.GetMailOutput(); }
        }
        public void masterRun(Entitron.Entity.User user, string moduleName, string url)
        {
            ActiveUser = user;

            _activeModule = GetRunableModule(moduleName);
            if (_activeModule == null)
                throw new ModuleNotFoundOrEnabledException(moduleName);

            _activeModule.run(url);
        }

        public Module GetModule(string moduleName)
        {
            if (!isModuleEnabled(moduleName))
                throw new ModuleNotFoundOrEnabledException(moduleName);

            if (!_modules.ContainsKey(moduleName))
                _modules[moduleName] = GetNewModuleInstance(moduleName);

            return _modules[moduleName];
        }
        public RunableModule GetRunableModule(string moduleName)
        {
            Module module = GetModule(moduleName);
            if (module is RunableModule)
                return (RunableModule)module;

            throw new ModuleNotFoundOrEnabledException(moduleName);
        }

        private bool isModuleEnabled(string moduleName)
        {
            if (_enabledModules == null)
                _enabledModules = (_modules["Entitron"] as Entitron.Entitron).GetStaticTables().Modules.Where(m => m.IsEnabled);

            return _enabledModules.FirstOrDefault(m => m.Name == moduleName) != null;
        }
        private Module GetNewModuleInstance(string moduleName)
        {
            switch (moduleName)
            {
                case "CORE":
                    return this;
                case "Entitron":
                    return new Entitron.Entitron(this);
                case "Mozaic":
                    return new Mozaic.Mozaic(this);
                case "Tapestry":
                    return new Tapestry.Tapestry(this);
                case "Persona":
                    return new Persona.Persona(this);
                default:
                    throw new ModuleNotFoundOrEnabledException(moduleName);
            }
        }
    }
}
