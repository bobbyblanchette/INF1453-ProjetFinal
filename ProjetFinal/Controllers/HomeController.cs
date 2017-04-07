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
        public ActionResult Index(string searchString = null, string category = null)
        {
            OleDbConnection con = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=J:\Dev\Repos\INF1453\ProjetFinal\ProjetFinal\Atlas.mdb;Persist Security Info=False");


            string query = "select * from Books";
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query += " where Title like @searchString";
            }
            else if (!string.IsNullOrWhiteSpace(category))
                query += " where Category = @category";
                        
            
            OleDbCommand searchCmd = new OleDbCommand(query, con);

            if (!string.IsNullOrWhiteSpace(searchString))
                searchCmd.Parameters.AddWithValue("@searchString", "%" + searchString + "%");
            if (!string.IsNullOrWhiteSpace(category))
                searchCmd.Parameters.AddWithValue("@category", category);

            OleDbCommand categoriesCmd = new OleDbCommand("select distinct Category from Books", con);

            con.Open();

            var searchResultsTable = new DataTable();
            searchResultsTable.Load(searchCmd.ExecuteReader());

            var categoriesTable = new DataTable();
            categoriesTable.Load(categoriesCmd.ExecuteReader());

            con.Close();
            
            ProjetFinal.Models.HomeModel model = new Models.HomeModel();
            foreach (DataRow row in searchResultsTable.Rows)
            {
                model.Books.Add(new Models.BookModel()
                {
                    Title = row["Title"].ToString(),
                    Author = row["Author"].ToString(),
                    Description = row["Description"].ToString(),
                    Category = row["Category"].ToString(),
                    CoverUrl = row["CoverUrl"].ToString(),
                    Price = decimal.Parse(row["Price"].ToString()),
                    NbPages = int.Parse(row["NbPages"].ToString()),
                    ISBN = row["ISBN"].ToString(),
                    Year = int.Parse(row["Year"].ToString())
                });
            }
            foreach (DataRow row in categoriesTable.Rows)
            {
                model.Categories.Add(row["Category"].ToString());
            }

            return View(model);
        }

        public ActionResult BookDetails(Models.BookModel model)
        {
            return View(model);
        }

    }
}