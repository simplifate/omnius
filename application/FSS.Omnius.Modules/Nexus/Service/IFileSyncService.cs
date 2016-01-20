using FSS.Omnius.Modules.Entitron.Entity.Nexus;

namespace FSS.Omnius.Modules.Nexus.Service
{
    public interface IFileSyncService
    {
        void DownloadFile(FileMetadata file);
        void UploadFile(FileMetadata file);
    }
}
