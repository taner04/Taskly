# Taskly

A modern, full-stack task management application built with .NET and React.

## 📋 Overview

Taskly is a professional todo application that helps you organize, prioritize, and track your tasks. With a responsive UI and robust backend, it provides a seamless experience for managing your daily productivity.

## 🚀 Features

- ✅ **Create & Manage Todos** - Add, edit, and delete tasks with titles and descriptions
- 🎯 **Priority Levels** - Organize tasks by Low, Normal, or High priority
- 🔍 **Smart Filtering** - Filter todos by priority or completion status
- 🔎 **Search** - Find todos quickly by title
- 📊 **Dual View Modes** - Switch between grid and list views
- 🔐 **User Authentication** - Secure login with Auth0
- 📱 **Responsive Design** - Works seamlessly on desktop and mobile devices
- 🐳 **Docker Support** - Easy deployment with Docker containers

## 🔧 Getting Started

### Prerequisites

- [**.NET 9**](https://dotnet.microsoft.com/download) or later
- [**Node.js**](https://nodejs.org/) (latest version)
- [**Docker & Docker Compose**](https://www.docker.com/)
- [**Auth0**](https://auth0.com/) application as "Single Page Application"

### Installation

1. **Clone the repository**

```bash
git clone https://github.com/taner04/Taskly.git
cd Taskly
```

2. **Run the initialization script**

```bash
chmod +x init.sh
./init.sh
```

The init script will:

- Create necessary `.env` files
- Set up `appsettings.json` configuration
- Build the .NET API
- Build the Web frontend
- Build Docker images

3. **Configure Auth0 credentials**

After running the init script, update the generated files with your Auth0 credentials:

**`src/Web/.env`**

```env
VITE_AUTH0_DOMAIN=your-auth0-domain
VITE_AUTH0_CLIENT_ID=your-auth0-client-id
```

**`src/Api/appsettings.json`**

```json
"Auth0": {
  "Domain": "your-auth0-domain",
  "Audience": "your-auth0-audience"
}
```

**Where to find these credentials:**

- **Domain**: Your Auth0 tenant domain (e.g., `dev-xxxxx.us.auth0.com`)
- **Client ID**: The application's client ID from Auth0 dashboard
- **Audience**: Your API identifier in Auth0 (create one if it doesn't exist)

4. **Start the application**

```bash
cd tools/AppHost
dotnet run
```

This launches the Aspire dashboard at the browser, where you can view and manage all services.

The MigrationService will automatically apply all pending migrations to the database when the application starts.

### Creating New Migrations (Development Only)

When you make changes to your data models, you need to create a new migration:

```bash
# For macOS/Linux
./add-migration.sh "YourMigrationName"
```

The new migration will be automatically applied the next time you run the Aspire orchestrator.

## 🎨 Using Taskly

### Filtering & Search

- Use the **Priority filter** dropdown to view only High, Normal, or Low priority tasks
- Use the **Status filter** to show only completed or incomplete tasks
- Use the **Search bar** to find todos by title
- Combine filters for more specific results

### View Modes

- Click the **Grid icon** for card-based view (default)
- Click the **List icon** for compact list view

### Managing Todos

- **Double-click** a todo card to view/edit details
- **Refresh button** to reload todos from the server
- Check the **Completed badge** to see task status

## 🏗️ Architecture

### Backend (API)

- **Framework**: ASP.NET Core with Minimal APIs
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core
- **Features**:
  - RESTful API endpoints for todo management
  - User authentication and authorization
  - Query filtering by priority, completion status, and title
  - Request validation and error handling

### Frontend (Web)

- **Framework**: React 18 with TypeScript
- **Build Tool**: Vite
- **Styling**: CSS with responsive design
- **Features**:
  - Modern UI with Tailwind CSS principles
  - Real-time todo state management
  - Auth0 integration for secure authentication
  - Modal-based todo detail view

### Infrastructure

- **Orchestration**: .NET Aspire
- **Containerization**: Docker & Docker Compose
- **Database**: PostgreSQL with pgAdmin
- **Migrations**: Automated database setup

## 📡 API Endpoints

### Todos

- `GET /api/todos` - Get all todos (with optional filters)
  - Query Parameters:
    - `priority` - Filter by priority (0=Low, 1=Normal, 2=High)
    - `isCompleted` - Filter by completion status (true/false)
    - `title` - Search by todo title
- `POST /api/todos` - Create a new todo
- `PUT /api/todos/{id}` - Update a todo
- `DELETE /api/todos/{id}` - Delete a todo

## 📦 Project Structure

```
Taskly/
├── src/
│   ├── Api/                 # .NET Core API
│   │   ├── Features/        # Feature modules (Todos, etc.)
│   │   ├── Infrastructure/  # Data access layer
│   │   └── Program.cs       # API configuration
│   └── Web/                 # React frontend
│       ├── src/
│       │   ├── components/  # React components
│       │   ├── services/    # API integration
│       │   ├── types/       # TypeScript types
│       │   └── App.tsx      # Main component
│       └── Dockerfile       # Web container image
├── tools/
│   ├── AppHost/             # .NET Aspire host
│   ├── MigrationService/    # Database migrations
│   └── ServiceDefaults/     # Shared service configuration
└── tests/
    ├── UnitTests/           # Unit tests
    └── IntegrationTests/    # Integration tests
```

## 🧪 Testing

Run unit tests:

```bash
dotnet test tests/UnitTests
```

Run integration tests:

```bash
dotnet test tests/IntegrationTests
```

## 🐳 Deployment

### Using Aspire Dashboard

The application is pre-configured in the Aspire AppHost to appear in the dashboard. Services include:

- PostgreSQL database with pgAdmin
- .NET API service
- Migration service (automated DB setup)
- Web frontend (containerized)

## 📝 Development

### Tech Stack Summary

- **Backend**: [C#](https://docs.microsoft.com/en-us/dotnet/csharp/), [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet), [EF Core](https://docs.microsoft.com/en-us/ef/core/), [PostgreSQL](https://www.postgresql.org/)
- **Frontend**: [TypeScript](https://www.typescriptlang.org/), [React](https://react.dev/), [Vite](https://vitejs.dev/), [CSS](https://developer.mozilla.org/en-US/docs/Web/CSS)
- **DevOps**: [Docker](https://www.docker.com/), [Docker Compose](https://docs.docker.com/compose/), [.NET Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/)
- **Testing**: [xUnit](https://xunit.net/)
- **Auth**: [Auth0](https://auth0.com/)
- **Package Management**: [NuGet](https://www.nuget.org/), [npm](https://www.npmjs.com/)

### Code Quality

- Minimal APIs for clean endpoint definitions
- CQRS pattern with MediatR
- Entity validation and error handling
- TypeScript strict mode for type safety
- ESLint configuration for code consistency

## 📄 License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## 👤 Author

Taner04 - [GitHub Profile](https://github.com/taner04)

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.
