using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace LibraryAngular.Models
{
    public class ContextBooks:DbContext
    {
        public ContextBooks()
            :base("conn")
        {

        }
  
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<FilePath> FilePaths { get; set; }
        public DbSet<Comment> Comments { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

    }
}