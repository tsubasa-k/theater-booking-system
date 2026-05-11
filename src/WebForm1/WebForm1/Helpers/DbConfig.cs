using System.Configuration;

namespace WebForm1.Helpers
{
    /// <summary>
    /// 集中存放兩個資料庫連線字串，所有 .aspx.cs 透過此類別取得。
    /// 連線字串本身存放於 Web.config 的 &lt;connectionStrings&gt; 區段。
    /// </summary>
    public static class DbConfig
    {
        /// <summary>
        /// 使用者帳號資料庫 (Data.accdb) 的連線字串。
        /// </summary>
        public static string DataDb =>
            ConfigurationManager.ConnectionStrings["DataDb"].ConnectionString;

        /// <summary>
        /// 預約 / 劃位資料庫 (Booking.accdb) 的連線字串。
        /// </summary>
        public static string BookingDb =>
            ConfigurationManager.ConnectionStrings["BookingDb"].ConnectionString;
    }
}
