using System.Data;

namespace FSS.Omnius.Modules.Entitron.DB
{
    public class DBView : Tabloid
    {
        public DBView(DBConnection db) : base(db)
        {
        }
        
        public string Sql { get; set; }

        private bool? _isInDB;
        public override bool IsInDB
        {
            get
            {
                if (_isInDB == null)
                {
                    if (string.IsNullOrWhiteSpace(Name))
                        return false;

                    IDbCommand command = _db.CommandSet.EXISTS_view(_db, Name);
                    using (DBReader reader = _db.ExecuteCommand(command))
                    {
                        reader.Read();
                        _isInDB = (int)reader["exists"] == 1;
                    }
                }

                return _isInDB.Value;
            }
        }

        public DBView Create()
        {
            _db.Commands.Enqueue(_db.CommandSet.CREATE_view(_db, this));

            return this;
        }

        public DBView Alter()
        {
            _db.Commands.Enqueue(_db.CommandSet.UPDATE_view(_db, this));

            return this;
        }

        public void Drop()
        {
            _db.Commands.Enqueue(_db.CommandSet.DROP_view(_db, Name));
        }

        public override string ToString() => $"DBView [{Name}][{_db.Application.Name}]";
    }
}
