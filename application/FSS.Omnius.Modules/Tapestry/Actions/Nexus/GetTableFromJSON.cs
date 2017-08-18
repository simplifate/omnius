using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Nexus.Service;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Nexus
{
    /// <summary>
    /// Provede SOAP volání a vrátí výsledek. Jako InputVars očekává název webové služby (WSName) a název metody (MethodName).
    /// Dále je třeba ve form collection předat všechny parametry, které metoda očekává (můžou být prázdné) ve formátu Param[...]
    /// </summary>
    [NexusRepository]
    public class GetTableFromJSON : Action
    {
        public override int Id
        {
            get
            {
                return 30101;
            }
        }
        public override int? ReverseActionId
        {
            get
            {
                return null;
            }
        }
        public override string[] InputVar
        {
            get
            {
                return new string[] { "InputJSON"};
            }
        }
        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Result" };
            }
        }
        public override string Name
        {
            get
            {
                return "Get Table From JSON";
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            var jArray = vars["InputJSON"] as JArray;
            List<DBItem> dbItems = new List<DBItem>();
            if (jArray != null) {
                //iterate jsonObject in the array
                foreach (var jsonobject in jArray.Children()) {
                    DBItem item = new DBItem();
                    foreach (JProperty property in ((JObject)jsonobject).Properties()) {
                        item.createProperty(0, property.Name, property.Value);
                    } //create properties for item
                    dbItems.Add(item); //add item to the list
                }
            }
            outputVars["Result"] = dbItems;
        }
    }
}
