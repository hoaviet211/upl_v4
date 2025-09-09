cd ..\src\UPL
dotnet ef migrations add Init_Manual -o Data/Migrations
dotnet ef database update
