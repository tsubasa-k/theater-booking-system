# 表演場地管理系統 (Performance Venue Booking System)

> 網際網路資料庫課程 (2023) 期末作業


一個使用 ASP.NET WebForms + Microsoft Access 資料庫實作的線上「表演場地預約 + 線上劃位」系統，含三類使用者（管理員 / 表演團體 / 顧客），支援預約、查詢、劃位、Email 通知。

> 📄 想看實際畫面與系統流程介紹，請看 [**REPORT.md**](REPORT.md)。

---

## 系統功能

| 角色 | 可用功能 |
|------|---------|
| **系統管理員** | 管理所有使用者帳密、查看 / 新增 / 刪除預約、會員管理 |
| **表演團體** | 註冊登入、查詢場地出借狀態、預約場地（成功後寄 Email 通知） |
| **顧客** | 註冊登入、查詢場地、線上劃位（含多人同時劃位處理） |

主要頁面：

- `Login.aspx` — 登入
- `Register.aspx` — 註冊（預設 role = `user`，需手動把管理員的 role 在 `Data.accdb` 改成 `admin`）
- `Main.aspx` — 主頁（依登入身分顯示不同選項）
- `Booking.aspx` — 表演場地預約（含查詢與預約）
- `Appoint.aspx` — 線上劃位
- `Membership.aspx` — 會員管理(管理員)
- `ManageApp.aspx` — 預約管理(管理員)

---

## 技術棧

| 類別 | 使用工具 / 版本 |
|------|----------------|
| 框架 | **ASP.NET WebForms**, .NET Framework **4.8.1** |
| 語言 | **C#** |
| 資料庫 | **Microsoft Access** (`.accdb`)，透過 **OleDb (ACE 12.0)** 連線 |
| 前端 | Bootstrap (專案範本附帶) + jQuery + 內嵌 CSS |
| 寄信 | `System.Net.Mail.SmtpClient` 走 **Gmail SMTP** (`smtp.gmail.com:587`, SSL) |
| IDE | **Visual Studio 2022**（含 ASP.NET 與網頁開發 工作負載） |

---

## 專案結構

```
theater-booking-system/
├── README.md
├── .gitignore
├── REPORT.md                       ← 系統介紹報告（含畫面截圖）
├── database/                       ← Access 資料庫（程式預期擺在 D:\Database\）
│   ├── Booking.accdb               ← 預約 / 劃位資料
│   ├── Data.accdb                  ← 使用者帳號資料（已預塞 demo 帳號）
│   └── seed_demo_accounts.sql      ← demo 帳號的 SQL 種子檔
├── images/                         ← 系統畫面截圖（供 REPORT.md 引用）
└── src/
    └── WebForm1/                   ← 主專案
        ├── WebForm1.sln
        └── WebForm1/
            ├── *.aspx, *.aspx.cs   ← 各功能頁面
            ├── Web.config
            ├── packages.config
            └── ...
```

---

## 如何在本機跑起來

### 1. 環境需求

需要事先安裝好以下軟體：

| 軟體 | 用途 | 下載 |
|------|------|------|
| **Visual Studio 2022** Community 以上 | IDE 與內建 IIS Express | https://visualstudio.microsoft.com/zh-hant/downloads/ |
| **ASP.NET 與網頁開發** 工作負載 | 編譯 / 執行 WebForms 專案 | VS Installer 內勾選 |
| **.NET Framework 4.8.1 Developer Pack** | 目標框架 | https://dotnet.microsoft.com/download/dotnet-framework/net481 |
| **Microsoft Access Database Engine 2010 Redistributable** (64-bit, ACE) | OleDb 12.0 連線 .accdb | https://www.microsoft.com/en-us/download/details.aspx?id=13255 |

> ⚠️ **ACE 64-bit vs 32-bit**：若系統已裝 Office 32-bit，安裝 ACE 64-bit 時可能要加 `/passive` 參數。Visual Studio 預設以 IIS Express 64-bit 跑，所以 ACE 也建議裝 64-bit。

### 2. 取得程式碼

```powershell
git clone https://github.com/tsubasa-k/theater-booking-system.git
cd theater-booking-system
```

### 3. 把 Access 資料庫放到程式碼預期的位置

⚠️ **重要**：因為所有 `.cs` 檔案中的連線字串**寫死在 `D:\Database\`**：

```csharp
string Dbc = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=D:\\Database\\Data.accdb";
```

可以進行以下處理：

**方法 A (最簡單)**：在 D 槽建立 `D:\Database\` 資料夾，並把本 repo 的 `database/` 內兩個 .accdb 複製過去：

```powershell
New-Item -ItemType Directory -Force "D:\Database"
Copy-Item ".\database\*.accdb" "D:\Database\"
```

**方法 B (修改路徑)**：用 Visual Studio 的「在檔案中尋找」(Ctrl+Shift+H) 全文置換：

- 尋找：`D:\\Database\\`
- 取代為：想要的路徑（記得 `\` 在 C# 字串中要寫成 `\\`）
- 影響範圍：選「整個方案」

### 4. 設定 Gmail 寄信（可選但建議）

預約成功會寄 Email 通知。為避免帳密外洩，本 repo 把寄件帳密替換成空字串：

```csharp
// Booking.aspx.cs 約 line 406, Appoint.aspx.cs 約 line 304
string fromEmail = "";
string fromPassword = "";
```

要讓寄信功能可用：

1. 用一個 Gmail 帳號登入 → https://myaccount.google.com/security
2. 開啟「兩步驟驗證」(2FA)
3. 在「應用程式密碼」(App passwords) 產生一組 16 字元密碼
4. 在 `Booking.aspx.cs` 與 `Appoint.aspx.cs` 中填入：
   ```csharp
   string fromEmail = "你的email@gmail.com";
   string fromPassword = "16字元應用程式密碼";
   ```

> 💡 **提醒**：別把帶有真實密碼的版本 push 到公開 repo。建議改用 `Web.config` 的 `<appSettings>` 並把該檔案加進 `.gitignore`，或使用使用者祕密 / 環境變數。

### 5. 在 Visual Studio 開啟並執行

1. 打開 `src/WebForm1/WebForm1.sln`
2. **還原 NuGet 套件**：方案總管右鍵方案 → **還原 NuGet 套件 (Restore NuGet Packages)**（首次需要連網下載）
3. 確認設定 `WebForm1` 為起始專案
4. 按 **F5** 啟動偵錯（或 Ctrl+F5 不偵錯啟動）
5. IIS Express 會自動啟動，瀏覽器跳出 `http://localhost:xxxxx/Login.aspx`

### 6. 第一次使用流程

本 repo 的 `database/Data.accdb` 已預先建立 3 個 demo 帳號可直接使用（見下方「Demo 帳號」表）。
若想自己另外註冊新帳號：

1. 進到 `Register.aspx` 註冊一個帳號（預設 role = `user`）
2. 若要把該帳號升級為管理員，需手動用 Microsoft Access 打開 `D:\Database\Data.accdb`，把該帳號的 `role` 欄位改成 `admin`
3. 回 `Login.aspx` 登入

---

## Demo 帳號

`database/Data.accdb` 已預先塞入以下 3 個帳號，clone 並依步驟 3 把 .accdb 放到 `D:\Database\` 後即可直接登入測試：

| 帳號 | 密碼 | 角色 | 用途 |
|------|------|------|------|
| `admin` | `Admin@2023` | admin | 登入後可進入「會員管理」「預約管理」等管理頁面 |
| `demo_user` | `User@2023` | user | 測試線上劃位 (Appoint.aspx) |
| `demo_performer` | `Perf@2023` | user | 測試場地預約 (Booking.aspx)，username 標示為「示範表演團體」 |

> ⚠️ 這些密碼是明文儲存（受限於原專案設計，見下方「問題與限制」），僅供本機 demo / 課堂展示，請不要在正式環境使用相同帳密。
>
> 對應的 SQL 與重新匯入方式詳見 [`database/seed_demo_accounts.sql`](database/seed_demo_accounts.sql)。

---

## 問題與限制

因為這是 2023 年的課程project作業，由於只是簡單的資料庫系統，並未詳細考慮關於資安上的問題，因此可能會有以下的問題與限制：

| # | 議題 | 說明 |
|---|------|------|
| 1 | **明文密碼** | 註冊時直接以明文存到 `Data.password` 欄位，未做雜湊（應改用 `PBKDF2` / `bcrypt` / `Argon2`） |
| 2 | **SQL Injection 風險** | 多處查詢用字串拼接（如 `Login.aspx.cs` 的 `WHERE account = '" + AC + "'`），應一律改用 `Parameters.AddWithValue` |
| 3 | **連線字串硬編碼** | `D:\Database\` 寫死在每個 .cs，應改放 `Web.config` `<connectionStrings>` |
| 4 | **寄信憑證硬編碼** | 已於是先在此 repo 移除 Gmail 應用程式密碼，需要使用者自行填入 |
| 5 | **無 HTTPS / CSRF 防護** | WebForms 預設的 `EnableViewStateMac` 只擋部分情境，敏感操作建議加 `<%@ AntiForgeryToken %>` |
| 6 | **無並發控制細節** | 線上劃位雖題目要求考慮多人同時劃位，實作上應加上 transaction / row-level lock 才完整 |
