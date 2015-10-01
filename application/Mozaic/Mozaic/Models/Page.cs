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
    public class Page
    {
        #region attributes
        [Required]
        public int Id { get; set; }
        
        [Required]
        [DefaultValue("")]
        public string Relations { get; set; }
        
        [Required]
        public int MasterTemplateId { get; set; }
        public Template MasterTemplate { get; set; }
        #endregion


        public string Render(DBItem Model, DBMozaic entity = null)
        {
            entity = entity ?? new DBMozaic();

            return entity.Templates.FirstOrDefault(t => t.Id == MasterTemplateId)
                .Render(parseKeyValueString(Relations), Model, entity);
        }
        private Dictionary<string, string> parseKeyValueString(string value, char relationSeparator = ';', char keyValueSeparator = '=')
        {
            Dictionary<string, string> output = new Dictionary<string, string>();
            foreach(string pair in value.Split(relationSeparator))
            {
                int index = pair.IndexOf(keyValueSeparator);
                if (index != -1)
                    output.Add(
                        pair.Substring(0, index),
                        pair.Substring(index + 1)
                    );
            }

            return output;
        }
    }
}