using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;

namespace FSS.Omnius.Modules.Tapestry2.Actions.App
{
    public partial class Grid : ActionManager
    {
        public class CachedDataResult
        {
            public DateTime resultTime;
            public Object data;
        }
        public static Dictionary<string, CachedDataResult> dataCache = new Dictionary<string, CachedDataResult>();

        [Action(3011, "Read cached data", "IsRecalculationNecessary", "Data")]
        public static (bool, object) ReadCachedData(COREobject core, string CacheKey, int MaxAge, int? Id = null)
        {
            string cacheKey = Id != null ? $"{CacheKey}_{Id.ToString()}" : CacheKey;

            // Set initial default values to recalculate data
            bool isRecalculationNecessary = true;
            object dataObject = null;

            if (MaxAge > 0)
            {
                // If data with appropriate Key were cached before, retrieve them
                if (dataCache.ContainsKey(cacheKey))
                {
                    CachedDataResult item = dataCache[cacheKey];
                    // Calculate age of data and compare it with required MaxAge
                    double diff = (DateTime.UtcNow - item.resultTime).TotalSeconds;
                    // Use cached data instead of recalculating if all requirements were met
                    if (diff <= MaxAge)
                    {
                        isRecalculationNecessary = false;
                        dataObject = item.data;
                    }
                }
            }

            return (isRecalculationNecessary, dataObject);
        }

        [Action(3013, "Save data to cache")]
        public static void SaveDataToCache(COREobject core, string CacheKey, object Value, int? Id = null)
        {
            string cacheKey = Id != null ? $"{CacheKey}_{Id.ToString()}" : CacheKey;
            
            if (dataCache.ContainsKey(cacheKey))
            {
                // If data with the appropriate cacheKey were found, update data
                dataCache[cacheKey].resultTime = DateTime.UtcNow;
                dataCache[cacheKey].data = Value;
            }
            else
            {
                // Else, add data with the new cacheKey
                dataCache.Add(cacheKey, new CachedDataResult()
                {
                    resultTime = DateTime.UtcNow,
                    data = Value
                });
            }
        }
    }
}
