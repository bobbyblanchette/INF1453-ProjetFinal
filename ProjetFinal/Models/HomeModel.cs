using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjetFinal.Models
{
    public class HomeModel
    {
        public HomeModel()
        {
            this.Books = new List<BookModel>();
            this.Categories = new List<string>();
            this.currentCategory = "Tous";
        }
        public List<BookModel> Books { get; set; }

        public List<string> Categories { get; set; }
        public string currentCategory { get; set; }
        public List<List<BookModel>> GetBookChunks(int chunkSize)
        {
            return this.Books.Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }
}