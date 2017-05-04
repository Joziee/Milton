using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database
{
    public static class Hash
    {
        /// <summary>
		/// Generates a random 32 byte string to be used as a salt when hashing passwords
		/// </summary>
		/// <returns></returns>
		public static String GetSalt(int length = 32)
        {
            using (var rng = RNGCryptoServiceProvider.Create())
            {
                byte[] salt = new byte[length];
                rng.GetBytes(salt);
                return ASCIIEncoding.ASCII.GetString(salt);
            }
        }

        /// <summary>
        /// Generates a hashed value from the input and salt values
        /// </summary>
        /// <param name="input"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static String GetHash(String input, String salt = "")
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = ASCIIEncoding.ASCII.GetBytes(String.Concat(salt, input));
                byte[] hash = sha.ComputeHash(bytes);
                return ASCIIEncoding.ASCII.GetString(hash);
            }
        }
    }
}
