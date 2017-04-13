using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ProjetFinal.Controllers
{
    public class UserController : Controller
    {
        /// <summary>
        /// Retourne la vue Login.cshtml avec un modèle LoginModel vide
        /// </summary>
        /// <returns>La vue Login.cshtml avec un modèle LoginModel vide</returns>
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Retourne la vue Register.cshtml avec un modèle RegisterModel vide
        /// </summary>
        /// <returns>La vue Register.cshtml avec un modèle RegisterModel vide</returns>
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Retourne la vue "Library.cshtml", avec un modèle HomeModel.cs contenant les livres qui sont dans la librairie de l'utilisateur, filtrés selon les paramètres entrés
        /// </summary>
        /// <param name="searchString">Mots clés de recherche</param>
        /// <param name="category">Catégorie sélectionnée</param>
        /// <returns>La vue "Library.cshtml", avec un modèle HomeModel</returns>
        public ActionResult Library(string searchString = null, string category = null)
        {
            ProjetFinal.Models.HomeModel model = new Models.HomeModel();

            string connString = ConfigurationManager.ConnectionStrings["AtlasDB"].ConnectionString;
            using (var conn = new OleDbConnection(connString))
            {
                string query = "select * from [Books] where [Id] in (select [BookId] from [Sales] where [UserId] = (select [Id] from [Users] where [Username] = @username))";
                if (!string.IsNullOrWhiteSpace(searchString))
                    query += " and [Title] like @searchString";
                else if (!string.IsNullOrWhiteSpace(category))
                    query += " and [Category] = @category";
                query += " order by [Rating] desc, [Title] asc";

                OleDbCommand searchCmd = new OleDbCommand(query, conn);

                searchCmd.Parameters.AddWithValue("@username", User.Identity.GetUserName());
                if (!string.IsNullOrWhiteSpace(searchString))
                    searchCmd.Parameters.AddWithValue("@searchString", "%" + searchString + "%");
                else if (!string.IsNullOrWhiteSpace(category))
                {
                    searchCmd.Parameters.AddWithValue("@category", category);
                    model.currentCategory = category;
                }



                OleDbCommand categoriesCmd = new OleDbCommand("select distinct [Category] from [Books]", conn);

                conn.Open();

                var searchResultsTable = new DataTable();
                using (var reader = searchCmd.ExecuteReader())
                    searchResultsTable.Load(reader);

                var categoriesTable = new DataTable();
                using (var reader = categoriesCmd.ExecuteReader())
                    categoriesTable.Load(reader);

                conn.Close();
                searchCmd.Dispose();
                categoriesCmd.Dispose();

                model.Books = Utils.deserialize(searchResultsTable);
                foreach (DataRow row in categoriesTable.Rows)
                {
                    model.Categories.Add(row["Category"].ToString());
                }

            }
            return View(model);
        }

        /// <summary>
        /// Retourne le fichier PDF selon le lien contenu dans le modèle BookModel entré en paramètre
        /// </summary>
        /// <param name="book">Le modèle BookModel dont on veut le fichier PDF</param>
        /// <returns>Fichier de type PDF</returns>
        [HttpPost]
        public ActionResult DownloadBook(Models.BookModel book)
        {
            string file = "~/App_Data/" + book.DlLink;
            string contentType = "application/pdf";
            return File(file, contentType, Path.GetFileName(file));
        }

        /// <summary>
        /// Vérifie la validité du modèle LoginModel entré en argument, et, si ce modèle est valide, utilises FormsAuthentication pour connecter l'utilisateur
        /// </summary>
        /// <param name="user">Modèle LoginModel du form de connexion</param>
        /// <returns>Rediriges vers Index() dans le HomeController</returns>
        [HttpPost]
        public ActionResult Login(Models.LoginModel user)
        {
            if (ModelState.IsValid)
            {
                FormsAuthentication.SetAuthCookie(user.Username, user.RememberMe);
                return RedirectToAction("Index", "Home");
            }
            return View(user);
        }

        /// <summary>
        /// Vérifie la validité du modèle RegisterModel entré en argument, et, si ce modèle est valide, crée la requête pour inscrire l'utilisateur
        /// </summary>
        /// <param name="user">Modèle LoginModel du form d'inscription</param>
        /// <returns>Rediriges vers Index() dans le HomeController</returns>
        [HttpPost]
        public ActionResult Register(Models.RegisterModel user)
        {
            if (ModelState.IsValid)
            {
                string connString = ConfigurationManager.ConnectionStrings["AtlasDB"].ConnectionString;
                using (var conn = new OleDbConnection(connString))
                {
                    string query = "insert into Users (Username, [Password], FirstName, LastName, PhoneNumber, Admin) values (@username, @password, @firstName, @lastName, @phoneNumber, @admin)";
                    OleDbCommand cmd = new OleDbCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", user.Username);
                    cmd.Parameters.AddWithValue("@password", SaltAndHash(user.Password));
                    cmd.Parameters.AddWithValue("@firstName", user.FirstName ?? "");
                    cmd.Parameters.AddWithValue("@lastName", user.LastName ?? "");
                    cmd.Parameters.AddWithValue("@phoneNumber", user.PhoneNumber ?? "");
                    cmd.Parameters.AddWithValue("@admin", false);

                    conn.Open();

                    cmd.ExecuteNonQuery();

                    conn.Close();
                    cmd.Dispose();

                    return Login(new Models.LoginModel()
                    {
                        Username = user.Username,
                        Password = user.Password,
                        RememberMe = true
                    });
                }
            }
            return View(user);
        }

        /// <summary>
        /// Déconnecte l'utilisateur avec FormsAuthentication
        /// </summary>
        /// <returns>Rediriges vers Index() dans le HomeController</returns>
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Crée la requête pour ajouter une entrée dans la table Sales de la base de données, reliant le ID de l'utilisateur au ID d'un livre
        /// </summary>
        /// <param name="id">Le ID du livre acheté</param>
        /// <returns>Rediriges vers Index() dans le HomeController</returns>
        public ActionResult ConfirmPurchase(int id)
        {
            string connString = ConfigurationManager.ConnectionStrings["AtlasDB"].ConnectionString;
            using (var conn = new OleDbConnection(connString))
            {
                string query = "insert into [Sales] ([UserId], [BookId]) select [Id], @bookId from [Users] where [Username] = @username";
                OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("@bookId", id);
                cmd.Parameters.AddWithValue("@username", User.Identity.GetUserName());

                conn.Open();

                cmd.ExecuteNonQuery();

                conn.Close();
                cmd.Dispose();
            }
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Crée un salt, pour ensuite générer un hash avec la méthode generateHash et retourner un string contenant le salt et le hash, séparés par un "|"
        /// </summary>
        /// <param name="password">Le mot de passe à hasher</param>
        /// <returns>Un string contenant le salt et le hash, séparés par un "|"</returns>
        private static string SaltAndHash(string password)
        {
            string salt = CreateSalt(32);
            string hash = Utils.generateHash(password, salt);
            return salt + "|" + hash;
        }

        /// <summary>
        /// Crée un salt aléatoire à l'aide de la méthode RNGCryptoServiceProvider (pour avoir un nombre aléatoire riche)
        /// </summary>
        /// <param name="size">La taille du salt à créer</param>
        /// <returns>Le salt, en int</returns>
        private static string CreateSalt(int size)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size];
            rng.GetBytes(buff);
            return Convert.ToBase64String(buff);
        }
    }
}