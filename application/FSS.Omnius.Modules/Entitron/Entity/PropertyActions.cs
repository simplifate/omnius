using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Entity
{
    public class PropertyActions
    {
        private HashSet<Tuple<Func<PropertyInfo, object, bool>, Action<PropertyInfo, IEntity, object>>> _actions;

        public Func<PropertyInfo, object, bool> GlobalCondition { get; set; }
        public Func<PropertyInfo, object, bool> RecurseCondition { get; set; }

        public PropertyActions()
        {
            _actions = new HashSet<Tuple<Func<PropertyInfo, object, bool>, Action<PropertyInfo, IEntity, object>>>();
            GlobalCondition = (prop, val) => true;
            RecurseCondition = (prop, val) => true;
        }

        public void AddAction(Func<PropertyInfo, object, bool> condition, Action<PropertyInfo, IEntity, object> action)
        {
            _actions.Add(new Tuple<Func<PropertyInfo, object, bool>, Action<PropertyInfo, IEntity, object>>(condition, action));
        }

        public void Run(IEntity entity, bool recurse = true)
        {
            if (entity == null)
                return;

            PropertyInfo[] properties = entity.GetType().GetProperties();
            foreach(PropertyInfo property in properties)
            {
                object value = property.GetValue(entity);
                // action
                foreach (var action in _actions)
                {
                    // if condition
                    if (GlobalCondition(property, value) && action.Item1(property, value))
                        action.Item2(property, entity, value);
                }

                // recurse
                if (recurse && RecurseCondition(property, value))
                {
                    // IEntity
                    if (property.PropertyType.GetInterfaces().Contains(typeof(IEntity)))
                    {
                        Run((IEntity)value, recurse);
                    }
                    // collection
                    else if (property.PropertyType.GenericTypeArguments.Any() && property.PropertyType.GenericTypeArguments.First().GetInterfaces().Contains(typeof(IEntity)))
                    {
                        foreach(IEntity item in (IEnumerable<dynamic>)property.GetValue(entity))
                        {
                            Run(item, recurse);
                        }
                    }
                }
            }
        }
    }

    public class DoublePropertyActions
    {
        //                           property, originVal, newVal,              name, originEntity, newEntity, originVal, newValue, setOrigin, setNew
        private HashSet<Tuple<Func<PropertyInfo, object, object, bool>, Action<PropertyInfo, IEntity, IEntity, object, object>>> _actions;

        public bool addAndRemove { set { addNew = value; removeOld = value; } }
        public bool addNew { get; set; }
        public Action<dynamic, dynamic> addFunc { get; set; }
        public bool removeOld { get; set; }
        public Action<dynamic, dynamic> removeFunc { get; set; }
        public Action<IEntity, int> newItemFunc { get; set; }
        public Action<IEntity> oldItemFunc { get; set; }
        public Func<PropertyInfo, object, object, bool> GlobalCondition { get; set; }
        public Func<PropertyInfo, object, object, bool> RecurseCondition { get; set; }


        public DoublePropertyActions()
        {
            _actions = new HashSet<Tuple<Func<PropertyInfo, object, object, bool>, Action<PropertyInfo, IEntity, IEntity, object, object>>>();
            addNew = true;
            addFunc = (list, item) => list.Add(item);
            removeOld = true;
            removeFunc = (list, item) => list.Remove(item);

            newItemFunc = (newItem, deep) => { };
            oldItemFunc = (originItem) => { };

            GlobalCondition = (prop, originVal, newVal) => true;
            RecurseCondition = (prop, originVal, newVal) => true;
        }

        public void AddAction(Func<PropertyInfo, object, object, bool> condition, Action<PropertyInfo, IEntity, IEntity, object, object> action)
        {
            _actions.Add(new Tuple<Func<PropertyInfo, object, object, bool>, Action<PropertyInfo, IEntity, IEntity, object, object>>(condition, action));
        }

        public void Run(IEntity originEntity, IEntity newEntity, int deep, bool recurse = false)
        {
            if (originEntity == null || newEntity == null)
                return;

            PropertyInfo[] properties = newEntity.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                try
                {
                    object originValue = property.GetValue(originEntity);
                    object newValue = property.GetValue(newEntity);

                    // action
                    foreach (var action in _actions)
                    {
                        // if condition
                        if (GlobalCondition(property, originValue, newValue) && action.Item1(property, originValue, newValue))
                            // run action
                            action.Item2(property, originEntity, newEntity, originValue, newValue);
                    }

                    // recurse
                    if (recurse && RecurseCondition(property, originValue, newValue))
                    {
                        // IEntity
                        if (property.PropertyType.GetInterfaces().Contains(typeof(IEntity)))
                            Run((IEntity)originValue, (IEntity)newValue, deep + 1, recurse);

                        // collection
                        else if (property.PropertyType.GenericTypeArguments.Any() && property.PropertyType.GenericTypeArguments.First().GetInterfaces().Contains(typeof(IEntity)))
                        {
                            IEnumerator<IEntity> originEnumerator = (originValue as dynamic).GetEnumerator();
                            IEnumerator<IEntity> newEnumerator = (newValue as dynamic).GetEnumerator();

                            originEnumerator.Reset();
                            newEnumerator.Reset();
                            HashSet<dynamic> toAdd = new HashSet<dynamic>();
                            while (newEnumerator.MoveNext())
                            {
                                // recurse
                                if (originEnumerator.MoveNext())
                                    Run(originEnumerator.Current, newEnumerator.Current, deep + 1, recurse);
                                // add new
                                else
                                {
                                    do
                                    {
                                        if (addNew)
                                            toAdd.Add((dynamic)newEnumerator.Current);

                                        newItemFunc(newEnumerator.Current, deep);
                                    }
                                    while (newEnumerator.MoveNext());
                                }
                            }
                            // remove old
                            HashSet<dynamic> toRemove = new HashSet<dynamic>();
                            while (originEnumerator.MoveNext())
                            {
                                if (removeOld)
                                    toRemove.Add((dynamic)originEnumerator.Current);

                                oldItemFunc(originEnumerator.Current);
                            }

                            // update list
                            if (addNew)
                                foreach (dynamic item in toAdd)
                                {
                                    addFunc(originValue, item);
                                }
                            if (removeOld)
                                foreach (dynamic item in toRemove)
                                {
                                    removeFunc(originValue, item);
                                }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Exception in property [{property.Name}] of object [{originEntity.GetType().Name}({originEntity.GetId()})]", ex);
                }
            }
        }
    }
}
