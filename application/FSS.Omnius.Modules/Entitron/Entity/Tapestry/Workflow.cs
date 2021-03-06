﻿namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    using Master;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Hermes;
    [Table("Tapestry_WorkFlow")]
    public class WorkFlow : IEntity
    {
        public WorkFlow()
        {
            Blocks = new HashSet<Block>();
            Children = new HashSet<WorkFlow>();
        }

        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        [Index("Unique_workflowNameApp", 2, IsUnique = true)]
        public string Name { get; set; }
        public int? InitBlockId { get; set; }
        public int? ParentId { get; set; }
        [Index("Unique_workflowNameApp", 1, IsUnique = true)]
        public int ApplicationId { get; set; }
        public int TypeId { get; set; }
        public bool IsInMenu { get; set; }
        [Index("Unique_workflowNameApp", 3, IsUnique = true)]
        public bool IsTemp { get; set; }
        public virtual Application Application { get; set; }
        public virtual ICollection<Block> Blocks { get; set; }
        public virtual Block InitBlock { get; set; }
        public virtual WorkFlowType Type { get; set; }
        public virtual ICollection<WorkFlow> Children { get; set; }
        public virtual WorkFlow Parent { get; set; }
    }
}
