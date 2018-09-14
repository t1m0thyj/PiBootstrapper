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

            //little magic here
            //Rfc2898DeriveBytes class has restriction of salt size to >= 8
            //but rfc2898 not (see http://www.ietf.org/rfc/rfc2898.txt)
            //we use Reflection to setup private field to avoid this restriction

            if (saltBytes.Length >= 8)
                pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 4096);
            else
            {
                //use dummy salt here, we replace it later vie reflection
                pbkdf2 = new Rfc2898DeriveBytes(password, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 4096);

                var saltField = typeof(Rfc2898DeriveBytes).GetField("m_salt", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
                saltField.SetValue(pbkdf2, saltBytes);
            }

            //get 256 bit PMK key
            byte[] outBytes = pbkdf2.GetBytes(32);
            return BitConverter.ToString(outBytes).Replace("-", "").ToLower();
        }

        public static string GetConfig(string networkName, string password)
        {
            List<string> config = new List<string>()
            {
                "network={",
                "\tssid=\"" + networkName + "\"",
                "\tscan_ssid=1",
                "\tkey_mgmt=WPA-PSK",
                "\tpsk=" + ComputeHash(password, networkName),
                "}"
            };

            return String.Join("\n", config);
        }
    }
}
