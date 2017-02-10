using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.CORE
{
    public interface ITranslator
    {
        string _(string sourceString);
        string _(string sourceString, params object[] args);
    }
}
}
