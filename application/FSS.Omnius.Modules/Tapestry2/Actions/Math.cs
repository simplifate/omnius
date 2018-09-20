using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;
using FSS.Omnius.Modules.Tapestry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry2.Actions
{
    public class Math : ActionManager
    {
        [Action(4000, "Math: Add", "Result")]
        public static object Add(COREobject core, object A, object B)
        {
            if (A is int && B is int)
                return (int)A + (int)B;
            else
                return Convert.ToDouble(A) + Convert.ToDouble(B);
        }

        [Action(4001, "Math: Subtract", "Result")]
        public static object Subtract(COREobject core, object A, object B)
        {
            if (A is int && B is int)
                return (int)A - (int)B;
            else
                return Convert.ToDouble(A) - Convert.ToDouble(B);
        }

        [Action(4002, "Math: Multiply", "Result")]
        public static object Multiply(COREobject core, object A, object B)
        {
            if (A is int && B is int)
                return (int)A * (int)B;
            else
                return Convert.ToDouble(A) * Convert.ToDouble(B);
        }

        [Action(4003, "Math: Divide", "Result")]
        public static object Divide(COREobject core, object A, object B, bool AsInteger = false)
        {
            if (AsInteger)
                return Convert.ToInt64(A) / Convert.ToInt64(B);
            else
                return Convert.ToDouble(A) / Convert.ToDouble(B);
        }

        [Action(4004, "Math: Modulo", "Result")]
        public static object Modulo(COREobject core, object A, object B, bool AsInteger = false)
        {
            if (AsInteger)
                return Convert.ToInt64(A) % Convert.ToInt64(B);
            else
                return Convert.ToDouble(A) % Convert.ToDouble(B);
        }

        [Action(4005, "Math: Column autosum", "Result")]
        public static object ColumnAutosum(COREobject core, List<DBItem> TableData, string ColumnName)
        {
            if (TableData.Count == 0)
                return 0;

            if (TableData[0][ColumnName] is int)
            {
                int sum = 0;
                foreach (var row in TableData)
                {
                    sum += (int)row[ColumnName];
                }
                return sum;
            }
            else
            {
                double sum = 0;
                foreach (var row in TableData)
                {
                    sum += Convert.ToDouble(row[ColumnName]);
                }
                return sum;
            }
        }

        [Action(4006, "Math: Column average", "Result")]
        public static double ColumnAverage(COREobject core, List<DBItem> TableData, string ColumnName)
        {
            if (TableData.Count == 0)
                return 0.0;

            double sum = 0;
            foreach (var row in TableData)
            {
                sum += Convert.ToDouble(row[ColumnName]);
            }
            return sum / TableData.Count;
        }

        [Action(4007, "Math: Scalar product", "Result")]
        public static List<DBItem> ScalarProduct(COREobject core, List<DBItem> TableData, string ColumnName, Double Multiplier, string TargetColumnName = null)
        {
            if (TargetColumnName != null)
            {
                foreach (var row in TableData)
                {
                    row[TargetColumnName] = Convert.ToDouble(row[ColumnName]) * Multiplier;
                }
            }
            else
            {
                foreach (var row in TableData)
                {
                    row[ColumnName] = Convert.ToDouble(row[ColumnName]) * Multiplier;
                }
            }

            return TableData;
        }

        [Action(4009, "Math: Add in vector", "Result")]
        public static List<DBItem> AddInVector(COREobject core, List<DBItem> TableData, string ColumnA, string ColumnB, string ResultColumn)
        {
            if (TableData.Count == 0)
                return TableData;

            var firstRow = TableData[0];
            bool createNewColumn = !firstRow.HasProperty(ResultColumn);
            bool integerMode = firstRow[ColumnA] is int && firstRow[ColumnB] is int;

            if (createNewColumn)
            {
                if (integerMode)
                {
                    foreach (var row in TableData)
                    {
                        row[ResultColumn] = (int)row[ColumnA] + (int)row[ColumnB];
                    }
                }
                else
                {
                    foreach (var row in TableData)
                    {
                        row[ResultColumn] = Convert.ToDouble(row[ColumnA]) + Convert.ToDouble(row[ColumnB]);
                    }
                }
            }
            else
            {
                if (integerMode)
                {
                    foreach (var row in TableData)
                    {
                        row[ResultColumn] = (int)row[ColumnA] + (int)row[ColumnB];
                    }
                }
                else
                {
                    foreach (var row in TableData)
                    {
                        row[ResultColumn] = Convert.ToDouble(row[ColumnA]) + Convert.ToDouble(row[ColumnB]);
                    }
                }
            }

            return TableData;
        }

        [Action(4008, "Math: Vector union", "Result")]
        public static List<DBItem> VectorUnion(COREobject core, List<DBItem> VectorA, List<DBItem> VectorB, string ColumnName, string Identifiers)
        {
            List<string> identifiers = Identifiers.Split(',').ToList();
            DBItem rightItem;
            var usageMap = new Dictionary<DBItem, bool>();
            foreach (var item in VectorB)
            {
                usageMap.Add(item, false);
            }
            switch (identifiers.Count)
            {
                case 1:
                    foreach (var leftItem in VectorA)
                    {
                        rightItem = VectorB.FirstOrDefault(c => c[identifiers[0]] == leftItem[identifiers[0]]);

                        if (rightItem != null)
                        {
                            leftItem[ColumnName] = Convert.ToDouble(leftItem[ColumnName]) + Convert.ToDouble(rightItem[ColumnName]);
                            usageMap[rightItem] = true;
                        }
                    }
                    break;
                case 2:
                    foreach (var leftItem in VectorA)
                    {
                        rightItem = VectorB.FirstOrDefault(c => c[identifiers[0]] == leftItem[identifiers[0]]
                                && c[identifiers[1]] == leftItem[identifiers[1]]);

                        if (rightItem != null)
                        {
                            leftItem[ColumnName] = Convert.ToDouble(leftItem[ColumnName]) + Convert.ToDouble(rightItem[ColumnName]);
                            usageMap[rightItem] = true;
                        }
                    }
                    break;
                case 3:
                    foreach (var leftItem in VectorA)
                    {
                        rightItem = VectorB.FirstOrDefault(c => c[identifiers[0]] == leftItem[identifiers[0]]
                                && c[identifiers[1]] == leftItem[identifiers[1]]
                                && c[identifiers[2]] == leftItem[identifiers[2]]);

                        if (rightItem != null)
                        {
                            leftItem[ColumnName] = Convert.ToDouble(leftItem[ColumnName]) + Convert.ToDouble(rightItem[ColumnName]);
                            usageMap[rightItem] = true;
                        }
                    }
                    break;
                default:
                    throw new InvalidOperationException("Tapestry VectorSum action: problem with identifiers");
            }
            foreach (var item in usageMap.Where(c => c.Value == false).Select(c => c.Key))
            {
                VectorA.Add(item);
            }
            return VectorA;
        }

        [Action(4010, "Math: Subtract in vector", "Result")]
        public static List<DBItem> SubtractInVector(COREobject core, List<DBItem> TableData, string ColumnA, string ColumnB, string ResultColumn)
        {
            if (TableData.Count == 0)
                return TableData;

            var firstRow = TableData[0];
            bool integerMode = firstRow[ColumnA] is int && firstRow[ColumnB] is int;

            if (integerMode)
            {
                foreach (var row in TableData)
                {
                    row[ResultColumn] = (int)row[ColumnA] - (int)row[ColumnB];
                }
            }
            else
            {
                foreach (var row in TableData)
                {
                    row[ResultColumn] = Convert.ToDouble(row[ColumnA]) - Convert.ToDouble(row[ColumnB]);
                }
            }

            return TableData;
        }

        [Action(4011, "Math: Multiply in vector", "Result")]
        public static List<DBItem> MultiplyInVector(COREobject core, List<DBItem> TableData, string ColumnA, string ColumnB, string ResultColumn)
        {
            if (TableData.Count == 0)
                return TableData;

            var firstRow = TableData[0];
            bool integerMode = firstRow[ColumnA] is int && firstRow[ColumnB] is int;

            if (integerMode)
            {
                foreach (var row in TableData)
                {
                    row[ResultColumn] = (int)row[ColumnA] * (int)row[ColumnB];
                }
            }
            else
            {
                foreach (var row in TableData)
                {
                    row[ResultColumn] = Convert.ToDouble(row[ColumnA]) * Convert.ToDouble(row[ColumnB]);
                }
            }

            return TableData;
        }

        [Action(4012, "Math: Divide in vector", "Result")]
        public static List<DBItem> ColumnAutosum(COREobject core, List<DBItem> TableData, string ColumnA, string ColumnB, string ResultColumn)
        {
            if (TableData.Count == 0)
                return TableData;
            var firstRow = TableData[0];
            bool integerMode = firstRow[ColumnA] is int && firstRow[ColumnB] is int;

            if (integerMode)
            {
                foreach (var row in TableData)
                {
                    row[ResultColumn] = (int)row[ColumnA] / (int)row[ColumnB];
                }
            }
            else
            {
                foreach (var row in TableData)
                {
                    row[ResultColumn] = Convert.ToDouble(row[ColumnA]) / Convert.ToDouble(row[ColumnB]);
                }
            }

            return TableData;
        }

        [Action(4013, "Math: Scalar division", "Result")]
        public static List<DBItem> ScalarDivision(COREobject core, List<DBItem> TableData, string ColumnName, double Divider, string TargetColumnName = null)
        {
            if (TargetColumnName != null)
            {
                foreach (var row in TableData)
                {
                    row[TargetColumnName] = Convert.ToDouble(row[ColumnName]) / Divider;
                }
            }
            else
            {
                foreach (var row in TableData)
                {
                    row[ColumnName] = Convert.ToDouble(row[ColumnName]) / Divider;
                }
            }

            return TableData;
        }

        [Action(4014, "Math: Copy in vector", "Result")]
        public static List<DBItem> CopyInVector(COREobject core, List<DBItem> TableData, string SourceColumn, string TargetColumn, object ConstantSource = null)
        {
            if (TableData.Count == 0)
                return TableData;

            var firstRow = TableData[0];

            if (ConstantSource != null)
            {
                foreach (var row in TableData)
                {
                    row[TargetColumn] = ConstantSource;
                }
            }
            else
            {
                foreach (var row in TableData)
                {
                    row[TargetColumn] = row[SourceColumn];
                }
            }

            return TableData;
        }

        [Action(4015, "Math: Round", "Result")]
        public static double Round(COREobject core, int Precision, double Value)
        {
            return System.Math.Round(Value, Precision);
        }

        [Action(4016, "Math: Convert to HEX", "Result")]
        public static string ConvertToHex(COREobject core, double Number, int MaxDecimals)
        {
            return TapestryUtils.DoubleToHex(Number, MaxDecimals);
        }

        [Action(4050, "Math: Exponentiation", "Result")]
        public static double Exponentiation(COREobject core, double Base, double Exponent)
        {
            return System.Math.Pow(Base, Exponent);
        }

        [Action(40015, "Math: Column autosum list object", "Result")]
        public static object ColumnAutosumListObject(COREobject core, string ColumnName, List<object> TableData)
        {
            if (TableData.Count == 0)
                return 0;

            if (((Dictionary<string, object>)(TableData[0]))[ColumnName] is int)
            {
                int sum = 0;
                foreach (var row in TableData)
                {
                    sum += (int)((Dictionary<string, object>)row)[ColumnName];
                }
                return sum;
            }
            else
            {
                double sum = 0;
                foreach (var row in TableData)
                {
                    sum += Convert.ToDouble(((Dictionary<string, object>)row)[ColumnName]);
                }
                return sum;
            }
        }
    }
}
