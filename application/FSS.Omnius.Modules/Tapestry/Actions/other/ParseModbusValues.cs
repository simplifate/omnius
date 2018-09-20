using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Nexus.Service;
using FSS.Omnius.Modules.Tapestry.Actions.Entitron;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    /// <summary>

    /// </summary>
    [EntitronRepository]
    public class ParseModbusValues : Action
    {
        public override int Id
        {
            get {
                return 30128;
            }
        }
        public override int? ReverseActionId
        {
            get {
                return null;
            }
        }
        public override string[] InputVar
        {
            get {
                return new string[] { "ModbusValuesArray" };
            }
        }
        public override string[] OutputVar
        {
            get {
                return new string[] { "Result" };
            }
        }
        public override string Name
        {
            get {
                return "Parse modbus values";
            }
        }

        private List<DataItem> dataMap = new List<DataItem>()
        {
            new DataItem() { Id = "EnergieChlazeni",   High = 0,  Low = 3  },
            new DataItem() { Id = "PrikonChlazeni",    High = 6,  Low = 9  },
            new DataItem() { Id = "EnergieDatCentrum", High = 12, Low = 15 },
            new DataItem() { Id = "PrikonDatCentrum",  High = 18, Low = 21 },
            new DataItem() { Id = "EnergieKGJ",        High = 24, Low = 27 },
            new DataItem() { Id = "PrikonKGJ",         High = 30, Low = 33 }
        };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            JToken data = (JToken)vars["ModbusValuesArray"];
            JObject modbusValues = new JObject();

            for (var i = 0; i < dataMap.Count; i++) {
                DataItem item = dataMap[i];
                double value = 0;
                int exp = 6;
                int k = 0;
                for (var j = item.High; j < item.Low; j++) {
                    var b = (double)data[j];
                    value += b * System.Math.Pow(0x10, exp - k * 2);
                    k++;
                }

                modbusValues.Add(item.Id, value);
            }

            outputVars["Result"] = modbusValues;

        }
    }

    class DataItem
    {
        public string Id;
        public int High;
        public int Low;
    }
}