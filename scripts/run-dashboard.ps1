param(
    [string]$ApiBaseUrl = 'http://moeen.somee.com/'
)

$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

$env:MOEEN_API_BASE_URL = $ApiBaseUrl

Write-Host "=== Moeen Dashboard ===" -ForegroundColor Cyan
Write-Host "Working directory: $root"
Write-Host "Dashboard ports: https://localhost:7220 and http://localhost:5077"
Write-Host "API base URL: $env:MOEEN_API_BASE_URL"
Write-Host ""

foreach ($port in 5077, 7220) {
    $listeners = Get-NetTCPConnection -LocalPort $port -State Listen -ErrorAction SilentlyContinue
    if ($listeners) {
        $pids = ($listeners | Select-Object -ExpandProperty OwningProcess -Unique) -join ', '
        Write-Warning "Port $port is already in use. Process id(s): $pids. Stop the old Dashboard/app or change the launch profile port."
    }
}

Write-Host "Starting Dashboard..." -ForegroundColor Yellow
dotnet run --project .\Moeen.Dashboard\Moeen.Dashboard.csproj --launch-profile https
