using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SP_FTP_Manager.Helper
{
    public static class PasswordCipher
    {
        public static byte[] Encrypt(string password)
        {
            return ProtectedData.Protect(UTF8Encoding.UTF8.GetBytes(password??""), null, DataProtectionScope.CurrentUser);
        }

        public static string Decrypt(byte[] encrypted)
        {
            if(encrypted==null)
            {
                return "";
            }
            return UTF8Encoding.UTF8.GetString(ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser));
        }
    }
}
