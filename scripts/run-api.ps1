$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

Write-Host "=== Moeen API ===" -ForegroundColor Cyan
Write-Host "Working directory: $root"
Write-Host "Ports: https://localhost:7023 and http://localhost:5055"
Write-Host "Health check after startup: http://localhost:5055/health"
Write-Host "Swagger after startup: https://localhost:7023/swagger"
Write-Host ""

foreach ($port in 5055, 7023) {
    $listeners = Get-NetTCPConnection -LocalPort $port -State Listen -ErrorAction SilentlyContinue
    if ($listeners) {
        $pids = ($listeners | Select-Object -ExpandProperty OwningProcess -Unique) -join ', '
        Write-Warning "Port $port is already in use. Process id(s): $pids. Stop the old API/app or change the launch profile port."
    }
}

Write-Host "Starting API. First run may take time because migrations and seed data run automatically in Development." -ForegroundColor Yellow
dotnet run --project .\Moeen.Api\Moeen.Api.csproj --launch-profile https
