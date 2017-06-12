using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Nexus;
using FSS.Omnius.Modules.Nexus.Service;
using System.IO;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{


    [OtherRepository]
    class WebDavDeleteAction : Action
    {
        public override int Id
        {
            get
            {
                return 199;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "FileId" };
            }
        }

        public override string Name
        {
            get
            {
                return "WebDav Delete";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[0];
            }
        }

        public override int? ReverseActionId
        {
            get
            {
                return null;
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            if (!vars.ContainsKey("FileId"))
                return;

            int fileId = Convert.ToInt32(vars["FileId"]);

            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            var context = DBEntities.appInstance(core.Entitron.Application);
            FileMetadata fmd = context.FileMetadataRecords.Find(fileId);

            IFileSyncService serviceFileSync = new WebDavFileSyncService();
            serviceFileSync.DeleteFile(fmd);
        }
    }
}
