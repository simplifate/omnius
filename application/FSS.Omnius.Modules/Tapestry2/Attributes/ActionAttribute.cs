using System;

namespace FSS.Omnius.Modules.Tapestry2
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ActionAttribute : Attribute
    {
        public ActionAttribute(int Id, string Name, params string[] ReturnValues)
        {
            this.Id = Id;
            this.Name = Name;
            this.ReturnValues = ReturnValues;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string[] ReturnValues { get; set; }
    }
}
