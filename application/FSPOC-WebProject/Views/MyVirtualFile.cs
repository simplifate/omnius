using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;


namespace FSPOC_WebProject.Views
{
    public class MyVirtualFile: VirtualFile
    {
        private byte[] viewContent;

        public MyVirtualFile(string virtualPath, byte[] viewContent) : base(virtualPath)
    {
            this.viewContent = viewContent;
        }

        public override Stream Open()
        {
            return new MemoryStream(viewContent);
        }
    }
}