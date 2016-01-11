using FSS.Omnius.Modules.Tapestry;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    [Table("Tapestry_Actions")]
    public partial class Action
    {
        public Action()
        {
            Slaves = new HashSet<Action>();
            ActionRule_Actions = new HashSet<ActionRule_Action>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int? MasterId { get; set; }

        public virtual Action Master { get; set; }
        public virtual ICollection<Action> Slaves { get; set; }

        public virtual ICollection<ActionRule_Action> ActionRule_Actions { get; set; }
        
        public virtual ActionResultCollection run(Dictionary<string, object> vars)
        {
            // Action
            try
            {
                return Modules.Tapestry.Action.RunAction(Id, vars);
            }
            // Action Seq
            catch (KeyNotFoundException)
            {
                ActionResultCollection result = new ActionResultCollection();
                foreach (Action action in Slaves)
                {
                    result.Join = action.run(vars);
                }

                return result;
            }
        }
    }
}
