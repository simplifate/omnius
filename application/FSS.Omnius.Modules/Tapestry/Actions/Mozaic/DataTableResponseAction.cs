using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.DB;
using FSS.Omnius.Modules.Entitron.Entity.Mozaic.Bootstrap;
using FSS.Omnius.Modules.Entitron.Entity;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Tapestry.Actions.Mozaic
{
    [MozaicRepository]
    public class DataTableResponseAction : Action
    {
        public override int Id => 2007;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "v$Data" };

        public override string Name => "DataTable response";

        public override string[] OutputVar => new string[] { "Response" };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            // Init
            DBEntities context = COREobject.i.Context;
            ListJson<DBItem> data = (ListJson<DBItem>)vars["Data"];
            List<DBItem> ds = new List<DBItem>();
            string orderColumnName = null;

            if (data.Count() > 0) {
                /**********************************************************************************/
                /* Vytvoříme si datasource - všechny sloupce jako string, přidáme hiddenId a akce */
                /**********************************************************************************/
                // Musíme znovu vytvořit sloupec s akcema
                string dataTableId = (string)vars["__Button__"];
                int bootstrapPageId = (int)vars["BootstrapPageId"];
                string actionIcons = "";
                MozaicBootstrapComponent uic = context.MozaicBootstrapComponents.Where(c => c.ElmId == dataTableId && c.MozaicBootstrapPageId == bootstrapPageId).FirstOrDefault();

                if (uic != null) {
                    JToken attributes = JToken.Parse(uic.Attributes);
                    foreach (JToken attr in attributes) {
                        if ((string)attr["name"] == "data-actions") {
                            string actionsString = ((string)attr["value"]).Replace('\'', '"');
                            JToken actions = JToken.Parse(actionsString);

                            List<string> actionsIconsList = new List<string>();
                            foreach (JToken a in actions) {
                                actionsIconsList.Add($"<i title=\"{(string)a["title"]}\" class=\"{(string)a["icon"]}\" data-action=\"{(string)a["action"]}\" data-idparam=\"{(string)a["idParam"]}\" data-confirm=\"{(string)a["confirm"]}\"></i>");
                            }
                            if (actionsIconsList.Count() > 0) {
                                actionIcons = "<span class=\"text-nowrap\">" + string.Join(" ", actionsIconsList) + "</span>";
                            }
                            break;
                        }
                    }
                }

                List<string> displayNames = new List<string>();
                List<string> columnsNames = data[0].getColumnNames();
                try {
                    displayNames = data[0].getColumnDisplayNames().ToList();
                }
                catch (NullReferenceException) {
                    displayNames = columnsNames;
                }
                
                foreach (DBItem row in data) {
                    DBItem newRow = new DBItem(null, null);
                    int i = 0;

                    foreach (string prop in columnsNames) {
                        var value = row[prop];
                        var displayName = displayNames[columnsNames.IndexOf(prop)];
                        if (i == 0)
                            orderColumnName = displayName;

                        // Převedeme všechny hodnoty na string
                        if (value is bool) {
                            newRow[displayName] = (bool)value ? "Ano" : "Ne";
                        }
                        else if (value is DateTime) {
                            newRow[displayName] = ((DateTime)value).ToString("d. M. yyyy H:mm:ss");
                        }
                        else if (value is string) {
                            newRow[displayName] = (string)value;
                        }
                        else {
                            newRow[displayName] = value.ToString();
                        }

                        // pokud je sloupec id nebo Id nastavíme hiddenId
                        if (prop == "id" || prop == "Id") {
                            i++;
                            newRow["hiddenId"] = value.ToString();
                        }
                        i++;
                    }
                    // Pokud existují akce, přidáme je
                    if (!string.IsNullOrEmpty(actionIcons)) {
                        newRow["Akce"] = actionIcons;
                    }

                    ds.Add(newRow);
                }
            }

            /**********************************************************************************/
            /* Filtr dat                                                                      */
            /**********************************************************************************/
            List<DBItem> filteredData = new List<DBItem>();
            JToken columnsSearch = JToken.Parse((string)vars["columns"]);
            JToken search = JToken.Parse((string)vars["search"]);

            filteredData.AddRange(ds);

            foreach (JToken cs in columnsSearch) {
                string searchFor = (string)cs["search"]["value"];
                if (!string.IsNullOrEmpty(searchFor)) {
                    string columnName = (string)cs["data"];
                    filteredData = filteredData.Where(r => ((string)r[columnName]).ToLowerInvariant().Contains(searchFor.ToLowerInvariant())).ToList();
                }
            }
            string globalSearchFor = (string)search["value"];
            if (!string.IsNullOrEmpty(globalSearchFor)) {
                filteredData.Clear();

                foreach (DBItem row in ds) {
                    foreach (string prop in row.getColumnNames()) {
                        if (prop == "id" || prop == "Id" || prop == "hiddenId" || prop == "Akce")
                            continue;

                        List<DBItem> found = ds.Where(r => ((string)r[prop]).ToLowerInvariant().Contains(globalSearchFor.ToLowerInvariant())).ToList();
                        if (found.Count() > 0) {
                            foreach (DBItem item in found) {
                                if (!filteredData.Contains(item)) {
                                    filteredData.Add(item);
                                }
                            }
                        }
                    }
                }
            }

            /**********************************************************************************/
            /* Zpracujeme řazení                                                              */
            /**********************************************************************************/
            JToken order = JToken.Parse((string)vars["order"]);
            int j = 0;
            IOrderedEnumerable<DBItem> tmpData = filteredData.OrderBy(r => r[r.getColumnNames().First()]);
            var comparer = new NaturalComparer<string>(CompareNatural);

            foreach (JToken by in order) {
                string columnName = (string)columnsSearch[(int)by["column"]]["data"];
                string dir = (string)by["dir"];

                if (j == 0) {
                    tmpData = dir == "desc" ?
                        filteredData.OrderByDescending(r => (string)r[columnName], comparer) :
                        filteredData.OrderBy(r => (string)r[columnName], comparer);
                }
                else {
                    tmpData = dir == "desc" ?
                        tmpData.ThenByDescending(r => (string)r[columnName], comparer) :
                        tmpData.ThenBy(r => (string)r[columnName], comparer);
                }
                j++;
            }
            filteredData = tmpData.ToList();

            /**********************************************************************************/
            /* Zpracujeme stránkování                                                         */
            /**********************************************************************************/
            int start = Convert.ToInt32(vars["start"]);
            int length = Convert.ToInt32(vars["length"]);
            
            var pagedData = length == -1 ? filteredData.ToList() : filteredData.Skip(start).Take(length).ToList();

            /**********************************************************************************/
            /* Vrátíme výsledek                                                               */
            /**********************************************************************************/
            ListJson<DBItem> finalDS = new ListJson<DBItem>();
            finalDS.AddRange(pagedData);

            JToken response = new JObject();
            response["draw"] = Convert.ToInt32(vars["draw"]);
            response["recordsTotal"] = data.Count();
            response["recordsFiltered"] = filteredData.Count();
            response["data"] = finalDS.ToJson();

            outputVars["Response"] = response;
        }

        #region helper methods

        public static int CompareNatural(string strA, string strB)
        {
            return CompareNatural(strA, strB, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase);
        }

        public static int CompareNatural(string strA, string strB, CultureInfo culture, CompareOptions options)
        {
            CompareInfo cmp = culture.CompareInfo;
            int iA = 0;
            int iB = 0;
            int softResult = 0;
            int softResultWeight = 0;
            while (iA < strA.Length && iB < strB.Length) {
                bool isDigitA = Char.IsDigit(strA[iA]);
                bool isDigitB = Char.IsDigit(strB[iB]);
                if (isDigitA != isDigitB) {
                    return cmp.Compare(strA, iA, strB, iB, options);
                }
                else if (!isDigitA && !isDigitB) {
                    int jA = iA + 1;
                    int jB = iB + 1;
                    while (jA < strA.Length && !Char.IsDigit(strA[jA])) jA++;
                    while (jB < strB.Length && !Char.IsDigit(strB[jB])) jB++;
                    int cmpResult = cmp.Compare(strA, iA, jA - iA, strB, iB, jB - iB, options);
                    if (cmpResult != 0) {
                        // Certain strings may be considered different due to "soft" differences that are
                        // ignored if more significant differences follow, e.g. a hyphen only affects the
                        // comparison if no other differences follow
                        string sectionA = strA.Substring(iA, jA - iA);
                        string sectionB = strB.Substring(iB, jB - iB);
                        if (cmp.Compare(sectionA + "1", sectionB + "2", options) ==
                            cmp.Compare(sectionA + "2", sectionB + "1", options)) {
                            return cmp.Compare(strA, iA, strB, iB, options);
                        }
                        else if (softResultWeight < 1) {
                            softResult = cmpResult;
                            softResultWeight = 1;
                        }
                    }
                    iA = jA;
                    iB = jB;
                }
                else {
                    char zeroA = (char)(strA[iA] - (int)Char.GetNumericValue(strA[iA]));
                    char zeroB = (char)(strB[iB] - (int)Char.GetNumericValue(strB[iB]));
                    int jA = iA;
                    int jB = iB;
                    while (jA < strA.Length && strA[jA] == zeroA) jA++;
                    while (jB < strB.Length && strB[jB] == zeroB) jB++;
                    int resultIfSameLength = 0;
                    do {
                        isDigitA = jA < strA.Length && Char.IsDigit(strA[jA]);
                        isDigitB = jB < strB.Length && Char.IsDigit(strB[jB]);
                        int numA = isDigitA ? (int)Char.GetNumericValue(strA[jA]) : 0;
                        int numB = isDigitB ? (int)Char.GetNumericValue(strB[jB]) : 0;
                        if (isDigitA && (char)(strA[jA] - numA) != zeroA) isDigitA = false;
                        if (isDigitB && (char)(strB[jB] - numB) != zeroB) isDigitB = false;
                        if (isDigitA && isDigitB) {
                            if (numA != numB && resultIfSameLength == 0) {
                                resultIfSameLength = numA < numB ? -1 : 1;
                            }
                            jA++;
                            jB++;
                        }
                    }
                    while (isDigitA && isDigitB);
                    if (isDigitA != isDigitB) {
                        // One number has more digits than the other (ignoring leading zeros) - the longer
                        // number must be larger
                        return isDigitA ? 1 : -1;
                    }
                    else if (resultIfSameLength != 0) {
                        // Both numbers are the same length (ignoring leading zeros) and at least one of
                        // the digits differed - the first difference determines the result
                        return resultIfSameLength;
                    }
                    int lA = jA - iA;
                    int lB = jB - iB;
                    if (lA != lB) {
                        // Both numbers are equivalent but one has more leading zeros
                        return lA > lB ? -1 : 1;
                    }
                    else if (zeroA != zeroB && softResultWeight < 2) {
                        softResult = cmp.Compare(strA, iA, 1, strB, iB, 1, options);
                        softResultWeight = 2;
                    }
                    iA = jA;
                    iB = jB;
                }
            }
            if (iA < strA.Length || iB < strB.Length) {
                return iA < strA.Length ? 1 : -1;
            }
            else if (softResult != 0) {
                return softResult;
            }
            return 0;
        }

        #endregion
    }

    public class NaturalComparer<T> : IComparer<T>
    {
        private Comparison<T> _comparison;

        public NaturalComparer(Comparison<T> comparison)
        {
            _comparison = comparison;
        }

        public int Compare(T x, T y)
        {
            return _comparison(x, y);
        }
    }
}
