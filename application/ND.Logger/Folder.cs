using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Logger
{
    class Folder
    {
        private string pathToLogDir = "";
        private string rootDir = GeneralConfig.Config.RootDir;
        public string LogPath
        {
            get
            {
                return pathToLogDir;
            }
        }

        public Folder(string logDirectory)
        {
            pathToLogDir = Path.Combine(rootDir, logDirectory);
            if (!Directory.Exists(pathToLogDir))
                Directory.CreateDirectory(pathToLogDir);

        }


    }
}
