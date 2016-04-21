using System;
using System.Collections.Generic;
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
        public static void Update<T>(this T thisEntity, T newEntity, Type ignoreAttribute = null) where T : IEntity
        {
            IEnumerable<PropertyInfo> properties = thisEntity.GetType().GetProperties();
            if (ignoreAttribute != null)
                properties = properties.Where(p => p.GetCustomAttribute(ignoreAttribute) == null);

            foreach (var property in properties)
            {
                property.SetValue(thisEntity, property.GetValue(newEntity));
            }
        }

        public static void UpdateBasicsParams<T>(this T thisEntity, T newEntity, HashSet<Type> basicsTypes = null, Type ignoreAttribute = null) where T : IEntity
        {
            if (basicsTypes == null)
                basicsTypes = new HashSet<Type>
                {
                    typeof(int),
                    typeof(int?),
                    typeof(bool),
                    typeof(bool?),
                    typeof(string),
                    typeof(DateTime),
                    typeof(DateTime?)
                };

            IEnumerable<PropertyInfo> properties = thisEntity.GetType().GetProperties();
            if (ignoreAttribute != null)
                properties = properties.Where(p => basicsTypes.Contains(p.PropertyType) && p.GetCustomAttribute(ignoreAttribute) == null);

            foreach (var property in properties)
            {
                property.SetValue(thisEntity, property.GetValue(newEntity));
            }
        }

        public static void UpdateDeep<T>(this T thisEntity, T newEntity, HashSet<Type> basicsTypes = null, Type ignoreAttribute = null) where T : IEntity
        {
            // basics types
            if (basicsTypes == null)
                basicsTypes = new HashSet<Type>
                {
                    typeof(int),
                    typeof(int?),
                    typeof(bool),
                    typeof(bool?),
                    typeof(string),
                    typeof(DateTime),
                    typeof(DateTime?)
                };

            // get mapped properties
            IEnumerable<PropertyInfo> properties = newEntity.GetType().GetProperties();
            if (ignoreAttribute != null)
                properties = properties.Where(p => p.GetCustomAttribute(ignoreAttribute) == null);

            // update
            foreach (var property in properties)
            {
                // basics types
                if (basicsTypes.Contains(property.PropertyType))
                    property.SetValue(thisEntity, property.GetValue(newEntity));
                else
                {
                    object thisValue = property.GetValue(thisEntity);
                    object newValue = property.GetValue(newEntity);
                    // null
                    if (newValue == null)
                    {
                        property.SetValue(thisEntity, null);
                    }
                    // inner IEntity
                    else if (newValue is IEntity)
                    {
                        (thisValue as IEntity).UpdateDeep((IEntity)newValue, basicsTypes, ignoreAttribute);
                    }
                    // ICollection
                    else if (newValue.GetType().GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICollection<>)))
                    {
                        // Type itemType = property.PropertyType.GenericTypeArguments.First(); // type

                        IEnumerator<IEntity> thisEnumerator = (thisValue as dynamic).GetEnumerator();
                        IEnumerator<IEntity> newEnumerator = (newValue as dynamic).GetEnumerator();

                        thisEnumerator.Reset();
                        newEnumerator.Reset();
                        while (newEnumerator.MoveNext())
                        {
                            if (!thisEnumerator.MoveNext())
                            {
                                do
                                {
                                    (thisValue as dynamic).Add((dynamic)newEnumerator.Current);
                                }
                                while (newEnumerator.MoveNext());
                                property.SetValue(thisEntity, thisValue);
                            }
                            else
                                thisEnumerator.Current.UpdateDeep(newEnumerator.Current, basicsTypes, ignoreAttribute);
                        }
                    }
                }
            }
        }
    }
}
