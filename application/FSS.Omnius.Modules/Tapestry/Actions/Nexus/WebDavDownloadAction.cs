using System;
using System.Collections.Generic;
using System.Web;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Nexus;
using FSS.Omnius.Modules.Nexus.Service;

namespace FSS.Omnius.Modules.Tapestry.Actions.Nexus
{
    [NexusRepository]
    class WebDavDownloadAction : Action
    {
        public override int Id => 196;

        public override string[] InputVar => new string[] { "FileId" };

        public override string Name => "WebDav Download";
        
        public override string[] OutputVar => new string[0];

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            if (!vars.ContainsKey(InputVar[0]))
                return;

            int fileId = Convert.ToInt32(vars[InputVar[0]]);

            COREobject core = COREobject.i;
            DBEntities context = core.Context;
            FileMetadata fmd = context.FileMetadataRecords.Find(fileId);

            IFileSyncService serviceFileSync = new WebDavFileSyncService();
            serviceFileSync.DownloadFile(fmd);

            HttpContext httpContext = HttpContext.Current;
            HttpResponse response = httpContext.Response;

            response.Clear();
            response.StatusCode = 202;
            response.ContentType = "application/octet-stream";
            response.AddHeader("content-disposition", $"attachment; filename={fmd.Filename}");
            response.BinaryWrite(fmd.CachedCopy.Blob);
            response.Flush();
            response.Close();
            response.End();
        }
    }
}
