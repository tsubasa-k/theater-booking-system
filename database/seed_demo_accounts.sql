-- ============================================================
-- 表演場地管理系統 — Demo 帳號 SQL
-- 適用於 Microsoft Access (.accdb)，目標: D:\Database\Data.accdb 的 Data 資料表
-- 表結構: Num (long), account, password, username, role, email
--
--    這些密碼是明文儲存（受限於原專案設計），僅供本機 demo 展示，
--    請不要在正式環境使用相同帳密。
-- ============================================================

-- 1. 管理員帳號（登入後可看到 Membership / ManageApp 等管理頁面）
INSERT INTO Data (Num, account, [password], username, role, email)
VALUES (1, 'admin', 'Admin@2023', '系統管理員', 'admin', 'admin@example.com');

-- 2. 一般顧客帳號（用於測試線上劃位 Appoint.aspx）
INSERT INTO Data (Num, account, [password], username, role, email)
VALUES (2, 'demo_user', 'User@2023', '示範顧客', 'user', 'user@example.com');

-- 3. 表演團體帳號（用於測試場地預約 Booking.aspx）
INSERT INTO Data (Num, account, [password], username, role, email)
VALUES (3, 'demo_performer', 'Perf@2023', '示範表演團體', 'user', 'performer@example.com');

-- ============================================================
-- 如何手動執行
-- ============================================================
-- 方法 A：用 Microsoft Access 開啟 D:\Database\Data.accdb →「建立」→「查詢設計」
--         →「SQL 檢視」→ 貼上單一 INSERT → 執行（一次只能跑一條）
--
-- 方法 B：本 repo 已將以上 3 筆預先塞進 database/Data.accdb，
--         clone 後依 README 步驟 3 把 .accdb 複製到 D:\Database\ 即可直接使用
--
-- 方法 C：用 PowerShell 重新塞入（先清空後重塞）：
--   $conn = New-Object System.Data.OleDb.OleDbConnection
--   $conn.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=D:\Database\Data.accdb"
--   $conn.Open()
--   $c = $conn.CreateCommand(); $c.CommandText = "DELETE FROM Data"; [void]$c.ExecuteNonQuery()
--   # ...再用 $c.CommandText = 上面的 INSERT 語句逐一 ExecuteNonQuery()
--   $conn.Close()
