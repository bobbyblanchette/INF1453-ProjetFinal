using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Linq;
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
                string query = "select [Username], [Password] from [Users] where [Username] = @username";
                OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", this.Username);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    if (!reader.HasRows)
                        results.Add(new ValidationResult("Ce nom d'utilisateur n'existe pas.", new string[] { "Username" }));
                    else
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        string rawSaltAndHash = dt.Rows[0]["Password"].ToString();
                        string[] saltAndHash = rawSaltAndHash.Split('|');
                        if (Utils.generateHash(this.Password, saltAndHash[0]) != saltAndHash[1])
                            results.Add(new ValidationResult("Ce mot de passe est incorrect.", new string[] { "Password" }));
                    }
                conn.Close();
                cmd.Dispose();
                return results;
            }
        }
    }
}