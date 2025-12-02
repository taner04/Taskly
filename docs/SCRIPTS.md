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

- Creates `.env` file in `src/Web/`
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

- `appsettings.json` must exist
- .NET EF Core tools installed — [Installation guide](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

---

### [`build.py`](./build.py)

**Purpose:** Cross-platform build script for compiling .NET projects, running tests, building the web frontend, and optionally building Docker images.

**Usage:**

```bash
build.py [OPTIONS]
```

**Arguments:**

- `--dotnet` — Build only .NET projects (skip web build)
- `--web` — Build only web project (skip .NET build)
- `--test` — Run tests after building .NET projects
- `--docker` — Build Docker image for the frontend

**Examples:**

```bash
build.py                   # Full build (both .NET and web)
build.py --dotnet          # .NET only
build.py --web             # Web only
build.py --test            # Full build with tests
build.py --dotnet --test   # .NET only with tests
build.py --web --docker    # Web only with Docker image
```

**Output:**

- Compiles .NET projects
- Optionally runs unit and integration tests
- Builds the React frontend
- Optionally builds Docker image

**Requirements:**

- All configuration files must exist
- Migrations must be applied
- .NET 9 SDK installed
- Node.js 20+ installed
