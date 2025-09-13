UPL — Skeleton (ASP.NET Core MVC, .NET 9)

For full instructions, see the root `README.md`.

Quick dev setup:

- `dotnet restore`
- (optional) `dotnet tool install --global dotnet-ef`
- `dotnet ef database update`
- `dotnet run`

Notes:

- Default culture: `vi-VN` (data stored UTC; display localized).
- Seed accounts: `admin@gmail.com` / `testing`, `tam@gmail.com` / `testing`.

Structure:

- Areas: Public, Student, Staff, Admin
- Data: DbContext + EF Core migrations
- Domain: Entities + Enums
- Infrastructure: Generic repository + sample services
