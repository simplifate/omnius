using FSS.Omnius.Modules.Entitron.Entity.Cortex;

namespace FSS.Omnius.Modules.Cortex.Interface
{
    public interface ICortexAPI
    {
        void Create(Task t);
        void Change(Task t, Task original);
        void Delete(Task t);
        string List();
    }
}
