# UPL v4 — ASP.NET Core MVC (.NET 9)

Mẫu (skeleton) dự án ASP.NET Core MVC với phân tách theo Areas, dùng Entity Framework Core (SQL Server) và cấu hình sẵn nội địa hóa `vi-VN`. Dự án nhằm giúp khởi tạo nhanh một ứng dụng quản trị/nội dung cơ bản cho UPL.

## Yêu cầu

- .NET SDK 9.0
- SQL Server (Express/Developer hoặc bản cài trên máy). Chuỗi kết nối mặc định: `Server=.;Database=UPL;Trusted_Connection=True;TrustServerCertificate=True` (sử dụng Windows Authentication).
- Redis (tùy chọn): `localhost:6379` đã cấu hình sẵn nhưng chưa sử dụng trong business logic.

## Cấu hình

- Sửa chuỗi kết nối trong `src/UPL/appsettings.json` hoặc đặt biến môi trường `ConnectionStrings__Default` khi chạy/triển khai.
- Thiết lập riêng môi trường phát triển trong `src/UPL/appsettings.Development.json`.

## Thiết lập & chạy (Development)

```bash
cd src/UPL
dotnet restore
dotnet tool install --global dotnet-ef   # nếu chưa có
dotnet ef database update                # áp dụng migration có sẵn (InitialCreate)
dotnet run
```

Hoặc sử dụng script tiện ích trên Windows PowerShell:

- `scripts/run_dev.ps1` — chuyển thư mục sang `src/UPL` và chạy `dotnet run`.
- `scripts/migrate_dev.ps1` — thêm migration mới và `database update` (điều chỉnh tên migration trong script nếu cần).

## Tài khoản mẫu (seed)

- Admin: `admin@gmail.com` / `testing`
- Student: `tam@gmail.com` / `testing`

## Build & Publish (Production)

```bash
cd src/UPL
dotnet build -c Release
dotnet publish -c Release -o ./publish
```

Biến môi trường khuyến nghị khi triển khai:

- `ASPNETCORE_ENVIRONMENT=Production`
- `ConnectionStrings__Default` — chuỗi kết nối SQL Server của môi trường chạy.
- `Redis__Configuration` — cấu hình Redis nếu sử dụng cache.

Chạy thực thi đã publish (ví dụ trên Windows):

```bash
./publish/UPL.exe
```

## Docker (Portable)

- Yêu cầu: Docker Desktop (Windows/macOS) hoặc Docker Engine (Linux).

Chạy nhanh bằng Docker Compose:

```bash
docker compose up -d --build
# app: http://localhost:8080
```

Tùy chỉnh cổng/mật khẩu DB bằng `.env` (tạo từ `.env.example`):

```bash
cp .env.example .env    # Windows dùng copy
# chỉnh SA_PASSWORD, WEB_PORT, DB_PORT nếu cần
docker compose up -d --build
```

Lệnh tiện ích:

- `scripts/docker_up.ps1` — build & start
- `scripts/docker_down.ps1` — stop & remove
- `scripts/docker_logs.ps1` — xem log

Ghi chú:

- Biến `AUTO_MIGRATE=true` giúp app tự `Database.Migrate()` khi khởi động trong container.
- Chuỗi kết nối được cấp qua env `ConnectionStrings__Default` trong `docker-compose.yml`.
- Dữ liệu SQL lưu ở volume `mssql-data` (không mất khi restart). Xóa hẳn: `docker compose down -v`.

## Cấu trúc chính

- `src/UPL` — ứng dụng web (ASP.NET Core MVC).
- Areas: `Public`, `Student`, `Staff`, `Admin`.
- `Data`: `UplDbContext`, `Migrations`, Fluent API configurations (mỗi entity một file).
- `Domain`: Entities, Enums.
- `Infrastructure`: Generic repository + services mẫu (Course/Programme/Category).
- `Common`: tiện ích dùng chung (ví dụ `LocalizationExtensions`).

## Nội địa hóa & thời gian

- Mặc định văn hóa `vi-VN`. Dữ liệu thời gian lưu theo UTC, hiển thị theo múi giờ Việt Nam (Asia/Ho_Chi_Minh) ở tầng UI.

---

Góp ý/điều chỉnh hãy tạo PR hoặc issue trong repo.
