# Architecture

## Project Structure

```
Taskly/
├── src/                           # Source code
│   ├── api/                       # ASP.NET Core API
│   │   ├── Program.cs             # App entry point
│   │   ├── Api.csproj             # Project file
│   │   ├── GlobalUsings.cs        # Global using statements
│   │   ├── DependencyInjection.cs # DI configuration
│   │   ├── Abstractions/          # Interfaces
│   │   │   ├── IAggregate.cs
│   │   │   ├── IAuditable.cs
│   │   │   └── IEntity.cs
│   │   ├── Features/              # Feature-organized code
│   │   │   ├── Shared/            # Shared feature logic
│   │   │   └── Todos/             # Todo management
│   │   ├── Behaviors/             # MediatR behaviors
│   │   │   └── Logger/            # Logging behavior
│   │   ├── Composition/           # Configuration
│   │   │   ├── OpenApiDocumentTransformers/
│   │   │   └── ServiceExtensions/
│   │   ├── Infrastructure/        # Data & external services
│   │   │   └── Data/              # Database context & migrations
│   │   └── Properties/            # Launch settings
│   │
│   └── web/                       # React + TypeScript frontend
│       ├── package.json           # NPM dependencies
│       ├── vite.config.ts         # Vite configuration
│       ├── tsconfig.json          # TypeScript config
│       ├── index.html             # HTML entry point
│       ├── src/
│       │   ├── main.tsx           # React entry point
│       │   ├── App.tsx            # Root component
│       │   ├── components/        # Reusable components
│       │   ├── services/          # API clients
│       │   ├── contexts/          # React contexts
│       │   ├── types/             # TypeScript types
│       │   ├── assets/            # Static assets
│       │   └── styles/            # Styling
│       └── public/                # Public static files
│
├── tests/                         # Test projects
│   ├── IntegrationTests/          # API integration tests
│   │   ├── IntegrationTests.csproj
│   │   ├── GlobalUsings.cs
│   │   ├── Infrastructure/        # Test infrastructure
│   │   │   ├── TestingBase.cs
│   │   │   ├── WebApiFactory.cs
│   │   │   ├── Auth0Service.cs
│   │   │   └── Fixtures/
│   │   ├── Extensions/            # Test extensions
│   │   └── Tests/                 # Test classes
│   │       └── Todos/
│   │
│   └── UnitTests/                 # Unit tests
│       ├── UnitTests.csproj
│       └── Tests/
│
├── tools/                         # Tooling projects
│   ├── AppHost/                   # .NET Aspire orchestration
│   │   ├── AppHost.cs
│   │   ├── AppHost.csproj
│   │   └── Properties/
│   │
│   ├── MigrationService/          # Database migration service
│   │   ├── Program.cs
│   │   ├── Worker.cs
│   │   ├── MigrationService.csproj
│   │   └── Properties/
│   │
│   └── ServiceDefaults/           # Shared service configuration
│       ├── Extensions.cs
│       ├── AppHostConstants.cs
│       └── ServiceDefaults.csproj
│
├── .gitignore                     # Git ignore rules
├── Directory.Packages.props       # NuGet package management
├── global.json                    # .NET version config
├── LICENSE.md                     # MIT License
├── README.md                      # Main documentation
└── Taskly.sln                     # Solution file
```

## Directory Descriptions

### Backend (.NET)

The API is structured using feature-based organization:

- **`Features/`** — Feature-organized endpoints and handlers (e.g., Todos)
- **`Infrastructure/`** — Data access, migrations, and external services
- **`Composition/`** — Dependency injection and OpenAPI configuration
- **`Abstractions/`** — Domain interfaces

### Frontend (React + TypeScript)

The Web app uses Vite for fast development and building:

- **`components/`** — Reusable React components
- **`services/`** — API client and external service calls
- **`contexts/`** — React context for state management
- **`types/`** — TypeScript type definitions
- **`styles/`** — Application styling

## Technology Stack

### Backend

- **.NET 9** — Modern C# runtime
- **Entity Framework Core** — ORM for database access
- **ASP.NET Core** — Web framework
- **Auth0** — Authentication and authorization
- **.NET Aspire** — Cloud-native orchestration

### Frontend

- **React 18** — UI library
- **TypeScript** — Type-safe JavaScript
- **Vite** — Fast build tool and dev server
- **Auth0 React SDK** — Frontend authentication
- **TailwindCSS** — Utility-first CSS (optional)

### Infrastructure

- **Docker** — Containerization
- **PostgreSQL** — Database (in Aspire)
- **Python 3** — Automation scripts

## Key Features

- **Authentication** — Seamless Auth0 integration across frontend and backend
- **Feature-organized structure** — Code organized by business features, not technical layers
- **Automated setup** — Python scripts automate initialization and migrations
- **Testing** — Comprehensive integration and unit tests
- **Cloud-native ready** — Built with .NET Aspire for cloud deployment
