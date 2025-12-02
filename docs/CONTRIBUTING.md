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
- Node.js 20+
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
- **Database migrations** are managed through `scripts/CreateMigration.py`
- **Tests** are in `tests/IntegrationTests/` and `tests/UnitTests/`

**Running the API:**

```bash
dotnet run --project .\src\Api
```

**Creating a migration:**

```bash
python3 .\scripts\CreateMigration.py YourMigrationName
```

**Running tests:**

```bash
dotnet test
```

### Frontend Development

The frontend is a React app built with Vite and TypeScript:

- **Components** are in `src/Web/src/components/`
- **Services** handle API communication in `src/Web/src/services/`
- **Styling** uses the configured CSS framework in `src/Web/src/styles/`

**Running the development server:**

```bash
cd .\src\Web
npm install
npm run dev
```

**Building for production:**

```bash
npm run build
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

### TypeScript / Frontend

- Follow the existing code style
- Use TypeScript strictly (no `any` types unless absolutely necessary)
- Keep components focused and reusable
- Test components with meaningful test cases

## Making a Contribution

1. **Make your changes** on your feature branch
2. **Test thoroughly**:
   - Run backend tests: `dotnet test`
   - Test frontend manually or with unit tests
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

## Code of Conduct

Be respectful and professional in all interactions. We're all here to make Taskly better together!

## Questions?

If you have questions or need clarification, feel free to open an issue or reach out to the maintainers.
