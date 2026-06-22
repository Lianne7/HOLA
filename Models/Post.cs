using System;
using System.ComponentModel.DataAnnotations;

namespace HOLA.Models
{
    public class Post
    {
        public int Id { get; set; }
        [Required] public string Title { get; set; }
        [Required] public string Content { get; set; }

        public string Board { get; set; } = "general";
        public string Tags { get; set; } = "";

        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}