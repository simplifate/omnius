using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.Tapestry;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;

namespace FSS.Omnius.Modules.Entitron.Entity.Persona
{
    [Table("Persona_ActionRuleRights")]
    public partial class ActionRuleRight
    {
        [Key]
        [Column(Order = 1)]
        public int GroupId { get; set; }
        [Key]
        [Column(Order = 2)]
        public int ActionRuleId { get; set; }

        public bool Readable { get; set; }
        public bool Executable { get; set; }

        public virtual Group Group { get; set; }
        public virtual ActionRule ActionRule { get; set; }

        public static string GetSpecificRights(string groupName, string actionName, DBEntities e)
        {
            ActionRuleRight actionrulerights = e.ActionRuleRights
                .SingleOrDefault(arr => arr.Group.Name == groupName && arr.ActionRule.Name == actionName);

            return GetShort(actionrulerights);
        }
        public static string GetSpecificRights(Group group, ActionRule actionR, DBEntities e)
        {
            ActionRuleRight actionrulerights = e.ActionRuleRights
                .SingleOrDefault(arr => arr.Group.Id == group.Id && arr.ActionRule.Id == actionR.Id);

            return GetShort(actionrulerights);
        }

        public static string GetShort(ActionRuleRight arr)
        {
            if (arr == null)
                return "None";

            if (arr.Executable)
                return "E";
            if (arr.Readable)
                return "R";

            return "None";
        }
    }
}
