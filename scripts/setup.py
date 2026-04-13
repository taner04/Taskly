#!/usr/bin/env python3
# =========================================================================
# Taskly Setup Script
# Creates required configuration files if they do not yet exist.
# This script is intended to be run ONCE after cloning the repository.
# =========================================================================

import json
from pathlib import Path

from shared import project_root, console_logger
from create_migration import add_migration

# =========================================================================
# Content Templates
# =========================================================================

LOGGING_TEMPLATE = {
    "LogLevel": {
        "Default": "Information",
        "Microsoft.AspNetCore": "Warning"
    }
}

APPSETTINGS_API_TEMPLATE = {
    "Logging": LOGGING_TEMPLATE,
    "AllowedHosts": "*",
    "Auth0Config": {
        "Domain": "your-auth0-domain",
        "Audience": "your-auth0-audience",
        "ClientId": "your-auth0-client-id",
        "ClientSecret": "your-auth0-client-secret",
        "UsePersistentStorage": False
    },
    "ConnectionStrings": {
        "AzureBlobStorage": "your-azure-blob-storage-connection-string"
    },
      "WebHookConfig":{
        "SecretKey": "your-webhook-secret-key"
    },
    "EmailConfig": {
        "Host": "localhost",
        "Port": 25
    }
}

# =========================================================================
# Setup Tasks
# =========================================================================
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
        project_root / "src" / "Taskly.WebApi" / "appsettings.json", APPSETTINGS_API_TEMPLATE
    )


def create_initial_migration() -> None:
    """Create initial database migration."""
    migrations_dir = project_root / "src" / "Taskly.WebApi" / "Common" / "Infrastructure" / "Persistence" / "Migrations"
    
    # Check if migrations directory already exists (indicates Initial migration was already created)
    if migrations_dir.exists() and any(f.name.endswith("Initial.cs") for f in migrations_dir.glob("*.cs")):
        console_logger.success("Initial migration already exists")
        return
    
    add_migration("Initial")

# =========================================================================
# Main Entry Point
# =========================================================================


def main() -> None:
    """Initialize all required configuration files."""
    console_logger.info("Initializing Taskly configuration files...")
    create_appsettings_api()
    create_initial_migration()
    console_logger.success("Initialization completed!")

if __name__ == "__main__":
    main()
