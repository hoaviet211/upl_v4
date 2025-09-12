UPL — Skeleton (ASP.NET Core MVC, .NET 9)

Tài liệu đầy đủ xem tại `README.md` ở thư mục gốc của repo.

Thiết lập nhanh (Development)

- dotnet restore
- dotnet tool install --global dotnet-ef (nếu chưa có)
- dotnet ef database update (đã có migration: InitialCreate)
- dotnet run

Ghi chú

- Văn hóa mặc định: vi-VN. Thời gian lưu UTC; hiển thị theo Asia/Ho_Chi_Minh.
- Đây là skeleton: chưa có auth flow đầy đủ, caching hay nghiệp vụ phức tạp.

Tài khoản seed

- Admin: admin@gmail.com / testing
- Student: tam@gmail.com / testing

Structure

- Areas: Public, Student, Staff, Admin.
- Data: DbContext + Fluent API configurations (mỗi entity một file).
- Domain: Entities + Enums.
- Infrastructure: Generic repository + sample services.

