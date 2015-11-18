using System;
using System.Collections.Generic;
using Entitron.Entity;

namespace FSS.FSPOC.Entitron.Entity.Tapestry
{
    public class Workflow
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastChangeTime { get; set; }
        public virtual ICollection<WorkflowCommit> WorkflowCommits { get; set; }

        public Workflow()
        {
            WorkflowCommits = new List<WorkflowCommit>();
        }

        public int InitBlockId { get; set; }//atribut předán ze třídy workflow v namespacu Entitron.Entity
        public int? ParentId { get; set; }//atribut předán ze třídy workflow v namespacu Entitron.Entity
        public int ApplicationId { get; set; }//atribut předán ze třídy workflow v namespacu Entitron.Entity
        public int TypeId { get; set; }//atribut předán ze třídy workflow v namespacu Entitron.Entity
        public virtual Application Application { get; set; }//atribut předán ze třídy workflow v namespacu Entitron.Entity
        public virtual ICollection<Block> Blocks { get; set; }//atribut předán ze třídy workflow v namespacu Entitron.Entity
        public virtual Block InitBlock { get; set; }//atribut předán ze třídy workflow v namespacu Entitron.Entity
        public virtual WorkFlowType Type { get; set; }//atribut předán ze třídy workflow v namespacu Entitron.Entity
        public virtual ICollection<Workflow> Children { get; set; }//atribut předán ze třídy workflow v namespacu Entitron.Entity
        public virtual Workflow Parent { get; set; }//atribut předán ze třídy workflow v namespacu Entitron.Entity
    }
}
