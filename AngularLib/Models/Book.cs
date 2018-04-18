using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryAngular.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Year { get; set; }

        public string CoverType { get; set; }
        public byte[] Cover { get; set; }
        public int ViewCount { get; set; }
        public int AuthorId { get; set; }
        public int GenreId { get; set; }
        public virtual Author Author { get; set; }
        public virtual Genre Genre { get; set; }
        public virtual ICollection<FilePath> FilePaths { get; set; }

    }
}