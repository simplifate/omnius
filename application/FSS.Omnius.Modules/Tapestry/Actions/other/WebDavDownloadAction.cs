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
    class WebDavDownloadAction : Action
    {
        public override int Id
        {
            get
            {
                return 196;
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
                return "WebDav Download";
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
            if (!vars.ContainsKey(InputVar[0]))
                return;

            int fileId = Convert.ToInt32(vars[InputVar[0]]);

            using (var entities = DBEntities.instance)
            {
                FileMetadata fmd = entities.FileMetadataRecords.Find(fileId);

                IFileSyncService serviceFileSync = new WebDavFileSyncService();
                serviceFileSync.DownloadFile(fmd);

                HttpContext context = HttpContext.Current;
                HttpResponse response = context.Response;

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
}
