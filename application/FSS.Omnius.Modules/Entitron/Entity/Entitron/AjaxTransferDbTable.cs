using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSS.Omnius.Entitron.Entity.Entitron
{
    [Table("Entitron_AjaxTransferDbTable")]
    public class AjaxTransferDbTable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public List<AjaxTransferDbColumn> Columns { get; set; }
        public List<AjaxTransferDbIndex> Indices { get; set; }

        public AjaxTransferDbTable()
        {
            Columns = new List<AjaxTransferDbColumn>();
            Indices = new List<AjaxTransferDbIndex>();
        }
    }
}