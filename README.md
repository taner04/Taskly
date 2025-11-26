# Taskly

A modern, full-stack task management application built with .NET and React.

## 📋 Prerequisites

- **[Docker](https://www.docker.com/)** — Required for containerization
- **[.NET 9](https://dotnet.microsoft.com/download)** — Required for backend development
- **[Node.js 20+](https://nodejs.org/)** — Required for frontend development
- **[Python 3](https://www.python.org/)** *(optional)* — Simplifies project setup and build automation. Without Python, you'll need to manually execute build steps.

## 🚀 Installation

### 1. Clone the repository

```bash
git clone https://github.com/taner04/Taskly
```

### 2. Initialize the project

**With Python (Recommended):**
```bash
python3 .\scripts\init.py
```
This creates the `.env` and `appsettings.json` files needed for the project.

**Without Python (Manual Setup):**

Create a `.env` file in `.\src\Taskly.Web`, `appsettings.json` files in `.\src\Taskly.Api`, and `appsettings.integration.json` in `.\tests\Taskly.IntegrationTests` with the configuration values shown in the next step.

### 3. Configure Auth0 credentials

Update the generated or manually created files with your Auth0 tenant values:

**`.env`:**
```env
AUTH0_DOMAIN=your-auth0-domain
AUTH0_CLIENT_ID=your-client-id
```

**`appsettings.json`:**
```json
{
    "Auth0": {
        "Domain": "your-auth0-domain",
        "Audience": "your-auth0-audience"
    }
}
```

**`appsettings.integration.json`:**
```json
{
    "Auth0": {
        "Domain": "your-auth0-domain",
        "Client_Id": "your-auth0-client-id",
        "Client_Secret": "your-auth0-client-secret",
        "Audience": "your-auth0-audience",
        "Grant_Type": "client_credentials"
    }
}
```

### 4. Create a database migration

**With Python:**
```bash
python3 .\scripts\create-migration.py InitialCreate
```

**Without Python:**
```bash
dotnet ef migrations add InitialCreate --project .\src\Taskly.Data
```

### 5. Build Docker images

**With Python:**
```bash
python3 .\scripts\build.py --docker
```

**Without Python:**
```bash
docker build -f .\Dockerfile -t taskly:latest .
```

### 6. Run with Aspire

Execute the AppHost project to access the applications via the dashboard.

```bash
dotnet run --project .\src\Taskly.AppHost
```

---

## 📚 Documentation

### Overview

This module contains Python utility scripts for managing the Taskly project initialization, database migrations, and build processes.

### [`build.py`](./scripts/build.py)

**Purpose:** Cross-platform build script for compiling .NET projects, running tests, building the web frontend, and optionally building the docker image for the frontend.

**Usage:** `python3 build.py [--docker]`

**Requirements:** Migrations folder, .env file, and appsettings.json files must exist

### [`init.py`](./scripts/init.py)

**Purpose:** One-time initialization script that creates required configuration files with template values for the Taskly project.

**Usage:** `python3 init.py`

**Output:** Creates configuration files with placeholder values requiring manual configuration

### [`create-migration.py`](./scripts/create-migration.py)

**Purpose:** Wrapper script for creating Entity Framework Core database migrations using dotnet CLI tools.

**Usage:** `python3 create-migration.py <migration-name>`

**Example:** `python3 create-migration.py AddTodoTable`

**Output:** Creates migration files in the configured Migrations folder

### Shared Dependencies

All scripts import [`shared.py`](./scripts/shared.py) module:
- `console_logger` — Logging utility with info(), error(), success() methods
- `project_root` — Base project directory path

## 📄 License

This project is licensed under the MIT License — see the [LICENSE.md](LICENSE.md) file for details.

## 👤 Author

[Taner04](https://github.com/taner04)

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.
