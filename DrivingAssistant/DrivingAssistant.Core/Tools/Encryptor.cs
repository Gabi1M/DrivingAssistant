using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DrivingAssistant.Core.Tools
{
    public static class Encryptor
    {
        //=========================================================================
        public static string EncryptPassword(string password)
        {
            const string encryptionKey = "0ram@1234xxxxxxxxxxtttttuuuuuiiiiio";
            var clearBytes = Encoding.Unicode.GetBytes(password);
            using var encryptor = Aes.Create();
            var pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            if (encryptor != null)
            {
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using var ms = new MemoryStream();
                using (var cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                    cs.Close();
                }

                password = Convert.ToBase64String(ms.ToArray());
            }

            return password;
        }

        //=========================================================================
        public static string DecryptPassword(string password)
        {
            const string encryptionKey = "0ram@1234xxxxxxxxxxtttttuuuuuiiiiio";
            password = password.Replace(" ", "+");
            var cipherBytes = Convert.FromBase64String(password);
            using var encryptor = Aes.Create();
            var pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            if (encryptor != null)
            {
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using var ms = new MemoryStream();
                using (var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.Close();
                }

                password = Encoding.Unicode.GetString(ms.ToArray());
            }

            return password;
        }

        //=========================================================================
        public static string Encrypt_SHA256(string data)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
            var builder = new StringBuilder();
            foreach (var _byte in bytes)
            {
                builder.Append(_byte.ToString("x2"));
            }

            return builder.ToString();
        }
    }
}
