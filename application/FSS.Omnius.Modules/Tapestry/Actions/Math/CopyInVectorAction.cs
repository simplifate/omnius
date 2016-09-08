using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [MathRepository]
    class CopyInVectorAction : Action
    {
        public override int Id
        {
            get
            {
                return 4014;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "TableData", "SourceColumn", "TargetColumn", "?ConstantSource" };
            }
        }

        public override string Name
        {
            get
            {
                return "Math: Copy in vector";
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
            var tableData = (List<DBItem>)vars["TableData"];
            string sourceColumn = (string)vars["SourceColumn"];
            string targetColumn = (string)vars["TargetColumn"];
            if (tableData.Count == 0)
                return;
            var firstRow = tableData[0];
            bool createNewColumn = !firstRow.HasProperty(targetColumn);

            if (vars.ContainsKey("ConstantSource"))
            {
                object constant = vars["ConstantSource"];
                if (createNewColumn)
                {
                    foreach (var row in tableData)
                    {
                        row.createProperty(0, targetColumn, constant);
                    }
                }
                else
                {
                    foreach (var row in tableData)
                    {
                        row[targetColumn] = constant;
                    }
                }
            }
            else
            {
                if (createNewColumn)
                {
                    foreach (var row in tableData)
                    {
                        row.createProperty(0, targetColumn, row[sourceColumn]);
                    }
                }
                else
                {
                    foreach (var row in tableData)
                    {
                        row[targetColumn] = row[sourceColumn];
                    }
                }
            }
            outputVars["Result"] = tableData;
        }
    }
}
