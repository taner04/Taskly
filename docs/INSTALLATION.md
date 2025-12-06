# Installation Guide

## Requirements

### Runtime & Tools

- **.NET 9 SDK** — [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Docker Desktop** — [Download](https://www.docker.com/products/docker-desktop)
- **Python 3.8+** _(optional)_ — [Download](https://www.python.org/downloads/) (for automation scripts)

### Auth0 Setup

- Auth0 account — [Sign up free](https://auth0.com/signup)
- SPA application credentials
- Machine-to-Machine application credentials

## Installation Steps

### 1. Clone the repository

```bash
git clone https://github.com/taner04/Taskly
```

### 2. Initialize the project

**With Python (Recommended):**

```bash
python3 .\scripts\setup.py
```

This creates the required `appsettings.json` and `appsettings.integration.json` files.

**Without Python (Manual Setup):**

Create `appsettings.json` in `.\src\Api` and `appsettings.integration.json` in `.\tests\IntegrationTests` with the configuration values shown in the next step.

### 3. Configure Auth0 credentials

Update the generated or manually created files with your Auth0 tenant values:

#### Getting Auth0 Credentials

> [!INFO]
> You need to create **two separate Auth0 applications**:
>
> - **SPA Application** for the API (`appsettings.json`)
> - **Machine-to-Machine Application** for Integration Tests (`appsettings.integration.json`)

1. Go to [Auth0 Dashboard](https://manage.auth0.com/)
2. Navigate to **Applications** → **Applications**
3. Click **+ Create Application**
4. Choose your application type:
   - **Single Page Web Applications** (for API/SPA)
   - **Machine to Machine Applications** (for Integration Tests)
5. For M2M app, select an API (or create one if needed)
6. Copy your `Domain`, `Client ID`, and `Client Secret` from the application settings

#### Update Configuration Files

Both configuration files have the same structure. Use **SPA credentials** for `appsettings.json` and **M2M credentials** for `appsettings.integration.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Auth0": {
    "Domain": "your-auth0-domain",
    "Audience": "your-api-audience",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "UsePersistentStorage": false,
    "Grant_Type": "client_credentials" // Only in appsettings.integration.json
  },
  "ConnectionStrings": {
    "AzureBlobStorage": "your-azure-blob-storage-connection-string"
  }
}
```

**Note:** Add `"Grant_Type": "client_credentials"` only in `appsettings.integration.json` for M2M authentication.

### 4. Run with Aspire

Execute the AppHost project to run the API and managed services:

```bash
dotnet run --project .\tools\AppHost
```
