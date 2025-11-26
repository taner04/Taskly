#!/usr/bin/env python3
# =========================================================================
# Taskly Initialization Script
# Creates required configuration files if they do not yet exist.
# This script is intended to be run ONCE after cloning the repository.
# =========================================================================

import json
from pathlib import Path
from shared import project_root, console_logger

# =========================================================================
# Content Templates
# =========================================================================

ENV_CONTENT = """# Auth0 Configuration
VITE_AUTH0_DOMAIN=your-auth0-domain
VITE_AUTH0_CLIENT_ID=your-auth0-client-id
"""

APPSETTINGS_CONTENT = {
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

# =========================================================================
# Setup Tasks
# =========================================================================

def create_env_file():
    env_file = project_root / "src" / "Web" / ".env"

    if env_file.exists():
        console_logger.success(".env already exists")
        return

    env_file.parent.mkdir(parents=True, exist_ok=True)
    env_file.write_text(ENV_CONTENT)

    console_logger.success("Created .env file")


def create_appsettings():
    appsettings_file = project_root / "src" / "Api" / "appsettings.json"

    if appsettings_file.exists():
        console_logger.success("appsettings.json already exists")
        return

    appsettings_file.parent.mkdir(parents=True, exist_ok=True)
    appsettings_file.write_text(json.dumps(APPSETTINGS_CONTENT, indent=2))

    console_logger.success("Created appsettings.json")


# =========================================================================
# Main Entry Point
# =========================================================================

def main():
    console_logger.info("Initializing Taskly configuration files...")
    create_env_file()
    create_appsettings()
    console_logger.success("Initialization completed! You can now configure your values.")


if __name__ == "__main__":
    main()
