using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry2.Actions.App
{
    public class AS : ActionManager
    {
        [Action(1038, "Max value restriction", "Result")]
        public static List<DBItem> MaxValue(COREobject core, List<DBItem> TableData, string Column, int MaxValue)
        {
            if (TableData.Count == 0)
                return new List<DBItem>();

            List<DBItem> listItems = new List<DBItem>();
            int currentValue = 0;
            foreach (DBItem row in TableData)
            {
                listItems.Add(row);
                currentValue += (int)row[Column];

                if (currentValue >= MaxValue)
                    break;
            }

            return listItems;
        }
    }
}
