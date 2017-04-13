using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjetFinal.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Retourne la vue "Index.cshtml", avec un modèle HomeModel contenant les livres, filtrés selon les paramètres entrés
        /// </summary>
        /// <param name="searchString">Mots clés de recherche</param>
        /// <param name="category">Catégorie sélectionnée</param>
        /// <returns>La vue "Index.cshtml", avec un modèle HomeModel</returns>
        public ActionResult Index(string searchString = null, string category = null)
        {
            ProjetFinal.Models.HomeModel model = new Models.HomeModel();

            string connString = ConfigurationManager.ConnectionStrings["AtlasDB"].ConnectionString;
            using (var conn = new OleDbConnection(connString))
            {
                string query = "select * from [Books]";
                if (!string.IsNullOrWhiteSpace(searchString))
                    query += " where [Title] like @searchString";
                else if (!string.IsNullOrWhiteSpace(category))
                    query += " where [Category] = @category";
                query += " order by [Rating] desc, [Title] asc";

                OleDbCommand searchCmd = new OleDbCommand(query, conn);

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
        /// Retourne la vue "BookDetails.cshtml", avec un modèle BookModel contenant les détails du livre demandé dans les paramètres
        /// </summary>
        /// <param name="id">ID du livre demandé</param>
        /// <returns>La vue "BookDetails.cshtml", avec un modèle BookModel</returns>
        public ActionResult BookDetails(int id)
        {
            ProjetFinal.Models.BookModel model;

            string connString = ConfigurationManager.ConnectionStrings["AtlasDB"].ConnectionString;
            using (var conn = new OleDbConnection(connString))
            {
                string query = "select * from [Books] where [Id] = @id";

                OleDbCommand bookCmd = new OleDbCommand(query, conn);
                bookCmd.Parameters.AddWithValue("@id", id);

                bool isOwnedByUser = false;
                

                conn.Open();

                var dataTable = new DataTable();
                using (var reader = bookCmd.ExecuteReader())
                    dataTable.Load(reader);

                if (Request.IsAuthenticated)
                {
                    OleDbCommand isOwnedCmd = new OleDbCommand("select * from [Sales] where [BookId] = @bookId and [UserId] = (select [Id] from [Users] where [Username] = @username)", conn);
                    isOwnedCmd.Parameters.AddWithValue("@username", User.Identity.GetUserName());
                    isOwnedCmd.Parameters.AddWithValue("@bookId", id);
                    using (var reader = isOwnedCmd.ExecuteReader())
                        isOwnedByUser = reader.HasRows;
                    isOwnedCmd.Dispose();
                }

                conn.Close();
                bookCmd.Dispose();

                model = Utils.deserialize(dataTable).FirstOrDefault();
                model.IsOwned = isOwnedByUser;

            }
                return View(model);
        }

        
    }
}