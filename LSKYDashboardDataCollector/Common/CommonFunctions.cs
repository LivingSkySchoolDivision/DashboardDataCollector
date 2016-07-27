using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace LSKYDashboardDataCollector.Common
{
    public static class CommonFunctions
    {
        public static string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public static string escapeCharacters(string input)
        {
            return input.Replace('"', '\'').Replace("\r\n"," ").Replace("\n"," ");
        }

        public static bool ParseBool(string thisValue)
        {
            if (String.IsNullOrEmpty(thisValue))
            {
                return false;
            }
            else
            {
                bool parsedBool = false;
                bool.TryParse(thisValue, out parsedBool);
                return parsedBool;
            }
        }

        /// <summary>
        /// Remove special characters that JSON doesn't like
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string SanitizeForJSON(string input)
        {
            string working = input;

            // If the first bunch of characters and last bunch of characters are a div, remove them
            working = Regex.Replace(working, @"<div class=.*?ExternalClass.*?>", string.Empty);
            if (working.Length > 6)
            {
                if (working.Substring(working.Length - 6, 6).ToLower() == "</div>")
                {
                    working = working.Substring(0, working.Length - 6);
                }
            }

            // Remove garbage that sharepoint adds
            working = working.Replace("<p>​</p>", "");

            // Remove characters that JSON doesn't like
            working = working.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "").Replace("\r", "");

            return working.Trim();
        }
    }
}