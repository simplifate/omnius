using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.DB;
using FSS.Omnius.Modules.Tapestry2;
using System.Collections.Generic;

namespace FSS.Omnius.Modules.CORE
{
    public abstract class COREbase
    {
        public User User { get; set; }
        public Application Application { get; set; }
        public Application ApplicationShared => Application.SystemApp();
        public DBEntities Context { get; set; }
        public DBEntities AppContext => Context;
        private DBConnection _entitron;
        public DBConnection Entitron
        {
            get
            {
                if (_entitron == null)
                    _entitron = new DBConnection(this);

                return _entitron;
            }
        }
        private DBConnection _entitronShared { get; set; }
        public DBConnection EntitronShared
        {
            get
            {
                if (_entitronShared == null)
                    _entitronShared = new DBConnection(this, true);

                return _entitronShared;
            }
        }

        public BlockAttribute BlockAttribute { get; set; }
        public string Executor { get; set; }
        public int ModelId { get; set; }
        public int DeleteId { get; set; }
        public ITranslator Translator { get; set; }
        public Message Message { get; set; }
        public Dictionary<string, object> Data { get; set; }
        public Dictionary<string, object> CrossBlockRegistry { get; set; }
    }
}
