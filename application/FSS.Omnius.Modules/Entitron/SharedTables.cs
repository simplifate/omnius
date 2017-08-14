using FSS.Omnius.Modules.Entitron.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron
{
    /// <summary>
    /// Contains informations about shared tables
    /// </summary>
    public static class SharedTables
    {
        /// <summary>
        /// Id of app containing shared tables scheme
        /// </summary>
        private static int? _AppId;
        public static int AppId
        {
            get
            {
                if (_AppId == null)
                    _AppId = DBEntities.instance.Applications.Single(a => a.IsSystem).Id;

                return _AppId.Value;
            }
        }

        /// <summary>
        /// Prefix of shared tables
        /// </summary>
        public const string Prefix = "SharedTables";
    }
}
