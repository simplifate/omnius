namespace FSS.FSPOC.BussinesObjects.Entities.Actions
{
    public class ActionActionRule
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public short Order { get; set; }
        public ActionRule ActionRule { get; set; }
        public int ActionRuleId { get; set; }

        public Action Action { get; set; }
        public int ActionId { get; set; }
    }
}