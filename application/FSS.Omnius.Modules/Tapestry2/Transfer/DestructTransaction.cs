using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry2.Transfer
{
    [Serializable]
    public class DestructTransaction
    {
        public BlockAttribute BlockAttribute { get; set; }

        public ResponseTransaction HttpResponse { get; set; }
        public string JsonResults { get; set; }
        public Dictionary<string, object> Data { get; set; }
        public Dictionary<string, object> CrossBlockRegistry { get; set; }
    }
}
