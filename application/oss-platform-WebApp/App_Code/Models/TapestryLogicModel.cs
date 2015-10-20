using System.Collections.Generic;

namespace FSPOC.Models.Tapestry.Logic
{
    public enum Operators
    {
        Equal,
        NotEqual,
        Greater,
        GreaterOrEqual,
        Lesser,
        LesserOrEqual
    }
    public enum Relations
    {
        AND,
        OR,
        XOR
    }
    public class ConditionSet
    {
        public int Id { get; set; }
        public virtual ICollection<Condition> Conditions { get; set; }

        public Item AssociatedItem { get; set; }

        public ConditionSet()
        {
            Conditions = new List<Condition>();
        }
    }
    public class Condition
    {
        public int Id { get; set; }
        public int Relation { get; set; }
        public Item SourceItem { get; set; }
        public string SourceProperty { get; set; }
        public int Operator { get; set; }
        public string Constant { get; set; }
    }
}
