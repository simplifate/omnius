using FSS.Omnius.Modules.Entitron.Queryable;
using System.Linq;

namespace FSS.Omnius.Modules.Entitron.DB
{
    public class Tabloid
    {
        public Tabloid(DBConnection db)
        {
            _db = db;

            _column = new EntityManager<DBColumn>(db, this);
        }

        protected DBConnection _db;

        public string RealName => _db.CommandSet.ToRealTableName(_db.Application, Name, false);
        public string Name { get; set; }
        protected EntityManager<DBColumn> _column;
        public EntityManager<DBColumn> Columns => _column;
        public virtual bool IsInDB { get; }

        public Select Select(params string[] columns)
        {
            return new Select(_db, Name, columns);
        }
        public DBItem SelectById(int id)
        {
            return Select().Where(c => c.Column(DBCommandSet.PrimaryKey).Equal(id)).ToList().FirstOrDefault();
        }

        public override string ToString() => $"Tabloid [{Name}][{_db.Application.Name}]";
    }
}
