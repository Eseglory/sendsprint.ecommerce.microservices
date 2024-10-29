using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace sendsprint.ecommerce.Common
{
    public static class CodeGenerator
    {

        private static Random random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string RandomString(int length)
        {

            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private static Random randomnumber = new Random();
        const string nums = "0123456789";

        public static string RandomNumber(int length)
        {

            return new string(Enumerable.Repeat(nums, length)
                .Select(s => s[randomnumber.Next(s.Length)]).ToArray());
        }

        private static Random randomalphabet = new Random();
        const string charsalphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public static string RandomAlphabetString(int length)
        {

            return new string(Enumerable.Repeat(charsalphabet, length)
                .Select(s => s[randomalphabet.Next(s.Length)]).ToArray());
        }

        public static string randomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }
    }
}
