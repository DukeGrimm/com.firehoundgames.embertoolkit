using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EmberToolkit.Encryption.Helper
{
    public static class Keymaster
    {
        public static byte[] GenerateKey(string seed)
        {
            byte[] seedBytes = Encoding.UTF8.GetBytes(seed);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedSeed = sha256.ComputeHash(seedBytes);

                byte[] key = new byte[16]; // AES 128-bit key size
                Array.Copy(hashedSeed, 0, key, 0, key.Length);

                return key;
            }
        }
    }
}
