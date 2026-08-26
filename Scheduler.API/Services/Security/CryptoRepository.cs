using System.Security.Cryptography;
using System.Text;

namespace Scheduler.API.Services.Security
{
    public class CryptoRepository: ICrypto
    {
        public string Encrypt(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = GetSHA256Key("your-16-byte-key!");
                aes.IV = Encoding.UTF8.GetBytes("16-byte-IV-12345");

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        private  byte[] GetSHA256Key(string key)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                byte[] hash = sha256.ComputeHash(keyBytes);

                // Use the first 16 bytes (128 bits) for the AES key
                byte[] aesKey = new byte[16];
                Array.Copy(hash, aesKey, aesKey.Length);

                return aesKey; // Return the key of valid length
            }
        }



        public string Decrypt(string cipherText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = GetSHA256Key("your-16-byte-key!");
                aes.IV = Encoding.UTF8.GetBytes("16-byte-IV-12345");

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
