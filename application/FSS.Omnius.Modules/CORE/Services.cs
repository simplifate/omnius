using System;
using System.IO;

namespace FSS.Omnius.Modules.CORE
{
    public class Services
    {
        public static string GetFileVersion(string path)
        {
            string absolutePath = AppDomain.CurrentDomain.BaseDirectory + path;

            DateTime lastModified = File.GetLastWriteTime(absolutePath);
            return path + "?version=" + lastModified.Ticks.ToString();
        }
    }
}
