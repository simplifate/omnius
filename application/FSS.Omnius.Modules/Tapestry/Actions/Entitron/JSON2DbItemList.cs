using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.CORE;
using FSS.Omnius.Modules.Entitron.Sql;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class JSON2DbItemListAction : Action
    {
        public override int Id
        {
            get
            {
                return 1047;
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
                return new string[] { "TableName", "BaseName", "Data", "?ItemName" };
            }
        }

        public override string Name
        {
            get
            {
                return "JSON to DbItem list";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Result" };
            }
        }
        
        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> invertedVars, Message message)
        {
            // init
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];

            if(!vars.ContainsKey("TableName")) {
                throw new Exception("Tapestry action JSON 2 DBItemList: TableName is required");
            }
            if(!vars.ContainsKey("BaseName")) {
                throw new Exception("Tapestry action JSON 2 DBItemList: BaseName is required");
            }
            if(!vars.ContainsKey("Data")) {
                throw new Exception("Tapestry action JSON 2 DBItemList: Data is required");
            }

            JToken data = (JToken)vars["Data"];
            string tableName = (string)vars["TableName"];
            string baseName = (string)vars["BaseName"];
            string itemName = vars.ContainsKey("ItemName") ? (string)vars["ItemName"] : "item";

            /****************************************************************************************
            ** MOCKUP DATA                                                                         **
            *****************************************************************************************
            string jsonText;
            try {
                XmlDocument xml = new XmlDocument();
                xml.Load("c:/users/mnvk8/Downloads/response.xml");
                jsonText = JsonConvert.SerializeXmlNode(xml);
            }
            catch (Exception e) {
                if (e is ArgumentNullException || e is XmlException) {
                    jsonText = "";// JsonConvert.SerializeObject(response);
                }
                else {
                    throw e;
                }
            }
            JToken data = JToken.Parse(jsonText);
            ****************************************************************************************/

            var table = core.Entitron.GetDynamicTable(tableName);

            Dictionary<string, DBColumn> columnExists = new Dictionary<string, DBColumn>();
            Dictionary<string, DataType> columnType = new Dictionary<string, DataType>();

            var items = data.SelectToken($"$..{baseName}.{itemName}");
            foreach (JToken item in items) {
                DBItem entity = new DBItem();
                foreach (JProperty pair in item) 
                {
                    // Zjistíme, jestli ten slupec v tabulce vůbec existuje
                    string columnName = pair.Name.ToLowerInvariant();
                    if(!columnExists.ContainsKey(columnName)) {
                        DBColumn column = table.columns.Where(c => c.Name == columnName).FirstOrDefault();

                        columnExists.Add(columnName, column);
                        if(column != null) {
                            columnType.Add(columnName, DataType.ByDBColumnTypeName(column.type));
                        }
                    }

                    if(columnExists[columnName] != null) { 
                        entity.createProperty(columnExists[columnName].ColumnId, columnName, Convertor.convert(columnType[columnName], (string)pair));
                    }
                }
                table.Add(entity);
            }

            core.Entitron.Application.SaveChanges();

            // return
            outputVars["Result"] = true;
        }
    }
}
