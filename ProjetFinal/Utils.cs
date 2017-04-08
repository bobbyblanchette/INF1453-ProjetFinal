using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace ProjetFinal
{
    public class Utils
    {
        public static string generateHash(string password, string salt)
        {
            string hash = SHA256Hash(password + salt);
            for (int i = 0; i < 5000; i++)
                hash = SHA256Hash(hash + salt);
            return hash;
        }

        public static string SHA256Hash(string value)
        {
            SHA256 hash = SHA256.Create();
            System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
            byte[] combined = encoder.GetBytes(value);
            return Convert.ToBase64String(hash.ComputeHash(combined)).ToLower().Replace("-", "");
        }
    }
}