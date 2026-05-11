using System;
using System.Security.Cryptography;
using System.Text;

namespace WebForm1.Helpers
{
    /// <summary>
    /// PBKDF2 (SHA-256) 密碼雜湊 + 懶惰遷移工具。
    ///
    /// 儲存格式 (string)：
    ///   v1${iterations}${base64(salt)}${base64(hash)}
    ///
    /// 例：
    ///   v1$100000$Tm9TYWx0SXNGYWtl$abc1234...
    ///
    /// 「懶惰遷移」：登入時若資料庫存的不是上述格式 (假設是舊的明文密碼)，
    /// 程式仍以常數時間比對。比對成功後立刻把明文升級為雜湊存回 DB，
    /// 不需要中斷性遷移 script。
    /// </summary>
    public static class PasswordHelper
    {
        private const string FormatPrefix = "v1$";
        private const int SaltBytes = 16;
        private const int HashBytes = 32;
        private const int DefaultIterations = 100_000;

        /// <summary>
        /// 雜湊一個明文密碼供寫入 DB。
        /// </summary>
        public static string Hash(string plain)
        {
            if (plain == null) throw new ArgumentNullException(nameof(plain));
            byte[] salt = new byte[SaltBytes];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            byte[] hash = Pbkdf2(plain, salt, DefaultIterations, HashBytes);
            return $"{FormatPrefix}{DefaultIterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        }

        /// <summary>
        /// 比對使用者輸入的明文與資料庫存的字串 (可能是雜湊或舊明文)。
        /// </summary>
        /// <param name="plain">使用者輸入</param>
        /// <param name="stored">資料庫中的值</param>
        /// <param name="isLegacyPlain">true 表示 stored 是舊明文 (需要呼叫端後續升級)</param>
        public static bool Verify(string plain, string stored, out bool isLegacyPlain)
        {
            isLegacyPlain = false;
            if (plain == null || stored == null) return false;

            if (stored.StartsWith(FormatPrefix))
            {
                return VerifyHashed(plain, stored);
            }

            // 舊明文：常數時間比對，避免 timing attack
            isLegacyPlain = true;
            return ConstantTimeEquals(
                Encoding.UTF8.GetBytes(plain),
                Encoding.UTF8.GetBytes(stored));
        }

        private static bool VerifyHashed(string plain, string stored)
        {
            try
            {
                var parts = stored.Split('$');
                // 格式：["v1", "100000", "<salt>", "<hash>"]
                if (parts.Length != 4) return false;
                int iter = int.Parse(parts[1]);
                byte[] salt = Convert.FromBase64String(parts[2]);
                byte[] expected = Convert.FromBase64String(parts[3]);
                byte[] actual = Pbkdf2(plain, salt, iter, expected.Length);
                return ConstantTimeEquals(actual, expected);
            }
            catch
            {
                return false;
            }
        }

        private static byte[] Pbkdf2(string password, byte[] salt, int iterations, int outputLen)
        {
            // .NET Framework 4.8 沒有 Rfc2898DeriveBytes.Pbkdf2 靜態方法，
            // 改用建構式版，並指定 HashAlgorithmName.SHA256。
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(outputLen);
            }
        }

        private static bool ConstantTimeEquals(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }
            return diff == 0;
        }
    }
}
