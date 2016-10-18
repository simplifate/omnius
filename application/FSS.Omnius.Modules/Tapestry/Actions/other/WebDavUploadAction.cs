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
    class WebDavUploadAction : Action
    {
        public override int Id
        {
            get
            {
                return 195;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[0];
            }
        }

        public override string Name
        {
            get
            {
                return "WebDav Upload";
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
            var files = HttpContext.Current.Request.Files;
            if (files == null)
                return;

            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            string appName = core.Entitron.AppName;

            using (var entities = DBEntities.instance)
            {
                foreach (string fileName in files)
                {
                    HttpPostedFile file = HttpContext.Current.Request.Files[fileName];

                    if (file.ContentLength == 0)
                        continue;

                    FileMetadata fmd = new FileMetadata();
                    fmd.AppFolderName = core.Entitron.AppName;
                    fmd.CachedCopy = new FileSyncCache();

                    byte[] streamBytes = new byte[file.ContentLength];
                    file.InputStream.Read(streamBytes, 0, file.ContentLength);
                    fmd.CachedCopy.Blob = streamBytes;

                    fmd.Filename = Path.GetFileName(file.FileName);
                    fmd.TimeChanged = DateTime.Now;
                    fmd.TimeCreated = DateTime.Now;
                    fmd.Version = 0;

#warning webdavservers parametrizovat
                    fmd.WebDavServer = entities.WebDavServers.First();

                    entities.FileMetadataRecords.Add(fmd);
                    entities.SaveChanges(); //ukládat po jednom souboru

                    IFileSyncService service = new WebDavFileSyncService();
                    service.UploadFile(fmd);

                    //TODO?: předat dalším akcim identifikátory
                    //outputVars.Add("input_" + fileName, fmd.Id);
                }
            }
        }
    }
}
