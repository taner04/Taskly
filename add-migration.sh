#!/usr/bin/env bash

# =========================================================================
# Add Migration Script
# Creates a new EF Core migration for the Taskly API
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
# Configuration
# =========================================================================
PROJECT="src/Api/Api.csproj"
STARTUP_PROJECT="src/Api/Api.csproj"
CONTEXT="Api.Infrastructure.Data.ApplicationDbContext"
OUTPUT_DIR="Infrastructure/Data/Migrations"
CONFIGURATION="Debug"

# =========================================================================
# Check tooling
# =========================================================================
command -v dotnet >/dev/null 2>&1 || error "dotnet SDK not installed or not in PATH."

# =========================================================================
# Read migration name
# =========================================================================
if [ -z "$1" ]; then
  read -rp "Enter migration name: " MIGRATION_NAME
else
  MIGRATION_NAME=$1
fi

[ -z "$MIGRATION_NAME" ] && error "Migration name cannot be empty."

info "Using migration name: $MIGRATION_NAME"

# =========================================================================
# Check for duplicate migration
# =========================================================================
if [ -d "$OUTPUT_DIR" ]; then
  if find "$OUTPUT_DIR" -type f -iname "*${MIGRATION_NAME}*.cs" | grep -q .; then
    error "A migration containing the name '$MIGRATION_NAME' already exists in '$OUTPUT_DIR'."
  fi
fi

# =========================================================================
# Validate EF tooling
# =========================================================================
info "Checking EF tooling..."
dotnet ef --help >/dev/null 2>&1 || error "dotnet-ef not installed or not working."

# =========================================================================
# Build project
# =========================================================================
info "Building project..."
dotnet build "$STARTUP_PROJECT" --configuration "$CONFIGURATION" >/dev/null 2>&1 \
  || error "Build failed. Migration aborted."

# =========================================================================
# Perform migration
# =========================================================================
info "Running EF migration..."

if dotnet ef migrations add "$MIGRATION_NAME" \
  --project "$PROJECT" \
  --startup-project "$STARTUP_PROJECT" \
  --context "$CONTEXT" \
  --configuration "$CONFIGURATION" \
  --output-dir "$OUTPUT_DIR"; then
    success "Migration '$MIGRATION_NAME' created successfully."
else
    error "EF migration command failed – no files created."
fi
