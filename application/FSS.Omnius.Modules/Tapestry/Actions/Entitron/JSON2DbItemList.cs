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
                return new string[] { "TableName" };
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
            string tableName = (string)vars["TableName"];

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
            JToken json = JToken.Parse(jsonText);



            // return
            outputVars["Result"] = true;
        }
    }
}
