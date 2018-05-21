using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Entity
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ImportExportPropertyAttribute : Attribute
    {
        public ImportExportPropertyAttribute()
        {
            IsKey = false;
        }

        public bool IsKey { get; set; }
    }
}
