using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjetFinal.Controllers
{
    public class HomeController : Controller
    {
        public static string connString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=J:\Dev\Repos\INF1453\ProjetFinal\ProjetFinal\Atlas.mdb;Persist Security Info=False";
        public ActionResult Index(string searchString = null, string category = null)
        {
            OleDbConnection conn = new OleDbConnection(connString);


            string query = "select * from Books";
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query += " where Title like @searchString";
            }
            else if (!string.IsNullOrWhiteSpace(category))
                query += " where Category = @category";
                        
            
            OleDbCommand searchCmd = new OleDbCommand(query, conn);

            if (!string.IsNullOrWhiteSpace(searchString))
                searchCmd.Parameters.AddWithValue("@searchString", "%" + searchString + "%");
            if (!string.IsNullOrWhiteSpace(category))
                searchCmd.Parameters.AddWithValue("@category", category);

            OleDbCommand categoriesCmd = new OleDbCommand("select distinct Category from Books", conn);

            conn.Open();

            var searchResultsTable = new DataTable();
            searchResultsTable.Load(searchCmd.ExecuteReader());

            var categoriesTable = new DataTable();
            categoriesTable.Load(categoriesCmd.ExecuteReader());

            conn.Close();
            
            ProjetFinal.Models.HomeModel model = new Models.HomeModel();
            model.Books = Deserialize(searchResultsTable);
            foreach (DataRow row in categoriesTable.Rows)
            {
                model.Categories.Add(row["Category"].ToString());
            }

            return View(model);
        }

        public ActionResult BookDetails(int id)
        {
            OleDbConnection conn = new OleDbConnection(connString);


            string query = "select * from Books where Id = @id";
            
            OleDbCommand cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);

            conn.Open();

            var dataTable = new DataTable();
            dataTable.Load(cmd.ExecuteReader());

            conn.Close();

            ProjetFinal.Models.BookModel model = Deserialize(dataTable).FirstOrDefault();
            
            return View(model);
        }

        public List<Models.BookModel> Deserialize(DataTable dt)
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
                    Rating = int.Parse(row["Rating"].ToString())
                });
            }
            return books;
        }
    }
}