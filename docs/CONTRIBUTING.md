# Contributing to Taskly

Thank you for your interest in contributing to Taskly! We welcome contributions from the community and appreciate your effort to help improve this project.

## Getting Started

1. **Fork the repository** on GitHub
2. **Clone your fork** locally:
   ```bash
   git clone https://github.com/your-username/Taskly.git
   cd Taskly
   ```
3. **Create a feature branch**:
   ```bash
   git checkout -b feature/your-feature-name
   ```

## Development Setup

### Prerequisites

- .NET 9 SDK
- Docker (optional, for local database)
- Python 3 (for automation scripts)

### Initial Setup

Run the setup script to initialize your development environment:

```bash
python3 .\scripts\Setup.py
```

This will create the necessary configuration files with placeholder values. Update them with your development settings.

## Development Workflow

### Backend Development

The backend is built with ASP.NET Core and uses a feature-based architecture:

- **Features** are organized in `src/Api/Features/`
- **Database migrations** are managed through `scripts/create_migration.py`
- **Tests** are in `tests/IntegrationTests/` and `tests/UnitTests/`

**Running the API:**

```bash
dotnet run --project .\src\Api
```

**Creating a migration:**

```bash
python3 .\scripts\create_migration.py YourMigrationName
```

**Running tests:**

```bash
dotnet test
```

## Code Guidelines

### General

- Keep commits focused and descriptive
- Write clear commit messages: "Add feature X" or "Fix bug in Y"
- One feature/fix per branch
- Test your changes before submitting

### C# / Backend

- Follow Microsoft's [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use meaningful variable and method names
- Ensure all tests pass before submitting a PR
- **For new features**: Implement comprehensive tests for the endpoint covering:
  - ✅ Happy path (successful request)
  - ❌ Validation errors (invalid input)
  - 🔒 Authorization failures (unauthorized/forbidden)
  - 🔍 Not found errors (missing resources)
  - ⚠️ Edge cases and boundary conditions
  - Place tests in `tests/IntegrationTests/Tests/` following feature organization

## Making a Contribution

1. **Make your changes** on your feature branch
2. **Test thoroughly**:
   - Run backend tests: `dotnet test`
   - Test the full application flow
3. **Commit your changes** with descriptive messages:
   ```bash
   git commit -m "Add feature description"
   ```
4. **Push to your fork**:
   ```bash
   git push origin feature/your-feature-name
   ```
5. **Create a Pull Request** on GitHub with:
   - Clear title and description
   - Reference to any related issues (e.g., "Closes #123")
   - List of changes made
   - Any breaking changes or important notes

## Pull Request Process

1. Ensure your PR addresses a single feature or bug fix
2. Include tests for new functionality
3. Update documentation if needed
4. Ensure all CI checks pass
5. Be responsive to review feedback
6. Maintainers will merge once approved

## Reporting Issues

Found a bug? Have a feature request? Please open an issue on GitHub with:

- **Clear title** describing the issue
- **Description** of the problem or requested feature
- **Steps to reproduce** (for bugs)
- **Expected vs. actual behavior**
- **Environment** details (OS, .NET version, Node version, etc.)
