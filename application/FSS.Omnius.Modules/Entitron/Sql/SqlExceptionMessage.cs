using Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    public static class SqlExceptionMessage
    {
        public static void TransformAndLogMessage(Exception ex, SqlQuery query)
        {
            ex = new Exception($"SQL exception [{query}] query: {query.sqlString}", ex);

            // cannot create FK - data references to non-existing row
            if (Regex.Match(ex.InnerException.Message, "^The ALTER TABLE statement conflicted with the FOREIGN KEY constraint .*The conflict occurred in database.*$").Success)
            {
                throw new Exception("Cannot create foreign key - data references to non-existing row.", ex);
            }
            Log.Error(string.Format("Entitron: sql query '{0}' could not be executed!", query.ToString()));
            throw ex;
        }
    }
}
