using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjetFinal.Models
{
    public class BookModel
    {
        [Required]
        [Display(Name = "Titre")]
        public string Title { get; set; }
        [Required]
        [Display(Name = "Auteur")]
        public string Author { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public string CoverUrl { get; set; }
        [Required]
        [UIHint("Currency")]
        public decimal Price { get; set; }
        public int NbPages { get; set; }
        public string ISBN { get; set; }
        public int Year { get; set; }
        [UIHint("Rating")]
        public int Rating { get; set; }
    }
}
