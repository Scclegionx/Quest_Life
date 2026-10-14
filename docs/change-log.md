# Nhật ký thay đổi theo ngày

File này ghi lại **mỗi ngày đã tạo, sửa hoặc xóa những file nào** trong dự án, kèm một đoạn tóm tắt ngắn. Ngày theo giờ máy bạn (UTC+7). Ngày mới nhất ở trên cùng.

**Về độ chính xác:** dự án chưa dùng git nên các ngày 12 và 13/10 được dựng lại từ dấu thời gian của file. Với mỗi file chỉ biết được **ngày tạo** và **ngày sửa cuối**, không biết các lần sửa ở giữa. Một số file bị ghi đè nguyên bản có thể hiện ngày tạo lệch (đã chỉnh tay những chỗ mình biết). Danh sách file đã xóa dựa trên những gì đã làm trong các phiên làm việc. Từ 14/10 trở đi, mình sẽ cập nhật file này trực tiếp mỗi khi sửa code. Muốn tự kiểm tra một ngày bất kỳ, chạy `docs/changed-files.sh 2026-10-14`.

---

## 2026-10-14

**Tóm tắt**
- Sửa lỗi nhãn "Number of undefined" trên form tạo task: dữ liệu lĩnh vực gửi xuống JS có tên thuộc tính viết thường (`unit`, `isHours`, `max`) nhưng JS đọc tên viết hoa. Sửa xong thì nhãn đúng ("Number of hours"), ô số giờ nhận được số thập phân (bước 0,25) và có giới hạn tối đa.
- Có chỉnh phần Activity (heatmap) rồi **đã trả về như cũ theo yêu cầu**, nên các file này được "chạm" trong ngày nhưng không đổi nội dung so với hôm qua.
- Thêm trang **Guide** (`/Help`) hướng dẫn toàn bộ tính năng bằng tiếng Anh, công khai không cần đăng nhập: 15 mục (bắt đầu nhanh, tài khoản và múi giờ, nhiệm vụ, thưởng và phạt, nhân vật, bảng xếp hạng, raid, shop, pet, thành tựu, dashboard, profile, điện thoại và cài app, FAQ). Các con số (bảng thưởng theo độ khó, đường cong cấp, giá pet, hệ số chuỗi...) được đọc trực tiếp từ DB/settings nên luôn khớp với game.
- Thêm **nút "?" cố định ở góc dưới trái** mọi trang (ẩn trên chính trang Guide) để mở trang hướng dẫn; trên điện thoại nút nằm phía trên thanh tab dưới.
- Thêm nhật ký thay đổi này.
- Đánh giá khả năng làm trang admin (rollback tiền/EXP/HP/mua đồ), chưa sửa code.
- **Tích hợp Discord, đợt 1** (không dùng đăng nhập bằng Discord): người dùng gõ slash command trong kênh Quest-Life của server Discord. Web nhận lệnh tại `POST /discord/interactions` (kiểm tra chữ ký Ed25519 của Discord) và gọi lại chính các service của website, không có API riêng.
  - Nối tài khoản bằng mã dùng một lần: **Settings → Link Discord** hiện mã (hết hạn sau 10 phút, dùng một lần) → gõ `/link code:...` trong Discord. Bảng mới `discord_links` và `discord_link_codes` (migration `DiscordIntegration`).
  - Lệnh: `/link`, `/unlink`, `/me`, `/quests` (kèm nút ✅ hoàn thành, nút ＋ habit, nút 🔄), `/done` (gợi ý tên khi gõ), `/todo`, `/daily add`, `/habit add|plus|minus`, `/help`. Trả lời dạng embed có màu, riêng tư (ephemeral), tắt hết mention.
  - Bảo vệ: chữ ký + chống phát lại theo thời gian, giới hạn gõ sai mã (5 lần/10 phút), tùy chọn chỉ cho dùng trong một kênh, escape markdown trong tên task.
  - Cấu hình trong mục `Discord` (token để trong user-secrets hoặc biến môi trường, không ghi vào file). Có thể tự đăng ký lệnh lên server khi khởi động.
  - Refactor nhỏ để dùng chung: `TaskBoardService` (lấy danh sách task cho Dashboard và Discord), `TaskFormValidator` (kiểm tra form dùng chung cho web và Discord).
  - Thêm mục "Discord commands" vào trang Guide, và tài liệu cài đặt `docs/discord-setup.md`.
- **Tài liệu triển khai AWS** `docs/deploy-aws-ec2.md`: chọn cấu hình EC2 (khuyên `t3.small`, 2 vCPU / 2 GiB), tạo máy, Elastic IP, tên miền, cài Docker, đưa code lên, file `.env`, chạy bằng Docker Compose, bật Discord, cập nhật, sao lưu, bảo mật, sự cố thường gặp. Kết luận: **không cần đặt biến môi trường trong AWS Console**, chúng nằm trong file `.env` trên máy EC2.
- Viết lại phần 3 của `docs/discord-setup.md` chi tiết hơn: các nguồn cấu hình và thứ tự ưu tiên, mỗi giá trị lấy ở đâu trên Discord, user-secrets, biến môi trường, cách kiểm tra app đã nhận cấu hình, lỗi thường gặp.
- Chuẩn bị cho production: thêm bộ file `docker-compose.prod.yml` (Caddy lo HTTPS + web + db, không mở cổng database), `Caddyfile`, `.env.production.example`; sửa `.dockerignore` (trước đó không loại được `bin/obj` lồng trong `TodoApp/`).
- Sửa lỗi sẽ xảy ra khi chạy trong Docker: **khóa mã hóa đăng nhập không được lưu nên mỗi lần deploy là mọi người bị đăng xuất**. Thêm tùy chọn `DataProtection:KeysPath` (lưu khóa vào volume). Đã kiểm chứng: đăng nhập còn sau khi khởi động lại khi giữ khóa, mất khi đổi thư mục khóa.
- Thêm mục **HTTPS** vào `docs/deploy-aws-ec2.md` (mục 11): vì sao Discord và PWA bắt buộc HTTPS, Caddy tự cấp và gia hạn chứng chỉ Let's Encrypt, điều kiện để cấp được (DNS, cổng 80/443, dùng tên miền chứ không dùng IP), lệnh kiểm tra (`curl`, `openssl`, log Caddy, cookie `Secure`), cách kiểm tra endpoint Discord từ ngoài (phải ra 401), cách kiểm tra PWA, bảng sự cố HTTPS, và tùy chọn nâng cao (CA thử nghiệm để không bị giới hạn, HSTS). Các mục sau được đánh số lại và `discord-setup.md` trỏ sang đúng mục. `Caddyfile` thêm chú thích cho các tùy chọn này.

**File tạo mới**
- `(gốc dự án)/` — `.env.production.example`, `Caddyfile`, `docker-compose.prod.yml`
- `TodoApp/` — `Program.cs`, `TodoApp.csproj`
- `TodoApp/Controllers/` — `DiscordController.cs`, `HelpController.cs`
- `TodoApp/Migrations/` — `20261014092434_DiscordIntegration.Designer.cs`, `20261014092434_DiscordIntegration.cs`
- `TodoApp/Models/Entities/` — `Discord.cs`
- `TodoApp/Services/` — `TaskBoardService.cs`, `TaskFormValidator.cs`
- `TodoApp/Services/Discord/` — `DiscordCommandRegistrar.cs`, `DiscordCommands.cs`, `DiscordFormat.cs`, `DiscordInteractionHandler.cs`, `DiscordLinkService.cs`, `DiscordModels.cs`, `DiscordOptions.cs`, `DiscordSignatureVerifier.cs`
- `TodoApp/Views/Help/` — `Index.cshtml`
- `docs/` — `change-log.md`, `changed-files.sh`, `deploy-aws-ec2.md`, `discord-setup.md`

**File sửa**
- `(gốc dự án)/` — `.dockerignore`
- `TodoApp/` — `appsettings.json`
- `TodoApp/Controllers/` — `AccountController.cs`, `DashboardController.cs`, `TasksController.cs`
- `TodoApp/Data/` — `AppDbContext.cs`
- `TodoApp/Migrations/` — `AppDbContextModelSnapshot.cs`
- `TodoApp/ViewModels/` — `AuthViewModels.cs`
- `TodoApp/Views/Account/` — `Settings.cshtml`
- `TodoApp/Views/Shared/` — `_Layout.cshtml`
- `TodoApp/Views/Tasks/` — `Form.cshtml`
- `TodoApp/wwwroot/css/` — `site.css`
- `TodoApp/Views/Shared/_Heatmap.cshtml`, `TodoApp/Views/Dashboard/Index.cshtml`, `TodoApp/Views/Profile/Index.cshtml` — sửa rồi trả về như cũ theo yêu cầu, **không có thay đổi thực**.

**File xóa:** không có.

---

## 2026-10-13

**Tóm tắt** (ngày làm nhiều nhất)
- **Nhiệm vụ**: habit / daily / todo, độ khó, ưu tiên, hạn chót, lĩnh vực + nỗ lực, lặp theo ngày/tuần/tháng/năm. Thưởng vàng/EXP theo độ khó, hệ số chuỗi, phạt máu tự động, chết thì về cấp 1, sát thương lên raid boss theo chỉ số nhân vật.
- **Tiến trình nền** `PenaltySweeper`: tự trừ máu khi bỏ lỡ daily hoặc quá hạn todo, kể cả khi không ai mở web.
- **Chuẩn hóa database** theo `docs/database-design.md`: chuyển sang PostgreSQL `todo_db`, 34 bảng (nhân vật, túi đồ, pet, sổ cái, thông báo, raid, skill...), danh mục nằm trong DB, mỗi người có múi giờ riêng. Gộp các migration cũ thành `InitialSchema`.
- **Chế độ mock** đổi từ file JSON sang database trong bộ nhớ.
- **Toàn bộ giao diện và dữ liệu danh mục chuyển sang tiếng Anh** (migration `EnglishCatalog`).
- **Trang Players** (tìm người chơi) và **trang Profile** công khai, thêm biểu đồ (Chart.js) và heatmap cho Dashboard/Profile, layout rộng hơn.
- **Giao diện điện thoại**: thanh tab dưới, bảng "More", tab Habits/Dailies/To-Dos, nút "+" nổi, biểu đồ vuốt ngang.
- **PWA**: manifest, service worker, trang offline, bộ icon, banner gợi ý cài đặt.
- **Dữ liệu cứng chuyển vào DB** (migration `ConfigurableGameData`): tên bậc theo cấp, huy hiệu, độ hiếm, múi giờ, 33 số cân bằng trong `game_settings`.

**File tạo mới**
- `TodoApp/Controllers/` — `DashboardController.cs`, `PlayersController.cs`, `ProfileController.cs`, `TasksController.cs`
- `TodoApp/Data/` — `CatalogSeed.cs`, `GameCatalog.cs`, `GameSettings.cs`
- `TodoApp/Migrations/` — `20261013075259_InitialSchema.Designer.cs`, `20261013075259_InitialSchema.cs`, `20261013082301_EnglishCatalog.Designer.cs`, `20261013082301_EnglishCatalog.cs`, `20261013084614_ConfigurableGameData.Designer.cs`, `20261013084614_ConfigurableGameData.cs`, `AppDbContextModelSnapshot.cs`
- `TodoApp/Models/` — `TaskItem.cs`
- `TodoApp/Models/Entities/` — `Catalog.cs`, `Character.cs`, `Raids.cs`, `Users.cs`
- `TodoApp/Models/Game/` — `Stats.cs`
- `TodoApp/Services/` — `ActivityStatsService.cs`, `BadgeService.cs`, `CharacterService.cs`, `ChartJson.cs`, `ClaimsExtensions.cs`, `DemoDataSeeder.cs`, `PenaltySweeper.cs`, `ProfileService.cs`, `RaidService.cs`, `Rewards.cs`, `TaskService.cs`, `TimeZones.cs`
- `TodoApp/ViewModels/` — `ProfileViewModels.cs`, `TaskViewModels.cs`
- `TodoApp/Views/Account/` — `Settings.cshtml`
- `TodoApp/Views/Players/` — `Index.cshtml`
- `TodoApp/Views/Profile/` — `Index.cshtml`
- `TodoApp/Views/Shared/` — `_ActivityCharts.cshtml`, `_Heatmap.cshtml`
- `TodoApp/Views/Tasks/` — `Form.cshtml`
- `TodoApp/wwwroot/` — `manifest.webmanifest`, `offline.html`, `sw.js`
- `TodoApp/wwwroot/icons/` — `apple-touch-icon.png`, `favicon-48.png`, `icon-192.png`, `icon-512.png`, `icon-maskable-512.png`
- `TodoApp/wwwroot/js/` — `charts.js`, `mobile.js`, `pwa.js`
- `TodoApp/wwwroot/lib/` — `chart.umd.min.js`
- `docs/` — `database-design.md`

**File sửa** (đã có từ 12/10, sửa lần cuối ngày 13)
- `(gốc dự án)/` — `Dockerfile`
- `TodoApp/` — `Program.cs`, `TodoApp.csproj`, `appsettings.Development.json`, `appsettings.json`
- `TodoApp/Controllers/` — `AccountController.cs`, `AchievementsController.cs`, `CharacterController.cs`, `GameControllerBase.cs`, `HomeController.cs`, `PetController.cs`, `RaidController.cs`, `ShopController.cs`
- `TodoApp/Data/` — `AppDbContext.cs`
- `TodoApp/Models/` — `ActivityLog.cs`
- `TodoApp/Models/Game/` — `Achievements.cs`, `Items.cs`, `Pets.cs`, `Raid.cs`
- `TodoApp/Services/` — `AchievementService.cs`, `AppTime.cs`, `DataSourceMode.cs`, `Gamification.cs`, `LeaderboardService.cs`, `PetLogic.cs`, `ShopLogic.cs`
- `TodoApp/ViewModels/` — `AuthViewModels.cs`, `DashboardViewModel.cs`, `GamePages.cs`, `GameViewModels.cs`
- `TodoApp/Views/Account/` — `Login.cshtml`, `Register.cshtml`
- `TodoApp/Views/Achievements/` — `Index.cshtml`
- `TodoApp/Views/Character/` — `Index.cshtml`
- `TodoApp/Views/Home/` — `Index.cshtml`
- `TodoApp/Views/Pet/` — `Index.cshtml`
- `TodoApp/Views/Raid/` — `Index.cshtml`
- `TodoApp/Views/Shared/` — `_Layout.cshtml`
- `TodoApp/Views/Shop/` — `Index.cshtml`
- `TodoApp/wwwroot/css/` — `site.css`

**File xóa**
- `TodoApp/Repositories/` (toàn bộ: các interface `IUserRepository`, `ITodoRepository`, `IActivityRepository`, `IPlayerStateRepository`, `IRaidRepository`, `ITaskRepository` và các bản `Ef/`, `Json/`) — chuyển sang dùng `AppDbContext` trực tiếp trong các service.
- `TodoApp/Services/JsonDataSeeder.cs`, `TodoApp/Services/PlayerService.cs` — thay bằng `DemoDataSeeder.cs` và `CharacterService.cs`.
- `TodoApp/Models/User.cs`, `TodoApp/Models/TodoItem.cs`, `TodoApp/Models/Game/PlayerData.cs` — thay bằng các entity trong `Models/Entities/` và `Models/TaskItem.cs`.
- `TodoApp/Controllers/ActivityController.cs` — ô ghi nhận nhanh bị bỏ, hoạt động giờ được ghi khi hoàn thành task.
- `TodoApp/MockData/` (các file JSON demo) — thay bằng database trong bộ nhớ.
- `TodoApp/Migrations/` bản cũ (`InitialCreate`, `AddActivityLogs`, `AddPlayerState`, `TasksAndRaid`) — gộp lại thành `InitialSchema`.
- Một số trang HTML kiểm tra tạm trong `wwwroot/` (`_shot.html`, `_swcheck.html`...) đã tạo ra để chụp màn hình và test rồi xóa ngay.

---

## 2026-10-12

**Tóm tắt**
- Tạo dự án ASP.NET Core MVC (.NET 9) trong `TodoList/TodoApp`, thêm `Dockerfile`, `docker-compose.yml` (web + PostgreSQL), `.env`.
- Kết nối PostgreSQL bằng EF Core (Npgsql), cơ chế chọn nguồn dữ liệu (JSON mock / Database / Auto).
- Đăng ký, đăng nhập, đăng xuất (cookie).
- Giao diện kiểu game tối màu, bảng xếp hạng 5 lĩnh vực (học tập, chơi game, thể thao, không ăn vặt, bỏ thói quen xấu), XP và cấp độ.
- Raid boss hằng tuần, cửa hàng, nhân vật và túi đồ, nuôi pet (ấp trứng, chuồng, lò ấp), danh hiệu và thành tựu.
- Tài liệu thiết kế database để bạn duyệt.

**File tạo mới**
- `(gốc dự án)/` — `.dockerignore`, `.env`, `.env.example`, `.gitignore`, `Dockerfile`, `docker-compose.yml`
- `TodoApp/` — `Program.cs`, `TodoApp.csproj`, `appsettings.Development.json`, `appsettings.json`
- `TodoApp/Controllers/` — `AccountController.cs`, `AchievementsController.cs`, `CharacterController.cs`, `GameControllerBase.cs`, `HomeController.cs`, `PetController.cs`, `RaidController.cs`, `ShopController.cs`
- `TodoApp/Data/` — `AppDbContext.cs`
- `TodoApp/Models/` — `ActivityLog.cs`, `ErrorViewModel.cs`
- `TodoApp/Models/Game/` — `Achievements.cs`, `Items.cs`, `Pets.cs`, `Raid.cs`
- `TodoApp/Properties/` — `launchSettings.json`
- `TodoApp/Services/` — `AchievementService.cs`, `AppTime.cs`, `DataSourceMode.cs`, `Gamification.cs`, `LeaderboardService.cs`, `PetLogic.cs`, `ShopLogic.cs`
- `TodoApp/ViewModels/` — `AuthViewModels.cs`, `DashboardViewModel.cs`, `GamePages.cs`, `GameViewModels.cs`
- `TodoApp/Views/` — `_ViewImports.cshtml`, `_ViewStart.cshtml`
- `TodoApp/Views/Account/` — `Login.cshtml`, `Register.cshtml`
- `TodoApp/Views/Achievements/` — `Index.cshtml`
- `TodoApp/Views/Character/` — `Index.cshtml`
- `TodoApp/Views/Dashboard/` — `Index.cshtml`
- `TodoApp/Views/Home/` — `Index.cshtml`, `Privacy.cshtml`
- `TodoApp/Views/Pet/` — `Index.cshtml`
- `TodoApp/Views/Raid/` — `Index.cshtml`
- `TodoApp/Views/Shared/` — `Error.cshtml`, `_Layout.cshtml`, `_Layout.cshtml.css`, `_ValidationScriptsPartial.cshtml`
- `TodoApp/Views/Shop/` — `Index.cshtml`
- `TodoApp/wwwroot/` — `favicon.ico`
- `TodoApp/wwwroot/css/` — `site.css`
- `TodoApp/wwwroot/js/` — `site.js`

**File sửa / xóa:** không có (ngày đầu của dự án).
