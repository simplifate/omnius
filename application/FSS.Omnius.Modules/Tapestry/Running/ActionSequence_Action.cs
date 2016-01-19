using FSS.Omnius.Modules.Tapestry;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    public partial class ActionSequence_Action
    {
        public static List<ActionBase> GetActions(int MasterId, DBEntities context)
        {
            List<ActionBase> result = new List<ActionBase>();

            foreach(ActionSequence_Action asa in context.ActionSequence_Actions.Where(asa => asa.Id == MasterId).OrderBy(asa => asa.Order))
            {
                result.Add(Action.All[asa.ChildId]);
            }

            return result;
        }
    }
}
