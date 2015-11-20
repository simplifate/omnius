using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSS.Omnius.Entitron.Entity.Entitron
{
    [Table("Entitron_AjaxTransferDbIndex")]
    public class AjaxTransferDbIndex
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Unique { get; set; }
        public List<string> ColumnNames { get; set; }

        public AjaxTransferDbIndex()
        {
            ColumnNames = new List<string>();
        }
    }
}