using System;

namespace FSS.Omnius.Modules.Tapestry2
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class)]
    public class BlockAttribute : Attribute
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string ModelTableName { get; set; }
        public int BootstrapPageId { get; set; }
        public int MozaicPageId { get; set; }
    }
}
