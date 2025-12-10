# Scripts Documentation

This folder contains Python utility scripts for managing the Taskly project initialization, database migrations, and build processes.

## Overview

All scripts import the [`shared.py`](./shared.py) module which provides:

- `console_logger` — Logging utility with info(), error(), success() methods
- `project_root` — Base project directory path

## Available Scripts

### [`setup.py`](./setup.py)

**Purpose:** One-time initialization script that creates required configuration files with template values for the Taskly project.

**Usage:**

```bash
setup.py
```

**Output:**

- Creates `appsettings.json` in `src/Api/`
- Creates `appsettings.integration.json` in `tests/IntegrationTests/`
- Automatically creates the initial database migration

**Requirements:** None (first-time setup)

---

### [`create_migration.py`](./create_migration.py)

**Purpose:** Wrapper script for creating Entity Framework Core database migrations using dotnet CLI tools.

**Usage:**

```bash
create_migration.py <migration-name>
```

**Example:**

```bash
create_migration.py AddTodoTable
```

**Output:** Creates migration files in `src/Api/Infrastructure/Data/Migrations/`

**Requirements:**

- .NET EF Core tools installed — [Installation guide](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

---

### [`build.py`](./build.py)

**Purpose:** Cross-platform build script for compiling .NET projects and running tests.

**Usage:**

```bash
build.py [OPTIONS]
```

**Arguments:**

- `--dotnet` — Build .NET projects (default)
- `--test` — Run tests after building .NET projects

**Examples:**

```bash
build.py                # Build .NET projects
build.py --test         # Build .NET projects with tests
```

**Output:**

- Compiles .NET projects
- Optionally runs unit and integration tests

**Requirements:**

- All configuration files must exist
- Migrations must be applied
- .NET 9 SDK installed
