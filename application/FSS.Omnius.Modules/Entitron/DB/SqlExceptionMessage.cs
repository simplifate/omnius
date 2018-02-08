using Logger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.DB
{
    public static class SqlExceptionMessage
    {
        public static Exception TransformAndLogMessage(Exception ex, IDbCommand command)
        {
            ex = new Exception($"SQL exception query: {command.CommandText}", ex);

            // cannot create FK - data references to non-existing row
            if (Regex.Match(ex.InnerException.Message, "^The ALTER TABLE statement conflicted with the FOREIGN KEY constraint .*The conflict occurred in database.*$").Success)
            {
                ex = new Exception("Cannot create foreign key - data references to non-existing row.", ex);
            }
            Log.Error($"Entitron: sql query '{command.CommandText}' could not be executed!");
            return ex;
        }
    }
}
