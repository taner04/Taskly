# Installation Guide

## Prerequisites

- **[Docker](https://www.docker.com/)** — Required for containerization
- **[.NET 9](https://dotnet.microsoft.com/download)** — Required for backend development
- **[Node.js 20+](https://nodejs.org/)** — Required for frontend development
- **[Python 3](https://www.python.org/)** _(optional)_ — Simplifies project setup and build automation. Without Python,
  you'll need to manually execute build steps.

## Installation Steps

### 1. Clone the repository

```bash
git clone https://github.com/taner04/Taskly
```

### 2. Initialize the project

**With Python (Recommended):**

```bash
python3 .\scripts\Setup.py
```

This creates the `.env` and `appsettings.json` files needed for the project.

**Without Python (Manual Setup):**

Create a `.env` file in `.\src\Web`, `appsettings.json` files in `.\src\Api`, and
`appsettings.integration.json` in `.\tests\IntegrationTests` with the configuration values shown in the next step.

### 3. Configure Auth0 credentials

Update the generated or manually created files with your Auth0 tenant values:

#### Getting Auth0 Credentials

**For Web Application (SPA):**

1. Go to [Auth0 Dashboard](https://manage.auth0.com/)
2. Navigate to **Applications** → **Applications**
3. Click **+ Create Application**
4. Choose **Single Page Web Applications**
5. Configure the application:
   - **Allowed Callback URLs:** `http://localhost:3000`
   - **Allowed Logout URLs:** `http://localhost:3000`
   - **Allowed Web Origins:** `http://localhost:3000`
6. Copy your `Domain` and `Client ID` from the application settings

**For Integration Tests (Machine-to-Machine):**

1. Go to [Auth0 Dashboard](https://manage.auth0.com/)
2. Navigate to **Applications** → **Applications**
3. Click **+ Create Application**
4. Choose **Machine to Machine Applications**
5. Select an API (or create one if needed)
6. Copy your `Domain`, `Client ID`, and `Client Secret` from the application settings

#### Update Configuration Files

**Important:** The `Domain` and `Audience` values are the **same across all configuration files**. However, `Client ID` and `Client Secret` differ depending on the application type:

- **Web/Frontend** uses credentials from the **SPA application**
- **Integration Tests** uses credentials from the **M2M application**

**`.env` (for Web/Frontend - use SPA credentials):**

```env
VITE_AUTH0_DOMAIN=your-auth0-domain
VITE_AUTH0_CLIENT_ID=your-client-id-from-spa
```

**`appsettings.json` (for API):**

```json
{
  "Auth0": {
    "Domain": "your-auth0-domain",
    "Audience": "your-api-audience"
  }
}
```

**`appsettings.integration.json` (for Integration Tests - use M2M credentials):**

```json
{
  "Auth0": {
    "Domain": "your-auth0-domain",
    "Client_Id": "your-client-id-from-m2m",
    "Client_Secret": "your-client-secret-from-m2m",
    "Audience": "your-api-audience",
    "Grant_Type": "client_credentials"
  }
}
```

### 4. Run with Aspire

Execute the AppHost project to access the applications via the dashboard.

```bash
dotnet run --project .\tools\AppHost
```
