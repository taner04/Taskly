#!/usr/bin/env python3
# =========================================================================
# Shared Utilities Module
# Common logging functions and utilities for Taskly scripts
# =========================================================================

import subprocess
import sys
import shutil
from pathlib import Path

# Color codes
RED = '\033[0;31m'
GREEN = '\033[0;32m'
BLUE = '\033[0;34m'
ORANGE = '\033[0;33m'
NC = '\033[0m'  # No Color


def error(message: str) -> None:
    """Print error message and exit."""
    print(f"{RED}[ERROR]{NC} {message}", file=sys.stderr)
    sys.exit(1)


def success(message: str) -> None:
    """Print success message."""
    print(f"{GREEN}[SUCCESS]{NC} {message}")


def info(message: str) -> None:
    """Print info message."""
    print(f"{BLUE}[INFO]{NC} {message}")


def warning(message: str) -> None:
    """Print warning message."""
    print(f"{ORANGE}[WARNING]{NC} {message}")


def command_exists(command: str) -> bool:
    """Check if a command exists in PATH."""
    return shutil.which(command) is not None


def run_command(command: list, capture_output: bool = False, error_msg: str = None) -> subprocess.CompletedProcess:
    """Run a shell command and return the result."""
    try:
        result = subprocess.run(command, check=True, capture_output=capture_output, text=True)
        return result
    except subprocess.CalledProcessError as e:
        if error_msg:
            error(error_msg)
        else:
            error(f"Command failed: {' '.join(command)}")


def get_command_output(command: list) -> str:
    """Run a command and return its output."""
    try:
        result = subprocess.run(command, check=True, capture_output=True, text=True)
        return result.stdout.strip()
    except subprocess.CalledProcessError:
        return ""


def get_project_root() -> Path:
    """Get the project root directory (parent of scripts folder)."""
    return Path(__file__).parent.parent
