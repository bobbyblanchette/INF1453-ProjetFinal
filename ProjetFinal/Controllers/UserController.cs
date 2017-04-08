using System;
using System.Collections.Generic;
using System.Configuration;
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
                    string query = "insert into Users (Username, Password, FirstName, LastName, PhoneNumber, Admin) values (@username, @password, @firstName, @lastName, @phoneNumber, @admin)";
                    OleDbCommand cmd = new OleDbCommand(query, conn);
                    cmd.Parameters.Add(new OleDbParameter("@username", user.Username));
                    cmd.Parameters.Add(new OleDbParameter("@password", SHA1Encode(user.Password)));
                    cmd.Parameters.Add(new OleDbParameter("@firstName", user.FirstName));
                    cmd.Parameters.Add(new OleDbParameter("@lastName", user.LastName));
                    cmd.Parameters.Add(new OleDbParameter("@phoneNumber", user.PhoneNumber));
                    cmd.Parameters.Add(new OleDbParameter("@admin", false));

                    conn.Open();

                    cmd.ExecuteNonQuery();

                    cmd.Dispose();
                    conn.Close();
                    
                }
            }
            return View(user);
        }


        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        private static string SHA1Encode(string value)
        {
            var hash = SHA1.Create();
            var encoder = new System.Text.ASCIIEncoding();
            var combined = encoder.GetBytes(value ?? "");
            return BitConverter.ToString(hash.ComputeHash(combined)).ToLower().Replace("-", "");
        }
    }
}