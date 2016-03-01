namespace FSS.Omnius.Modules.Entitron.Service
{
    public interface IBackupGeneratorService
    {
        void ExportAllDatabaseDesignerData(string filename);
        void ExportApplication(int id);
    }
}
