using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;


namespace Logger
{
    class LogFile
    {
        public string LogPath
        {
            get;
            private set;
        }
        public double SizeMB
        {
            get
            {
                return (double)Log.BytesToMegabytes(this.Size);
            }
        }
        public long Size
        {
            get;
            private set;
        }
        public FileInfo Info
        {
            get;
            private set;
        }
        public LogElement Config
        {
            get;
            private set;
        }
        public LogFile(string logPath, LogElement config)
        {
            LogPath = logPath;
            Config = config;
            Info = new FileInfo(LogPath);
            Size = (Info.Exists) ? Info.Length : 0;            
        }
        public bool TooBig()
        {
            bool tooBig = false;
            tooBig = (SizeMB >= (double)Config.MaxLogSize) ? true : false;

            return tooBig;
        }
        // Archive this log file to zip            
        public void Archive()
        {            
            int terseNumber = LastTerseNumber()+1;
            string logFileNameWithoutExtension = Path.GetFileNameWithoutExtension(Info.FullName);            
            string zipName = string.Format("{0}.{1}.terse.zip", logFileNameWithoutExtension, terseNumber);
            string zipFilePath = Path.Combine(Info.Directory.FullName, zipName);
            
            using (var memoryStream = new MemoryStream())
            {
                // Creates a new, blank zip file (in memory)
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    // Add log.txt to archive
                    archive.CreateEntryFromFile(Info.FullName, Info.Name, CompressionLevel.Fastest);
                }
                using (var fileStream = new FileStream(zipFilePath, FileMode.Create))
                {
                    // Creates "log.#.terse.zip" file in file system
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    memoryStream.CopyTo(fileStream);
                }
            }
        }
        
        // Remove this log file from file system
        public void Remove()
        {
            if (File.Exists(Info.FullName))
            {
                File.Delete(Info.FullName);
            }
        }

        // Get last number in 
        private int LastTerseNumber()
        {
            int number = 0;
            string dirPath = GeneralConfig.Config.RootDir + Config.Dir;
            string suffix = "*.terse.*";
            string[] terseFilePaths = Directory.GetFiles(Info.Directory.FullName, suffix);
            number = terseFilePaths.Count();
            return number;
        }
    }
}
