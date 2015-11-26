using FSS.Omnius.Modules.Tapestry.Actions;
using System;
using System.Collections.Generic;

namespace FSS.Omnius.Modules.Tapestry
{
    public abstract class Action
    {
        private static Type[] _repositories;
        private static Dictionary<int, Type> _actions;
        private static Dictionary<int, string> _actionNames;
        public static Type[] Repositories
        {
            get
            {
                if (_repositories == null)
                    INIT();

                return _repositories;
            }
        }
        public static Dictionary<int, Type> All
        {
            get
            {
                if (_actions == null)
                    INIT();

                return _actions;
            }
        }
        public static Dictionary<int, string> AllNames
        {
            get
            {
                if (_actionNames == null)
                {
                    _actionNames = new Dictionary<int, string>();
                    foreach (var pair in All)
                    {
                        _actionNames.Add(pair.Key, (string)pair.Value.GetProperty("Name").GetValue(null, null));
                    }
                }

                return _actionNames;
            }
        }
        public static Type GetAction(int id)
        {
            if (_actions == null)
                INIT();

            return _actions[id];
        }
        public static ActionResult RunAction(int id, Dictionary<string, object> vars)
        {
            Type actionType = GetAction(id);
            Action action = (Action)Activator.CreateInstance(actionType);

            return action.run(vars);
        }
        public static void INIT()
        {
            _repositories = typeof(ActionRepositoryAttribute).GetNestedTypes();
            _actions = new Dictionary<int, Type>();
            foreach (Type type in typeof(Action).GetNestedTypes())
            {
                int id = (int)type.GetProperty("Id").GetValue(null, null);
                _actions.Add(id, type);
            }
        }

        public abstract int Id { get; }
        public abstract string Name { get; }
        public abstract string[] InputVar { get; }
        public abstract string[] OutputVar { get; }
        
        public abstract ActionResult run(Dictionary<string, object> vars);
    }
}
