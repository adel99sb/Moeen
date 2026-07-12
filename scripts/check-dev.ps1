$ErrorActionPreference = 'Continue'

$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

Write-Host "=== Moeen Development Check ===" -ForegroundColor Cyan
Write-Host "Root: $root"
Write-Host ""

Write-Host "[1] dotnet version" -ForegroundColor Yellow
dotnet --version
Write-Host ""

Write-Host "[2] SQL Server LocalDB" -ForegroundColor Yellow
$sqllocaldb = Get-Command sqllocaldb -ErrorAction SilentlyContinue
if ($sqllocaldb) {
    sqllocaldb info
} else {
    Write-Warning "sqllocaldb command was not found. Install SQL Server Express LocalDB or Visual Studio SQL Server Data Tools, or change Moeen.Api/appsettings.json connection string to an available SQL Server."
}
Write-Host ""

Write-Host "[3] Port listeners" -ForegroundColor Yellow
foreach ($port in 5055, 7023, 5077, 7220) {
    $listeners = Get-NetTCPConnection -LocalPort $port -State Listen -ErrorAction SilentlyContinue
    if ($listeners) {
        foreach ($listener in $listeners) {
            Write-Host "Port $port is used by PID $($listener.OwningProcess)" -ForegroundColor Red
        }
    } else {
        Write-Host "Port $port is free" -ForegroundColor Green
    }
}
Write-Host ""

Write-Host "[4] API health" -ForegroundColor Yellow
try {
    $health = Invoke-RestMethod -Uri 'http://localhost:5055/health' -TimeoutSec 5
    $health | ConvertTo-Json -Depth 5
} catch {
    Write-Warning "API health check failed on http://localhost:5055/health. Start API first using scripts/run-api.cmd. Error: $($_.Exception.Message)"
}
Write-Host ""

Write-Host "[5] Useful commands" -ForegroundColor Yellow
Write-Host "Run API:       .\scripts\run-api.cmd"
Write-Host "Run Dashboard: .\scripts\run-dashboard.cmd"
Write-Host "Swagger:       https://localhost:7023/swagger"
Write-Host "Dashboard:     https://localhost:7220"
