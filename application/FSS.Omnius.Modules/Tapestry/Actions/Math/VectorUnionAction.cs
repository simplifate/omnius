using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using System.Linq;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [MathRepository]
    class VectorUnionAction : Action
    {
        public override int Id
        {
            get
            {
                return 4008;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "VectorA", "VectorB", "ColumnName", "Identifiers" };
            }
        }

        public override string Name
        {
            get
            {
                return "Math: Vector union";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[]
                {
                    "Result"
                };
            }
        }

        public override int? ReverseActionId
        {
            get
            {
                return null;
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            var vectorA = (List<DBItem>)vars["VectorA"];
            var vectorB = (List<DBItem>)vars["VectorB"];
            string columnName = (string)vars["ColumnName"];
            List<string> identifiers = ((string)vars["Identifiers"]).Split(',').ToList();
            DBItem rightItem;
            var usageMap = new Dictionary<DBItem, bool>();
            foreach (var item in vectorB)
            {
                usageMap.Add(item, false);
            }
            switch (identifiers.Count)
            {
                case 1:
                    foreach (var leftItem in vectorA)
                    {
                        rightItem = vectorB.FirstOrDefault(c => c[identifiers[0]] == leftItem[identifiers[0]]);

                        if (rightItem != null)
                        {
                            leftItem[columnName] = Convert.ToDouble(leftItem[columnName]) + Convert.ToDouble(rightItem[columnName]);
                            usageMap[rightItem] = true;
                        }
                    }
                    break;
                case 2:
                    foreach (var leftItem in vectorA)
                    {
                        rightItem = vectorB.FirstOrDefault(c => c[identifiers[0]] == leftItem[identifiers[0]]
                                && c[identifiers[1]] == leftItem[identifiers[1]]);

                        if (rightItem != null)
                        {
                            leftItem[columnName] = Convert.ToDouble(leftItem[columnName]) + Convert.ToDouble(rightItem[columnName]);
                            usageMap[rightItem] = true;
                        }
                    }
                    break;
                case 3:
                    foreach (var leftItem in vectorA)
                    {
                        rightItem = vectorB.FirstOrDefault(c => c[identifiers[0]] == leftItem[identifiers[0]]
                                && c[identifiers[1]] == leftItem[identifiers[1]]
                                && c[identifiers[2]] == leftItem[identifiers[2]]);

                        if (rightItem != null)
                        {
                            leftItem[columnName] = Convert.ToDouble(leftItem[columnName]) + Convert.ToDouble(rightItem[columnName]);
                            usageMap[rightItem] = true;
                        }
                    }
                    break;
                default:
                    throw new InvalidOperationException("Tapestry VectorSum action: problem with identifiers");
            }
            foreach (var item in usageMap.Where(c => c.Value==false).Select(c=> c.Key))
            {
                vectorA.Add(item);
            }
            outputVars["Result"] = vectorA;
        }
    }
}
