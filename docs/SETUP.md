# Setup Guide

## Requirements

### Runtime & Tools

- **.NET 9 SDK** — [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Docker Desktop** — [Download](https://www.docker.com/products/docker-desktop)
- **Python 3.8+** _(optional)_ — [Download](https://www.python.org/downloads/) (for automation scripts)

### Auth0 Setup

- Auth0 account — [Sign up free](https://auth0.com/signup)
- SPA application credentials

## Setup Steps

### 1. Clone the repository

```bash
git clone https://github.com/taner04/Taskly
```

### 2. Initialize the project

**With Python (Recommended):**

```bash
python3 .\scripts\setup.py
```

This creates the required `appsettings.json` file.

**Without Python (Manual Setup):**

Create `appsettings.json` in `.\src\Api` with the configuration values shown in the next step.

### 3. Configure Auth0 credentials

Update the generated or manually created `appsettings.json` file with your Auth0 tenant values:

#### Getting Auth0 Credentials

> [!NOTE]
> You only need to create **one Auth0 SPA application** for the API (`appsettings.json`).
> Integration tests use mocked JWT tokens and do not require Auth0 credentials.

1. Go to [Auth0 Dashboard](https://manage.auth0.com/)
2. Navigate to **Applications** → **Applications**
3. Click **+ Create Application**
4. Choose **Single Page Web Applications**
5. Copy your `Domain`, `Client ID`, and `Client Secret` from the application settings

#### Update Configuration File

Update `appsettings.json` in `.\src\Api` with your SPA credentials:

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
    "UsePersistentStorage": false
  },
  "ConnectionStrings": {
    "AzureBlobStorage": "your-azure-blob-storage-connection-string"
  }
}
```

### 4. Run with Aspire

Execute the AppHost project to run the API and managed services:

```bash
dotnet run --project .\tools\AppHost
```

> [!NOTE]
> The **ReminderService** is automatically configured with Aspire and requires no additional setup. The Papercut SMTP
> configuration (Host: localhost, Port: 25) is pre-configured and will work out of the box when running through Aspire.

---

## Authorization & Access Control

The API implements **role-based access control (RBAC)** using Auth0 roles:

### Policies

- **User Policy** — Default policy for regular users
  - Can create, read, update, and delete their own todos
  - Can manage tags and attachments
  - Can set reminders for their todos

- **Admin Policy** — Administrative access
  - All User policy permissions
  - Access to admin-only endpoints
  - Can view system-wide statistics and logs

### Role Claims

Roles are transmitted via JWT claims using the pattern: `{Audience}/roles`

Example claim:
```
"https://taskly-api/roles": "User"
```

Ensure your Auth0 application is configured to include role claims in the JWT token for proper authorization.
