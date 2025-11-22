#!/usr/bin/env bash
# =========================================================================
# Taskly Initialization Script
# This script initializes and builds the entire Taskly project including:
# - Creating necessary .env files
# - Setting up appsettings configuration
# - Building the .NET API
# - Building the Web frontend
# - Building Docker images
#
# NOTE: This script should only be run via the Aspire project.
#       The Aspire orchestrator handles the proper setup and execution
#       of all services in the correct order.
# =========================================================================
set -e

# Color codes
RED='\033[0;31m'
GREEN='\033[0;32m'
BLUE='\033[0;34m'
ORANGE='\033[0;33m'
NC='\033[0m' # No Color

# Helper functions
error() {
  echo -e "${RED}[ERROR]${NC} $1" >&2
  exit 1
}

success() {
  echo -e "${GREEN}[SUCCESS]${NC} $1"
}

info() {
  echo -e "${BLUE}[INFO]${NC} $1"
}

warning() {
  echo -e "${ORANGE}[WARNING]${NC} $1"
}

# =========================================================================
# Check Prerequisites
# =========================================================================
info "Checking prerequisites..."

command -v dotnet >/dev/null 2>&1 || error "dotnet SDK not installed or not in PATH."
DOTNET_VERSION=$(dotnet --version | cut -d. -f1)
[ "$DOTNET_VERSION" -lt 9 ] && error "dotnet 9 or later required. Found version: $(dotnet --version)"
success "dotnet $(dotnet --version) found"

command -v node >/dev/null 2>&1 || error "Node.js not installed or not in PATH."
success "Node.js $(node --version) found"

command -v npm >/dev/null 2>&1 || error "npm not installed or not in PATH."
success "npm $(npm --version) found"

# =========================================================================
# Setup Environment Files
# =========================================================================
info "Setting up environment files..."

# Web .env setup
WEB_ENV="src/Web/.env"
if [ ! -f "$WEB_ENV" ]; then
  warning "Creating $WEB_ENV (please update with your values)"
  cat > "$WEB_ENV" << 'EOF'
# Auth0 Configuration
VITE_AUTH0_DOMAIN=your-auth0-domain
VITE_AUTH0_CLIENT_ID=your-auth0-client-id
EOF
  warning "Please update $WEB_ENV with your configuration values"
else
  success "$WEB_ENV already exists"
fi

# =========================================================================
# Setup appsettings Configuration
# =========================================================================
info "Checking appsettings configuration..."

API_APPSETTINGS="src/Api/appsettings.json"
if [ ! -f "$API_APPSETTINGS" ]; then
  warning "Creating $API_APPSETTINGS"
  cat > "$API_APPSETTINGS" << 'EOF'
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
    "Audience": "your-auth0-audience"
  }
}
EOF
  success "Created $API_APPSETTINGS"
else
  success "$API_APPSETTINGS already exists"
fi

# =========================================================================
# Build .NET Projects
# =========================================================================
info "Building .NET projects..."

info "Restoring NuGet packages..."
dotnet restore src/Api/Api.csproj || error "Failed to restore API packages"
dotnet restore tests/UnitTests/UnitTests.csproj || error "Failed to restore UnitTests packages"
dotnet restore tests/IntegrationTests/IntegrationTests.csproj || error "Failed to restore IntegrationTests packages"
success "NuGet packages restored"

info "Building API project..."
dotnet build src/Api/Api.csproj -c Release || error "Failed to build API"
success "API project built"

info "Building test projects..."
dotnet build tests/UnitTests/UnitTests.csproj -c Release || error "Failed to build UnitTests"
dotnet build tests/IntegrationTests/IntegrationTests.csproj -c Release || error "Failed to build IntegrationTests"
success "Test projects built"

# =========================================================================
# Build Web Project
# =========================================================================
info "Building Web project..."

info "Installing npm dependencies..."
cd src/Web
npm install || error "Failed to install npm dependencies"
success "Dependencies installed"

info "Building Web application..."
npm run build || error "Failed to build Web application"
success "Web application built"

cd - > /dev/null

# =========================================================================
# Build Docker Images
# =========================================================================
info "Building Docker images..."

if command -v docker >/dev/null 2>&1; then
  info "Building Web Docker image..."
  docker build -t taskly-web:latest src/Web || error "Failed to build Web Docker image"
  success "Web Docker image built: taskly-web:latest"
else
  warning "Docker not found. Skipping Docker image builds."
fi

# =========================================================================
# Build Summary
# =========================================================================
echo ""
echo "# ==========================================================================="
success "Build completed successfully!"
echo "# ==========================================================================="
echo ""
echo "Next steps:"
echo "1. Update configuration files with your actual values:"
echo "   - $WEB_ENV"
echo "   - $API_APPSETTINGS"
echo ""
echo "2. Run the Aspire orchestrator:"
echo "   cd tools/AppHost && dotnet run"
echo ""
echo "# ==========================================================================="
