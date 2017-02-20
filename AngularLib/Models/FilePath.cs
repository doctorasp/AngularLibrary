using System.ComponentModel.DataAnnotations;

namespace LibraryAngular.Models
{
    public class FilePath
    {
        public int FilePathId { get; set; }
        [StringLength(255)]
        public string FileName { get; set; }
        public int BookID { get; set; }
        public virtual Book Book { get; set; }
    }
}