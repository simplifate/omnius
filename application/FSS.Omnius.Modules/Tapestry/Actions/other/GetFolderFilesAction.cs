using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    /// <summary>
    /// Vrací List<string> obsahující výčet souborů z adresáře v parametru Path, nebo v root adresáři IIS, když je Path prázdné.
    /// WithFullPaths je defaultně true, false vrací jména souborů bez jejich kompletní cest.
    /// </summary>
    [OtherRepository]
    class GetFolderFilesAction : Action
    {
        public override int Id
        {
            get
            {
                return 118999;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "?Path", "?WithFullPaths"};
            }
        }

        public override string Name
        {
            get
            {
                return "Get Folder Files";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Files" };
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
            string path = vars.ContainsKey("Path") ? (string)vars["Path"] : "";
            bool withFullPaths = vars.ContainsKey("WithFullPaths") ? (bool)vars["WithFullPaths"] : true;
            List<string> files;
            if (string.IsNullOrEmpty(path))
                files = new List<string>(Directory.GetFiles(Directory.GetCurrentDirectory()));
            else
                files = new List<string>(Directory.GetFiles(path));
            if (!withFullPaths)
            {
                for(int i = 0; i < files.Count; ++i)
                    files[i] = files[i].Substring(files[i].LastIndexOf('\\') + 1, files[i].Length - files[i].LastIndexOf('\\') - 1);
            }
            
            outputVars["Files"] = files;
        }
    }
}
