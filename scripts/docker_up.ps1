$ErrorActionPreference = "Stop"
Write-Host "Building and starting containers..." -ForegroundColor Cyan
docker compose up -d --build
Write-Host "Done. Web: http://localhost:${env:WEB_PORT -ne $null ? $env:WEB_PORT : 8080}" -ForegroundColor Green

