namespace Mozaic.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DBMozaic : DbContext
    {
        public DBMozaic()
            : base("name=DBMozaic")
        {
        }

        public virtual DbSet<Application> Applications { get; set; }
        public virtual DbSet<Template> Templates { get; set; }
        public virtual DbSet<TemplateCategory> TemplateCategories { get; set; }
        public virtual DbSet<Page> Pages { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Template>()
                        .HasRequired(t => t.Category)
                        .WithMany(c => c.Templates)
                        .HasForeignKey(t => t.CategoryId);

            modelBuilder.Entity<TemplateCategory>()
                        .HasOptional(c => c.Parent)
                        .WithMany(c => c.Children)
                        .HasForeignKey(c => c.ParentId);

            modelBuilder.Entity<Page>()
                        .HasMany(p => p.Css)
                        .WithMany(c => c.Pages)
                        .Map(cs =>
                        {
                            cs.MapLeftKey("PageId");
                            cs.MapRightKey("CssId");
                            cs.ToTable("Mozaic_CssPages");
                        });
        }
    }
}
