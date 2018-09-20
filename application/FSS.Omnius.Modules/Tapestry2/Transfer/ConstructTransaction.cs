using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry2.Transfer
{
    [Serializable]
    public class ConstructTransaction
    {
        public RequestTransaction httpRequest { get; set; }
        public string efConnectionString { get; set; }
        public string Username { get; set; }
        public string ApplicationName { get; set; }
        public int ModelId { get; set; }
        public int DeleteId { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }
}
