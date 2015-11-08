namespace FSS.FSPOC.BussinesObjects.Service
{
    public interface IActionService
    {
        T GetParam<T>() where T : class;
        void AddParam<T>(T param) where T : class;
    }
}