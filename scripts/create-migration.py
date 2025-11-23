#!/usr/bin/env python3
# =========================================================================
# Add Migration Script - Creates a new EF Core migration for the Taskly API
# =========================================================================

import sys
import os
from pathlib import Path

from shared import (
    error,
    success,
    info,
    warning,
    command_exists,
    run_command,
    get_project_root,
)


# =========================================================================
# Check if a migration with the same name already exists
# =========================================================================
def check_for_existing_migration(migration_name: str, output_dir: Path) -> None:
    if output_dir.exists():
        for migration_file in output_dir.glob(f"*{migration_name}*.cs"):
            error(f"A migration containing the name '{migration_name}' already exists in '{output_dir.relative_to(get_project_root())}'.")


# =========================================================================
# Main entry point
# =========================================================================
def main() -> None:
    try:
        project_root = get_project_root()
        os.chdir(project_root)

        # =========================================================================
        # Configuration
        # =========================================================================
        PROJECT = "src/Api/Api.csproj"
        STARTUP_PROJECT = "src/Api/Api.csproj"
        CONTEXT = "Api.Infrastructure.Data.ApplicationDbContext"
        OUTPUT_DIR = project_root / "src" / "Api" / "Infrastructure" / "Data" / "Migrations"
        CONFIGURATION = "Debug"

        # =========================================================================
        # Check tooling
        # =========================================================================
        if not command_exists("dotnet"):
            error("dotnet SDK not installed or not in PATH.")

        # =========================================================================
        # Read migration name
        # =========================================================================
        if len(sys.argv) > 1:
            migration_name = sys.argv[1]
        else:
            migration_name = input("Enter migration name: ").strip()

        if not migration_name:
            error("Migration name cannot be empty.")

        info(f"Using migration name: {migration_name}")

        # =========================================================================
        # Create migrations folder if it doesn't exist
        # =========================================================================
        OUTPUT_DIR.mkdir(parents=True, exist_ok=True)
        info(f"Migrations folder ready at {OUTPUT_DIR.relative_to(project_root)}")

        # =========================================================================
        # Check for duplicate migration
        # =========================================================================
        check_for_existing_migration(migration_name, OUTPUT_DIR)

        # =========================================================================
        # Validate EF tooling
        # =========================================================================
        info("Checking EF tooling...")
        run_command(["dotnet", "ef", "--help"], capture_output=True, error_msg="dotnet-ef not installed or not working.")

        # =========================================================================
        # Build project
        # =========================================================================
        info("Building project...")
        run_command(
            ["dotnet", "build", STARTUP_PROJECT, "--configuration", CONFIGURATION],
            capture_output=True,
            error_msg="Build failed. Migration aborted."
        )

        # =========================================================================
        # Perform migration
        # =========================================================================
        info("Running EF migration...")

        result = run_command(
            [
                "dotnet", "ef", "migrations", "add", migration_name,
                "--project", PROJECT,
                "--startup-project", STARTUP_PROJECT,
                "--context", CONTEXT,
                "--configuration", CONFIGURATION,
                "--output-dir", str(OUTPUT_DIR.relative_to(project_root))
            ],
            error_msg="EF migration command failed – no files created."
        )

        success(f"Migration '{migration_name}' created successfully.")

    # =========================================================================
    # Handle keyboard interrupt
    # =========================================================================
    except KeyboardInterrupt:
        print("\n")
        warning("Migration creation interrupted by user")
        sys.exit(1)
    # =========================================================================
    # Handle unexpected errors
    # =========================================================================
    except Exception as e:
        error(f"Unexpected error: {str(e)}")


if __name__ == "__main__":
    main()
