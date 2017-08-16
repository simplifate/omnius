using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RethinkDb.Driver.Model;

namespace FSS.Omnius.Modules.Nexus.Service
{
    public class NexusExtDBResult : Result
    {
        public new List<object> GeneratedKeys = new List<object>();
        public new List<string> Warnings = new List<string>();

        public NexusExtDBResult()
        {
            this.Deleted = 0;
            this.DatabasesDropped = 0;
            this.DatabasesCreated = 0;
            this.Ready = 1;
            this.Changes = new JArray();
            this.Skipped = 0;
            this.TablesDropped = 0;
            this.FirstError = "";
            this.Errors = 0;
            this.Unchanged = 0;
            this.Replaced = 0;
            this.Inserted = 0;
            this.TablesCreated = 0;
        }

        public NexusExtDBResult(Result r)
        {
            this.Deleted = r.Deleted;
            this.DatabasesDropped = r.DatabasesDropped;
            this.DatabasesCreated = r.DatabasesCreated;
            this.Ready = r.Ready;
            this.Changes = r.Changes;
            this.Skipped = r.Skipped;
            this.TablesDropped = r.TablesDropped;
            this.FirstError = r.FirstError;
            this.Errors = r.Errors;
            this.Unchanged = r.Unchanged;
            this.Replaced = r.Replaced;
            this.Inserted = r.Inserted;
            this.TablesCreated = r.TablesCreated;

            if (r.Warnings.Length > 0) {
                this.Warnings.AddRange(r.Warnings);
            }

            if (r.GeneratedKeys.Length > 0) {
                foreach(Guid k in r.GeneratedKeys) {
                    this.GeneratedKeys.Add(k);
                }
            }
        }
    }
}
