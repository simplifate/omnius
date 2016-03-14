using FSS.Omnius.Modules.Entitron;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.CORE
{
    public class ListJson<T> : List<T>, IToJson where T : DBItem
    {
        public JToken ToJson()
        {
            JArray result = new JArray();
            foreach (T item in this)
            {
                result.Add(item.ToJson());
            }

            return result;
        }
    }
}
