#!/usr/bin/env python3
# =========================================================================
# Taskly Setup Script
# Creates required configuration files if they do not yet exist.
# This script is intended to be run ONCE after cloning the repository.
# =========================================================================

import json
from pathlib import Path

from Shared import project_root, console_logger
from CreateMigration import add_migration

# =========================================================================
# Content Templates
# =========================================================================

ENV_CONTENT = """
VITE_AUTH0_DOMAIN=your-auth0-domain
VITE_AUTH0_CLIENT_ID=your-auth0-client-id
"""

APPSETTINGS_API = {
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning",
        }
    },
    "AllowedHosts": "*",
    "Auth0": {
        "Domain": "your-auth0-domain",
        "Audience": "your-auth0-audience",
    },
}

APPSETTINGS_INTEGRATION_TEST = {
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning",
        }
    },
    "Auth0": {
        "Domain": "your-auth0-domain",
        "Client_Id": "your-auth0-client-id",
        "Client_Secret": "your-auth0-client-secret",
        "Audience": "your-auth0-audience",
        "Grant_Type": "client_credentials",
    },
}

# =========================================================================
# Setup Tasks
# =========================================================================


def create_env_file() -> None:
    """Create .env file for the Web application."""
    env_file = project_root / "src" / "Web" / ".env"

    if env_file.exists():
        console_logger.success(".env already exists")
        return

    env_file.parent.mkdir(parents=True, exist_ok=True)
    env_file.write_text(ENV_CONTENT)

    console_logger.success("Created .env file")


def create_appsettings(file_path: Path, content: dict) -> None:
    """Create appsettings.json file with given content."""
    if file_path.exists():
        console_logger.success("appsettings.json already exists")
        return

    file_path.parent.mkdir(parents=True, exist_ok=True)
    file_path.write_text(json.dumps(content, indent=2))

    console_logger.success("Created appsettings.json")


def create_appsettings_api() -> None:
    """Create appsettings.json for the API project."""
    create_appsettings(
        project_root / "src" / "Api" / "appsettings.json", APPSETTINGS_API
    )


def create_appsettings_integration_test() -> None:
    """Create appsettings.integration.json for integration tests."""
    create_appsettings(
        project_root / "tests" / "IntegrationTests" / "appsettings.integration.json",
        APPSETTINGS_INTEGRATION_TEST,
    )


# =========================================================================
# Main Entry Point
# =========================================================================


def main() -> None:
    """Initialize all required configuration files."""
    console_logger.info("Initializing Taskly configuration files...")
    create_env_file()
    create_appsettings_api()
    create_appsettings_integration_test()
    console_logger.success("Initialization completed! You can now configure your values.")

    console_logger.info("Creating initial database migration...")
    add_migration("InitialCreate")

if __name__ == "__main__":
    main()
