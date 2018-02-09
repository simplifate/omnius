using Logger;
using System;
using System.Linq;
using System.Data;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace FSS.Omnius.Modules.Entitron.DB
{
    public static class SqlExceptionMessage
    {
        public static Exception TransformAndLogMessage(Exception ex, IDbCommand command)
        {
            /// Message
            string message = ex.Message;
            // cannot create FK - data references to non-existing row
            if (Regex.Match(message, "^The ALTER TABLE statement conflicted with the FOREIGN KEY constraint .*The conflict occurred in database.*$").Success)
            {
                message = "Cannot create foreign key - data references to non-existing row.";
            }

            /// Params
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            foreach (IDataParameter param in command.Parameters)
            {
                paramDict.Add(param.ParameterName, param.Value);
            }
            
            /// Exception & Log
            ex = new EntitronException($"SQL exception '{message}' query: {command.CommandText} --- params: {string.Join(", ", paramDict.Select(pair => $"{pair.Key}: {pair.Value.ToString()}"))}", command.CommandText, command.Parameters, ex);

            Log.Error($"Entitron: sql query '{command.CommandText}' could not be executed!");

            return ex;
        }
    }

    public class EntitronException : Exception
    {
        public EntitronException(string message, string query, IDataParameterCollection parameters, Exception innerException) : base(message, innerException)
        {
            Query = query;
            Params = parameters;
        }

        public string Query { get; set; }
        public IDataParameterCollection Params { get; set; }
    }
}
