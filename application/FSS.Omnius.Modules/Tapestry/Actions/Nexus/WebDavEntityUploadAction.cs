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
    class WebDavEntityUploadAction : Action
    {
        public override int Id
        {
            get
            {
                return 197;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "ModelEntityName", "InputNames", "WebDavServerName", "DescriptionInput", "?NewId" };    //TODO: české tagy?
            }
        }

        public override string Name
        {
            get
            {
                return "WebDav entity upload";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] {  };
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
            string modelEntityName;
            if(!vars.ContainsKey(InputVar[0]))
            {
                if (!vars.ContainsKey("__TableName__"))
                    return; //TODO?: exceptiona ? obecné ošetřování vstupních parametrů - jinak než exception ?

                modelEntityName = vars["__TableName__"].ToString();
            }
            else
            {
                modelEntityName = vars[InputVar[0]].ToString();
            }

            var files = HttpContext.Current.Request.Files;
            if (files == null)
                return;

            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            string appName = core.Entitron.AppName;

            string[] inputNames;
            if (vars.ContainsKey(InputVar[1]))
            {
                inputNames = vars[InputVar[1]].ToString().Split(',');
            }
            else
            {
                inputNames = files.Cast<string>().ToArray();
            }

            string[] descriptionInputs = null;
            if(vars.ContainsKey(InputVar[3]))
            {
                descriptionInputs = vars[InputVar[3]].ToString().Split(',');
            }

            int newId;
            if (vars.ContainsKey("NewId"))
                newId = Convert.ToInt32(vars["NewId"]);
            else
                newId = Convert.ToInt32(vars["__ModelId__"]);
            
            var entities = DBEntities.appInstance(core.Entitron.Application);
            foreach (string fileName in files)
            {
                HttpPostedFile file = HttpContext.Current.Request.Files[fileName];

                if (file.ContentLength == 0 || !(inputNames == null || inputNames.Contains(fileName)))  //prázdný soubor, nebo bez filtru na inputy (mohou být dvě akce pro jinou entitu), nebo splňuje filtr na input name
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

                string name = vars.ContainsKey(InputVar[2]) ? vars[InputVar[2]].ToString() : string.Empty;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    fmd.WebDavServer = entities.WebDavServers.Single(a => a.Name == name);
                }
                else
                    fmd.WebDavServer = entities.WebDavServers.First();

                if (descriptionInputs != null && descriptionInputs.Length > 0)
                {
                    int inputIndex = Array.IndexOf(inputNames, fileName);
                    string descInp = descriptionInputs[inputIndex];

                    fmd.Description = vars[descInp].ToString();
                }
                else if (vars.ContainsKey(fileName + "_description"))
                {
                    fmd.Description = vars[fileName + "_description"].ToString();
                }

                fmd.ModelEntityId = newId;
                fmd.ModelEntityName = modelEntityName;
                fmd.Tag = fileName; //TODO: český čitelný název (systémová tabulka s klíčema a hodnotama?)

                entities.FileMetadataRecords.Add(fmd);
                entities.SaveChanges(); //ukládat po jednom souboru

                IFileSyncService service = new WebDavFileSyncService();
                service.UploadFile(fmd);
            }
        }
    }
}
