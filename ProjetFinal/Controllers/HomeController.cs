using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjetFinal.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ProjetFinal.Models.HomeModel model = new Models.HomeModel();
            for (int i = 0; i < 8; i++)
                model.Books.Add(new Models.BookModel()
                {
                    Title = "test",
                    Author = "George",
                    Description = "George is a book",
                    CoverUrl = "http://emojipedia-us.s3.amazonaws.com/cache/ce/c6/cec6e314b3096a288a0d5e83ff0b352d.png",
                    Price = 15.99m,
                    Year = 1995,
                    ISBN = "no"
                });
            return View(model);
        }
    }
}