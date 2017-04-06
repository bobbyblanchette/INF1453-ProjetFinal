﻿using System.Collections.Generic;
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
        public string CoverUrl { get; set; }
        [Required]
        public decimal Prix { get; set; }
        public int NbPages { get; set; }
        public string ISBN { get; set; }
        public int Year { get; set; }
    }
}