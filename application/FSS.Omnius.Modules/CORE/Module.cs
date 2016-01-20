using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Specialized;
using FSS.Omnius.Modules.Entitron.Entity.Persona;

namespace FSS.Omnius.Modules.Entitron.Entity.CORE
{
    [NotMapped]
    public abstract class RunableModule : Module
    {
        public abstract void run(User user, string url, NameValueCollection post);
        public abstract string GetHtmlOutput();
        public abstract string GetJsonOutput();
        public abstract string GetMailOutput();
    }
}
