using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FSS.Omnius.Modules.Entitron.Entity;

namespace Newtonsoft.Json
{
    public class ImportExportIgnoreAttribute : Attribute
    {
        public bool IsKey { get; set; }
        public bool IsLink { get; set; }
        public bool IsLinkKey { get; set; }
        public bool IsParent { get; set; }
        public bool IsParentKey { get; set; }

        public ImportExportIgnoreAttribute(bool IsLink = false, bool IsKey = false, bool IsLinkKey = false)
        {
            this.IsLink = IsLink;
            this.IsKey = IsKey;
            this.IsLinkKey = IsLinkKey;
        }
    }

    public class LinkToEntityAttribute : Attribute
    {
        public Type EntityType { get; set; }
        public bool MultipleItems { get; set; }

        private string _propertyName;
        private Type[] _types;

        public LinkToEntityAttribute(Type EntityType, bool MultipleItems = false)
        {
            this.EntityType = EntityType;
            this.MultipleItems = MultipleItems;
        }
        public LinkToEntityAttribute(string propertyName, params Type[] types)
        {
            EntityType = null;
            MultipleItems = false;

            _propertyName = propertyName;
            _types = types;
        }

        public Type GetType(IEntity entity)
        {
            if (EntityType != null)
                return EntityType;

            var property = entity.GetType().GetProperties().First(p => p.Name == _propertyName);
            int index = (int)property.GetValue(entity);

            return _types[index];
        }
    }
}
