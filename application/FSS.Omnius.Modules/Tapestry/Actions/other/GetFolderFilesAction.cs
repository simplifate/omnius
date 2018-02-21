using System.Collections.Generic;
using System.IO;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    /// <summary>
    /// Vrací List<string> obsahující výčet souborů z adresáře v parametru Path, nebo v root adresáři IIS, když je Path prázdné.
    /// WithFullPaths je defaultně true, false vrací jména souborů bez jejich kompletní cest.
    /// </summary>
    [OtherRepository]
    class GetFolderFilesAction : Action
    {
        public override int Id => 118999;

        public override string[] InputVar => new string[] { "?Path", "?WithFullPaths"};

        public override string Name => "Get Folder Files";

        public override string[] OutputVar => new string[] { "Files" };

        public override int? ReverseActionId => null;

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
