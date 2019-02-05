using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Security.Cryptography;

namespace PiBootstrapper
{
    class WpaPersonal
    {
        private static string ComputeHash(string password, string networkName)
        {
            // Code based on https://stackoverflow.com/a/28631507/5504760
            Rfc2898DeriveBytes pbkdf2;
            byte[] saltBytes = Encoding.ASCII.GetBytes(networkName);

            // Rfc2898DeriveBytes class has restriction of salt size to >= 8
            // but not in rfc2898 (see http://www.ietf.org/rfc/rfc2898.txt)
            // Reflection used to setup private field to avoid the restriction
            if (saltBytes.Length >= 8)
                pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 4096);
            else
            {
                // Use dummy salt here, we replace it later via reflection
                pbkdf2 = new Rfc2898DeriveBytes(password, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 4096);

                var saltField = typeof(Rfc2898DeriveBytes).GetField("m_salt", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
                saltField.SetValue(pbkdf2, saltBytes);
            }

            // Get 256 bit PMK key
            byte[] outBytes = pbkdf2.GetBytes(32);
            return BitConverter.ToString(outBytes).Replace("-", "").ToLower();
        }

        public static string GetConfig(string networkName, string password, bool shouldEncrypt)
        {
            if (shouldEncrypt)
            {
                password = ComputeHash(password, networkName);
            }
            else
            {
                password = '"' + password.Replace("\"", "\\\"") + '"';
            }

            string[] config = new string[]
            {
                "network={",
                "\tssid=\"" + networkName + "\"",
                "\tscan_ssid=1",
                "\tkey_mgmt=WPA-PSK",
                "\tpsk=" + password,
                "}"
            };

            return string.Join("\n", config);
        }
    }
}
