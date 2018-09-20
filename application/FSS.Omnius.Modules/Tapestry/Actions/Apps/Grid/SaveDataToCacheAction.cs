using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Tapestry.Actions.Nexus;
using FSS.Omnius.Modules.Entitron.Entity;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    /// <summary>
    /// Akce prijme cacheKey požadovaných dat a zjistuje, zda data s pozadovaným cacheKey byla jiz v minulosti nacachovana.
    /// Pokud ano, updatuje nova data (vstupni promenna Value) se stejnym cacheKey.
    /// Pokud ne, prida data s novym cacheKey.
    /// K zapisovanym datum je vzdy prirazen akualni UTC cas.
    /// </summary>
    [NexusRepository]
    public class SaveDataToCacheAction : Action
    {

        public override int Id
        {
            get
            {
                return 3013;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "CacheKey", "Value" , "?Id" };
            }
        }

        public override string Name
        {
            get
            {
                return "Save data to cache";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] {};
            }
        }

        public override int? ReverseActionId
        {
            get
            {
                return null;
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            string cacheKey = (vars.ContainsKey("Id")) ? (string)vars["CacheKey"] + "_" + ((int)vars["Id"]).ToString() : (string)vars["CacheKey"];
            Object value = (Object)vars["Value"];

            // Retrieve static dataCache object (not implemented as singleton!)
            Dictionary<string, CachedDataResult> dataCache = ReadCachedDataAction.dataCache;

            if (dataCache.ContainsKey(cacheKey))
            {
                // If data with the appropriate cacheKey were found, update data
                dataCache[cacheKey].resultTime = DateTime.UtcNow;
                dataCache[cacheKey].data = value;
            }
            else
            {
                // Else, add data with the new cacheKey
                dataCache.Add(cacheKey, new CachedDataResult()
                {
                    resultTime = DateTime.UtcNow,
                    data = value
                });
            }
        }
    }
}
