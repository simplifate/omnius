using FSS.Omnius.Modules.Entitron.Entity.CORE;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;

namespace FSS.Omnius.Modules.Entitron.Entity
{
    public interface IEntity
    {
    }

    public static class EntityExtension
    {
        public static JObject ToJson<T>(this T thisEntity) where T : IEntity
        {
            JObject result = new JObject();

            Type t = thisEntity.GetType();
            foreach (PropertyInfo property in t.GetProperties())
            {
                if (result[property.Name] == null)
                {
                    if (DataType.BaseTypes.Contains(property.PropertyType))
                    {
                        result.Add(property.Name, new JValue(property.GetValue(thisEntity)));
                    }
                    else if (property.PropertyType.IsEnum)
                    {
                        result.Add(property.Name, new JValue((int)property.GetValue(thisEntity)));
                    }
                }
            }

            return result;
        }

        public static void FromJson<T>(this T thisEntity, JToken json, params string[] ignoreParams) where T : IEntity
        {
            Type t = thisEntity.GetType();
            foreach (PropertyInfo property in t.GetProperties())
            {
                try
                {
                    if (DataType.BaseTypes.Contains(property.PropertyType) && !ignoreParams.Contains(property.Name) && property.CanWrite && json[property.Name] != null)
                    {
                        object value = (json[property.Name] as JValue).Value;
                        if (value != null && value.GetType() == typeof(Int64))
                            value = Convert.ToInt32(value);

                        property.SetValue(thisEntity, value);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static void CopyPropertiesFrom<T>(this T thisEntity, T from, IEnumerable<string> skip = null) where T : IEntity
        {
            Type t = thisEntity.GetType();
            foreach(PropertyInfo property in t.GetProperties())
            {
                if ((DataType.BaseTypes.Contains(property.PropertyType) || property.PropertyType.IsEnum) && (skip == null || !skip.Contains(property.Name)))
                    property.SetValue(thisEntity, property.GetValue(from));
            }
        }

        public static int GetId(this IEntity thisEntity)
        {
            return (int)thisEntity.GetType().GetProperties().FirstOrDefault(p => p.Name == "Id").GetValue(thisEntity);
        }

        //public static void Update<T>(this T thisEntity, T newEntity) where T : IEntity
        //{
        //    DoublePropertyActions pActions = new DoublePropertyActions();
        //    pActions.AddAction(
        //        (prop, thisVal, newVal) => prop.CanWrite && prop.Name != "Id" && (DataType.BaseTypes.Contains(prop.PropertyType) || thisVal == null),
        //        (prop, thisE, newE, thisVal, newVal) => prop.SetValue(thisE, newVal)
        //    );
        //    pActions.Run(thisEntity, newEntity, 0, false);
        //}
        //public static void UpdateDeep<T>(this T thisEntity, T newEntity, DBEntities context, HashSet<Type> basicsTypes = null) where T : IEntity
        //{
        //    // INIT
        //    if (basicsTypes == null)
        //        basicsTypes = DataType.BaseTypes;

        //    DoublePropertyActions pActions = new DoublePropertyActions
        //    {
        //        GlobalCondition = (prop, thisVal, newVal) => prop.GetCustomAttribute<ImportExportIgnoreAttribute>() == null || prop.GetCustomAttribute<ImportExportIgnoreAttribute>().IsLinkKey,
        //        RecurseCondition = (prop, thisVal, newVal) => prop.GetCustomAttribute<ImportExportIgnoreAttribute>() == null,
        //        addAndRemove = true,
        //        removeFunc = (list, item) => context.Entry(item).State = EntityState.Deleted,
        //    };
        //    // basics types || new inner link
        //    pActions.AddAction(
        //        (prop, thisVal, newVal) => basicsTypes.Contains(prop.PropertyType) || thisVal == null,
        //        (prop, thisE, newE, thisVal, newVal) => prop.SetValue(thisE, newVal)
        //    );

        //    pActions.Run(thisEntity, newEntity, 0, true);
        //}
        
        //public static Dictionary<Tuple<Type, int>, IEntity> MapIds<T>(this T thisEntity, T newEntity) where T : IEntity
        //{
        //    Dictionary<Tuple<Type, int>, IEntity> idMapping = new Dictionary<Tuple<Type, int>, IEntity>();

        //    DoublePropertyActions pActions = new DoublePropertyActions
        //    {
        //        RecurseCondition = (prop, thisVal, newVal) => prop.GetCustomAttribute<ImportExportIgnoreAttribute>() == null,
        //        addNew = true,
        //        removeOld = false
        //    };
        //    pActions.newItemFunc = (newItem, deep) => pActions.Run(newItem, newItem, deep + 1, true);
        //    // map Id
        //    pActions.AddAction(
        //        (prop, thisVal, newVal) => { var attr = prop.GetCustomAttribute<ImportExportIgnoreAttribute>(); return attr != null && attr.IsKey; },
        //        (prop, thisE, newE, thisVal, newVal) => idMapping[new Tuple<Type, int>(newE.GetType(), (int)newVal)] = thisE
        //    );

        //    pActions.Run(thisEntity, newEntity, 0, true);
        //    return idMapping;
        //}

        //public static void UpdateEntityLinks<T>(this T thisEntity, Dictionary<Tuple<Type, int>, IEntity> idMapping) where T : IEntity
        //{
        //    PropertyActions pActions = new PropertyActions
        //    {
        //        RecurseCondition = (prop, val) => prop.GetCustomAttribute<ImportExportIgnoreAttribute>() == null
        //    };
        //    // links
        //    pActions.AddAction(
        //        (prop, value) => { var attr = prop.GetCustomAttribute<ImportExportIgnoreAttribute>(); return attr != null && attr.IsLink; },
        //        (prop, entity, value) =>
        //        {
        //            // has Id
        //            PropertyInfo idProperty = entity.GetType().GetProperties().FirstOrDefault(p => p.Name == $"{prop.Name}Id" || p.Name == $"{prop.Name}_Id");
        //            if (idProperty != null)
        //            {
        //                int? id = (int?)idProperty.GetValue(entity);
        //                if (id != null)
        //                {
        //                    Tuple<Type, int> key = new Tuple<Type, int>(prop.PropertyType, id.Value);
        //                    if (idMapping.ContainsKey(key))
        //                    {
        //                        IEntity targetE = idMapping[key];
        //                        idProperty.SetValue(entity, targetE.GetId());
        //                        prop.SetValue(entity, targetE);
        //                    }
        //                    else if (idProperty.PropertyType.IsGenericType && idProperty.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
        //                    {
        //                        // TODO - warning
        //                    }
        //                    else
        //                    {
        //                        // TODO - problém
        //                    }
        //                }
        //            }
        //        }
        //    );

        //    pActions.Run(thisEntity, true);
        //}

        //public static void UpdateEntityIdLinks<T>(this T thisEntity, Dictionary<Tuple<Type, int>, IEntity> idMapping, Func<ImportExportIgnoreAttribute, bool> attributeCondition = null) where T : IEntity
        //{
        //    PropertyActions pActions = new PropertyActions
        //    {
        //        RecurseCondition = (prop, val) => prop.GetCustomAttribute<ImportExportIgnoreAttribute>() == null,
        //        GlobalCondition = (prop, val) => prop.GetCustomAttribute<ImportExportIgnoreAttribute>() == null && prop.GetCustomAttribute<LinkToEntityAttribute>() != null
        //    };
        //    // attribute LinkToEntity - multiple items - string
        //    pActions.AddAction(
        //        (prop, val) => prop.GetCustomAttribute<LinkToEntityAttribute>().MultipleItems,
        //        (prop, entity, value) =>
        //        {
        //            string ids = (string)prop.GetValue(entity);
        //            // id is filled
        //            if (!string.IsNullOrWhiteSpace(ids))
        //            {
        //                List<int> newIds = new List<int>();
        //                foreach (int id in ids.Split(',').Select(i => Convert.ToInt32(i)))
        //                {
        //                    Tuple<Type, int> key = new Tuple<Type, int>(prop.GetCustomAttribute<LinkToEntityAttribute>().GetType(entity), id);
        //                    // link to non-deleted row
        //                    if (idMapping.ContainsKey(key))
        //                    {
        //                        IEntity targetE = idMapping[key];
        //                        newIds.Add(targetE.GetId());
        //                    }
        //                    else
        //                    {
        //                        // TODO: warning
        //                    }
        //                }
        //                string newIdsString = string.Join(",", newIds);
        //                prop.SetValue(entity, newIdsString);
        //            }
        //        }
        //    );
        //    // attribute LinkToEntity - single target - int
        //    pActions.AddAction(
        //        (prop, val) => !prop.GetCustomAttribute<LinkToEntityAttribute>().MultipleItems,
        //        (prop, entity, value) =>
        //        {
        //            int? id = (int?)prop.GetValue(entity);
        //            // id is filled
        //            if (id != null)
        //            {
        //                Tuple<Type, int> key = new Tuple<Type, int>(prop.GetCustomAttribute<LinkToEntityAttribute>().GetType(entity), id.Value);
        //                // link to non-deleted row
        //                if (idMapping.ContainsKey(key))
        //                {
        //                    IEntity targetE = idMapping[key];
        //                    prop.SetValue(entity, targetE.GetId());
        //                }
        //                else
        //                {
        //                    // TODO: warning
        //                }
        //            }
        //        }
        //    );

        //    pActions.Run(thisEntity, true);
        //}
    }
}
