# Suggested Development Commands

## Build Commands
```bash
# Build the entire solution
dotnet build src/PlanningCenter.Api.sln

# Build specific project
dotnet build src/PlanningCenter.Api.Client/PlanningCenter.Api.Client.csproj

# Clean and rebuild
dotnet clean src/PlanningCenter.Api.sln
dotnet build src/PlanningCenter.Api.sln

# Restore dependencies
dotnet restore src/PlanningCenter.Api.sln
```

## Testing Commands
```bash
# Run all unit tests
dotnet test src/PlanningCenter.Api.Client.Tests/PlanningCenter.Api.Client.Tests.csproj

# Run integration tests (requires configuration)
dotnet test src/PlanningCenter.Api.Client.IntegrationTests/PlanningCenter.Api.Client.IntegrationTests.csproj

# Run specific test project
dotnet test src/PlanningCenter.Api.Client.Tests/

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run code coverage analysis (PowerShell script)
.\scripts\run-code-coverage.ps1
```

## Example Project Commands
```bash
# Run console examples
dotnet run --project examples/PlanningCenter.Api.Client.Console/
dotnet run --project examples/PlanningCenter.Api.Client.Fluent.Console/

# Run background worker example
dotnet run --project examples/PlanningCenter.Api.Client.Worker/

# Run CLI tool
cd examples/PlanningCenter.Api.Client.CLI
dotnet run -- --help
```

## Windows System Commands
```cmd
# Directory listing
dir
ls  # If PowerShell

# File operations
copy [source] [destination]
move [source] [destination]
del [file]

# Git operations
git status
git add .
git commit -m "message"
git push
git pull

# Process management
tasklist
taskkill /PID [process_id]

# Find files
dir /s [filename]
findstr /s "text" *.cs
```

## PowerShell Specific Commands
```powershell
# List files
Get-ChildItem
ls

# Find text in files
Select-String "pattern" *.cs -Recurse

# Process management
Get-Process
Stop-Process -Name "process"

# Environment variables
$env:VARIABLE_NAME
```

## Package Management
```bash
# Add NuGet package
dotnet add package [PackageName]

# Remove NuGet package
dotnet remove package [PackageName]

# List packages
dotnet list package

# Update packages
dotnet restore
```