using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entitron;
using Entitron.Entity;

namespace Mozaic
{
    public class MozaicTemplate
    {
        private Template _template;

        private string startKey = "<%% ";
        private string endingKey = " %%>";
        private string cssKey = "<%%css%%>";

        public MozaicTemplate(Template template)
        {
            _template = template;
        }

        public string Render(Page master, Dictionary<string, string> Relations, DBItem Model, DBEntities entity, string address = null)
        {
            string output = string.Copy(_template.Html);

            // replace css
            int indexOfCss = output.IndexOf(cssKey);
            if (indexOfCss != -1)
            {
                output = output.Remove(indexOfCss, cssKey.Length);
                output = output.Insert(indexOfCss, string.Join("", master.Css.Select(c => c.Value)));
            }

            // replace other
            for (int startIndex = 0; (startIndex = output.IndexOf(startKey, startIndex)) != -1;)
            {
                int innerStartIndex = startIndex + startKey.Length;
                int innerEndIndex = output.IndexOf(endingKey, innerStartIndex);
                string key = output.Substring(innerStartIndex, innerEndIndex - innerStartIndex).Trim();

                output = output.Remove(startIndex, (innerEndIndex + endingKey.Length) - startIndex);

                string currentAddress = address != null ? string.Format("{0}.{1}", address, key) : key;
                // there is no relation
                if (!Relations.ContainsKey(currentAddress))
                    continue;
                string[] items = Relations[currentAddress].Split(',');

                string replacement = "";
                foreach (string item in items)
                {
                    string[] a = item.Split(':');
                    if (a.Length < 1) continue; // wrong format
                    string type = a[0];
                    string name = a[1];

                    switch (type)
                    {
                        case "V": // value
                            replacement += name;
                            break;
                        case "P": // partial
                            replacement += new MozaicTemplate(entity.Templates.FirstOrDefault(t => t.Name == name))
                                .Render(master, Relations, Model, entity, currentAddress);
                            break;
                        case "D": // datasource
                            replacement += Model[name];
                            break;
                        case "DL": // datasource list
                            if (a.Length < 2) continue; // wrong format
                            string templateName = a[2];
                            foreach (DBItem listItem in (List<DBItem>)Model[name] ?? new List<DBItem>())
                            {
                                listItem["__parent__"] = Model;
                                replacement += new MozaicTemplate(entity.Templates.FirstOrDefault(t => t.Name == templateName))
                                    .Render(master, Relations, listItem, entity, currentAddress);
                            }
                            break;
                        default:
                            throw new FormatException("Template relations in wrong format");
                    }
                }

                output = output.Insert(startIndex, replacement);
            }

            return output;
        }
    }
}
