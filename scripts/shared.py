#!/usr/bin/env python3
# =========================================================================
# Shared Utilities Module
# Common logging functions and utilities for Taskly scripts
# =========================================================================

import subprocess
import sys
import shutil
import platform
from pathlib import Path

# Color codes
RED = '\033[0;31m'
GREEN = '\033[0;32m'
BLUE = '\033[0;34m'
ORANGE = '\033[0;33m'
NC = '\033[0m'  # No Color


# =========================================================================
# Print error message and exit
# =========================================================================
def error(message: str) -> None:
    print(f"{RED}[ERROR]{NC} {message}", file=sys.stderr)
    sys.exit(1)


# =========================================================================
# Print success message
# =========================================================================
def success(message: str) -> None:
    print(f"{GREEN}[SUCCESS]{NC} {message}")


# =========================================================================
# Print info message
# =========================================================================
def info(message: str) -> None:
    print(f"{BLUE}[INFO]{NC} {message}")


# =========================================================================
# Print warning message
# =========================================================================
def warning(message: str) -> None:
    print(f"{ORANGE}[WARNING]{NC} {message}")


# =========================================================================
# Check if a command exists in PATH
# =========================================================================
def command_exists(command: str) -> bool:
    return shutil.which(command) is not None


# =========================================================================
# Run a shell command and return the result
# =========================================================================
def run_command(command: list, capture_output: bool = False, error_msg: str = None) -> subprocess.CompletedProcess:
    try:
        # On Windows, use shell=True to properly resolve commands like npm
        shell = platform.system() == "Windows"
        result = subprocess.run(command, check=True, capture_output=capture_output, text=True, shell=shell)
        return result
    except (subprocess.CalledProcessError, FileNotFoundError) as e:
        if error_msg:
            error(error_msg)
        else:
            error(f"Command failed: {' '.join(command)}")


# =========================================================================
# Run a command and return its output
# =========================================================================
def get_command_output(command: list) -> str:
    try:
        # On Windows, use shell=True to properly resolve commands
        shell = platform.system() == "Windows"
        result = subprocess.run(command, check=True, capture_output=True, text=True, shell=shell)
        return result.stdout.strip()
    except (subprocess.CalledProcessError, FileNotFoundError):
        return ""


# =========================================================================
# Get the project root directory (parent of scripts folder)
# =========================================================================
def get_project_root() -> Path:
    return Path(__file__).parent.parent
