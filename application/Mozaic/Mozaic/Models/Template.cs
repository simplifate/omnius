using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;
using Entitron;

namespace Mozaic.Models
{
    public class Template
    {
        #region Attributes
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        
        [Required]
        [DefaultValue("")]
        public string Html { get; set; }

        [Required]
        [DefaultValue("")]
        public string Css { get; set; }
        
        [Required]
        public int CategoryId { get; set; }
        public TemplateCategory Category { get; set; }
        #endregion

        private string startKey = "<%% ";
        private string endingKey = " %%>";
        private string cssKey = "<%%css%%>";

        public string Render(Dictionary<string, string> Relations, DBItem Model, DBMozaic entity = null, string address = null)
        {
            string output = string.Copy(Html);

            // replace css
            int indexOfCss = output.IndexOf(cssKey);
            if (indexOfCss != -1)
            {
                output = output.Remove(indexOfCss, cssKey.Length);
                output = output.Insert(indexOfCss, Css);
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
                foreach(string item in items)
                {
                    string[] a = item.Split(':');
                    if (a.Length < 1) continue; // wrong format
                    string type = a[0];
                    string name = a[1];

                    switch (type)
                    {
                        case "P": // partial
                            replacement += entity.Templates.FirstOrDefault(t => t.Name == name)
                                .Render(Relations, Model, entity, currentAddress);
                            break;
                        case "D": // datasource
                            replacement += Model[name];
                            break;
                        case "DL": // datasource list
                            if (a.Length < 2) continue; // wrong format
                            string templateName = a[2];
                            foreach(DBItem listItem in (List<DBItem>)Model[name] ?? new List<DBItem>())
                            {
                                listItem["__parent__"] = Model;
                                replacement += entity.Templates.FirstOrDefault(t => t.Name == templateName)
                                    .Render(Relations, listItem, entity, currentAddress);
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