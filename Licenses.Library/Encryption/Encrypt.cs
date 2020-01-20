using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Licenses.Library.Encryption
{
    public static class Encrypt
    {
        private static string passPhrase = "693d6f11-";         // can be any string
        private static string saltValue = "e0fe-48d5";          // can be any string
        private static string hashAlgorithm = "SHA1";           // can be "MD5"
        private static int passwordIterations = 2;              // can be any number
        private static string initVector = "a37768a452de74fb";  // must be 16 bytes
        private static int keySize = 256;                       // can be 192 or 128

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string EncryptString(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            PasswordDeriveBytes password = new PasswordDeriveBytes(
                                                            passPhrase,
                                                            saltValueBytes,
                                                            hashAlgorithm,
                                                            passwordIterations);

            byte[] keyBytes = password.GetBytes(keySize / 8);

            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;

            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(
                                                             keyBytes,
                                                             initVectorBytes);

            MemoryStream memoryStream = new MemoryStream();

            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                         encryptor,
                                                         CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();

            byte[] cipherTextBytes = memoryStream.ToArray();

            memoryStream.Close();
            cryptoStream.Close();

            // Convert encrypted data into a base64-encoded string.
            string cipherText = Convert.ToBase64String(cipherTextBytes);
            return cipherText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public static string DecryptString(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

            PasswordDeriveBytes password = new PasswordDeriveBytes(
                                                            passPhrase,
                                                            saltValueBytes,
                                                            hashAlgorithm,
                                                            passwordIterations);

            byte[] keyBytes = password.GetBytes(keySize / 8);

            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;

            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(
                                                             keyBytes,
                                                             initVectorBytes);

            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                          decryptor,
                                                          CryptoStreamMode.Read);

            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes,
                                                       0,
                                                       plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();

            string plainText = Encoding.UTF8.GetString(plainTextBytes,
                                                       0,
                                                       decryptedByteCount);
            return plainText;
        }
    }
}
