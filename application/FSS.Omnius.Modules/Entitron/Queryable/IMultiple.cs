using System.Data;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Entitron.Queryable
{
    public interface IMultiple
    {
        void Start(bool first);
        string ToSql(DBCommandSet set, IDbCommand command);
        string Separator { get; }
    }
}
