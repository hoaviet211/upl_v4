$ErrorActionPreference = "Stop"
Write-Host "Tailing logs (Ctrl+C to stop)..." -ForegroundColor Cyan
docker compose logs -f web db

