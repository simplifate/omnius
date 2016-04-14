using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace FSS.Omnius.Modules.Entitron.Entity
{
    public interface IEntity
    {
    }

    public static class EntityExtension
    {
        public static void Update<T>(this T thisEntity, T newEntity) where T : IEntity
        {
            var properties = thisEntity.GetType().GetProperties().Where(p => p.GetCustomAttribute(typeof(NotMappedAttribute)) == null);
            foreach (var property in properties)
            {
                property.SetValue(thisEntity, property.GetValue(newEntity));
            }
        }
    }
}
