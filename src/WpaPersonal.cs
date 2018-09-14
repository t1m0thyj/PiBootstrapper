using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace PiBootstrapper
{
    class WpaPersonal
    {
        private static string ComputeHash(string password, string salt)
        {
            // Setup the password generator
            byte[] salt = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            Rfc2898DeriveBytes pwdGen = new Rfc2898DeriveBytes("P@$$w0rd", salt, 1000);

            // generate an RC2 key
            byte[] key = pwdGen.GetBytes(16);
            byte[] iv = pwdGen.GetBytes(8);

            // setup an RC2 object to encrypt with the derived key
            RC2CryptoServiceProvider rc2 = new RC2CryptoServiceProvider();
            rc2.Key = key;
            rc2.IV = iv;

            // now encrypt with it
            byte[] plaintext = Encoding.UTF8.GetBytes("Message");
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, rc2.CreateEncryptor(), CryptoStreamMode.Write);

            cs.Write(plaintext, 0, plaintext.Length);
            cs.Close();
            byte[] encrypted = ms.ToArray();
        }

        public static string GetConfig(string networkName, string password)
        {
            List<string> config = new List<string>()
            {
                "network={",
                "\tssid=\"" + networkName + "\"",
                "\tpsk=\"" + password + "\"",
                "\tkey_mgmt=WPA-PSK",
                "}"
            };

            return String.Join("\n", config);
        }
    }
}
