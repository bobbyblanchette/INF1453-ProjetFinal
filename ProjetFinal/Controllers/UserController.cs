using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ProjetFinal.Controllers
{
    public class UserController : Controller
    {
        public ActionResult Library(string searchString = null, string category = null)
        {
            ProjetFinal.Models.HomeModel model = new Models.HomeModel();

            string connString = ConfigurationManager.ConnectionStrings["AtlasDB"].ConnectionString;
            using (var conn = new OleDbConnection(connString))
            {
                string query = "select * from [Books] where [Id] = (select [BookId] from [Sales] where [UserId] = (select [Id] from [Users] where [Username] = @username))";
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
                searchResultsTable.Load(searchCmd.ExecuteReader());

                var categoriesTable = new DataTable();
                categoriesTable.Load(categoriesCmd.ExecuteReader());

                conn.Close();


                model.Books = Utils.deserialize(searchResultsTable);
                foreach (DataRow row in categoriesTable.Rows)
                {
                    model.Categories.Add(row["Category"].ToString());
                }

            }
            return View(model);
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

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
                    cmd.Parameters.Add(new OleDbParameter("@username", user.Username));
                    cmd.Parameters.Add(new OleDbParameter("@password", SaltAndHash(user.Password)));
                    cmd.Parameters.Add(new OleDbParameter("@firstName", user.FirstName ?? ""));
                    cmd.Parameters.Add(new OleDbParameter("@lastName", user.LastName ?? ""));
                    cmd.Parameters.Add(new OleDbParameter("@phoneNumber", user.PhoneNumber ?? ""));
                    cmd.Parameters.Add(new OleDbParameter("@admin", false));

                    conn.Open();

                    cmd.ExecuteNonQuery();

                    cmd.Dispose();
                    conn.Close();

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


        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ConfirmPurchase(int id)
        {
            string connString = ConfigurationManager.ConnectionStrings["AtlasDB"].ConnectionString;
            using (var conn = new OleDbConnection(connString))
            {
                string query = "insert into [Sales] ([UserId], [BookId]) select [Id], @bookId from [Users] where [Username] = @username";
                OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.Add(new OleDbParameter("@bookId", id));
                cmd.Parameters.Add(new OleDbParameter("@username", User.Identity.GetUserName()));

                conn.Open();

                cmd.ExecuteNonQuery();

                cmd.Dispose();
                conn.Close();
            }
            return RedirectToAction("Index", "Home");
        }

        private static string SaltAndHash(string password)
        {
            string salt = CreateSalt(32);
            string hash = Utils.generateHash(password, salt);
            return salt + "|" + hash;
        }

        private static string CreateSalt(int size)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size];
            rng.GetBytes(buff);
            return Convert.ToBase64String(buff);
        }
    }
}