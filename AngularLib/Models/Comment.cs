using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace LibraryAngular.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [AllowHtml]
        public string CommentBody { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime CommentTime { get; set; }
        public int Rating { get; set; }
        public virtual int BookId { get; set; }
        public Book Book { get; set; }
    }
}