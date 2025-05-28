using System.Security.Cryptography;
using System.Text;

namespace McpNetwork.Charli.Server.Helpers
{
    internal sealed class SecurityHelper
    {
        public static string HashPassword(string password, out string securityCode)
        {
            byte[] salt;
            byte[] buffer2;
            ArgumentNullException.ThrowIfNull(password);

            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }

            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);

            var newSecurityCode = ByteToString(salt);
            newSecurityCode = newSecurityCode.ToLower();
            securityCode = string.Format("{0}-{1}-{2}-{3}-{4}", newSecurityCode.Substring(0, 8), newSecurityCode.Substring(8, 4), newSecurityCode.Substring(12, 4), newSecurityCode.Substring(16, 4), newSecurityCode.Substring(20, 12));
            return Convert.ToBase64String(dst);
        }

        public static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            if (hashedPassword == null)
            {
                return false;
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            byte[] array = Convert.FromBase64String(hashedPassword);
            if (array.Length != 49 || array[0] != 0)
            {
                return false;
            }
            byte[] array2 = new byte[16];
            Buffer.BlockCopy(array, 1, array2, 0, 16);
            byte[] array3 = new byte[32];
            Buffer.BlockCopy(array, 17, array3, 0, 32);
            byte[] bytes;
            using (Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, array2, 1000))
            {
                bytes = rfc2898DeriveBytes.GetBytes(32);
            }

            return ByteArraysEqual(array3, bytes);
        }

        private static string ByteToString(byte[] buff)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < buff.Length; i++)
            {
                sb.Append(buff[i].ToString("X2")); // hex format
            }
            var fctResult = sb.ToString();
            return fctResult;

        }

        private static bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }
            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }
            bool flag = true;
            for (int i = 0; i < a.Length; i++)
            {
                flag &= a[i] == b[i];
            }
            return flag;
        }
    }
}
