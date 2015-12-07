using System;
using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.Entitron.Entity.CORE;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Specialized;

namespace FSS.Omnius.Modules.CORE
{
    [NotMapped]
    public class CORE : Module
    {
        private IQueryable<Module> _enabledModules = null;

        private Dictionary<string, Module> _modules = new Dictionary<string, Module>();

        private RunableModule _activeModule;
        public User ActiveUser { get; set; }
        public CORE()
        {
            Name = "CORE";

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
        public void masterRun(User user, string moduleName, string url, NameValueCollection fc)
        {
            ActiveUser = user;

            _activeModule = GetRunableModule(moduleName);
            if (_activeModule == null)
                throw new ModuleNotFoundOrEnabledException(moduleName);

            _activeModule.run(url, fc);
        }
        
        public RunableModule GetRunableModule(string moduleName)
        {
            // not enabled
            if (!isModuleEnabled(moduleName))
                throw new ModuleNotFoundOrEnabledException(moduleName);

            // create new instance
            if (!_modules.ContainsKey(moduleName))
            {
                switch (moduleName)
                {
                    case "Tapestry":
                        _modules["Tapestry"] = new Tapestry.Tapestry(this);
                        break;
                    default:
                        throw new ModuleNotFoundOrEnabledException(moduleName);
                }
            }

            // return
            return (RunableModule)_modules[moduleName];
        }
        public Entitron.Entitron Entitron
        {
            get
            {
                if (!isModuleEnabled("Entitron"))
                    throw new ModuleNotFoundOrEnabledException("Entitron");

                if (!_modules.ContainsKey("Entitron"))
                    _modules["Entitron"] = new Entitron.Entitron(this);

                return (Entitron.Entitron)_modules["Entitron"];
            }
        }
        public Mozaic.Mozaic Mozaic
        {
            get
            {
                if (!isModuleEnabled("Mozaic"))
                    throw new ModuleNotFoundOrEnabledException("Mozaic");

                if (!_modules.ContainsKey("Mozaic"))
                    _modules["Mozaic"] = new Mozaic.Mozaic(this);

                return (Mozaic.Mozaic)_modules["Mozaic"];
            }
        }
        public Tapestry.Tapestry Tapestry
        {
            get
            {
                if (!isModuleEnabled("Tapestry"))
                    throw new ModuleNotFoundOrEnabledException("Tapestry");

                if (!_modules.ContainsKey("Tapestry"))
                    _modules["Tapestry"] = new Tapestry.Tapestry(this);

                return (Tapestry.Tapestry)_modules["Tapestry"];
            }
        }
        public Persona.Persona Persona
        {
            get
            {
                if (!isModuleEnabled("Persona"))
                    throw new ModuleNotFoundOrEnabledException("Persona");

                if (!_modules.ContainsKey("Persona"))
                    _modules["Persona"] = new Persona.Persona(this);

                return (Persona.Persona)_modules["Persona"];
            }
        }

        private bool isModuleEnabled(string moduleName)
        {
            if (_enabledModules == null)
                _enabledModules = (_modules["Entitron"] as Entitron.Entitron).GetStaticTables().Modules.Where(m => m.IsEnabled);

            return _enabledModules.Any(m => m.Name == moduleName);
        }
    }
}
