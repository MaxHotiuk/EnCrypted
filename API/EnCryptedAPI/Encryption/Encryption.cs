using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace EnCryptedAPI.Encryption
{
    public static class Encryption
    {
        private const int KeySize = 256;
        private const int SaltSize = 32; // 32 bytes = 256 bits
        private const int Iterations = 100000;

        // Encrypts a plain text with a given passphrase
        public static string Encrypt(string plainText, string passPhrase)
        {
            // Convert plain text to Base64
            string base64PlainText = Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));

            // Generate a random salt and IV
            var salt = GenerateRandomBytes(SaltSize);
            var iv = GenerateRandomBytes(SaltSize / 2); // IV size for AES is usually half the key size (128 bits)

            // Derive the encryption key from the passphrase using PBKDF2
            using (var key = new Rfc2898DeriveBytes(passPhrase, salt, Iterations, HashAlgorithmName.SHA256))
            {
                var keyBytes = key.GetBytes(KeySize / 8); // Key size in bytes

                // Create AES instance and set the parameters
                using (var aes = Aes.Create())
                {
                    aes.KeySize = KeySize;
                    aes.BlockSize = 128;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Key = keyBytes;
                    aes.IV = iv;

                    // Encrypt the plain text
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            var plainTextBytes = Encoding.UTF8.GetBytes(base64PlainText);
                            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                            cryptoStream.FlushFinalBlock();

                            // Combine salt, IV, and encrypted data into one byte array
                            var cipherBytes = salt.Concat(iv).Concat(memoryStream.ToArray()).ToArray();
                            return Convert.ToBase64String(cipherBytes);
                        }
                    }
                }
            }
        }

        // Decrypts the cipher text using the same passphrase
        public static string Decrypt(string cipherText, string passPhrase)
        {
            // Convert the Base64-encoded cipher text to a byte array
            var cipherBytes = Convert.FromBase64String(cipherText);

            // Extract the salt, IV, and encrypted data from the cipher text
            var salt = cipherBytes.Take(SaltSize).ToArray();
            var iv = cipherBytes.Skip(SaltSize).Take(SaltSize / 2).ToArray();
            var encryptedBytes = cipherBytes.Skip(SaltSize + (SaltSize / 2)).ToArray();

            // Derive the key from the passphrase using the salt
            using (var key = new Rfc2898DeriveBytes(passPhrase, salt, Iterations, HashAlgorithmName.SHA256))
            {
                var keyBytes = key.GetBytes(KeySize / 8);

                // Create AES instance and set the parameters
                using (var aes = Aes.Create())
                {
                    aes.KeySize = KeySize;
                    aes.BlockSize = 128;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Key = keyBytes;
                    aes.IV = iv;

                    // Decrypt the encrypted data
                    using (var memoryStream = new MemoryStream(encryptedBytes))
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            using (var streamReader = new StreamReader(cryptoStream, Encoding.UTF8))
                            {
                                string base64PlainText = streamReader.ReadToEnd(); // Read the decrypted Base64 string
                                return Encoding.UTF8.GetString(Convert.FromBase64String(base64PlainText)); // Convert Base64 to original plain text
                            }
                        }
                    }
                }
            }
        }

        // Helper method to generate a random byte array of a given size
        private static byte[] GenerateRandomBytes(int size)
        {
            var randomBytes = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return randomBytes;
        }
    }
}