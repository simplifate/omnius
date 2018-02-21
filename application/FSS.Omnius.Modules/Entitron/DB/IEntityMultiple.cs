using System.Collections.Generic;
using System.Data;

namespace FSS.Omnius.Modules.Entitron.DB
{
    public interface IEntityMultiple<T> where T : IEntityMultiple<T>
    {
        // pseudostatic
        HashSet<T> Load(Tabloid tabloid);

        void AddToDB();
        void RemoveFromDB();
        bool Compare(T item);
    }
}
