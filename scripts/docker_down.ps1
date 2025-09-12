$ErrorActionPreference = "Stop"
Write-Host "Stopping and removing containers..." -ForegroundColor Yellow
docker compose down
Write-Host "Done." -ForegroundColor Green

