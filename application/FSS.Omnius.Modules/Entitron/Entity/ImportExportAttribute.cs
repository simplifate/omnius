using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Entity
{
    /// <summary>
    /// Type
    ///     - PARENT: parent in the entity model tree
    ///     - CHILD: child in the entity model tree, usually collection
    ///     - LINKREQUIRED: link, this object has id, not nullable
    ///     - LINKOPTIONAL: link, this object has id, nullable
    ///     - LINKCHILD: object is linked by another object, usually collection
    /// <para>Branch - part of application that this belongs to; empty - export allways</para>
    /// <para>KeyFor - only for ids; parent should contain 1 item; what type this links to</para>
    /// <para>MultipleIdInString - string, that contains ids comma separated</para>
    /// <para>KeyForMultiple_property - this property is key for multiple object; differenced by int in another column</para>
    /// <para>skipItem - skip this item if it doesn't have this required parameter</para>
    /// <para>skipPair - skip this item only when both params are null/unavailable</para>
    /// <para>exportCount - don't export all this objects, insert to parent-type attribute, requires exportOrder</para>
    /// <para>exportOrderColumn - order by column name, insert to parent-type attribute, requires exportCount</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ImportExportAttribute : Attribute
    {
        public ELinkType Type { get; }
        public string Branch { get; set; }

        public Type[] KeyFor { get; }
        public bool MultipleIdInString { get; set; }
        public string KeyForMultiple_property { get; set; }

        public bool skipItem { get; set; }
        public string[] skipPair { get; set; }

        public int exportCount { get; set; }
        public string exportOrderColumn { get; set; }
        public bool exportOrderDesc { get; set; }

        public ImportExportAttribute(ELinkType Type, params Type[] KeyFor)
        {
            this.Type = Type;
            this.KeyFor = KeyFor;
            exportCount = 0;
            exportOrderDesc = true;
            skipItem = false;
            skipPair = new string[] { };
        }
    }

    public enum ELinkType
    {
        Parent,
        Child,
        LinkRequired,
        LinkOptional,
        LinkChild
    }
}
