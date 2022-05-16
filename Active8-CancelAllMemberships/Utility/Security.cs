using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Active8_CancelAllMemberships.Utility
{
    public class Security
    {
        private static readonly int _primeEven = 89989;
        private static readonly int _primeOdd = 89527;
        private static readonly int _primeStatic = 95273;

        /// <summary>
        /// Uses TripleDESCryptoServiceProvider to encrypt based upon the given key. Different
        /// keys provide different results for the same plaintext variable.
        /// </summary>
        /// <param name="plaintext"> Text to encrypt. </param>
        /// <param name="key"></param>
        /// <returns>
        /// An encrypted string (about 3x the length of the given string) or the error message:
        /// "Failed to encrypt."
        /// </returns>
        public static string Encrypt(string plaintext, string key)
        {
            if (string.IsNullOrEmpty(plaintext))
            {
                return "";
            }

            try
            {
                TripleDESCryptoServiceProvider tripleDes = new TripleDESCryptoServiceProvider();

                // Initialize the crypto provider.
                tripleDes.Key = TruncateHash(key, tripleDes.KeySize / 8);
                tripleDes.IV = TruncateHash("", tripleDes.BlockSize / 8);

                // Convert the plaintext string to a byte array.
                byte[] plaintextBytes = Encoding.Unicode.GetBytes(plaintext);

                // Create the stream
                MemoryStream ms = new MemoryStream();
                // Create the encoder to write to the stream.
                CryptoStream encStream = new CryptoStream(ms, tripleDes.CreateEncryptor(), CryptoStreamMode.Write);

                // Use the crypto stream to write the byte array to the stream.
                encStream.Write(plaintextBytes, 0, plaintextBytes.Length);
                encStream.FlushFinalBlock();

                // Convert the encrypted stream to a printable string.
                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception ex)
            {
                return "Failed to encrypt.";
            }
        }

        /// <summary>
        /// Uses TripleDESCryptoServiceProvider to decrypt based upon the given key. If it
        /// fails, it assumes the key is off.
        /// </summary>
        /// <param name="ciphertext"> Encrypted text to decrypt. </param>
        /// <param name="key"> Key used during encryption. </param>
        /// <returns> Decrypted value or the error message: "Failed to decrypt." </returns>
        public static string Decrypt(string ciphertext, string key)
        {
            if (string.IsNullOrEmpty(ciphertext))
            {
                return "";
            }

            try
            {
                TripleDESCryptoServiceProvider tripleDes = new TripleDESCryptoServiceProvider();

                // Initialize the crypto provider.
                tripleDes.Key = TruncateHash(key, tripleDes.KeySize / 8);
                tripleDes.IV = TruncateHash("", tripleDes.BlockSize / 8);

                // Convert the encrypted text string to a byte array.
                byte[] encryptedBytes = Convert.FromBase64String(ciphertext);

                // Create the stream.
                MemoryStream ms = new MemoryStream();
                // Create the decoder to write to the stream.
                CryptoStream decStream = new CryptoStream(ms, tripleDes.CreateDecryptor(), CryptoStreamMode.Write);

                // Use the crypto stream to write the byte array to the stream.
                decStream.Write(encryptedBytes, 0, encryptedBytes.Length);
                decStream.FlushFinalBlock();

                // Convert the plaintext stream to a string.
                return Encoding.Unicode.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                return "Failed to decrypt.";
            }
        }
        private static byte[] TruncateHash(string key, int length)
        {
            // Create a SHA1 hash algorithm
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();

            // Hash the key.
            byte[] keyBytes = Encoding.Unicode.GetBytes(key);
            byte[] hash = sha1.ComputeHash(keyBytes);

            // Buffer or truncate the final hash size so it fits with the crypto class
            byte[] finalhash = new byte[length];
            Buffer.BlockCopy(hash, 0, finalhash, 0, hash.Length <= length ? hash.Length : length);

            return finalhash;
        }
    }
}
