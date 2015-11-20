using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Entitron.Entity.CORE;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSS.Omnius.Persona
{
    [NotMapped]
    public class Persona : Module
    {
        private CORE.CORE _CORE;

        public Persona(CORE.CORE core)
        {
            Name = "Persona";
            _CORE = core;
        }

        public bool UserCanExecuteActionRule(int ActionRuleId)
        {
            return _CORE.ActiveUser.Groups.Any(g => g.ActionRights.SingleOrDefault(ar => ar.ActionId == ActionRuleId && ar.Executable) != null);
        }
    }
}
