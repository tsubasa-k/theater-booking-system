using System;
using System.Collections.Generic;

namespace WebForm1.Helpers
{
    /// <summary>
    /// 場地顯示名稱（中文）與資料庫表名 (venue1/2/3) 之間的安全映射。
    /// 用白名單避免任何未來不小心把使用者輸入直接拼到 SQL table name。
    /// </summary>
    public static class VenueHelper
    {
        // 表名白名單。任何不在此清單中的字串都不被允許進入 SQL。
        private static readonly HashSet<string> AllowedTables = new HashSet<string>
        {
            "venue1", "venue2", "venue3"
        };

        // 中文顯示名稱 -> 表名
        private static readonly Dictionary<string, string> DisplayToTable = new Dictionary<string, string>
        {
            { "場地1", "venue1" },
            { "場地2", "venue2" },
            { "場地3", "venue3" }
        };

        // 表名 -> 容量（座位數）
        private static readonly Dictionary<string, int> TableToSeats = new Dictionary<string, int>
        {
            { "venue1", 49 },
            { "venue2", 80 },
            { "venue3", 64 }
        };

        /// <summary>
        /// 安全把顯示名稱轉成資料表名。未知值會 fallback 到 venue1。
        /// </summary>
        public static string GetTable(string displayName)
        {
            if (displayName != null && DisplayToTable.TryGetValue(displayName, out var table))
                return table;
            return "venue1";
        }

        /// <summary>
        /// 取得某場地的座位數。
        /// </summary>
        public static int GetSeatCount(string tableName)
        {
            return TableToSeats.TryGetValue(tableName, out var n) ? n : 49;
        }

        /// <summary>
        /// 把表名插入 SQL 之前必呼叫此方法防禦：未通過白名單即拋例外。
        /// </summary>
        public static string AssertSafeTable(string tableName)
        {
            if (tableName == null || !AllowedTables.Contains(tableName))
                throw new ArgumentException($"Unsafe table name: '{tableName}'");
            return tableName;
        }
    }
}
