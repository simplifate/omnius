using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Entitron
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
