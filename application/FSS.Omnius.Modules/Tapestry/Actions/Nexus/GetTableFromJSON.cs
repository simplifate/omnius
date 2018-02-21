using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Tapestry.Actions.Nexus
{
    /// <summary>
    /// Provede SOAP volání a vrátí výsledek. Jako InputVars očekává název webové služby (WSName) a název metody (MethodName).
    /// Dále je třeba ve form collection předat všechny parametry, které metoda očekává (můžou být prázdné) ve formátu Param[...]
    /// </summary>
    [NexusRepository]
    public class GetTableFromJSON : Action
    {
        public override int Id => 30101;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "InputJSON"};

        public override string[] OutputVar => new string[] { "Result" };

        public override string Name => "Get Table From JSON";

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            DBConnection db = Modules.Entitron.Entitron.i;

            var jArray = vars["InputJSON"] as JArray;
            List<DBItem> dbItems = new List<DBItem>();
            if (jArray != null) {
                //iterate jsonObject in the array
                foreach (var jsonobject in jArray.Children()) {
                    DBItem item = new DBItem(db, null);
                    foreach (JProperty property in ((JObject)jsonobject).Properties()) {
                        item[property.Name] = property.Value;
                    } //create properties for item
                    dbItems.Add(item); //add item to the list
                }
            }
            outputVars["Result"] = dbItems;
        }
    }
}
