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
using FSS.Omnius.Modules.Compass;
using System.IO;


namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class WebDavVarUploadAction : Action
    {
        public override int Id
        {
            get
            {
                return 190;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "FileContent", "FileName", "WebDavServerName" };
            }
        }

        public override string Name
        {
            get
            {
                return "WebDav Var Upload";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "UploadMetadataId" };
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
            COREobject core = COREobject.i;
            DBEntities context = core.Context;

            byte[] streamBytes = (byte[])vars["FileContent"];
            string fileName = vars.ContainsKey("FileName") ? (string)vars["FileName"] : "";
            string webDavServerName = vars.ContainsKey("WebDavServerName") ? (string)vars["WebDavServerName"] : "";

            if(string.IsNullOrEmpty(fileName)) {
                throw new Exception($"{Name}: File name is required.");
            }

            
            FileMetadata fmd = new FileMetadata();
            fmd.AppFolderName = core.Application.Name;
            fmd.CachedCopy = new FileSyncCache();

            fmd.CachedCopy.Blob = streamBytes;

            fmd.Filename = fileName;
            fmd.TimeChanged = DateTime.Now;
            fmd.TimeCreated = DateTime.Now;
            fmd.Version = 0;
            fmd.ModelEntityName = "";
            fmd.Tag = "";

            if (!string.IsNullOrWhiteSpace(webDavServerName)) {
                fmd.WebDavServer = context.WebDavServers.Single(a => a.Name == webDavServerName);
            }
            else {
                fmd.WebDavServer = context.WebDavServers.First();
            }

            context.FileMetadataRecords.Add(fmd);
            context.SaveChanges(); //ukládat po jednom souboru

            IFileSyncService service = new WebDavFileSyncService();
            service.UploadFile(fmd);

            outputVars.Add(this.OutputVar[0], fmd.Id);
        }
    }
}
