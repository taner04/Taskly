#!/usr/bin/env python3
# =========================================================================
# Taskly Initialization Script
# This script initializes and builds the entire Taskly project including:
# - Creating necessary .env files
# - Setting up appsettings configuration
# - Building the .NET API
# - Building the Web frontend
# - Building Docker images
# =========================================================================

import sys
import os
import json
from pathlib import Path

from shared import (
    error,
    success,
    info,
    warning,
    command_exists,
    run_command,
    get_command_output,
    get_project_root,
)


# =========================================================================
# Check if all required tools are installed
# =========================================================================
def check_prerequisites() -> None:
    info("Checking prerequisites...")

    if not command_exists("dotnet"):
        error("dotnet SDK not installed or not in PATH.")

    dotnet_version_output = get_command_output(["dotnet", "--version"])
    if not dotnet_version_output:
        error("Could not determine dotnet version")

    dotnet_version = int(dotnet_version_output.split(".")[0])
    if dotnet_version < 9:
        error(f"dotnet 9 or later required. Found version: {dotnet_version_output}")

    success(f"dotnet {dotnet_version_output} found")

    if not command_exists("node"):
        error("Node.js not installed or not in PATH.")

    node_version = get_command_output(["node", "--version"])
    success(f"Node.js {node_version} found")

    if not command_exists("npm"):
        error("npm not installed or not in PATH.")

    npm_version = get_command_output(["npm", "--version"])
    success(f"npm {npm_version} found")


# =========================================================================
# Setup environment files
# =========================================================================
def setup_env_files() -> None:
    info("Setting up environment files...")

    project_root = get_project_root()
    web_env = project_root / "src" / "Web" / ".env"

    if not web_env.exists():
        warning(f"Creating {web_env.relative_to(project_root)} (please update with your values)")
        env_content = """# Auth0 Configuration
VITE_AUTH0_DOMAIN=your-auth0-domain
VITE_AUTH0_CLIENT_ID=your-auth0-client-id
"""
        web_env.parent.mkdir(parents=True, exist_ok=True)
        web_env.write_text(env_content)
        warning(f"Please update {web_env.relative_to(project_root)} with your configuration values")
    else:
        success(f"{web_env.relative_to(project_root)} already exists")


# =========================================================================
# Setup appsettings configuration
# =========================================================================
def setup_appsettings() -> None:
    info("Checking appsettings configuration...")

    project_root = get_project_root()
    api_appsettings = project_root / "src" / "Api" / "appsettings.json"

    if not api_appsettings.exists():
        warning("Creating appsettings.json")
        appsettings_content = {
            "Logging": {
                "LogLevel": {
                    "Default": "Information",
                    "Microsoft.AspNetCore": "Warning"
                }
            },
            "AllowedHosts": "*",
            "Auth0": {
                "Domain": "your-auth0-domain",
                "Audience": "your-auth0-audience"
            }
        }
        api_appsettings.parent.mkdir(parents=True, exist_ok=True)
        api_appsettings.write_text(json.dumps(appsettings_content, indent=2))
        success("Created appsettings.json")
    else:
        success(f"{api_appsettings.relative_to(project_root)} already exists")


# =========================================================================
# Build .NET projects
# =========================================================================
def build_dotnet_projects() -> None:
    info("Building .NET projects...")

    project_root = get_project_root()
    os.chdir(project_root)

    info("Restoring NuGet packages...")
    run_command(["dotnet", "restore", "src/Api/Api.csproj"], "Failed to restore API packages")
    run_command(["dotnet", "restore", "tests/UnitTests/UnitTests.csproj"], "Failed to restore UnitTests packages")
    run_command(["dotnet", "restore", "tests/IntegrationTests/IntegrationTests.csproj"], "Failed to restore IntegrationTests packages")
    success("NuGet packages restored")

    info("Building API project...")
    run_command(["dotnet", "build", "src/Api/Api.csproj", "-c", "Release"], "Failed to build API")
    success("API project built")

    info("Building test projects...")
    run_command(["dotnet", "build", "tests/UnitTests/UnitTests.csproj", "-c", "Release"], "Failed to build UnitTests")
    run_command(["dotnet", "build", "tests/IntegrationTests/IntegrationTests.csproj", "-c", "Release"], "Failed to build IntegrationTests")
    success("Test projects built")


# =========================================================================
# Build Web project
# =========================================================================
def build_web_project() -> None:
    info("Building Web project...")

    project_root = get_project_root()
    web_dir = project_root / "src" / "Web"
    original_dir = Path.cwd()

    try:
        os.chdir(web_dir)

        info("Installing npm dependencies...")
        run_command(["npm", "install"], "Failed to install npm dependencies")
        success("Dependencies installed")

        info("Building Web application...")
        run_command(["npm", "run", "build"], "Failed to build Web application")
        success("Web application built")

    finally:
        os.chdir(original_dir)


# =========================================================================
# Build Docker images
# =========================================================================
def build_docker_images() -> None:
    info("Building Docker images...")

    project_root = get_project_root()
    os.chdir(project_root)

    if command_exists("docker"):
        info("Building Web Docker image...")
        run_command(["docker", "build", "-t", "taskly-web:latest", "src/Web"], "Failed to build Web Docker image")
        success("Web Docker image built: taskly-web:latest")
    else:
        warning("Docker not found. Skipping Docker image builds.")


# =========================================================================
# Print build summary
# =========================================================================
def print_summary() -> None:
    print("")
    print("=" * 81)
    success("Build completed successfully!")
    print("=" * 81)
    print("")
    print("Next steps:")
    print("1. Update configuration files with your actual values:")
    print("   - src/Web/.env")
    print("   - src/Api/appsettings.json")
    print("")
    print("2. Run the Aspire orchestrator:")
    print("   cd tools/AppHost && dotnet run")
    print("")
    print("=" * 81)


# =========================================================================
# Main entry point
# =========================================================================
def main() -> None:
    try:
        check_prerequisites()
        setup_env_files()
        setup_appsettings()
        build_dotnet_projects()
        build_web_project()
        build_docker_images()
        print_summary()
    except KeyboardInterrupt:
        print("\n")
        warning("Build interrupted by user")
        sys.exit(1)
    except Exception as e:
        error(f"Unexpected error: {str(e)}")


if __name__ == "__main__":
    main()
