using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry
{
    internal class KeyValueString
    {
        private Dictionary<string, string> _value;
        private Dictionary<string, object> _result;

        public Dictionary<string, string> value { get { return _value; } }
        public Dictionary<string, object> result { get { return _result; } }

        public KeyValueString(string value)
        {
            _value = SplitKeyValue(value);
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
                object realValue = parseValue(pair.Value, vars);
                _result.Add(pair.Key, realValue);
            }
        }

        public bool CompareResolved(Dictionary<string, object> vars)
        {
            if (_result == null)
                Resolve(vars);

            foreach(var pair in _result)
            {
                object key = parseValue(pair.Key, vars);
                if (!Equals(key, _result[pair.Key]))
                    return false;
            }

            return true;
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
        private object parseValue(string input, Dictionary<string, object> vars)
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
                catch (MissingFieldException e)
                {
                    // TODO: LOG warning
                    return null;
                }
            }
        }

        private static object GetChainedProperty(string chainedKey, Dictionary<string, object> vars)
        {
            int index = chainedKey.IndexOf('.');
            if (index == -1)
            {
                if (vars.ContainsKey(chainedKey))
                    return vars[chainedKey];
                else
                    throw new MissingFieldException($"Missing key '{chainedKey}' on list [{vars.ToString()}].");
            }

            string key = chainedKey.Substring(0, index);
            return GetChainedProperty(vars[key], chainedKey.Substring(index + 1));
        }

        private static object GetChainedProperty(object item, string propertyName)
        {
            foreach (string singleProperty in propertyName.Split('.'))
            {
                if (HasProperty(item, singleProperty))
                    item = item.GetType().GetProperty(singleProperty).GetValue(item);
                else
                    throw new MissingFieldException($"Missing field '{singleProperty}' on item [{item.ToString()}]. ('{propertyName}' on [{item.ToString()}])");
            }
            return item;
        }

        /// <summary>
        /// Has the item property of given name
        /// </summary>
        /// <param name="item"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private static bool HasProperty(object item, string propertyName)
        {
            if (item == null)
                return false;

            return item.GetType().GetProperty(propertyName) != null;
        }
    }
}
