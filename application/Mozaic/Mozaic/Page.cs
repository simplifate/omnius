using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entitron;
using Entitron.Entity;

namespace Mozaic
{
    public class MozaicPage
    {
        private Page _page;

        public MozaicPage(Page page)
        {
            _page = page;
        }

        public string Render(DBItem Model, DBEntities entity)
        {
            return new MozaicTemplate(entity.Templates.FirstOrDefault(t => t.Id == _page.MasterTemplateId))
                .Render(_page, parseKeyValueString(_page.Relations), Model, entity);
        }
        private Dictionary<string, string> parseKeyValueString(string value, char relationSeparator = ';', char keyValueSeparator = '=')
        {
            Dictionary<string, string> output = new Dictionary<string, string>();
            foreach (string pair in value.Split(relationSeparator))
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
