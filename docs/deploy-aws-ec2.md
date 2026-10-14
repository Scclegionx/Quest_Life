# Triển khai QuestLife lên AWS (một máy EC2)

Hướng dẫn từng bước để đưa web app lên một máy ảo EC2, chạy bằng Docker, có HTTPS và tên miền. Làm theo thứ tự từ trên xuống. Thời gian khoảng 1–2 giờ cho lần đầu.

## 1. Tổng quan

```
Người dùng / Discord
        │ HTTPS (443)
        ▼
 ┌──────────── máy EC2 (Ubuntu) ─────────────────────────────┐
 │  Caddy  ──►  web (ASP.NET, cổng 8080 nội bộ)  ──►  db (PostgreSQL) │
 │  (tự lấy chứng chỉ HTTPS)      (chạy migration lúc khởi động)       │
 └────────────────────────────────────────────────────────────┘
```

- **Caddy**: nhận mọi truy cập từ internet, tự xin và gia hạn chứng chỉ HTTPS miễn phí (Let's Encrypt), chuyển tiếp vào web app. Discord và PWA đều bắt buộc HTTPS nên cần phần này.
- **web**: chính web app. Khi khởi động nó tự tạo/cập nhật bảng trong database (migration).
- **db**: PostgreSQL, dữ liệu nằm trong một Docker volume. **Không mở cổng ra ngoài**, chỉ web app nối vào được.
- Cả ba chạy bằng một lệnh `docker compose` (file `docker-compose.prod.yml`).

**Bạn cần có:** tài khoản AWS (cần thẻ thanh toán), một **tên miền** (hoặc subdomain miễn phí, xem mục 5), và máy tính của bạn có `ssh` và `rsync`.

**Chi phí tham khảo** (giá thay đổi theo vùng và thời điểm, hãy kiểm tra trang giá của AWS): máy `t3.small` khoảng 15–20 USD/tháng, ổ đĩa 30 GB khoảng 2–3 USD/tháng, địa chỉ IPv4 công khai khoảng 3–4 USD/tháng. Tài khoản AWS mới có thể có tín dụng hoặc free tier; xem mục Billing.

## 2. Chọn cấu hình máy EC2

| Lựa chọn | CPU / RAM | Nhận xét |
|---|---|---|
| **`t3.small`** (khuyên dùng) | 2 vCPU / 2 GiB | Đủ thoải mái cho web + PostgreSQL + Caddy và vẫn build được image Docker ngay trên máy |
| `t3.micro` | 2 vCPU / 1 GiB | Chạy được cho vài người dùng, nhưng khi build image dễ hết RAM. **Bắt buộc thêm swap** (mục 7) |
| `t3.medium` | 2 vCPU / 4 GiB | Dư dả hơn nếu có nhiều người dùng cùng lúc hoặc muốn chạy thêm dịch vụ khác |

- Dùng loại máy `t3`/`t3a` (x86). Nếu muốn rẻ hơn có thể dùng `t4g.small` (ARM); image Docker .NET và PostgreSQL đều có bản ARM, nhưng mình chưa kiểm thử bản đó nên với lần đầu hãy chọn x86.
- **Hệ điều hành:** Ubuntu Server 24.04 LTS (giống máy dev của bạn).
- **Ổ đĩa:** 30 GiB `gp3` là dư cho giai đoạn đầu.
- **Vùng (Region):** chọn vùng gần người dùng nhất, ví dụ Singapore (`ap-southeast-1`).

Ước lượng RAM khi chạy: PostgreSQL 150–300 MB + web 150–250 MB + Caddy 30 MB + hệ điều hành và Docker khoảng 300 MB, tức khoảng 0,8–1 GB. Riêng lúc **build** image cần thêm khoảng 1–1,5 GB, nên 2 GiB là mức an toàn.

## 3. Chuẩn bị tài khoản AWS

1. Đăng ký AWS, đăng nhập bằng tài khoản root, bật **MFA** (Security credentials → Multi-factor authentication).
2. Đặt **cảnh báo chi phí** để không bị bất ngờ: *Billing and Cost Management → Budgets → Create budget → Monthly cost budget*, đặt ví dụ 25 USD và nhập email nhận cảnh báo.
3. Chọn **Region** ở góc trên bên phải của Console (ví dụ *Asia Pacific (Singapore)*). Mọi bước sau làm trong cùng một vùng.

## 4. Tạo máy EC2 trên Console

*EC2 → Instances → Launch instances*, điền:

| Mục | Giá trị |
|---|---|
| Name | `questlife` |
| Application and OS Images | **Ubuntu** → *Ubuntu Server 24.04 LTS* (64-bit x86) |
| Instance type | `t3.small` |
| Key pair | *Create new key pair* → tên `questlife`, loại RSA, định dạng `.pem` → tải về và **cất kỹ** (mất file này là mất đường SSH) |
| Network settings → Edit → Firewall (security groups) | *Create security group* với 3 luật bên dưới |
| Configure storage | `30` GiB, loại `gp3` |

**Luật tường lửa (Security group, chiều Inbound):**

| Loại | Cổng | Nguồn | Vì sao |
|---|---|---|---|
| SSH | 22 | **My IP** | Chỉ máy bạn được SSH vào |
| HTTP | 80 | Anywhere (0.0.0.0/0) | Let's Encrypt cần cổng này để cấp chứng chỉ, và để chuyển http sang https |
| HTTPS | 443 | Anywhere (0.0.0.0/0) | Người dùng và Discord truy cập |

Bấm **Launch instance**. **Không mở cổng 5432 (PostgreSQL) và 8080** ra internet.

> Nếu IP nhà bạn đổi (mạng không cố định), SSH sẽ bị chặn. Khi đó sửa lại luật SSH (*Security group → Edit inbound rules → My IP*), hoặc dùng nút **Connect → EC2 Instance Connect** ngay trên Console để mở terminal trên trình duyệt.

## 5. IP cố định và tên miền

**IP cố định (Elastic IP):** mặc định IP của máy đổi mỗi khi bạn dừng rồi bật lại, nên cần IP cố định.

1. *EC2 → Network & Security → Elastic IPs → Allocate Elastic IP address → Allocate*.
2. Chọn IP vừa tạo → *Actions → Associate Elastic IP address* → chọn instance `questlife` → *Associate*.

Ghi lại địa chỉ này (ví dụ `13.250.10.20`), gọi là `<IP>` bên dưới.

**Tên miền:** tạo bản ghi **A** trỏ tên miền về `<IP>`.
- Nếu đã có tên miền: vào trang quản lý DNS (Route 53 hoặc nhà đăng ký), tạo bản ghi `A`, tên `questlife` (thành `questlife.ten-mien-cua-ban.com`), giá trị `<IP>`, TTL 300.
- Nếu chưa có: dùng subdomain miễn phí của **DuckDNS** (duckdns.org), ví dụ `questlife.duckdns.org` trỏ về `<IP>`.

Kiểm tra DNS đã đúng chưa (có thể mất vài phút): `nslookup questlife.ten-mien-cua-ban.com` phải ra `<IP>`. **Đừng làm tiếp khi DNS chưa đúng**, vì Caddy sẽ không xin được chứng chỉ.

## 6. Kết nối SSH

Trên máy tính của bạn (nơi đã tải file `questlife.pem`):

```bash
chmod 400 ~/Downloads/questlife.pem         # SSH từ chối key có quyền quá rộng
ssh -i ~/Downloads/questlife.pem ubuntu@<IP>
```

Lần đầu SSH hỏi xác nhận, gõ `yes`. Tên người dùng mặc định của Ubuntu là `ubuntu`.

## 7. Cài Docker trên máy EC2

Chạy trên máy EC2 (đang trong phiên SSH). Đây là cách cài chính thức từ Docker:

```bash
sudo apt-get update
sudo apt-get install -y ca-certificates curl rsync
sudo install -m 0755 -d /etc/apt/keyrings
sudo curl -fsSL https://download.docker.com/linux/ubuntu/gpg -o /etc/apt/keyrings/docker.asc
sudo chmod a+r /etc/apt/keyrings/docker.asc
echo "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.asc] https://download.docker.com/linux/ubuntu $(. /etc/os-release && echo "${UBUNTU_CODENAME:-$VERSION_CODENAME}") stable" | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null
sudo apt-get update
sudo apt-get install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin

sudo usermod -aG docker ubuntu     # để chạy docker không cần sudo
exit                                 # thoát rồi SSH vào lại để quyền có hiệu lực
```

SSH vào lại và kiểm tra:

```bash
docker run --rm hello-world          # in ra "Hello from Docker!"
docker compose version
```

**Nếu dùng `t3.micro` (1 GiB RAM), thêm swap trước khi build:**

```bash
sudo fallocate -l 2G /swapfile && sudo chmod 600 /swapfile
sudo mkswap /swapfile && sudo swapon /swapfile
echo '/swapfile none swap sw 0 0' | sudo tee -a /etc/fstab
free -h                               # dòng Swap phải ra 2.0Gi
```

**Giới hạn dung lượng log của Docker** (mặc định log không bị giới hạn và có thể đầy ổ đĩa):

```bash
echo '{ "log-driver": "json-file", "log-opts": { "max-size": "10m", "max-file": "3" } }' | sudo tee /etc/docker/daemon.json
sudo systemctl restart docker
```

## 8. Đưa code lên server

Chạy trên **máy tính của bạn** (không phải trên EC2), trong thư mục chứa `TodoList/`:

```bash
rsync -avz --delete \
  --exclude 'bin/' --exclude 'obj/' --exclude '.git/' \
  --exclude '.env' --exclude 'appsettings.Development.json' \
  -e "ssh -i ~/Downloads/questlife.pem" \
  TodoList/ ubuntu@<IP>:~/questlife/
```

Giải thích: lệnh sao chép cả thư mục sang `~/questlife` trên server, **bỏ qua** thư mục build (`bin/`, `obj/`), file `.env` và file cấu hình dev (có mật khẩu DB máy bạn). Các mục `--exclude` cũng bảo vệ file `.env` trên server không bị `--delete` xóa. Không có `rsync` thì dùng `scp -r -i ... TodoList ubuntu@<IP>:~/questlife`.

> Sau này khi code đã nằm trên GitHub, bạn có thể thay bước này bằng `git clone` (repo riêng tư cần deploy key) và `git pull` để cập nhật.

## 9. Cấu hình biến môi trường (file `.env`)

### Có cần đặt biến môi trường trong AWS Console không?

**Không.** Với cách triển khai này (EC2 + Docker Compose), biến môi trường nằm trong một **file `.env` ngay trên máy EC2**; `docker compose` đọc file đó và đưa vào container. Giao diện "Environment variables" trên Console chỉ có ở các dịch vụ khác (Elastic Beanstalk, ECS, App Runner, Lambda), bạn không dùng trong hướng dẫn này. Nếu sau này cần lưu bí mật trên AWS thay vì file, xem mục 16.

### Tạo file `.env` trên server

SSH vào server:

```bash
cd ~/questlife
cp .env.production.example .env
openssl rand -hex 24                 # in ra một chuỗi ngẫu nhiên: dùng làm mật khẩu database
nano .env                            # sửa các giá trị (Ctrl+O để lưu, Ctrl+X để thoát)
chmod 600 .env                       # chỉ chủ file đọc được
```

Các giá trị cần sửa:

| Biến | Điền gì |
|---|---|
| `DOMAIN` | Tên miền đã trỏ về `<IP>`, ví dụ `questlife.ten-mien-cua-ban.com` (không có `https://`) |
| `POSTGRES_DB`, `POSTGRES_USER` | Giữ `questlife` hoặc đổi tùy ý |
| `POSTGRES_PASSWORD` | Chuỗi ngẫu nhiên ở trên. **Chỉ dùng chữ và số** (không có `;` `=` hay khoảng trắng) vì nó nằm trong chuỗi kết nối |
| `DISCORD_*` | Để trống lúc này, làm ở mục 12 |

> **Cẩn thận:** mật khẩu database chỉ được áp dụng **lần đầu** khi volume dữ liệu được tạo. Nếu sau đó bạn sửa `POSTGRES_PASSWORD` trong `.env`, web sẽ không kết nối được nữa. Hãy đặt đúng ngay từ đầu (xem mục 17 nếu lỡ sai).

## 10. Chạy ứng dụng

Trên server, trong `~/questlife`:

```bash
docker compose -f docker-compose.prod.yml --env-file .env up -d --build
```

Lần đầu mất vài phút vì phải tải image và build. Xem tiến trình và kiểm tra:

```bash
docker compose -f docker-compose.prod.yml ps                  # 3 dịch vụ caddy, web, db phải ở trạng thái Up (db: healthy)
docker compose -f docker-compose.prod.yml logs --tail=50 web  # phải thấy "Applying migration ..." rồi "Now listening on: http://[::]:8080"
docker compose -f docker-compose.prod.yml logs --tail=30 caddy  # phải thấy dòng báo đã lấy được certificate cho tên miền
```

Rồi mở `https://<DOMAIN>` trên trình duyệt: phải thấy trang bảng xếp hạng, có ổ khóa HTTPS. Đăng ký một tài khoản để thử. **Bản chạy thật không có dữ liệu demo**, mọi thứ bắt đầu trống (người chơi mới tự đăng ký).

Các lệnh dùng hằng ngày:

```bash
docker compose -f docker-compose.prod.yml logs -f --tail=100 web   # xem log trực tiếp (Ctrl+C để thoát)
docker compose -f docker-compose.prod.yml restart web              # khởi động lại web
docker compose -f docker-compose.prod.yml stop                     # dừng tất cả (giữ nguyên dữ liệu)
docker compose -f docker-compose.prod.yml up -d                    # bật lại
```

> **Không bao giờ chạy** `docker compose down -v` (hoặc `docker volume rm`): tùy chọn `-v` **xóa luôn dữ liệu database**. Dùng `stop` hoặc `down` (không `-v`).

Khi máy EC2 khởi động lại, các container tự bật lại (`restart: unless-stopped`).

## 11. HTTPS: bật, kiểm tra và dùng cho Discord và PWA

**Vì sao bắt buộc phải có HTTPS:**
- **Discord** chỉ gọi vào địa chỉ `https://` có **chứng chỉ hợp lệ do một tổ chức cấp chứng chỉ công khai ký** (chứng chỉ tự ký không được). Cổng phải là 443 và phải trả lời trong 3 giây.
- **PWA** (cài app lên màn hình chính, chạy offline-page): trình duyệt chỉ cho đăng ký service worker và hiện nút cài đặt trên HTTPS (riêng `localhost` được miễn).
- Cookie đăng nhập mới được gắn cờ `Secure` khi người dùng vào bằng HTTPS.

### 11.1 HTTPS trong stack này đã được lo sẵn

Bạn **không cần cài thêm chứng chỉ hay công cụ nào**. Khi chạy mục 10, container **Caddy** làm các việc sau, hoàn toàn tự động:

1. Thấy tên miền trong `DOMAIN`, nó nhờ **Let's Encrypt** cấp chứng chỉ miễn phí (chứng minh bạn sở hữu tên miền bằng cách Let's Encrypt gọi ngược vào cổng 80/443 của máy bạn).
2. Lưu chứng chỉ trong volume `caddy_data` và **tự gia hạn** trước khi hết hạn (chứng chỉ sống khoảng 90 ngày). Bạn không phải làm gì.
3. Tự chuyển mọi truy cập `http://` sang `https://`.
4. Chuyển yêu cầu vào web app và báo cho web app biết "người dùng đang dùng HTTPS" (nhờ `ASPNETCORE_FORWARDEDHEADERS_ENABLED` trong compose), nên cookie được gắn `Secure`.

**Điều kiện để việc xin chứng chỉ thành công** (kiểm tra trước khi chạy mục 10):

| Điều kiện | Cách kiểm tra |
|---|---|
| Tên miền trỏ đúng về Elastic IP | `nslookup <DOMAIN>` ra đúng `<IP>` (mục 5) |
| Security group mở cổng **80 và 443** cho mọi nơi | Xem lại bảng ở mục 4 |
| `DOMAIN` trong `.env` viết **đúng tên miền**, không có `https://`, không dùng địa chỉ IP | `grep DOMAIN ~/questlife/.env` |
| Máy EC2 đang chạy và có IP cố định | Console → EC2 |

> **Không thể xin chứng chỉ cho địa chỉ IP trần.** Phải có tên miền (hoặc subdomain miễn phí như DuckDNS, mục 5). Truy cập bằng `https://<IP>` sẽ luôn báo lỗi chứng chỉ, đó là bình thường: hãy luôn dùng tên miền.

### 11.2 Kiểm tra HTTPS đã chạy

Sau `docker compose ... up -d --build` đợi khoảng một phút, rồi trên **máy tính của bạn** (hoặc trên server):

```bash
# 1) Truy cập HTTPS phải trả về 200 và có chữ "Caddy" trong header
curl -I https://<DOMAIN>

# 2) Truy cập HTTP phải bị chuyển hướng sang HTTPS (mã 308)
curl -I http://<DOMAIN>

# 3) Xem chứng chỉ: nhà cấp phải là Let's Encrypt, và ngày hết hạn khoảng 90 ngày sau
openssl s_client -connect <DOMAIN>:443 -servername <DOMAIN> </dev/null 2>/dev/null | openssl x509 -noout -subject -issuer -dates

# 4) Log của Caddy phải có dòng báo đã lấy được chứng chỉ
docker compose -f docker-compose.prod.yml logs caddy | grep -i "certificate obtained"
```

Trên trình duyệt: mở `https://<DOMAIN>` phải thấy **ổ khóa** cạnh địa chỉ. Đăng nhập rồi mở DevTools (F12) → **Application → Cookies**: cookie `.AspNetCore.Cookies` phải có dấu tick ở cột **Secure** và **HttpOnly**.

### 11.3 Dùng cho Discord

1. Làm xong mục 12 (đặt biến `DISCORD_*`) để Discord được bật.
2. Kiểm tra endpoint đã gọi được từ ngoài internet, chạy trên máy tính của bạn:

```bash
curl -i -X POST https://<DOMAIN>/discord/interactions -H "Content-Type: application/json" -d '{}'
```

| Kết quả | Ý nghĩa |
|---|---|
| `401` kèm chữ `invalid request signature` | **Đúng như mong đợi.** HTTPS chạy, endpoint có mặt, và app từ chối vì yêu cầu thử của bạn không có chữ ký của Discord |
| `404` | Discord chưa được bật (`DISCORD_ENABLED=true` và `DISCORD_PUBLIC_KEY` chưa đặt, hoặc chưa khởi động lại web) |
| Lỗi chứng chỉ / không kết nối được | Quay lại mục 11.1 và bảng sự cố 11.5 |

3. Trong Discord Developer Portal → **General Information** → *Interactions Endpoint URL* dán `https://<DOMAIN>/discord/interactions` rồi **Save**. Dùng đúng địa chỉ **https**, không dùng `http` (Discord sẽ bị chuyển hướng và không kiểm tra được).

### 11.4 Dùng cho PWA (cài như app)

Khi HTTPS đã chạy, PWA tự hoạt động, không cần cấu hình thêm:

- **Android (Chrome):** mở `https://<DOMAIN>`, sau vài giây hiện thanh **Install** của QuestLife ở dưới màn hình, hoặc vào menu ⋮ → **Install app** / *Add to Home screen*.
- **iPhone/iPad (Safari):** nút **Share** → **Add to Home Screen** (chỉ Safari làm được).
- **Máy tính (Chrome/Edge):** biểu tượng cài đặt ở cuối thanh địa chỉ.

Kiểm tra PWA đủ điều kiện: mở trang trên Chrome máy tính → F12 → **Application**: mục **Manifest** không báo lỗi, mục **Service Workers** hiện `sw.js` ở trạng thái *activated and is running*.

Nếu bạn đổi icon hoặc tên app rồi thấy điện thoại vẫn hiện bản cũ, hãy gỡ app khỏi màn hình chính và cài lại.

### 11.5 Sự cố HTTPS thường gặp

| Hiện tượng | Nguyên nhân và cách xử lý |
|---|---|
| Không vào được `https://` (timeout / refused) | Cổng 443 chưa mở ở security group, hoặc container caddy không chạy: `docker compose ... ps` và `logs caddy` |
| Trình duyệt báo chứng chỉ sai/không tin cậy | Đang vào bằng **địa chỉ IP** thay vì tên miền; hoặc Caddy chưa lấy được chứng chỉ (xem log); hoặc đang dùng CA thử nghiệm (mục 11.6) |
| Log Caddy có `challenge failed`, `no valid A records` | DNS chưa trỏ đúng, cổng 80 bị chặn, hoặc `DOMAIN` gõ sai. Sửa rồi `docker compose ... restart caddy` |
| Log Caddy báo `too many failed authorizations` / `rate limit` | Let's Encrypt giới hạn số lần thử thất bại (đợi khoảng một giờ), hoặc dùng CA thử nghiệm ở mục 11.6 trong lúc sửa lỗi |
| Discord không lưu được Interactions Endpoint URL | Chạy lệnh `curl` ở mục 11.3: phải ra 401. Nếu 404: bật Discord. Nếu lỗi chứng chỉ: chưa có HTTPS hợp lệ. Nếu 401 mà Discord vẫn báo lỗi: `DISCORD_PUBLIC_KEY` sai (64 ký tự hex, lấy ở General Information) |
| Không thấy nút cài app | Chưa vào bằng HTTPS, hoặc đã cài rồi, hoặc bạn đã bấm ✕ (banner ẩn 14 ngày): dùng menu của trình duyệt |
| Mọi người bị đăng xuất sau mỗi lần cập nhật | Volume `dpkeys` bị mất (mục 13) |

### 11.6 Tùy chọn nâng cao

**Thử cấu hình mà không sợ bị Let's Encrypt chặn.** Trong lúc đang sửa lỗi DNS/tường lửa, dùng CA thử nghiệm của Let's Encrypt (không có giới hạn chặt, nhưng trình duyệt sẽ báo chứng chỉ không tin cậy). Thêm vào **đầu** file `Caddyfile`:

```
{
	acme_ca https://acme-staging-v02.api.letsencrypt.org/directory
}
```

Chạy lại `docker compose ... up -d`, khi mọi thứ chạy ổn thì **xóa khối đó** và chạy `docker compose ... down` (không `-v`) rồi `up -d` lại; Caddy sẽ xin chứng chỉ thật. Nếu vẫn thấy chứng chỉ thử nghiệm, xóa volume chứng chỉ: `docker volume rm questlife_caddy_data` (tên volume có thể khác, xem `docker volume ls`) rồi `up -d`.

**HSTS** (bắt trình duyệt luôn dùng HTTPS). Chỉ bật **sau khi** HTTPS đã chạy ổn định, vì trình duyệt sẽ từ chối http trong thời gian dài. Trong `Caddyfile`, thêm vào trong khối tên miền:

```
	header Strict-Transport-Security "max-age=31536000"
```

**Dùng chứng chỉ của AWS (ACM) thay cho Caddy:** ACM chỉ gắn được vào Load Balancer/CloudFront, tức phải thêm một **Application Load Balancer** (thêm khoảng 16 USD/tháng trở lên). Với một máy EC2 nhỏ, Caddy miễn phí và đủ dùng; chỉ cân nhắc ACM khi bạn chạy nhiều máy.

## 12. Bật Discord trên bản chạy thật

Làm sau khi mục 10 đã chạy tốt và bạn đã tạo app Discord theo [discord-setup.md](discord-setup.md) mục 2.

1. Trên server sửa `.env`: `nano ~/questlife/.env`, điền `DISCORD_ENABLED=true`, `DISCORD_PUBLIC_KEY`, `DISCORD_APPLICATION_ID`, `DISCORD_BOT_TOKEN`, `DISCORD_GUILD_ID`, (tùy chọn) `DISCORD_ALLOWED_CHANNEL_ID`, và đặt `DISCORD_REGISTER_COMMANDS=true`.
2. Áp dụng: `docker compose -f docker-compose.prod.yml --env-file .env up -d` (compose tự tạo lại container web vì cấu hình đổi).
3. Xem log: `... logs --tail=30 web` phải có `Discord: registered 9 slash commands`.
4. Đặt lại `DISCORD_REGISTER_COMMANDS=false` rồi chạy lại lệnh ở bước 2.
5. Trong Discord Developer Portal → **General Information** → *Interactions Endpoint URL* dán `https://<DOMAIN>/discord/interactions` và **Save**.
6. Đăng nhập web → Settings → Link Discord, rồi `/link` trong kênh Discord.

## 13. Cập nhật phiên bản mới

1. (Khuyên làm) Sao lưu database trước (mục 14).
2. Trên máy tính: chạy lại lệnh `rsync` ở mục 8.
3. Trên server:

```bash
cd ~/questlife
docker compose -f docker-compose.prod.yml --env-file .env up -d --build
docker image prune -f           # dọn image cũ cho đỡ tốn ổ đĩa
```

Migration chạy tự động khi web khởi động. Người dùng **không bị đăng xuất** nhờ khóa mã hóa được lưu trong volume `dpkeys`. Web ngừng vài giây trong lúc container được thay.

**Quay lại bản cũ:** nếu bản mới lỗi, `rsync` lại phiên bản code cũ từ máy bạn rồi chạy lệnh `up -d --build`. Nếu bản mới có migration làm đổi cấu trúc database thì cần khôi phục từ bản sao lưu ở mục 14.

## 14. Sao lưu dữ liệu

Dữ liệu người chơi nằm trong volume `pgdata`; **máy EC2 hỏng hoặc xóa nhầm là mất**. Hãy sao lưu định kỳ.

**Sao lưu hằng ngày bằng `pg_dump`.** Trên server tạo file `~/questlife/backup.sh`:

```bash
#!/usr/bin/env bash
set -euo pipefail
cd ~/questlife
mkdir -p ~/backups
f=~/backups/questlife-$(date +%F-%H%M).sql.gz
docker compose -f docker-compose.prod.yml exec -T db sh -c 'pg_dump -U "$POSTGRES_USER" "$POSTGRES_DB"' | gzip > "$f"
find ~/backups -name '*.sql.gz' -mtime +14 -delete      # giữ 14 ngày
echo "saved $f"
```

```bash
chmod +x ~/questlife/backup.sh
~/questlife/backup.sh                       # chạy thử một lần, phải thấy "saved ..."
crontab -e                                  # thêm dòng dưới để chạy lúc 3:30 sáng mỗi ngày
# 30 3 * * * /home/ubuntu/questlife/backup.sh >> /home/ubuntu/backup.log 2>&1
```

**Lưu bản sao ra khỏi máy** (nên làm, vì bản sao nằm cùng ổ đĩa không cứu được khi máy hỏng):
- Cách đơn giản: thỉnh thoảng tải về máy bạn: `scp -i ~/Downloads/questlife.pem ubuntu@<IP>:~/backups/*.sql.gz ./backups/`.
- Cách tự động: tạo một **S3 bucket**, tạo **IAM role** cho EC2 chỉ có quyền `s3:PutObject` vào bucket đó (*IAM → Roles → Create role → AWS service → EC2*, gắn vào máy bằng *EC2 → Actions → Security → Modify IAM role*), cài `sudo snap install aws-cli --classic`, rồi thêm vào cuối `backup.sh` dòng `aws s3 cp "$f" s3://TEN-BUCKET/`.
- Hoặc chụp **snapshot ổ đĩa** theo lịch: *EC2 → Lifecycle Manager → Create lifecycle policy* (hằng ngày, giữ 7 bản).

**Khôi phục** từ một bản sao lưu (vào một database trống; sẽ ghi đè dữ liệu hiện tại, hãy cẩn thận):

```bash
cd ~/questlife
docker compose -f docker-compose.prod.yml stop web
docker compose -f docker-compose.prod.yml exec -T db sh -c 'dropdb -U "$POSTGRES_USER" "$POSTGRES_DB" && createdb -U "$POSTGRES_USER" "$POSTGRES_DB"'
gunzip -c ~/backups/questlife-NGÀY.sql.gz | docker compose -f docker-compose.prod.yml exec -T db sh -c 'psql -U "$POSTGRES_USER" "$POSTGRES_DB"'
docker compose -f docker-compose.prod.yml start web
```

## 15. Bảo mật và vận hành

- **SSH:** chỉ cho IP của bạn (mục 4), dùng key, không dùng mật khẩu (Ubuntu AMI mặc định đã tắt đăng nhập bằng mật khẩu).
- **Chỉ mở cổng 22 (hạn chế), 80, 443.** Database không có cổng công khai.
- **File `.env`** chứa mật khẩu DB và token Discord: quyền `600`, không đưa lên git, không dán vào chat/ảnh chụp. Nếu lộ token Discord, vào Developer Portal → Bot → *Reset Token*.
- **Cập nhật hệ điều hành:** `sudo apt-get update && sudo apt-get upgrade -y` định kỳ (Ubuntu thường đã bật tự cập nhật bảo mật), rồi `sudo reboot` khi cần; container tự bật lại.
- **Dung lượng ổ đĩa:** kiểm tra `df -h`; dọn image không dùng bằng `docker system prune` (không có `-a --volumes`).
- **Cảnh báo:** nên có cảnh báo chi phí (mục 3) và cảnh báo CloudWatch khi máy lỗi (*EC2 → Instance → Status checks → Create alarm*).
- **Dừng máy (Stop) khác Xóa (Terminate):** Stop vẫn giữ ổ đĩa và dữ liệu (vẫn tính tiền ổ đĩa và IP); **Terminate xóa hết**.

## 16. Khi nào nên nâng cấp kiến trúc

| Nhu cầu | Hướng đi |
|---|---|
| Muốn AWS lo sao lưu và vá database | Dùng **Amazon RDS for PostgreSQL**: tạo instance, đổi chuỗi kết nối (`ConnectionStrings__Default`) trỏ tới endpoint RDS, bỏ dịch vụ `db` trong compose |
| Không muốn để bí mật trong file `.env` | Lưu trong **AWS Secrets Manager** hoặc **SSM Parameter Store**, gắn IAM role cho EC2, và dùng lệnh `aws secretsmanager get-secret-value` để ghi file `.env` lúc khởi động |
| Nhiều người dùng, cần chạy nhiều máy | Dùng load balancer. Lúc đó cần chia sẻ khóa mã hóa đăng nhập (Data Protection) giữa các máy (ví dụ lưu trên S3/Redis/database) thay vì volume cục bộ |
| Muốn tự build và triển khai khi push code | **GitHub Actions** build image, đẩy lên **ECR**, EC2 chỉ việc `docker compose pull` |

## 17. Sự cố thường gặp

| Hiện tượng | Nguyên nhân và cách xử lý |
|---|---|
| Trình duyệt treo / "connection timed out" | Security group thiếu luật 80/443, máy đang Stop, hoặc sai IP/tên miền |
| Cảnh báo chứng chỉ hoặc Caddy ghi lỗi "challenge failed" | DNS chưa trỏ đúng `<IP>`, cổng 80 bị chặn, hoặc `DOMAIN` gõ sai. Xem `logs caddy`. Sửa xong chạy `restart caddy` |
| 502 Bad Gateway | Container web chưa chạy hoặc bị lỗi: `docker compose ... ps` và `logs web` |
| Container web liên tục khởi động lại, log báo lỗi kết nối database / `password authentication failed` | Mật khẩu trong `.env` khác với mật khẩu đã dùng khi tạo volume lần đầu. Cách sửa không mất dữ liệu: `docker compose ... exec db psql -U questlife -c "ALTER USER questlife PASSWORD 'mật-khẩu-mới-trùng-với-.env'"`. Nếu chưa có dữ liệu quan trọng thì xóa volume `pgdata` và chạy lại |
| Build bị `Killed` hoặc exit code 137 | Hết RAM khi build: thêm swap (mục 7) hoặc dùng máy lớn hơn |
| `permission denied ... docker.sock` | Chưa vào nhóm `docker`: chạy `sudo usermod -aG docker ubuntu` rồi thoát và SSH lại |
| Mọi người bị đăng xuất sau mỗi lần cập nhật | Volume `dpkeys` hoặc biến `DataProtection__KeysPath` bị mất; dùng đúng `docker-compose.prod.yml` |
| Discord báo "The application did not respond" | Xem bảng lỗi trong discord-setup.md mục 7 |
| Quên mật khẩu admin DB, muốn xem dữ liệu | `docker compose ... exec db psql -U questlife questlife` (chạy trực tiếp trong container, không cần mật khẩu từ bên ngoài) |

## 18. Mình đã kiểm tra những gì (và chưa)

**Đã kiểm tra** (trên máy dev của bạn): `dotnet publish` thành công; bản publish chạy ở chế độ Production, chỉ đọc cấu hình từ biến môi trường, kết nối PostgreSQL, không nạp dữ liệu demo; cookie đăng nhập có cờ `Secure` khi đứng sau proxy gửi `X-Forwarded-Proto: https`; **đăng nhập vẫn còn sau khi khởi động lại** khi khóa mã hóa được lưu (và bị mất đăng nhập khi đổi thư mục khóa, đúng như dự đoán); cú pháp hai file compose hợp lệ; endpoint Discord từ chối yêu cầu thiếu hoặc sai chữ ký bằng mã 401 và trả 404 khi Discord chưa được bật (đúng như bảng ở mục 11.3).

**Chưa kiểm tra** (máy dev chưa có Docker, và mình không có tài khoản AWS của bạn): build image Docker, chạy Caddy và việc xin chứng chỉ Let's Encrypt thật (nên các lệnh kiểm tra ở mục 11.2 và log "certificate obtained" là theo cách Caddy hoạt động chứ chưa tự mình chạy), các lệnh trên EC2 thật. Các bước và lệnh trong tài liệu theo tài liệu chính thức của Docker và AWS, nhưng nếu bạn gặp lỗi ở đâu, hãy gửi nguyên nội dung lỗi (không kèm mật khẩu hay token), mình sẽ cùng xử lý và sửa lại tài liệu.
