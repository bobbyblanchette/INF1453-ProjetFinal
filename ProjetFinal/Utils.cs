using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace ProjetFinal
{
    public class Utils
    {
        /// <summary>
        /// Crée un modèle BookModel et le popule selon une dataTable recu en paramètre
        /// </summary>
        /// <param name="dt">La dataTable qu'on veut désérialiser</param>
        /// <returns>Le modèle BookModel désérialisé</returns>
        public static List<Models.BookModel> deserialize(DataTable dt)
        {
            List<Models.BookModel> books = new List<Models.BookModel>();
            foreach (DataRow row in dt.Rows)
            {
                books.Add(new Models.BookModel()
                {
                    Id = int.Parse(row["Id"].ToString()),
                    Title = row["Title"].ToString(),
                    Author = row["Author"].ToString(),
                    Description = row["Description"].ToString(),
                    Category = row["Category"].ToString(),
                    CoverUrl = row["CoverUrl"].ToString(),
                    Price = decimal.Parse(row["Price"].ToString()),
                    NbPages = int.Parse(row["NbPages"].ToString()),
                    ISBN = row["ISBN"].ToString(),
                    Year = int.Parse(row["Year"].ToString()),
                    Rating = int.Parse(row["Rating"].ToString()),
                    DlLink = row["DlLink"].ToString()
                });
            }
            return books;
        }

        /// <summary>
        /// Génère un hash à partir un password et un salt entrés en paramètre, avec 5000 itérations de SHA256, en réintroduisant le salt dans chaque itérations
        /// </summary>
        /// <param name="password">Le password à hasher</param>
        /// <param name="salt">Le salt</param>
        /// <returns>Le hash généré</returns>
        public static string generateHash(string password, string salt)
        {
            string hash = SHA256Hash(password + salt);
            for (int i = 0; i < 5000; i++)
                hash = SHA256Hash(hash + salt);
            return hash;
        }

        /// <summary>
        /// Génére un hash SHA256 à partir d'une valeur entrée en paramètre
        /// </summary>
        /// <param name="value">La valeur à hasher</param>
        /// <returns>Le hash généré</returns>
        public static string SHA256Hash(string value)
        {
            SHA256 hash = SHA256.Create();
            System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
            byte[] combined = encoder.GetBytes(value);
            return Convert.ToBase64String(hash.ComputeHash(combined)).ToLower().Replace("-", "");
        }
    }
}