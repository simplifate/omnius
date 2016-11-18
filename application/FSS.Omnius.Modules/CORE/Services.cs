using System;
using System.IO;

namespace FSS.Omnius.Modules.CORE
{
    public class Services
    {
        public static string GetFileVersion(string path)
        {
            //string absolutePath = AppDomain.CurrentDomain.BaseDirectory + path;

            string p = path.TrimStart('~');
            p = p.Replace('/', '\\');
            p = p.TrimStart('\\');
            string absolutePath = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + '\\' + p;

            DateTime lastModified = File.GetLastWriteTime(absolutePath);
            return path.TrimStart('~') + "?version=" + lastModified.Ticks.ToString();
        }
    }
}
