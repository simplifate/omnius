using System;
using FSS.Omnius.Modules.Entitron.Entity.Nexus;

namespace FSS.Omnius.Modules.Nexus.Service
{
    public interface IFileSyncService
    {
        void DownloadFile(FileMetadata file);
        void UploadFile(FileMetadata file);
        void DeleteFile(FileMetadata file);
    }

    public delegate void FileSyncServiceDownloadedEventHandler(object sender, FileSyncServiceDownloadedEventArgs args);    

    public class FileSyncServiceDownloadedEventArgs: EventArgs
    {
        public FileMetadata FileMetadata { get; set; }

        public FileSyncServiceDownloadedResult Result { get; set; }
    }


    public enum FileSyncServiceDownloadedResult : short
    {
        Success = 1,
        Error = 2
    }
}
