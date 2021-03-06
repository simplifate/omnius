﻿using System.Collections.Generic;

namespace FSS.Omnius.Modules.Entitron.Entity.Entitron
{
    public class AjaxTransferDbScheme : IEntity
    {
        public string CommitMessage { get; set; }
        public int? SchemeLockedForUserId { get; set; }
        public string SchemeLockedForUserName { get; set; }
        public int? CurrentSchemeCommitId { get; set; }
        public List<AjaxTransferDbTable> Tables { get; set; }
        public List<AjaxTransferDbRelation> Relations { get; set; }
        public List<AjaxTransferDbView> Views { get; set; }

        public object Shared = null;

        public AjaxTransferDbScheme()
        {
            Tables    = new List<AjaxTransferDbTable>();
            Relations = new List<AjaxTransferDbRelation>();
            Views     = new List<AjaxTransferDbView>();
        }
    }
    public class AjaxSchemeLockingStatus
    {
        public int lockStatusId { get; set; }
        public string lockedForUserName { get; set; }
    }
    public class AjaxTransferViewColumnList: IEntity
    {
        public AjaxTransferViewColumnList()
        {
            Columns = new List<string>();
        }
        public AjaxTransferViewColumnList(List<string> columns)
        {
            Columns = columns;
        }

        public List<string> Columns { get; set; }
    }
}
