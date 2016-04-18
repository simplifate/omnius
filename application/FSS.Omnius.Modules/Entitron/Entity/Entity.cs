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
        public static void Update<T>(this T thisEntity, T newEntity) where T : IEntity
        {
            var properties = thisEntity.GetType().GetProperties()
                .Where(p => p.GetCustomAttribute(typeof(NotMappedAttribute)) == null && p.GetSetMethod() != null);

            foreach (var property in properties)
            {
                var value = newEntity.GetType().GetProperty(property.Name).GetValue(newEntity);
                property.SetValue(thisEntity, value);
            }
        }

        public static void UpdateBasicsParams<T>(this T thisEntity, T newEntity, HashSet<Type> basicsTypes = null) where T : IEntity
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

            var properties = thisEntity.GetType().GetProperties()
                .Where(p => basicsTypes.Contains(p.PropertyType) && p.GetCustomAttribute(typeof(NotMappedAttribute)) == null);

            foreach (var property in properties)
            {
                property.SetValue(thisEntity, property.GetValue(newEntity));
            }
        }

        //public static void UpdateDeep<T>(this T thisEntity, T newEntity, HashSet<Type> basicsTypes = null) where T : IEntity
        //{
        //    // basics types
        //    if (basicsTypes == null)
        //        basicsTypes = new HashSet<Type>
        //        {
        //            typeof(int),
        //            typeof(int?),
        //            typeof(bool),
        //            typeof(bool?),
        //            typeof(string),
        //            typeof(DateTime),
        //            typeof(DateTime?)
        //        };

        //    // get mapped properties
        //    var properties = thisEntity.GetType().GetProperties()
        //        .Where(p => p.GetCustomAttribute(typeof(NotMappedAttribute)) == null);

        //    // update
        //    foreach (var property in properties)
        //    {
        //        // basics types
        //        if (basicsTypes.Contains(property.PropertyType))
        //            property.SetValue(thisEntity, property.GetValue(newEntity));
        //        else
        //        {
        //            object thisValue = property.GetValue(thisEntity);
        //            object newValue = newEntity.GetType().GetProperty(property.Name).GetValue(newEntity);
        //            if (newValue == null)
        //            {
        //            }
        //            // inner IEntity
        //            else if (thisValue is IEntity)
        //            {
        //                (thisValue as IEntity).UpdateDeep((IEntity)newValue);
        //            }
        //            // Collection
        //            else if (thisValue is ICollection)//  property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition().FullName == typeof(HashSet<>).FullName)
        //            {
        //                property.SetValue(thisEntity, newValue);
        //                //// property.PropertyType.GenericTypeArguments.First(); // type

        //                //IEnumerator<IEntity> thisEnum = (thisValue as HashSet<IEntity>).GetEnumerator();
        //                //IEnumerator<IEntity> newEnum = (newValue as HashSet<IEntity>).GetEnumerator();

        //                //thisEnum.Reset();
        //                //newEnum.Reset();
        //                //while (newEnum.MoveNext())
        //                //{
        //                //    if (!thisEnum.MoveNext())
        //                //    {
        //                //        do
        //                //        {
        //                //            (thisValue as HashSet<IEntity>).Add(newEnum.Current);
        //                //        }
        //                //        while (newEnum.MoveNext());
        //                //        property.SetValue(thisEntity, thisValue);
        //                //    }
        //                //    else
        //                //        thisEnum.Current.UpdateDeep(newEnum.Current, basicsTypes);
        //                //}
        //            }
        //        }
        //    }
        //}
    }
}
