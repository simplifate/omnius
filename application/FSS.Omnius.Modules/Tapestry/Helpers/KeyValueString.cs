using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry
{
    internal class KeyValueString
    {
        private Dictionary<string, string> _value;
        private Dictionary<string, object> _result;
        private bool _not;

        public Dictionary<string, string> value { get { return _value; } }
        public Dictionary<string, object> result { get { return _result; } }

        public KeyValueString(string value)
        {
            _not = (value != null && value.Length > 0)
                ? value[0] == '!'
                : _not = false;
            _value = SplitKeyValue(_not ? value.Substring(1) : value);
            _result = null;
        }
        
        public void Resolve(Dictionary<string, object> vars)
        {
            // prepare _result
            if (_result != null)
                _result.Clear();
            else
                _result = new Dictionary<string, object>();

            // resolve
            foreach(KeyValuePair<string, string> pair in _value)
            {
                object realValue = ParseValue(pair.Value, vars);
                _result.Add(pair.Key, realValue);
            }
        }

        public bool CompareResolved(Dictionary<string, object> vars)
        {
            if (_result == null)
                Resolve(vars);

            foreach(var pair in _result)
            {
                object key = ParseValue(pair.Key, vars);
                if (!Equals(key, _result[pair.Key]))
                    return _not;
            }

            return !_not;
        }

        /// <summary>
        /// change keys of value variable with keyMapping
        /// </summary>
        /// <param name="keyMapping">key mapping</param>
        public void ChangeKeysInValue(string keyMapping)
        {
            ChangeKeysInValue(SplitKeyValue(keyMapping));
        }
        public void ChangeKeysInValue(Dictionary<string, string> keyMapping)
        {
            foreach (KeyValuePair<string, string> mappingPair in keyMapping)
            {
                _value.ChangeKey(mappingPair.Value, mappingPair.Key);
            }
        }
        
        /// <summary>
        /// change keys with this.value in input
        /// </summary>
        /// <param name="input">Dict to change</param>
        public void ChangeKeysInInput(Dictionary<string, object> input)
        {
            foreach (var mappingPair in _value)
            {
                input.ChangeKey(mappingPair.Value, mappingPair.Key);
            }
        }

        /// <summary>
        /// gets string in format key=value;key2=value2
        /// </summary>
        /// <param name="value">string to split</param>
        /// <returns>separated keyValuePairs</returns>
        private Dictionary<string, string> SplitKeyValue(string value)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // empty value
            if (string.IsNullOrWhiteSpace(value))
                return result;

            string[] keyValues = value.Split(';');
            foreach (string keyValue in keyValues)
            {
                string[] separated = keyValue.Split('=');
                result.Add(separated[0], separated[1]);
            }

            return result;
        }

        /// <summary>
        /// take input and return its real value
        /// </summary>
        /// <param name="input">string to resolv</param>
        /// <param name="vars">variables to take values from</param>
        /// <returns>value of input</returns>
        public static object ParseValue(string input, Dictionary<string, object> vars)
        {
            // value
            if (input.Length > 1 && input[1] == '$')
            {
                return Convertor.convert(input[0], input.Substring(2));
            }
            // variable
            else
            {
                try
                {
                    return GetChainedProperty(input, vars);
                }
                // unknown variable
                catch (MissingFieldException)
                {
                    Logger.Log.Warn($"Variable '{input}' not found in vars [{string.Join(", ", vars.Select(v => v.Key))}]");
                    return null;
                }
            }
        }

        #region chained
        private static object GetChainedProperty(string chainedKey, Dictionary<string, object> vars)
        {
            int index = chainedKey.IndexOfAny(new char[] { '.', '[' });
            if (index == -1)
            {
                if (vars.ContainsKey(chainedKey))
                    return vars[chainedKey];
                else
                    throw new MissingFieldException($"Missing key '{chainedKey}' on list [{vars.ToString()}].");
            }

            string key = chainedKey.Substring(0, index);
            if (key == "__Model" || key == "__Result")
                return vars[chainedKey];
            return GetChained(vars[key], chainedKey.Substring(index));
        }
        
        private static object GetChained(object o, string calling)
        {
            string[] propertyCallings = calling.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string propertyCalling in propertyCallings)
            {
                o = resolveProperty(o, propertyCalling);
            }

            return o;
        }

        private static object resolveProperty(object o, string calling)
        {
            int iStart = calling.IndexOf('[');

            // no array
            if (iStart < 0)
                return resolveSingleProperty(o, calling);

            // array with property
            if (iStart > 0)
            {
                o = resolveSingleProperty(o, calling.Substring(0, iStart));
                calling = calling.Substring(iStart);
                iStart = 0;
            }

            int iEnd = calling.IndexOf(']', iStart);
            // single array
            if (iEnd == calling.Length - 1)
                return resolveSingleArray(o, calling.Substring(1, calling.Length - 2));

            // multiple arrays
            while (iStart >= 0)
            {
                o = resolveSingleArray(o, calling.Substring(iStart + 1, iEnd - iStart - 1));
                iStart = calling.IndexOf('[', iEnd);
                iEnd = iStart >= 0 ? calling.IndexOf(']', iStart) : -1;
            }

            return o;
        }

        private static object resolveSingleArray(object o, string calling)
        {
            var prop = o.GetType().GetMethod("get_Item", new Type[] { typeof(string) });
            return prop.Invoke(o, new object[] { calling });
        }

        private static object resolveSingleProperty(object o, string calling)
        {
            if (o is DBItem && ((DBItem)o).getColumnNames().Contains(calling))
                return ((DBItem)o)[calling];

            PropertyInfo property = o.GetType().GetProperty(calling);

            if (property == null)
                throw new MissingFieldException($"Missing field '{calling}' on item [{o.ToString()}].");

            return property.GetValue(o);
        }
        #endregion
    }
}
