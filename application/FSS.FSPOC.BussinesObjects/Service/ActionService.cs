using System;
using System.Collections.Generic;

namespace FSS.Omnius.BussinesObjects.Service
{
    public class ActionService : IActionService
    {
        private Dictionary<Type, object> ParamDictionary { get; } = new Dictionary<Type, object>();

        public T GetParam<T>() where T : class
        {
            if (ParamDictionary.ContainsKey(typeof(T)))
            {
                return (T)ParamDictionary[typeof(T)];
            }
            return null;
        }

        public void AddParam<T>(T param) where T : class
        {
            if (ParamDictionary.ContainsKey(typeof(T)))
            {
                throw new ArgumentException("Param already exist");
            }
            ParamDictionary.Add(typeof(T), param);

        }
    }
}