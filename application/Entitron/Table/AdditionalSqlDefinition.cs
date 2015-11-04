using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitron
{
    class AdditionalSqlDefinition : DBColumn
    {
        public string definition;

        public override string getSqlDefinition()
        {
            return definition;
        }
    }
}
