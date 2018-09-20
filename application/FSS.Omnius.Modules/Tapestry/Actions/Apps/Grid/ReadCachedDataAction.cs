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
using System.Security.Cryptography;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    public class CachedDataResult
    {
        public DateTime resultTime;
        public Object data;
    }

    /// <summary>
    /// Akce prijme cacheKey požadovaných dat a zjistuje, zda data s pozadovaným cacheKey byla jiz v minulosti nacachovana.
    /// Pokud ano a stari (TimeStamp) nacacheovani dat je mensi nez pozadovane maximalni stari dat, jsou vracena do outputu
    /// nacacheovana data.
    /// V ostatnich pripadech bude vracen boolean IsRecalculationNecessary s hodnotou true a output data s hodnotou null.
    /// Pozdeji v jedne z nasledujicich akci budou pozadovana data znovu nactena z databaze a nacacheovana.
    /// </summary>
    [NexusRepository]
    public class ReadCachedDataAction : Action
    {
        public static Dictionary<string, CachedDataResult> dataCache = new Dictionary<string, CachedDataResult>();

        public override int Id
        {
            get
            {
                return 3011;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "CacheKey", "MaxAge" , "?Id"};
            }
        }

        public override string Name
        {
            get
            {
                return "Read cached data";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[]
                {
                    "IsRecalculationNecessary", "Data"
                };
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
            int maxAge = (int)vars["MaxAge"];

            // Set initial default values to recalculate data
            bool isRecalculationNecessary = true;
            Object dataObject = null;

            if (maxAge > 0)
            {
                // If data with appropriate Key were cached before, retrieve them
                if (dataCache.ContainsKey(cacheKey))
                {
                    CachedDataResult item = dataCache[cacheKey];
                    // Calculate age of data and compare it with required MaxAge
                    double diff = (DateTime.UtcNow - item.resultTime).TotalSeconds;
                    // Use cached data instead of recalculating if all requirements were met
                    if (diff <= maxAge)
                    {
                        isRecalculationNecessary = false;
                        dataObject = item.data;
                    }
                }
            }

            outputVars["IsRecalculationNecessary"] = isRecalculationNecessary;
            outputVars["Data"] = dataObject;
        }
    }
}
