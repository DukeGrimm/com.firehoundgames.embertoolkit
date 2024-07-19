using EmberToolkit.Common.Interfaces.Configuration;
using EmberToolkit.Common.Interfaces.Encryption;
using EmberToolkit.Encryption.Helper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Cryptography;
using System.Text;

namespace EmberToolkit.Encryption.Controller
{
    public class AESController : IAESController
    {
        private readonly byte[] key;

        [ActivatorUtilitiesConstructor]
        public AESController(IEmberSettings settings)
        {
            this.key = Keymaster.GenerateKey(settings.saveKey);
        }
        public AESController(string inputKey)
        {
            this.key = Keymaster.GenerateKey(inputKey);
        }

        public string EncryptData(string jsonData)
        {
            byte[] iv;
            byte[] encryptedBytes;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.GenerateIV();
                iv = aesAlg.IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                byte[] jsonDataBytes = Encoding.UTF8.GetBytes(jsonData);

                using (var msEncrypt = new System.IO.MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(jsonDataBytes, 0, jsonDataBytes.Length);
                    }
                    encryptedBytes = msEncrypt.ToArray();
                }
            }

            byte[] combinedBytes = new byte[iv.Length + encryptedBytes.Length];
            Buffer.BlockCopy(iv, 0, combinedBytes, 0, iv.Length);
            Buffer.BlockCopy(encryptedBytes, 0, combinedBytes, iv.Length, encryptedBytes.Length);

            return Convert.ToBase64String(combinedBytes);
        }
        public string DecryptData(string encryptedData)
        {
            byte[] combinedBytes = Convert.FromBase64String(encryptedData);
            byte[] iv = new byte[16];
            byte[] encryptedBytes = new byte[combinedBytes.Length - iv.Length];

            Buffer.BlockCopy(combinedBytes, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(combinedBytes, iv.Length, encryptedBytes, 0, encryptedBytes.Length);

            byte[] decryptedBytes;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (var msDecrypt = new System.IO.MemoryStream(encryptedBytes))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var decryptedStream = new System.IO.MemoryStream())
                        {
                            csDecrypt.CopyTo(decryptedStream);
                            decryptedBytes = decryptedStream.ToArray();
                        }
                    }
                }
            }

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}
