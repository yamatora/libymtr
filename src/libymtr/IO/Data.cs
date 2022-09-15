using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace libymtr.IO {
    public static class Data {
        #region Serialize
        /// <summary>
        /// Serialize object with [Serializable()] attribute: Ignore property with [NonSerialized()] attribute
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] ObjectToBinary(object obj) {
            using(var ms = new MemoryStream()) {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
        /// <summary>
        /// Deserialize object
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static object BinaryToObject(byte[] bytes) {
            using(var ms = new MemoryStream(bytes)) {
                var bf = new BinaryFormatter();
                return bf.Deserialize(ms);
            }
        }
        #endregion

        #region Cryption
        private const int SIZE_CRYPT = 16;

        private static byte[] PerformCryption(ICryptoTransform transformer, byte[] data) {
            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, transformer, CryptoStreamMode.Write)) {
                cs.Write(data, 0, data.Length);
                cs.FlushFinalBlock();
                return ms.ToArray();
            }
        }
        /// <summary>
        /// Encrypt byt block cipher
        /// </summary>
        /// <param name="data"></param>
        /// <param name="password"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static byte[] Encrypt(byte[] data, string password, CipherMode mode = CipherMode.CBC) {
            using (var aes = Aes.Create()) {
                //  Set params
                aes.BlockSize = SIZE_CRYPT * 8;
                aes.KeySize = SIZE_CRYPT * 8;
                aes.Mode = mode;
                aes.Padding = PaddingMode.PKCS7;
                //  Get key
                var derive = new Rfc2898DeriveBytes(password, SIZE_CRYPT);
                byte[] key = derive.GetBytes(SIZE_CRYPT);
                byte[] salt = derive.Salt;
                aes.Key = key;
                //  Generate Initialization Vector
                aes.GenerateIV();

                //  Encrypt
                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                byte[] eData = PerformCryption(encryptor, data);

                //  Concat
                return Generic.ConcatArray(salt, aes.IV, eData);
            }
        }
        /// <summary>
        /// Decrypt by block cipher
        /// </summary>
        /// <param name="data"></param>
        /// <param name="password"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static byte[] Decrypt(byte[] data, string password, CipherMode mode = CipherMode.CBC) {
            using (var aes = Aes.Create()) {
                //  Set params
                aes.BlockSize = SIZE_CRYPT * 8;
                aes.KeySize = SIZE_CRYPT * 8;
                aes.Mode = mode;
                aes.Padding = PaddingMode.PKCS7;

                //  Read Salt, Initialization Vector, Data
                int p = 0;
                byte[] salt = Generic.GetPart(data, p, SIZE_CRYPT);
                p += salt.Length;
                byte[] iv = Generic.GetPart(data, p, SIZE_CRYPT);
                p += iv.Length;
                byte[] bData = Generic.GetPart(data, p, SIZE_CRYPT);
                p += bData.Length;

                //  Get key
                var derive = new Rfc2898DeriveBytes(password, salt);
                aes.Key = derive.GetBytes(SIZE_CRYPT);
                aes.IV = iv;

                //  Decrypt
                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                return PerformCryption(decryptor, bData);
            }
        }
        #endregion 
    }
}
