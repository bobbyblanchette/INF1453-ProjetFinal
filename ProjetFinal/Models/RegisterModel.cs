using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace ProjetFinal.Models
{
    public class RegisterModel
    {
        [Required]
        [Display(Name = "Nom d'utilisateur")]
        public string Username { get; set; }

        [Display(Name = "Prénom")]
        public string FirstName { get; set; }

        [Display(Name = "Nom")]
        public string LastName { get; set; }
        [Phone]
        [Display(Name = "Numéro de téléphone")]
        public string PhoneNumber { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Le {0} doit être d'une longueur minimale de {2}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmer le mot de passe ")]
        [Compare("Password", ErrorMessage = "La confirmation du mot de passe ne correspond pas au mot de passe écrit")]
        public string ConfirmPassword { get; set; }
    }
}