using System.Collections.Generic;

namespace FSPOC_WebProject.Models.DbDesigner
{
    namespace FSPOC.Models
    {
        public class AjaxTransferDbScheme
        {
            public string CommitMessage { get; set; }
            public List<AjaxTransferDbTable> Tables { get; set; }
            public List<AjaxTransferDbRelation> Relations { get; set; }
            public List<AjaxTransferDbView> Views { get; set; }

            public AjaxTransferDbScheme()
            {
                Tables    = new List<AjaxTransferDbTable>();
                Relations = new List<AjaxTransferDbRelation>();
                Views     = new List<AjaxTransferDbView>();
            }
        }
    }
}
