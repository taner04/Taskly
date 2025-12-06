# Architecture

![Architecture](./assets/Architecture.pdf)

---

## 📋 Overview

Taskly is a modern, enterprise-grade .NET 9 API built with cloud-native architecture. It demonstrates clean domain modeling, async-first design patterns, and production-ready security practices.

---

## 🏗️ Technology Stack

### Backend

- **.NET 9** — Modern C# runtime with nullable reference types
- **Entity Framework Core** — ORM with PostgreSQL provider
- **ASP.NET Core** — Web framework with minimal APIs
- **Auth0** — JWT-based authentication and authorization
- **Azure Blob Storage** — Cloud file storage with SAS URLs
- **Immediate.Apis** — CQRS framework with source-generated handlers
- **Immediate.Validations** — Declarative validation framework
- **Vogen** — Strongly-typed value object code generation
- **Refit** — Type-safe HTTP client generation
- **ErrorOr** — Functional error handling

### Infrastructure

- **PostgreSQL** — Relational database with ACID transactions
- **Azure Blob Storage** — Scalable file storage for attachments
- **Docker** — Containerization
- **.NET Aspire** — Cloud-native orchestration
- **Python 3** — Automation scripts

---

## 🔐 Authentication & Authorization

### JWT Bearer Authentication

- **Authority**: Auth0 (`dev-r2na1xjgqc87zkzh.us.auth0.com`)
- **Audience**: `https://taskly-api`
- **Token Validation**: JWT Bearer scheme applied to all protected endpoints
- **User Context**: User ID extracted from JWT claims and scoped to all requests

### Authorization Pattern

- `[Authorize]` attribute on protected endpoints
- `CurrentUserService` provides authenticated user context
- All data filtered by `UserId` for user isolation
- Claims-based access control throughout the application

### OpenAPI Integration

- Dual security schemes: JWT Bearer + OAuth2 (Authorization Code Flow)
- Bearer token scheme configured for interactive Scalar UI testing
- Auth0 credentials dynamically injected via configuration

---

## 🎯 Feature Organization

The API is organized around business features, not technical layers:

```
Features/
├── Todos/
│   ├── Endpoints/          # Handler-based endpoints (CQRS)
│   ├── Model/              # Domain entities + value objects
│   └── Exceptions/         # Feature-specific exceptions
├── Tags/                   # Tag management
├── Attachments/            # File upload/download orchestration
├── Users/                  # Auth & user context
└── Shared/                 # Common DTOs, exceptions, routes
```

### Each Feature Contains

- **Endpoints/** — CQRS handlers using Immediate.Apis `[Handler]` attribute
- **Model/** — Domain entities inheriting from `Entity<TId>`
- **Exceptions/** — Custom feature exceptions
- **Dtos/** — API response models with mapping from domain

---

## 🔄 Request/Response Pipeline

### Handler Pattern (Immediate.Apis)

```csharp
[Handler]                              // Immediate meta
[MapPost(Routes.Todos.Create)]        // HTTP route binding
[Authorize]                            // Security
public static partial class CreateTodo
{
    private static async ValueTask<Response> HandleAsync(
        Command command,                // Request DTO (validated)
        ApplicationDbContext context,   // DI injection
        CurrentUserService service)     // Scoped service
    {
        // Business logic
        return new Response(...);
    }
    
    internal static Created<Response> TransformResult(Response r)
    {
        return TypedResults.Created(...);  // Type-safe HTTP results
    }
    
    [Validate]                          // Immediate validation
    public record Command : IValidationTarget<Command> { ... }
    public record Response { ... }
}
```

### Middleware Pipeline

1. **Health Checks & Telemetry** — Aspire defaults
2. **OpenAPI Documentation** — Scalar UI for API exploration
3. **Authentication** — JWT token validation
4. **Authorization** — Claims-based access control
5. **HTTPS Redirection** — Security enforcement
6. **Endpoint Mapping** — Immediate handlers discovery
7. **Blob Storage Initialization** — Ensure containers exist

---

## 💾 Database Architecture

### Entity Relationships

```
Todo (1) ──────────────── (*) Attachment
           One-to-Many

Todo (*) ──────────────── (*) Tag
         Many-to-Many
         (TodoTags join table)
```

### Entity Base Hierarchy

```
IEntity<TId>     → Requires strongly-typed Id
    ↓
Auditable        → Tracks CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
    ↓
Entity<TId>      → Combines both
    ↓
Todo, Tag        → Concrete domain entities
```

### Value Objects (Vogen)

```csharp
[ValueObject<Guid>]
public readonly partial struct TodoId;  // Auto-generated to/from Guid conversion
```

Benefits:
- Type-safe IDs prevent accidental ID mixing
- Compile-time safety instead of runtime validation
- Seamless EF Core integration

### Database Auditing

The `AuditableInterceptor` automatically:
- Sets `CreatedAt` and `CreatedBy` on new entities
- Sets `UpdatedAt` and `UpdatedBy` on modified entities
- Extracts current user from scoped `CurrentUserService`

---

## 🔍 Error Handling

### Exception Hierarchy

```
ModelBaseException (abstract)
├── ModelNotFoundException<T>           → 404 Not Found
└── Feature-Specific Exceptions
    ├── TodoInvalidTitleException
    ├── TodoInvalidDescriptionException
    └── TagInvalidNameException
```

### Global Problem Details (RFC 7807)

All exceptions are converted to standardized HTTP responses:

```json
{
  "status": 400,
  "title": "Validation failed.",
  "detail": "...",
  "errorCode": "validation.error",
  "errors": { "Title": ["Title is required"] },
  "instance": "/api/todos",
  "extensions": {
    "method": "POST",
    "traceId": "...",
    "userId": "..."
  }
}
```

### Error Mapping

- Validation exceptions → 400 Bad Request with field errors
- Not found exceptions → 404 Not Found
- Unauthorized access → 401 Unauthorized
- Unhandled exceptions → 500 Internal Server Error

---

## 📦 Dependency Injection Architecture

### Composition Root (Program.cs)

All services configured through extension methods:

```csharp
services
    .AddServiceDefaults()                    // Telemetry, health checks
    .AddScalarOpenApi()                      // Interactive API docs
    .AddProblemDetailsConfig()               // Error handling
    .AddAuthenticationWithAuth0()            // JWT/OAuth setup
    .AddApplicationServices()                // Domain services
    .AddInfrastructureServices()             // DbContext, interceptors
    .AddImmediateHandlers()                  // CQRS handler registration
```

### Lifetime Management

- **Scoped** — `ApplicationDbContext`, `CurrentUserService` (per-request)
- **Singleton** — `BlobServiceClient`, `ILogger<T>` (reused across requests)

---

## 📂 File Storage (Azure Blob)

### Blob Service Integration

- **Singleton Client** — Reused connection across requests
- **SAS URL Generation** — Secure pre-signed URLs avoid exposing storage keys
- **Expiration Policies** — 5 minutes for downloads, 10 minutes for uploads

### Upload/Download Flow

1. Client requests download URL from API
2. API generates SAS URL with read permission (5 min expiry)
3. Client downloads file directly from blob storage
4. Similar flow for uploads with write permissions

---

## ✅ Validation Strategy

### Declarative Validation (Immediate.Validations)

```csharp
[Validate]
public record Command : IValidationTarget<Command>
{
    [NotEmpty] 
    [MaxLength(500)]
    public required string Title { get; init; }
}
```

### Domain-Level Validation

```csharp
// In entity constructor
if (title.Length is > MaxTitleLength or < MinTitleLength)
    throw new TodoInvalidTitleException(title.Length);
```

### Validation Pipeline

1. Immediate framework validates attributes
2. Handler invoked only if attributes pass
3. Domain entity constructor validates business rules
4. Both caught by global problem details handler

---

## 🧪 Testing Architecture

### Test Layers

**IntegrationTests:**
- `TestingBase` abstract class for shared test infrastructure
- `WebApiFactory` manages test server lifecycle
- Mock database (SQL Server in-memory or real PostgreSQL)
- Azure Blob Storage mock (Azurite)

**Key Testing Features:**
- Strongly-typed HTTP client via Refit (`IApiClient`)
- Dependency injection for mocking
- Async test lifecycle management
- Parallel test execution within collections

### Mock Infrastructure

```csharp
var services = new ServiceCollection()
    .AddMockDbContext(connection)           // In-memory DB
    .AddMockAuth0Service()                  // Auth mocking
    .AddMockBlobStorage();                  // Azurite

var server = new TestWebApplicationBuilder(services).Build();
```

---

## 🚀 Architectural Decisions

| Decision | Rationale |
|----------|-----------|
| **CQRS via Immediate.Apis** | Cleaner separation, auto source-generated, strongly-typed routing |
| **Value Objects (Vogen)** | Type safety for IDs, prevent accidental ID swapping |
| **Scoped DbContext** | Request-level isolation, automatic change tracking |
| **Interceptors** | Audit concerns without cluttering business logic |
| **Problem Details (RFC 7807)** | Standardized errors, client-friendly responses |
| **Auth0 JWT** | Managed authentication, no password storage needed |
| **SAS URLs** | Secure file storage, avoids exposing connection strings |
| **PostgreSQL** | ACID transactions, relational integrity, cost-effective |
| **Feature-Driven Layout** | Business domain organization, minimal coupling |
| **Refit Clients in Tests** | Type-safe HTTP calls, contract verification |

---

## 🔗 Component Interaction

The request flow follows this architecture:

1. **Client** → Requests a token from **Auth0** using client credentials
2. **Auth0** → Returns a JWT token to the client
3. **Client** → Sends API request with JWT token to the **API**
4. **API** → Validates the token and processes the request
5. **API** → Queries or updates data in the **Database**
6. **API** → For file uploads/downloads, generates signed URLs to **Azure Blob Storage**
7. **Client** → Uses the signed URL to upload/download files directly to/from **Azure Blob Storage**
8. **API** → Returns the response to the client

```
┌────────────┐
│   Auth0    │
└─────┬──────┘
      │
      │ 1. Request Token
      │ 2. Return JWT
      │
      ▼
┌──────────────────────┐
│   Client/Consumer    │
└──────────┬───────────┘
           │
           │ 3. API Request + JWT Token
           │
           ▼
     ┌─────────────────┐
     │   API (.NET)    │
     └─────────────────┘
           │
           ├─────────────────────┬──────────────────────┐
           │                     │                      │
           │ 5. Query/Update     │ 6. Generate          │ 8. Response
           │                     │    Signed URL        │
           ▼                     ▼                      │
     ┌─────────────┐      ┌────────────────────┐      │
     │  Database   │      │ Azure Blob Storage │      │
     └─────────────┘      └────────────────────┘      │
                                   ▲                   │
                                   │                   │
                                   │ 7. Upload/Download
                                   │ (Signed URL)
                                   │
                          ┌────────┴─────────┐
                          │                  │
                          Client/Consumer───┘
```

---

## 📚 Key Features

- **Authentication** — Seamless Auth0 integration with JWT validation
- **Feature-organized structure** — Code organized by business features, not technical layers
- **Automated setup** — Python scripts automate initialization and migrations
- **Testing** — Comprehensive integration and unit tests with mock infrastructure
- **Cloud-native ready** — Built with .NET Aspire for cloud deployment
- **Error Handling** — RFC 7807 Problem Details for standardized error responses
- **File Storage** — Secure Azure Blob Storage with pre-signed SAS URLs
- **Auditing** — Automatic tracking of who created/modified data and when
