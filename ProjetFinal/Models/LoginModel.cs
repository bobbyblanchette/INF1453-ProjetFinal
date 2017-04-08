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
    public class LoginModel : IValidatableObject
    {
        [Required]
        [Display(Name = "Nom d'utilisateur")]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; }
        [Display(Name = "Se souvenir de moi")]
        public bool RememberMe { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            string connString = ConfigurationManager.ConnectionStrings["AtlasDB"].ConnectionString;
            using (var conn = new OleDbConnection(connString))
            {
                string query = "select Username from Users where Username = @username and Password = @password";
                OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.Add(new OleDbParameter("@username", this.Username));
                cmd.Parameters.Add(new OleDbParameter("@password", SHA1Encode(this.Password)));

                conn.Open();
                OleDbDataReader reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                    results.Add(new ValidationResult("Le nom d'utilisateur ou le mot de passe est invalide ou n'existe pas.", new string[] { "Username", "Password" }));
                reader.Close();
                cmd.Dispose();
                conn.Close();
                return results;
            }
        }

        private static string SHA1Encode(string value)
        {
            return value;
            /*
            var hash = SHA1.Create();
            var encoder = new System.Text.ASCIIEncoding();
            var combined = encoder.GetBytes(value ?? "");
            return BitConverter.ToString(hash.ComputeHash(combined)).ToLower().Replace("-", "");
            */
        }
    }
}