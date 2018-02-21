using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Nexus;
using FSS.Omnius.Modules.Nexus.Service;

namespace FSS.Omnius.Modules.Tapestry.Actions.Nexus
{
    [NexusRepository]
    class WebDavDeleteAction : Action
    {
        public override int Id => 199;

        public override string[] InputVar => new string[] { "FileId" };

        public override string Name => "WebDav Delete";

        public override string[] OutputVar => new string[0];

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            if (!vars.ContainsKey("FileId"))
                return;

            int fileId = Convert.ToInt32(vars["FileId"]);

            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            var context = DBEntities.appInstance(core.Application);
            FileMetadata fmd = context.FileMetadataRecords.Find(fileId);

            IFileSyncService serviceFileSync = new WebDavFileSyncService();
            serviceFileSync.DeleteFile(fmd);
        }
    }
}
