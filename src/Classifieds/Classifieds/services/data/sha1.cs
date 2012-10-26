using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Classifieds.services.data
{
    public class SHA1
    {
        public static string GetSha(string plainText,string salt)
        {
            var encrypt= GenerateSaltedHash(Encoding.UTF8.GetBytes(plainText), Encoding.UTF8.GetBytes(salt));
            return Convert.ToBase64String(encrypt);
        }

        public static bool AreEqual(string plainText,string salt,string cypherText)
        {
            var encrypt = GenerateSaltedHash(Encoding.UTF8.GetBytes(plainText), Encoding.UTF8.GetBytes(salt));
            var cryptCypher = Convert.FromBase64String(cypherText);

            return CompareByteArrays(encrypt,cryptCypher);
        }


        private static byte[] GenerateSaltedHash(byte[] plainText, byte[] salt)
        {
            HashAlgorithm algorithm = new SHA256Managed();

            var plainTextWithSaltBytes =
                new byte[plainText.Length + salt.Length];

            for (var i = 0; i < plainText.Length; i++)
            {
                plainTextWithSaltBytes[i] = plainText[i];
            }
            for (var i = 0; i < salt.Length; i++)
            {
                plainTextWithSaltBytes[plainText.Length + i] = salt[i];
            }

            return algorithm.ComputeHash(plainTextWithSaltBytes);
        }

        public static bool CompareByteArrays(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
            {
                return false;
            }

            return !array1.Where((t, i) => t != array2[i]).Any();
        }
    }
}