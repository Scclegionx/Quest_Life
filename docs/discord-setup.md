# Cài đặt tích hợp Discord (đợt 1)

Mục tiêu: người dùng gõ slash command trong kênh Quest-Life của server Discord (ví dụ `/quests`, `/todo`, `/done`) để thao tác với QuestLife, bot trả về tin nhắn có màu sắc và nút bấm. **Không dùng đăng nhập bằng Discord**: tài khoản được nối bằng mã dùng một lần (Settings → Link Discord → `/link`).

## 1. Cách hoạt động (để hiểu trước khi cài)

- Discord gọi thẳng vào web app tại **`POST /discord/interactions`** mỗi khi có người gõ lệnh, bấm nút hoặc gõ dở để gợi ý. Không có tiến trình bot chạy riêng, và không có API riêng: endpoint này gọi đúng các service mà website đang dùng.
- Mỗi yêu cầu được Discord **ký (Ed25519)**. App kiểm tra chữ ký bằng *Public Key*; sai chữ ký thì trả 401. Nhờ đó Discord user ID trong lệnh là thật, không giả được.
- Bảng `discord_links` nối *Discord user ID ↔ tài khoản QuestLife*. Mã liên kết nằm ở bảng `discord_link_codes` (8 ký tự, hết hạn sau 10 phút, dùng một lần, sai 5 lần trong 10 phút thì bị chặn tạm).
- Mọi trả lời đều là **ephemeral** (chỉ người gõ lệnh thấy) và **tắt hết mention** (`@everyone` trong tên task không ping được ai).
- Discord yêu cầu trả lời trong **3 giây**; các lệnh hiện tại đều nhanh hơn nhiều.

## 2. Việc bạn làm trên Discord (khoảng 10 phút)

1. Vào <https://discord.com/developers/applications> → **New Application** → đặt tên `QuestLife`.
2. Tab **General Information**: copy **Application ID** và **Public Key**.
3. Tab **Bot**: bấm **Reset Token** và copy **token** (chỉ hiện một lần). Không cần bật Privileged Intents nào.
4. Tab **OAuth2 → URL Generator**: chọn scope **`bot`** và **`applications.commands`** (chưa cần quyền gì thêm), mở URL được tạo, chọn server *Friend Group* và đồng ý. (Cần quyền *Manage Server*.)
5. Bật **Developer Mode** (User Settings → Advanced), rồi chuột phải vào server → **Copy Server ID**, chuột phải vào kênh Quest-Life → **Copy Channel ID**.

> **Bảo mật:** token là mật khẩu của bot. Không dán vào chat, không commit lên git. Nếu lộ, vào tab Bot bấm Reset Token.

## 3. Cấu hình trong web app (chi tiết)

### 3.1 "Cấu hình" ở đây là gì?

Web app cần biết vài giá trị: app Discord của bạn là app nào (`ApplicationId`), khóa để kiểm tra chữ ký (`PublicKey`), token của bot... Những giá trị này **không nằm trong code**. App ASP.NET Core đọc chúng từ nhiều nguồn, **nguồn đứng sau ghi đè nguồn đứng trước**:

| # | Nguồn | Dùng khi nào | Có bị đưa lên git không? |
|---|---|---|---|
| 1 | `appsettings.json` | Giá trị mặc định (mục `Discord` đang để trống, `Enabled=false`) | Có, nên **không bao giờ** ghi token vào đây |
| 2 | `appsettings.Development.json` / `appsettings.Production.json` | Giá trị riêng cho từng môi trường (không bí mật) | Có |
| 3 | **user-secrets** | Chỉ khi chạy trên máy bạn (môi trường Development, ví dụ F5 hoặc `dotnet run`) | **Không** (lưu ngoài thư mục dự án) |
| 4 | **Biến môi trường** | Khi chạy Docker / trên server EC2 | Không |
| 5 | Tham số dòng lệnh | Hiếm dùng | Không |

Tóm lại: **trên máy bạn dùng user-secrets, trên server dùng biến môi trường (qua file `.env`)**. Hai cách đặt cùng một giá trị, chỉ khác tên viết:

| Khóa | Viết trong user-secrets | Viết thành biến môi trường |
|---|---|---|
| `Discord` → `PublicKey` | `Discord:PublicKey` (dấu hai chấm) | `Discord__PublicKey` (**hai** dấu gạch dưới) |

### 3.2 Mỗi giá trị lấy ở đâu

| Khóa | Lấy ở đâu | Bắt buộc? |
|---|---|---|
| `Enabled` | Bạn tự đặt `true` để bật tích hợp | Có |
| `PublicKey` | Developer Portal → app của bạn → **General Information** → *Public Key* (chuỗi **64 ký tự** gồm 0-9 và a-f) | **Có** (thiếu thì tích hợp coi như chưa bật) |
| `ApplicationId` | Cùng trang **General Information** → *Application ID* (dãy số dài) | Chỉ khi đăng ký lệnh |
| `BotToken` | **Bot** → *Reset Token* → copy (**chỉ hiện một lần**) | Chỉ khi đăng ký lệnh |
| `GuildId` | Bật Developer Mode → chuột phải tên server → *Copy Server ID* | Nên có (lệnh cập nhật ngay) |
| `AllowedChannelId` | Chuột phải kênh Quest-Life → *Copy Channel ID* | Không (bỏ trống = dùng ở mọi kênh) |
| `WebBaseUrl` | Địa chỉ web của bạn, ví dụ `https://questlife.example.com` | Không (để tiêu đề `/me` bấm được) |
| `RegisterCommandsOnStartup` | Bạn tự đặt `true`/`false` | Không (xem 3.6) |

### 3.3 Cách A: chạy trên máy bạn bằng user-secrets (khuyên dùng khi phát triển)

User-secrets là một file riêng của bạn nằm ngoài dự án (`~/.microsoft/usersecrets/<id>/secrets.json`), nên không thể vô tình commit. Dự án đã có sẵn mã định danh cho nó. Mở terminal:

```bash
cd "TodoList/TodoApp"

dotnet user-secrets set "Discord:Enabled" "true"
dotnet user-secrets set "Discord:PublicKey" "dán-public-key-64-ký-tự"
dotnet user-secrets set "Discord:ApplicationId" "dán-application-id"
dotnet user-secrets set "Discord:BotToken" "dán-bot-token"
dotnet user-secrets set "Discord:GuildId" "dán-server-id"
dotnet user-secrets set "Discord:AllowedChannelId" "dán-channel-id"        # tùy chọn
dotnet user-secrets set "Discord:WebBaseUrl" "https://địa-chỉ-web"          # tùy chọn
dotnet user-secrets set "Discord:RegisterCommandsOnStartup" "true"
```

Xem lại những gì đã đặt, sửa hoặc xóa:

```bash
dotnet user-secrets list                       # liệt kê (hiện cả giá trị, đừng chụp màn hình gửi người khác)
dotnet user-secrets set "Discord:GuildId" "123"   # đặt lại = ghi đè
dotnet user-secrets remove "Discord:BotToken"     # xóa một giá trị
dotnet user-secrets clear                         # xóa hết
```

Lưu ý quan trọng:
- Giá trị trong dấu nháy kép, **không có khoảng trắng hoặc xuống dòng thừa** ở đầu/cuối (lỗi hay gặp khi copy token).
- user-secrets **chỉ có tác dụng khi chạy Development** (mặc định khi bấm F5 hoặc `dotnet run`). Khi chạy Docker hay trên server chúng **không** đi theo; ở đó dùng biến môi trường.
- Mỗi lần đổi giá trị phải **khởi động lại app** (cấu hình chỉ đọc lúc app khởi động).

### 3.4 Cách B: biến môi trường tạm thời (thử nhanh, không lưu lại)

```bash
# Linux / macOS (một dòng, hai gạch dưới)
Discord__Enabled=true Discord__PublicKey=abc123... dotnet run

# Windows PowerShell
$env:Discord__Enabled = "true"
$env:Discord__PublicKey = "abc123..."
dotnet run
```

### 3.5 Cách C: trên server (Docker / AWS EC2)

Bạn **không sửa appsettings và không cần đặt biến môi trường trên giao diện AWS**. Trên server bạn viết các giá trị vào file `.env` (bản mẫu: `.env.production.example`), ví dụ:

```bash
DISCORD_ENABLED=true
DISCORD_PUBLIC_KEY=abc123...
DISCORD_APPLICATION_ID=123456789012345678
DISCORD_BOT_TOKEN=...
DISCORD_GUILD_ID=...
DISCORD_ALLOWED_CHANNEL_ID=...
DISCORD_REGISTER_COMMANDS=true
```

File `docker-compose.prod.yml` đọc các dòng `DISCORD_*` đó và chuyển thành biến môi trường `Discord__...` cho container web. Các bước đầy đủ nằm ở [deploy-aws-ec2.md](deploy-aws-ec2.md) (mục 9 và 12).

### 3.6 `RegisterCommandsOnStartup` hoạt động thế nào

- `true`: mỗi lần app khởi động, nó gửi danh sách 9 lệnh lên Discord (cần `ApplicationId` và `BotToken`). Với `GuildId` có giá trị, lệnh **xuất hiện ngay** trong server đó.
- Chỉ cần bật **lần đầu** và **mỗi khi danh sách lệnh thay đổi** (sau khi cập nhật phiên bản app). Bình thường để `false`.
- Thấy dòng log `Discord: registered 9 slash commands (guild ...)` là thành công.

### 3.7 Kiểm tra app đã nhận cấu hình chưa

| Cách kiểm tra | Kết quả đúng |
|---|---|
| Đăng nhập web → **Settings** → thẻ **Link Discord** | Hiện nút **Get a link code**. Nếu hiện "Discord integration is not enabled on this server" tức là `Enabled` chưa `true` hoặc `PublicKey` đang trống |
| Log khi khởi động (với `RegisterCommandsOnStartup=true`) | `Discord: registered 9 slash commands ...` |
| Developer Portal → bấm **Save** ở *Interactions Endpoint URL* | Lưu được (nếu báo lỗi: xem bảng bên dưới) |

### 3.8 Lỗi cấu hình thường gặp

| Hiện tượng | Nguyên nhân | Cách sửa |
|---|---|---|
| Thẻ Link Discord báo "not enabled" | `Enabled` không phải `true`, hoặc `PublicKey` trống | Đặt lại hai giá trị, **khởi động lại app** |
| Log `Discord request rejected: invalid signature` / Portal không lưu được URL | `PublicKey` sai (nhầm với Application ID hay token), thừa khoảng trắng, hoặc thiếu ký tự | Copy lại *Public Key* (đúng 64 ký tự hex: `echo -n "$KEY" \| wc -c` ra 64) |
| Log `ApplicationId and BotToken are required ... Skipped` | Bật đăng ký lệnh nhưng thiếu một trong hai | Điền đủ rồi khởi động lại |
| Log `registering commands failed with 401` | Bot token sai hoặc đã bị Reset | Reset Token lấy cái mới |
| Log `registering commands failed with 403/404` | `GuildId` sai, hoặc bot/app chưa được mời vào server đó | Kiểm tra Server ID, mời lại bằng scope `bot` + `applications.commands` |
| Đổi cấu hình mà không thấy tác dụng | App chưa được khởi động lại, hoặc giá trị đang bị nguồn ưu tiên cao hơn ghi đè (ví dụ biến môi trường ghi đè user-secrets) | Khởi động lại; kiểm tra các nguồn theo thứ tự ở 3.1 |

## 4. Cho Discord gọi được vào web app

Discord cần một địa chỉ **HTTPS công khai**.

- **Sau khi deploy AWS:** địa chỉ là `https://<tên-miền>/discord/interactions`. Cách bật HTTPS và kiểm tra endpoint nằm ở [deploy-aws-ec2.md](deploy-aws-ec2.md) mục 11.
- **Thử trên máy bạn:** chạy app (`dotnet run`) rồi mở đường hầm, ví dụ `ngrok http 5138` hoặc `cloudflared tunnel --url http://localhost:5138`, và dùng địa chỉ HTTPS nó cấp.

Vào trang **General Information** của application, dán địa chỉ vào **Interactions Endpoint URL** rồi **Save**. Discord sẽ gửi một yêu cầu kiểm tra có chữ ký; app phải đang chạy với `Discord:PublicKey` đúng thì mới lưu được.

## 5. Dùng thử

1. Chạy app (lần đầu với `RegisterCommandsOnStartup=true`); log sẽ ghi `Discord: registered 9 slash commands`.
2. Đăng nhập web → **Settings → Link Discord → Get a link code**.
3. Trong kênh Quest-Life gõ `/link`, dán mã. Bạn nhận thông báo xanh "Linked!".
4. Thử `/quests`, `/me`, `/todo title:...`, `/done`.

## 6. Các lệnh

| Lệnh | Việc làm |
|---|---|
| `/link code` , `/unlink` | Nối / hủy nối tài khoản |
| `/me` | Thẻ nhân vật (cấp, HP, mana, EXP, chỉ số, vàng, chuỗi ngày, pet đồng hành) |
| `/quests` | Danh sách habit/daily/to-do kèm nút ✅ hoàn thành, nút ＋ cho habit, nút 🔄 |
| `/done task` | Hoàn thành daily hoặc to-do (gõ để gợi ý tên) |
| `/todo title [difficulty] [priority] [category] [effort] [deadline] [description]` | Tạo to-do. `deadline`: `today`, `tomorrow`, `+3d`, `YYYY-MM-DD` |
| `/daily add title [repeat] [difficulty] [priority] [description]` | Tạo việc định kỳ |
| `/habit add`, `/habit plus`, `/habit minus` | Tạo habit, bấm + hoặc − |
| `/help` | Danh sách lệnh |

## 7. Gặp lỗi thì xem

| Hiện tượng | Nguyên nhân thường gặp |
|---|---|
| Không lưu được Interactions Endpoint URL | App chưa chạy, đường hầm sai, `Discord:Enabled`/`PublicKey` chưa đặt hoặc sai |
| Gõ `/` không thấy lệnh | Chưa đăng ký lệnh (xem log), sai `GuildId`, bot chưa được mời vào server bằng scope `applications.commands` |
| "The application did not respond" | Endpoint không tới được, hoặc server trả lời quá 3 giây, hoặc lỗi chữ ký (kiểm tra `PublicKey`) |
| "Link your account first" | Chưa `/link`, hoặc đã `/unlink` |
| "That code is not valid" | Mã hết hạn (10 phút), đã dùng, hoặc gõ sai; tạo mã mới |
| "Wrong channel" | Đang đặt `AllowedChannelId`; dùng đúng kênh hoặc bỏ cấu hình đó |

## 8. Đã kiểm tra những gì

Đã chạy 54 kiểm tra tự động trên Postgres thật bằng các yêu cầu **mô phỏng Discord có ký bằng khóa Ed25519 thử nghiệm**: chữ ký đúng/sai/quá hạn, luồng liên kết từ web, toàn bộ lệnh, nút bấm, gợi ý tự động, kiểm tra đầu vào, giới hạn số lần nhập sai mã, giới hạn theo kênh; và việc đăng ký lệnh lên một **máy chủ Discord giả** (đường dẫn, header `Bot <token>`, 9 lệnh đều đạt các quy tắc của Discord).

**Chưa thử với Discord thật** (cần tài khoản/token của bạn), nên lần chạy thật đầu tiên có thể lộ ra sai khác nhỏ; nếu có thì gửi mình thông báo lỗi.

## 9. Việc của các đợt sau

Đợt 2: `/raid`, `/leaderboard`, `/profile`, thông báo chủ động lên kênh (boss bị hạ, lên cấp, bị trừ máu) bằng bảng `notifications` có sẵn. Đợt 3: shop, pet, thẻ nhân vật dạng ảnh, bản tin tuần.
