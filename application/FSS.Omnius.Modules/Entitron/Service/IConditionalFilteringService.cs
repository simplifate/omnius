using System.Collections.Generic;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Entitron.Service
{
    public interface IConditionalFilteringService
    {
        bool MatchConditionSets(ICollection<TapestryDesignerConditionSet> conditionSets, DBItem entitronRow, Dictionary<string, object> tapestryVars);
    }
}
