using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace migration
{
    public class LNEncryption
    {

        const string DFADFKDFS_KEY = "34u8lllkdjfaieuteoijdfadngpiudapoietydfaku";
        const string d345908da9083kdjf_KEY = "3kljfao4kljlkjh84734984nvnns4-37858k";
        const string ewuroda0983475drt_KEY = "glkjhiuroiut4874987ri78754879fgFA$da0345";

        private static byte[] Decrypt(byte[] cipherData, byte[] Key, byte[] IV)
        {
            MemoryStream ms = new MemoryStream();
            Rijndael alg = Rijndael.Create();
            alg.Key = Key;
            alg.IV = IV;
            CryptoStream cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(cipherData, 0, cipherData.Length);
            cs.Close();
            byte[] decryptedData = ms.ToArray();
            return decryptedData;
        }


        private static byte[] Encrypt(byte[] clearData, byte[] Key, byte[] IV)
        {
            MemoryStream ms = new MemoryStream();
            Rijndael alg = Rijndael.Create();
            alg.Key = Key;
            alg.IV = IV;
            CryptoStream cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(clearData, 0, clearData.Length);
            cs.Close();
            byte[] encryptedData = ms.ToArray();
            return encryptedData;
        }

        private static string Encrypt(string clearText, string Password)
        {
            byte[] clearBytes = System.Text.Encoding.Unicode.GetBytes(clearText);
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            byte[] encryptedData = Encrypt(clearBytes, pdb.GetBytes(32), pdb.GetBytes(16));
            return Convert.ToBase64String(encryptedData);
        }



        private static string Decrypt(string cipherText, string Password)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            byte[] decryptedData = Decrypt(cipherBytes, pdb.GetBytes(32), pdb.GetBytes(16));
            return System.Text.Encoding.Unicode.GetString(decryptedData);
        }


        public static  string LNEncrypt(string clearText)
        {
            //This is the key in the code
            string KEY1 = DFADFKDFS_KEY + d345908da9083kdjf_KEY + ewuroda0983475drt_KEY;
            //this is the key from the config or call Wayne assembly to get the key
            string KEY2 = "34343434ldkfjadlkjflasdj";
            return Encrypt(Encrypt(clearText, KEY1), KEY2);
        }


        public static  string LNDecrypt(string cipherText)
        {
            //This is the key in the code
            string KEY1 = DFADFKDFS_KEY + d345908da9083kdjf_KEY + ewuroda0983475drt_KEY;
            //this is the key from the config or call Wayne assembly to get the key
            string KEY2 = "34343434ldkfjadlkjflasdj";
            return Decrypt(Decrypt(cipherText, KEY2), KEY1);
        }



    }
}
