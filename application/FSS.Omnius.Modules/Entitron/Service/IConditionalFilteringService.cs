using System.Collections.Generic;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;

namespace FSS.Omnius.Modules.Entitron.Service
{
    public interface IConditionalFilteringService
    {
        bool MatchConditionSets(ICollection<TapestryDesignerConditionSet> conditionSets, DBItem entitronRow);
    }
}
