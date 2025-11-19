#!/usr/bin/env pwsh

# ---------------------------------------------
# Configuration
# ---------------------------------------------
$PROJECT = "api/src/Api/Api.csproj"
$STARTUP_PROJECT = "api/src/Api/Api.csproj"
$CONTEXT = "Api.Infrastructure.Data.ApplicationDbContext"
$OUTPUT_DIR = "Infrastructure/Data/Migrations"
$CONFIGURATION = "Debug"

# ---------------------------------------------
# Helper functions
# ---------------------------------------------
function ErrorMsg($msg) {
    Write-Host "❌ ERROR: $msg" -ForegroundColor Red
    exit 1
}

function Success($msg) {
    Write-Host "✅ $msg" -ForegroundColor Green
}

function Info($msg) {
    Write-Host "🔹 $msg" -ForegroundColor Cyan
}

# ---------------------------------------------
# Check tooling
# ---------------------------------------------
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    ErrorMsg "dotnet SDK not installed or not in PATH."
}

# ---------------------------------------------
# Read migration name
# ---------------------------------------------
if ($args.Count -eq 0) {
    $MIGRATION_NAME = Read-Host "Enter migration name"
} else {
    $MIGRATION_NAME = $args[0]
}

if ([string]::IsNullOrWhiteSpace($MIGRATION_NAME)) {
    ErrorMsg "Migration name cannot be empty."
}

Info "Using migration name: $MIGRATION_NAME"

# ---------------------------------------------
# Check for duplicate migration in OUTPUT_DIR
# ---------------------------------------------
if (Test-Path $OUTPUT_DIR) {
    $duplicate = Get-ChildItem -Path $OUTPUT_DIR -Recurse -Filter "*$MIGRATION_NAME*.cs"
    if ($duplicate) {
        ErrorMsg "A migration containing the name '$MIGRATION_NAME' already exists in '$OUTPUT_DIR'."
    }
}

# ---------------------------------------------
# Validate EF tooling
# ---------------------------------------------
Info "Checking EF tooling..."
dotnet ef --help *> $null
if ($LASTEXITCODE -ne 0) {
    ErrorMsg "dotnet-ef not installed or not working."
}

# ---------------------------------------------
# Build project
# ---------------------------------------------
Info "Building project..."
dotnet build $STARTUP_PROJECT --configuration $CONFIGURATION *> $null
if ($LASTEXITCODE -ne 0) {
    ErrorMsg "Build failed. Migration aborted."
}

# ---------------------------------------------
# Perform migration
# ---------------------------------------------
Info "Running EF migration..."

dotnet ef migrations add $MIGRATION_NAME `
  --project $PROJECT `
  --startup-project $STARTUP_PROJECT `
  --context $CONTEXT `
  --configuration $CONFIGURATION `
  --output-dir $OUTPUT_DIR

if ($LASTEXITCODE -eq 0) {
    Success "Migration '$MIGRATION_NAME' created successfully."
} else {
    ErrorMsg "EF migration command failed – no files created."
}
