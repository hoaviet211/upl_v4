# UPL v4 — ASP.NET Core MVC (.NET 9)

Starter skeleton for an ASP.NET Core MVC app split by Areas, using Entity Framework Core (SQL Server) and Vietnamese localization by default (`vi-VN`). Includes a simple repository/service layer, Vite-based frontend bundling, and seed data to get you moving quickly.

## Requirements

- .NET SDK 9.0
- SQL Server (local or remote)
- Node.js 18+ (for building frontend assets with Vite)
- Docker Desktop/Engine (optional, for Docker Compose workflow)

## Quick Start (Development)

1) Backend

```
cd src/UPL
dotnet restore
# If you don't have EF tools: dotnet tool install --global dotnet-ef
dotnet ef database update   # applies existing migrations
dotnet run                  # launches on http://localhost:5070
```

2) Frontend assets (Vite → wwwroot/dist)

```
cd src/UPL
npm ci
npm run build               # outputs to wwwroot/dist
```

Convenience scripts (PowerShell):

- `scripts/run_dev.ps1` → `cd src/UPL && dotnet run`
- `scripts/migrate_dev.ps1` → add a migration + update database

## Seed Accounts

- Admin: `admin@gmail.com` / `testing`
- Student: `tam@gmail.com` / `testing`

## Build & Publish (Production)

```
cd src/UPL
dotnet build -c Release
dotnet publish -c Release -o ./publish
```

Recommended environment variables:

- `ASPNETCORE_ENVIRONMENT=Production`
- `ConnectionStrings__Default` → SQL Server connection string

Run the published app (Windows example):

```
./publish/UPL.exe
```

## Docker

Uses `docker-compose.yml` with two services: `db` (SQL Server) and `web` (ASP.NET Core).

Quick start:

```
docker compose up -d --build
# Web: http://localhost:8080
```

Customise ports and SA password via `.env` (see `.env.example`):

```
copy .env.example .env   # Windows
# edit SA_PASSWORD, WEB_PORT, DB_PORT if needed
docker compose up -d --build
```

Helper scripts:

- `scripts/docker_up.ps1` → build & start
- `scripts/docker_down.ps1` → stop & remove
- `scripts/docker_logs.ps1` → follow logs

Notes:

- Set `AUTO_MIGRATE=true` to run `Database.Migrate()` on startup inside the container.
- `ConnectionStrings__Default` is passed to the app from `docker-compose.yml`.
- SQL data persists in the `mssql-data` volume. Remove with `docker compose down -v`.

## Project Structure

- `src/UPL` → ASP.NET Core MVC web app
- Areas: `Public`, `Student`, `Staff`, `Admin`
- `Data`: `UplDbContext`, `Migrations`, EF Core configurations
- `Domain`: Entities, Enums
- `Infrastructure`: Generic repository + sample services
- `Common`: Shared helpers (incl. localization)

## Localization & Time

- Default culture is `vi-VN`. Data is stored in UTC and displayed using the Vietnamese locale in the UI.

## License

MIT — see `LICENSE`.

