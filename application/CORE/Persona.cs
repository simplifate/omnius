using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persona
{
    public class Persona : CORE.Module
    {
        private CORE.CORE _CORE;

        public Persona(CORE.CORE core) : base("Persona")
        {
            _CORE = core;
        }

        public bool UserCanExecuteActionRule(int ActionRuleId)
        {
            return _CORE.ActiveUser.Groups.Any(g => g.ActionRights.SingleOrDefault(ar => ar.ActionId == ActionRuleId && ar.Executable) != null);
        }
    }
}
