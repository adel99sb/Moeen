param(
    [string]$DeviceId,
    [string]$ApiUrl = "http://localhost:5055/health",
    [string]$AndroidSdkRoot,
    [switch]$SkipApiCheck,
    [switch]$SkipToolInstall,
    [switch]$SkipWorkloadInstall,
    [switch]$SkipAndroidSdkInstall,
    [switch]$NoPersistAndroidSdkEnv,
    [switch]$NoLaunch
)

$ErrorActionPreference = 'Stop'

try {
    [Net.ServicePointManager]::SecurityProtocol = [Net.ServicePointManager]::SecurityProtocol -bor [Net.SecurityProtocolType]::Tls12
} catch {
    # Continue with the runtime default if this PowerShell version handles TLS differently.
}
$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

$appProject = Join-Path $root 'Moeen.App\Moeen.App.csproj'
$mobileFramework = 'net10.0-android36.0'
$packageName = 'com.companyname.moeen.app'
$apkPath = Join-Path $root "Moeen.App\bin\Debug\$mobileFramework\com.companyname.moeen.app-Signed.apk"
$defaultAndroidSdkRoot = Join-Path $env:LOCALAPPDATA 'Android\Sdk'

function Write-Step([string]$Message) {
    Write-Host ""
    Write-Host "=== $Message ===" -ForegroundColor Cyan
}

function Write-Ok([string]$Message) {
    Write-Host "[OK] $Message" -ForegroundColor Green
}

function Write-Info([string]$Message) {
    Write-Host "[INFO] $Message" -ForegroundColor Yellow
}

function Fail([string]$Message) {
    Write-Host "[ERROR] $Message" -ForegroundColor Red
    exit 1
}

function Get-RetryDelaySeconds([int]$Attempt) {
    return [int][Math]::Min(30, 2 * $Attempt)
}

function Invoke-TextRequestWithRetry {
    param(
        [string]$Uri,
        [string]$Description,
        [int]$TimeoutSec = 60,
        [int]$MaxAttempts = 5
    )

    for ($attempt = 1; $attempt -le $MaxAttempts; $attempt++) {
        try {
            if ($attempt -gt 1) {
                Write-Info "Retrying $Description download ($attempt/$MaxAttempts)..."
            }

            $response = Invoke-WebRequest -UseBasicParsing -Uri $Uri -TimeoutSec $TimeoutSec
            if ([string]::IsNullOrWhiteSpace($response.Content)) {
                throw "$Description download returned empty content."
            }

            return $response.Content
        }
        catch {
            $message = $_.Exception.Message
            if ($attempt -ge $MaxAttempts) {
                Fail "Could not download $Description after $MaxAttempts attempts. Check internet or VPN access to Google Android downloads, then run the script again. Last error: $message"
            }

            Write-Warning "Could not download $Description ($attempt/$MaxAttempts): $message"
            Start-Sleep -Seconds (Get-RetryDelaySeconds $attempt)
        }
    }
}

function Invoke-FileDownloadWithRetry {
    param(
        [string]$Uri,
        [string]$OutFile,
        [string]$Description,
        [int]$TimeoutSec = 300,
        [int]$MaxAttempts = 5
    )

    $curl = Get-Command curl.exe -ErrorAction SilentlyContinue

    for ($attempt = 1; $attempt -le $MaxAttempts; $attempt++) {
        try {
            if (Test-Path $OutFile) {
                Remove-Item -Force $OutFile -ErrorAction SilentlyContinue
            }

            if ($attempt -gt 1) {
                Write-Info "Retrying $Description download ($attempt/$MaxAttempts)..."
            }

            if ($curl) {
                & $curl.Source --fail --location --retry 3 --retry-delay 2 --connect-timeout 30 --max-time $TimeoutSec --output $OutFile $Uri
                if ($LASTEXITCODE -ne 0) {
                    throw "curl.exe exited with code $LASTEXITCODE."
                }
            }
            else {
                Invoke-WebRequest -UseBasicParsing -Uri $Uri -OutFile $OutFile -TimeoutSec $TimeoutSec
            }

            $downloadedFile = Get-Item $OutFile -ErrorAction SilentlyContinue
            if (-not $downloadedFile -or $downloadedFile.Length -le 0) {
                throw "$Description download produced an empty file."
            }

            Write-Ok "$Description downloaded successfully."
            return
        }
        catch {
            $message = $_.Exception.Message
            if ($attempt -ge $MaxAttempts) {
                Fail "Could not download $Description after $MaxAttempts attempts. Check internet or VPN access to Google Android downloads, then run the script again. Last error: $message"
            }

            Write-Warning "Could not download $Description ($attempt/$MaxAttempts): $message"
            Start-Sleep -Seconds (Get-RetryDelaySeconds $attempt)
        }
    }
}

function Refresh-PathFromMachineAndUser {
    $machinePath = [Environment]::GetEnvironmentVariable('Path', 'Machine')
    $userPath = [Environment]::GetEnvironmentVariable('Path', 'User')
    $env:PATH = "$machinePath;$userPath"
}

function Get-Winget {
    return Get-Command winget -ErrorAction SilentlyContinue
}

function Ensure-Winget {
    $winget = Get-Winget
    if ($winget) {
        Write-Ok "winget found: $($winget.Source)"
        return $winget
    }

    if ($SkipToolInstall) {
        Write-Warning "winget is missing and -SkipToolInstall was provided. Automatic .NET/Java installation will not be available."
        return $null
    }

    Write-Info "winget is missing. Trying to install Microsoft App Installer from the official aka.ms/getwinget link..."

    $tempRoot = Join-Path ([System.IO.Path]::GetTempPath()) ('moeen-winget-' + [Guid]::NewGuid().ToString('N'))
    New-Item -ItemType Directory -Force -Path $tempRoot | Out-Null
    $bundlePath = Join-Path $tempRoot 'Microsoft.DesktopAppInstaller.msixbundle'

    try {
        Invoke-WebRequest -UseBasicParsing -Uri 'https://aka.ms/getwinget' -OutFile $bundlePath -TimeoutSec 300
        Add-AppxPackage -Path $bundlePath
        Refresh-PathFromMachineAndUser

        $winget = Get-Winget
        if ($winget) {
            Write-Ok "winget installed: $($winget.Source)"
            return $winget
        }

        Write-Warning "App Installer installation finished, but winget is still not visible in this PowerShell session. Reopen PowerShell and run the script again if .NET/Java installation is needed."
        return $null
    }
    catch {
        Write-Warning "Could not install winget automatically: $($_.Exception.Message)"
        Write-Warning "This can happen on old Windows versions, disabled Microsoft Store/App Installer policies, or missing Appx dependencies. Android SDK installation can still continue, but .NET/Java may need manual installation."
        return $null
    }
    finally {
        if (Test-Path $tempRoot) {
            Remove-Item -Recurse -Force $tempRoot -ErrorAction SilentlyContinue
        }
    }
}

function Install-WithWinget([string]$PackageId, [string]$FriendlyName) {
    if ($SkipToolInstall) {
        Fail "$FriendlyName is missing and -SkipToolInstall was provided. Install it manually, then re-run."
    }

    $winget = Ensure-Winget
    if (-not $winget) {
        Fail "$FriendlyName is missing and winget could not be installed automatically. Install $FriendlyName manually, then re-run."
    }

    Write-Info "$FriendlyName is missing. Trying to install it using winget package '$PackageId'..."
    winget install --id $PackageId --accept-package-agreements --accept-source-agreements
    Refresh-PathFromMachineAndUser
}

function Ensure-DotNetSdk {
    $dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
    if (-not $dotnet) {
        Install-WithWinget 'Microsoft.DotNet.SDK.10' '.NET SDK 10'
        $dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
    }

    if (-not $dotnet) {
        Fail ".NET SDK is still missing after install attempt. Install .NET SDK 10 manually and reopen PowerShell."
    }

    $version = dotnet --version
    if (-not ($version -like '10.*')) {
        Write-Warning "Detected dotnet version '$version'. The mobile project targets $mobileFramework, so .NET SDK 10 is recommended."
    }

    Write-Ok "dotnet found: $version"
}

function Ensure-Java {
    $java = Get-Command java -ErrorAction SilentlyContinue
    if (-not $java) {
        Install-WithWinget 'Microsoft.OpenJDK.17' 'Microsoft OpenJDK 17'
        $java = Get-Command java -ErrorAction SilentlyContinue
    }

    if ($java) {
        Write-Ok "java found: $($java.Source)"
    } else {
        Write-Warning "java is still not visible in PATH. .NET Android build may still work if it finds a bundled JDK, otherwise install OpenJDK 17 manually."
    }

    if (-not $env:JAVA_HOME) {
        $javaHomes = @(
            'C:\Program Files\Microsoft\jdk-17*',
            'C:\Program Files\Eclipse Adoptium\jdk-17*',
            'C:\Program Files\Java\jdk-17*'
        ) | ForEach-Object { Get-ChildItem -Path $_ -Directory -ErrorAction SilentlyContinue } | Sort-Object FullName -Descending

        $javaHome = $javaHomes | Select-Object -First 1
        if ($javaHome) {
            $env:JAVA_HOME = $javaHome.FullName
            [Environment]::SetEnvironmentVariable('JAVA_HOME', $javaHome.FullName, 'User')
            Write-Ok "JAVA_HOME set to: $($javaHome.FullName)"
        }
    }
}

function Test-AndroidSdkRoot([string]$SdkRoot) {
    if ([string]::IsNullOrWhiteSpace($SdkRoot)) {
        return $false
    }

    if (-not (Test-Path $SdkRoot)) {
        return $false
    }

    $hasPlatformTools = Test-Path (Join-Path $SdkRoot 'platform-tools')
    $hasPlatforms = Test-Path (Join-Path $SdkRoot 'platforms')
    $hasCmdlineTools = Test-Path (Join-Path $SdkRoot 'cmdline-tools\latest\bin\sdkmanager.bat')
    $hasLegacyTools = Test-Path (Join-Path $SdkRoot 'tools\bin\sdkmanager.bat')

    return ($hasPlatformTools -or $hasPlatforms -or $hasCmdlineTools -or $hasLegacyTools)
}

function Resolve-AndroidSdkRoot([string]$RequestedRoot) {
    if (-not [string]::IsNullOrWhiteSpace($RequestedRoot)) {
        Write-Info "Android SDK root was provided explicitly: $RequestedRoot"
        return $RequestedRoot
    }

    $candidates = @(
        $env:ANDROID_HOME,
        $env:ANDROID_SDK_ROOT,
        $defaultAndroidSdkRoot,
        (Join-Path $env:ProgramData 'Microsoft\AndroidSDK\25'),
        (Join-Path ${env:ProgramFiles(x86)} 'Android\android-sdk'),
        (Join-Path $env:ProgramFiles 'Android\android-sdk')
    ) | Where-Object { -not [string]::IsNullOrWhiteSpace($_) } | Select-Object -Unique

    foreach ($candidate in $candidates) {
        if (Test-AndroidSdkRoot $candidate) {
            Write-Ok "Found existing Android SDK: $candidate"
            return $candidate
        }
    }

    Write-Info "No existing Android SDK was found. A local SDK will be created at: $defaultAndroidSdkRoot"
    return $defaultAndroidSdkRoot
}

function Set-AndroidSdkEnvironment([string]$SdkRoot) {
    $cmdlineBin = Join-Path $SdkRoot 'cmdline-tools\latest\bin'
    $platformTools = Join-Path $SdkRoot 'platform-tools'

    $env:ANDROID_HOME = $SdkRoot
    $env:ANDROID_SDK_ROOT = $SdkRoot

    foreach ($path in @($platformTools, $cmdlineBin)) {
        if ((Test-Path $path) -and ($env:PATH -notlike "*$path*")) {
            $env:PATH = "$path;$env:PATH"
        }
    }

    if (-not $NoPersistAndroidSdkEnv) {
        [Environment]::SetEnvironmentVariable('ANDROID_HOME', $SdkRoot, 'User')
        [Environment]::SetEnvironmentVariable('ANDROID_SDK_ROOT', $SdkRoot, 'User')
        Write-Ok "ANDROID_HOME and ANDROID_SDK_ROOT were saved for the current Windows user."
    }
}

function Get-SdkManager([string]$SdkRoot) {
    $latest = Join-Path $SdkRoot 'cmdline-tools\latest\bin\sdkmanager.bat'
    if (Test-Path $latest) {
        return $latest
    }

    $legacy = Join-Path $SdkRoot 'tools\bin\sdkmanager.bat'
    if (Test-Path $legacy) {
        return $legacy
    }

    return $null
}

function Get-AndroidCommandLineToolsUrl {
    $repositoryUrl = 'https://dl.google.com/android/repository/repository2-1.xml'
    Write-Info "Reading Android repository metadata from Google..."

    [xml]$repository = Invoke-TextRequestWithRetry -Uri $repositoryUrl -Description 'Android repository metadata' -TimeoutSec 60 -MaxAttempts 5
    $packageNode = $repository.SelectSingleNode("//remotePackage[@path='cmdline-tools;latest']")
    if (-not $packageNode) {
        Fail "Could not find cmdline-tools;latest in Google's Android repository metadata."
    }

    $archiveNode = $packageNode.SelectSingleNode(".//archive[host-os='windows']")
    if (-not $archiveNode) {
        Fail "Could not find Windows command-line tools archive in Google's Android repository metadata."
    }

    $relativeUrl = $archiveNode.SelectSingleNode("complete/url").InnerText
    if ([string]::IsNullOrWhiteSpace($relativeUrl)) {
        Fail "Android command-line tools archive URL was empty."
    }

    if ($relativeUrl.StartsWith('http', [StringComparison]::OrdinalIgnoreCase)) {
        return $relativeUrl
    }

    return "https://dl.google.com/android/repository/$relativeUrl"
}

function Install-AndroidCommandLineTools([string]$SdkRoot) {
    $sdkManager = Get-SdkManager $SdkRoot
    if ($sdkManager) {
        Write-Ok "Android sdkmanager already exists: $sdkManager"
        return $sdkManager
    }

    if ($SkipAndroidSdkInstall) {
        Fail "Android SDK command-line tools are missing and -SkipAndroidSdkInstall was provided."
    }

    New-Item -ItemType Directory -Force -Path $SdkRoot | Out-Null
    $tempRoot = Join-Path ([System.IO.Path]::GetTempPath()) ('moeen-android-sdk-' + [Guid]::NewGuid().ToString('N'))
    New-Item -ItemType Directory -Force -Path $tempRoot | Out-Null

    try {
        $toolsUrl = Get-AndroidCommandLineToolsUrl
        $zipPath = Join-Path $tempRoot 'commandlinetools-win.zip'
        Write-Info "Downloading Android command-line tools..."
        Invoke-FileDownloadWithRetry -Uri $toolsUrl -OutFile $zipPath -Description 'Android command-line tools' -TimeoutSec 300 -MaxAttempts 5

        Write-Info "Extracting Android command-line tools..."
        Expand-Archive -Path $zipPath -DestinationPath $tempRoot -Force

        $expandedTools = Join-Path $tempRoot 'cmdline-tools'
        if (-not (Test-Path $expandedTools)) {
            Fail "Downloaded Android command-line tools archive did not contain cmdline-tools folder."
        }

        $latestRoot = Join-Path $SdkRoot 'cmdline-tools\latest'
        if (Test-Path $latestRoot) {
            Remove-Item -Recurse -Force $latestRoot
        }

        New-Item -ItemType Directory -Force -Path $latestRoot | Out-Null
        Copy-Item -Recurse -Force (Join-Path $expandedTools '*') $latestRoot

        $sdkManager = Join-Path $latestRoot 'bin\sdkmanager.bat'
        if (-not (Test-Path $sdkManager)) {
            Fail "sdkmanager was not found after extraction: $sdkManager"
        }

        Write-Ok "Android command-line tools installed under: $latestRoot"
        return $sdkManager
    }
    finally {
        if (Test-Path $tempRoot) {
            Remove-Item -Recurse -Force $tempRoot -ErrorAction SilentlyContinue
        }
    }
}

function Ensure-AndroidPackage([string]$SdkRoot, [string]$PackageId, [string]$ExpectedPath) {
    if (Test-Path (Join-Path $SdkRoot $ExpectedPath)) {
        Write-Ok "Android SDK package exists: $PackageId"
        return
    }

    $script:AndroidPackagesToInstall += $PackageId
}

function Ensure-AndroidSdk([string]$RequestedRoot) {
    Write-Step "Checking Android SDK"
    $sdkRoot = Resolve-AndroidSdkRoot $RequestedRoot
    Write-Host "Android SDK root: $sdkRoot"

    $sdkManager = Install-AndroidCommandLineTools $sdkRoot
    Set-AndroidSdkEnvironment $sdkRoot

    $script:AndroidPackagesToInstall = @()
    Ensure-AndroidPackage $sdkRoot 'platform-tools' 'platform-tools\adb.exe'
    Ensure-AndroidPackage $sdkRoot 'platforms;android-36' 'platforms\android-36\android.jar'
    Ensure-AndroidPackage $sdkRoot 'build-tools;35.0.0' 'build-tools\35.0.0\aapt.exe'

    if ($script:AndroidPackagesToInstall.Count -gt 0) {
        if ($SkipAndroidSdkInstall) {
            Fail "Missing Android SDK packages: $($script:AndroidPackagesToInstall -join ', ') and -SkipAndroidSdkInstall was provided."
        }

        Write-Info "Installing missing Android SDK packages: $($script:AndroidPackagesToInstall -join ', ')"
        & $sdkManager --sdk_root=$sdkRoot @script:AndroidPackagesToInstall
    }
    else {
        Write-Ok "Required Android SDK packages already exist."
    }

    Write-Info "Accepting Android SDK licenses if needed..."
    $yes = ("y`n" * 200)
    $yes | & $sdkManager --sdk_root=$sdkRoot --licenses | Out-Host

    $adbPath = Join-Path $sdkRoot 'platform-tools\adb.exe'
    if (-not (Test-Path $adbPath)) {
        Fail "adb was not installed at expected path: $adbPath"
    }

    Write-Ok "Android SDK is ready."
    return @{ SdkRoot = $sdkRoot; AdbPath = $adbPath }
}

function Ensure-MauiAndroidWorkload {
    if ($SkipWorkloadInstall) {
        Write-Info "Skipping MAUI workload installation because -SkipWorkloadInstall was provided."
        return
    }

    $workloads = dotnet workload list
    if ($workloads -match 'maui' -or $workloads -match 'android') {
        Write-Ok "MAUI/Android workload appears installed."
        return
    }

    Write-Info "MAUI/Android workload does not appear installed. Trying: dotnet workload install maui android"
    dotnet workload install maui android
}

function Get-ConnectedDevice([string]$RequestedDeviceId) {
    $deviceLines = adb devices | Select-String "\tdevice$" | ForEach-Object { $_.ToString().Split("`t")[0] }

    if ($RequestedDeviceId) {
        if ($deviceLines -notcontains $RequestedDeviceId) {
            Fail "Requested device '$RequestedDeviceId' is not connected. Run 'adb devices' and verify USB debugging is allowed."
        }
        return $RequestedDeviceId
    }

    if (-not $deviceLines -or $deviceLines.Count -eq 0) {
        Fail "No Android device is connected. Connect phone with USB, enable Developer Options + USB debugging, then accept the RSA popup."
    }

    if ($deviceLines.Count -gt 1) {
        Write-Host "Connected devices:" -ForegroundColor Yellow
        $deviceLines | ForEach-Object { Write-Host " - $_" }
        Fail "More than one Android device is connected. Re-run with: .\scripts\run-mobile.ps1 -DeviceId <device-id>"
    }

    return $deviceLines[0]
}

function Invoke-Adb([string]$SelectedDevice, [string[]]$AdbArgs) {
    if ($SelectedDevice) {
        & adb -s $SelectedDevice @AdbArgs
    } else {
        & adb @AdbArgs
    }
}

Write-Step "Moeen Mobile Android Runner"
Write-Host "Root: $root"
Write-Host "Project: $appProject"
Write-Host "Package: $packageName"
Write-Host "API expected from phone via adb reverse: http://127.0.0.1:5055/"

Write-Step "Checking tools"
Ensure-DotNetSdk
Ensure-Winget | Out-Null
Ensure-Java
$androidSdk = Ensure-AndroidSdk $AndroidSdkRoot
Write-Ok "adb ready: $($androidSdk.AdbPath)"
Ensure-MauiAndroidWorkload

Write-Step "Checking Android device"
$selectedDevice = Get-ConnectedDevice $DeviceId
Write-Ok "Using device: $selectedDevice"
Invoke-Adb $selectedDevice @('get-state') | Out-Null

Write-Step "Preparing API bridge"
if (-not $SkipApiCheck) {
    try {
        $health = Invoke-RestMethod -Uri $ApiUrl -TimeoutSec 5
        Write-Ok "API health check passed: $ApiUrl"
    } catch {
        Fail "API health check failed at '$ApiUrl'. Start the API first using .\scripts\run-api.ps1. Details: $($_.Exception.Message)"
    }
} else {
    Write-Info "Skipping API health check because -SkipApiCheck was provided."
}

Invoke-Adb $selectedDevice @('reverse', 'tcp:5055', 'tcp:5055') | Out-Null
Write-Ok "adb reverse tcp:5055 tcp:5055 is active. The phone can call http://127.0.0.1:5055/."

Write-Step "Building APK"
if (-not (Test-Path $appProject)) {
    Fail "Mobile project was not found: $appProject"
}

dotnet build $appProject -f $mobileFramework `
    -p:AndroidPackageFormat=apk `
    "-p:AndroidSdkDirectory=$($androidSdk.SdkRoot)" `
    -p:EmbedAssembliesIntoApk=true `
    -p:AndroidUseSharedRuntime=false `
    -p:UseSharedCompilation=false `
    -p:BuildInParallel=false `
    -maxcpucount:1 `
    -nodeReuse:false

if (-not (Test-Path $apkPath)) {
    Fail "APK was not produced at expected path: $apkPath"
}
Write-Ok "APK ready: $apkPath"

Write-Step "Installing or updating app"
Invoke-Adb $selectedDevice @('install', '-r', $apkPath)
Write-Ok "App installed/updated on device."

if (-not $NoLaunch) {
    Write-Step "Launching app"
    Invoke-Adb $selectedDevice @('shell', 'monkey', '-p', $packageName, '-c', 'android.intent.category.LAUNCHER', '1') | Out-Null
    Write-Ok "App launch command sent."
}

Write-Step "Done"
Write-Host "Mobile app is installed. Keep the API running while testing." -ForegroundColor Green
Write-Host "If API calls fail inside the app, run again while API is open, or check: .\scripts\check-dev.ps1" -ForegroundColor Yellow
