#!/usr/bin/env python3
"""
Run the Moeen MAUI Android app on a real USB Android device.

Default flow for a physical Android phone:
1. Verify dotnet and adb are available.
2. Verify at least one authorized Android device is connected.
3. Verify the local API is reachable on the PC.
4. Create adb reverse tcp mapping so the phone can call http://127.0.0.1:<api-port>/.
5. Restore, build, install, and run Moeen.App on the selected Android device.

Why adb reverse?
The app currently uses this Android BaseUrl:
    http://127.0.0.1:5055/
On a real phone, 127.0.0.1 means the phone itself. `adb reverse tcp:5055 tcp:5055`
forwards the phone's 127.0.0.1:5055 back to the PC's 127.0.0.1:5055.
"""

from __future__ import annotations

import argparse
import os
import shutil
import subprocess
import sys
import time
import urllib.error
import urllib.request
from pathlib import Path
from typing import Iterable


DEFAULT_PROJECT = "Moeen.App/Moeen.App.csproj"
DEFAULT_FRAMEWORK = "net9.0-android"
DEFAULT_API_PORT = 5055


class RunnerError(RuntimeError):
    pass


def print_step(message: str) -> None:
    print(f"\n=== {message} ===", flush=True)


def run(
    args: list[str],
    *,
    cwd: Path,
    env: dict[str, str] | None = None,
    check: bool = True,
    capture: bool = False,
) -> subprocess.CompletedProcess[str]:
    print("$ " + " ".join(args), flush=True)
    completed = subprocess.run(
        args,
        cwd=str(cwd),
        env=env,
        text=True,
        stdout=subprocess.PIPE if capture else None,
        stderr=subprocess.PIPE if capture else None,
    )

    if check and completed.returncode != 0:
        if capture:
            if completed.stdout:
                print(completed.stdout)
            if completed.stderr:
                print(completed.stderr, file=sys.stderr)
        raise RunnerError(f"Command failed with exit code {completed.returncode}: {' '.join(args)}")

    return completed


def find_repo_root(start: Path) -> Path:
    current = start.resolve()
    for path in [current, *current.parents]:
        if (path / "Moeen.sln").exists() and (path / DEFAULT_PROJECT).exists():
            return path
    raise RunnerError("Could not find Moeen.sln and Moeen.App/Moeen.App.csproj. Run this script from inside the Moeen repo.")


def find_adb() -> str:
    adb = shutil.which("adb")
    if adb:
        return adb

    candidates: list[Path] = []
    for env_name in ("ANDROID_HOME", "ANDROID_SDK_ROOT"):
        root = os.environ.get(env_name)
        if root:
            candidates.append(Path(root) / "platform-tools" / "adb.exe")
            candidates.append(Path(root) / "platform-tools" / "adb")

    local_app_data = os.environ.get("LOCALAPPDATA")
    if local_app_data:
        candidates.append(Path(local_app_data) / "Android" / "Sdk" / "platform-tools" / "adb.exe")

    for candidate in candidates:
        if candidate.exists():
            return str(candidate)

    raise RunnerError(
        "adb was not found. Install Android SDK Platform Tools or add adb to PATH. "
        "In Visual Studio Installer, make sure Android SDK setup/platform tools are installed."
    )


def require_tool(tool: str, help_text: str) -> str:
    path = shutil.which(tool)
    if not path:
        raise RunnerError(help_text)
    return path


def parse_adb_devices(output: str) -> tuple[list[str], list[str]]:
    authorized: list[str] = []
    unauthorized: list[str] = []

    for raw_line in output.splitlines():
        line = raw_line.strip()
        if not line or line.startswith("List of devices"):
            continue

        parts = line.split()
        if len(parts) < 2:
            continue

        serial, state = parts[0], parts[1]
        if state == "device":
            authorized.append(serial)
        elif state == "unauthorized":
            unauthorized.append(serial)

    return authorized, unauthorized


def select_device(adb: str, repo_root: Path, requested: str | None) -> str:
    result = run([adb, "devices"], cwd=repo_root, capture=True)
    authorized, unauthorized = parse_adb_devices(result.stdout or "")

    if unauthorized:
        raise RunnerError(
            "A connected Android device is unauthorized: "
            + ", ".join(unauthorized)
            + ". Unlock the phone and accept the USB debugging prompt, then rerun the script."
        )

    if requested:
        if requested not in authorized:
            raise RunnerError(
                f"Requested device '{requested}' was not found as an authorized device.\n"
                f"Authorized devices: {authorized or 'none'}"
            )
        return requested

    if not authorized:
        raise RunnerError(
            "No authorized Android device found.\n"
            "Checklist:\n"
            "  1. Enable Developer options on the phone.\n"
            "  2. Enable USB debugging.\n"
            "  3. Connect USB cable.\n"
            "  4. Accept the USB debugging prompt on the phone.\n"
            "  5. Confirm `adb devices` shows the device as `device`."
        )

    if len(authorized) > 1:
        print("Multiple devices found; using the first one. Pass --device SERIAL to choose explicitly.")
        for serial in authorized:
            print(f"  - {serial}")

    return authorized[0]


def check_api(api_port: int, timeout_seconds: float, skip: bool) -> None:
    if skip:
        print("Skipping local API reachability check.")
        return

    candidates = [
        f"http://127.0.0.1:{api_port}/swagger/index.html",
        f"http://127.0.0.1:{api_port}/swagger",
        f"http://localhost:{api_port}/swagger/index.html",
        f"http://localhost:{api_port}/swagger",
    ]

    last_error: Exception | None = None
    for url in candidates:
        try:
            with urllib.request.urlopen(url, timeout=timeout_seconds) as response:
                if 200 <= response.status < 500:
                    print(f"API reachable from PC: {url} -> HTTP {response.status}")
                    return
        except Exception as exc:  # noqa: BLE001 - we want a friendly troubleshooting message
            last_error = exc

    raise RunnerError(
        f"Could not reach the API locally on port {api_port}.\n"
        f"Last error: {last_error}\n\n"
        "Run the API first, preferably like this:\n"
        f"  dotnet run --project Moeen.Api --urls http://127.0.0.1:{api_port}\n\n"
        "If your API is already running on another port, rerun this script with --api-port <port>.\n"
        "If you intentionally want to skip this check, pass --skip-api-check."
    )


def setup_adb_reverse(adb: str, serial: str, repo_root: Path, port: int) -> None:
    run([adb, "-s", serial, "reverse", f"tcp:{port}", f"tcp:{port}"], cwd=repo_root)
    result = run([adb, "-s", serial, "reverse", "--list"], cwd=repo_root, capture=True)
    reverse_list = result.stdout or ""
    print(reverse_list.strip() or "No reverse mappings printed by adb.")

    expected = f"tcp:{port} tcp:{port}"
    if expected not in reverse_list.replace("\r", ""):
        print(
            "Warning: adb reverse did not print the expected mapping. "
            "The app may fail to reach the local API if the mapping was not created."
        )


def check_dotnet_environment(repo_root: Path) -> None:
    require_tool("dotnet", "dotnet was not found. Install .NET SDK 9 and make sure dotnet is in PATH.")

    info = run(["dotnet", "--list-sdks"], cwd=repo_root, capture=True)
    sdks = info.stdout or ""
    if not any(line.startswith("9.") for line in sdks.splitlines()):
        raise RunnerError(
            "No .NET 9 SDK found. The app targets net9.0-android.\n"
            "Install .NET SDK 9.x, then rerun this script."
        )

    workloads = run(["dotnet", "workload", "list"], cwd=repo_root, capture=True)
    workload_output = workloads.stdout or ""
    if "maui-android" not in workload_output and "android" not in workload_output:
        raise RunnerError(
            "MAUI Android workload was not found. Run:\n"
            "  dotnet workload install maui-android\n"
            "  dotnet workload restore"
        )


def run_mobile_app(
    repo_root: Path,
    project: str,
    framework: str,
    serial: str,
    configuration: str,
    no_restore: bool,
) -> None:
    project_path = repo_root / project
    if not project_path.exists():
        raise RunnerError(f"Project file not found: {project_path}")

    env = os.environ.copy()
    env["ANDROID_SERIAL"] = serial

    if not no_restore:
        print_step("Restoring project")
        run(["dotnet", "restore", project], cwd=repo_root, env=env)

    print_step("Building, installing, and running the Android app")
    run(
        [
            "dotnet",
            "build",
            project,
            "-f",
            framework,
            "-c",
            configuration,
            "-t:Run",
            f"-p:AndroidDeviceSerial={serial}",
        ],
        cwd=repo_root,
        env=env,
    )


def main(argv: Iterable[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description="Run Moeen.App on a real Android device.")
    parser.add_argument("--api-port", type=int, default=DEFAULT_API_PORT, help="Local API port forwarded to the phone. Default: 5055")
    parser.add_argument("--device", help="ADB device serial. If omitted, the first authorized device is used.")
    parser.add_argument("--project", default=DEFAULT_PROJECT, help=f"MAUI app project path. Default: {DEFAULT_PROJECT}")
    parser.add_argument("--framework", default=DEFAULT_FRAMEWORK, help=f"Target framework. Default: {DEFAULT_FRAMEWORK}")
    parser.add_argument("--configuration", default="Debug", choices=["Debug", "Release"], help="Build configuration. Default: Debug")
    parser.add_argument("--skip-api-check", action="store_true", help="Skip checking http://127.0.0.1:<api-port>/swagger before running.")
    parser.add_argument("--api-timeout", type=float, default=3.0, help="API check timeout in seconds. Default: 3")
    parser.add_argument("--no-restore", action="store_true", help="Skip dotnet restore before running.")
    args = parser.parse_args(list(argv) if argv is not None else None)

    try:
        repo_root = find_repo_root(Path.cwd())
        print(f"Repo root: {repo_root}")

        print_step("Checking .NET and MAUI workload")
        check_dotnet_environment(repo_root)

        print_step("Checking adb and connected Android device")
        adb = find_adb()
        device = select_device(adb, repo_root, args.device)
        print(f"Using Android device: {device}")

        print_step("Checking local API")
        check_api(args.api_port, args.api_timeout, args.skip_api_check)

        print_step("Creating adb reverse mapping for real Android device")
        setup_adb_reverse(adb, device, repo_root, args.api_port)
        print(f"Phone http://127.0.0.1:{args.api_port}/ now forwards to PC http://127.0.0.1:{args.api_port}/")

        run_mobile_app(
            repo_root=repo_root,
            project=args.project,
            framework=args.framework,
            serial=device,
            configuration=args.configuration,
            no_restore=args.no_restore,
        )

        print_step("Done")
        print("The app was deployed and launched on the Android device.")
        return 0

    except RunnerError as exc:
        print(f"\nERROR: {exc}", file=sys.stderr)
        return 1
    except KeyboardInterrupt:
        print("\nCancelled by user.", file=sys.stderr)
        return 130


if __name__ == "__main__":
    raise SystemExit(main())
