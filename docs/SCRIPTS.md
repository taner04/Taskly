# Scripts Documentation

This folder contains Python utility scripts for managing the Taskly project initialization, database migrations, and build processes.

## Overview

All scripts import the [`shared.py`](./shared.py) module which provides:

- `console_logger` — Logging utility with info(), error(), success() methods
- `project_root` — Base project directory path

## Available Scripts

### [`Setup.py`](./Setup.py)

**Purpose:** One-time initialization script that creates required configuration files with template values for the Taskly project.

**Usage:**

```bash
python3 Setup.py
```

**Output:**

- Creates `.env` file in `src/Web/`
- Creates `appsettings.json` in `src/Api/`
- Creates `appsettings.integration.json` in `tests/IntegrationTests/`
- Automatically creates the initial database migration

**Requirements:** None (first-time setup)

---

### [`CreateMigration.py`](./CreateMigration.py)

**Purpose:** Wrapper script for creating Entity Framework Core database migrations using dotnet CLI tools.

**Usage:**

```bash
python3 CreateMigration.py <migration-name>
```

**Example:**

```bash
python3 CreateMigration.py AddTodoTable
```

**Output:** Creates migration files in `src/Api/Infrastructure/Data/Migrations/`

**Requirements:**

- `appsettings.json` must exist
- .NET EF Core tools installed — [Installation guide](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

---

### [`Build.py`](./Build.py)

**Purpose:** Cross-platform build script for compiling .NET projects, running tests, building the web frontend, and optionally building Docker images.

**Usage:**

```bash
python3 Build.py [--docker]
```

**Arguments:**

- `--docker` — Optional flag to build Docker image for the frontend

**Output:**

- Compiles .NET projects
- Runs unit and integration tests
- Builds the React frontend
- Optionally builds Docker image

**Requirements:**

- All configuration files must exist
- Migrations must be applied
- .NET 9 SDK installed
- Node.js 20+ installed
