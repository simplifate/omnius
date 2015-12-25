using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSS.Omnius.Modules.Entitron.Entity.Nexus
{
    [Table("Nexus_WebDavServers")]
    public class WebDavServer
    {
        public int Id { get; set; }

        [Display(Name = "Název")]
        public string Name { get; set; }

        [Display(Name = "Bázové URL")]
        public string UriBasePath { get; set; }

        [Display(Name = "Anonymní přihlášení?")]
        public bool AnonymousMode { get; set; }

        [Display(Name = "Uživatel")]
        public string AuthUsername { get; set; }

        [Display(Name = "Heslo")]
        public string AuthPassword { get; set; }
    }

    [Table("Nexus_CachedFiles")]
    public class FileSyncCache
    {
        public int Id { get; set; }
        public byte[] Blob { get; set; }

        public virtual FileMetadata FileMetadata { get; set; }
    }

    [Table("Nexus_FileMetadataRecords")]
    public class FileMetadata
    {
        public int Id { get; set; }
        public string Filename { get; set; }
        public string AppFolderName { get; set; }
        public DateTime TimeCreated { get; set; }
        public DateTime TimeChanged { get; set; }
        public int Version { get; set; }

        public virtual WebDavServer WebDavServer { get; set; }
        public virtual FileSyncCache CachedCopy { get; set; }
    }
}
