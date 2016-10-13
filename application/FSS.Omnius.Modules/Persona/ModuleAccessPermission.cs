using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Entity.Persona
{
    public partial class ModuleAccessPermission
    {
        public bool hasAccess(string moduleName)
        {
            switch(moduleName)
            {
                case "CORE":
                    return Core;
                case "Master":
                    return Master;
                case "Tapestry":
                    return Tapestry;
                case "Entitron":
                    return Entitron;
                case "Mozaic":
                    return Mozaic;
                case "Persona":
                    return Persona;
                case "Nexus":
                    return Nexus;
                case "Setry":
                    return Sentry;
                case "Hermes":
                    return Hermes;
                case "Athena":
                    return Athena;
                case "Watchtower":
                    return Watchtower;
                case "Cortex":
                    return Cortex;
                case "Compass":
#warning TODO: Compass
                    return true;
                default:
                    throw new ArgumentException($"Unknown value: {moduleName}");
            }
        }
    }
}
