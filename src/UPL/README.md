UPL - Skeleton (ASP.NET Core MVC, .NET 9)

Setup

- dotnet restore
- dotnet tool install --global dotnet-ef
- dotnet ef database update (migration already added: InitialCreate)
- dotnet run

Notes

- Default culture: vi-VN. DateTime stored in UTC; display convert to Asia/Ho_Chi_Minh.
- Skeleton only: no auth flow, caching, or business logic.

Seeded Accounts

- Admin → admin@gmail.com / testing
- Student → tam@gmail.com / testing

Structure

- Areas: Public, Student, Staff, Admin.
- Data: DbContext + Fluent API configurations (one per entity).
- Domain: Entities + Enums.
- Infrastructure: Generic repository + sample CourseService.

