# Task Completion Checklist

## Required Steps When Task is Completed

### 1. Build Verification
```bash
# Ensure solution builds successfully
dotnet build src/PlanningCenter.Api.sln
```
**Expected Result**: Zero compilation errors

### 2. Unit Test Execution
```bash
# Run all unit tests
dotnet test src/PlanningCenter.Api.Client.Tests/PlanningCenter.Api.Client.Tests.csproj
```
**Expected Result**: All tests pass, no test failures

### 3. Integration Test Verification (Optional)
```bash
# Run integration tests if changes affect API interactions
dotnet test src/PlanningCenter.Api.Client.IntegrationTests/PlanningCenter.Api.Client.IntegrationTests.csproj
```
**Note**: Requires API credentials in `appsettings.local.json`

### 4. Code Coverage Analysis (For Significant Changes)
```powershell
# Run comprehensive code coverage analysis
.\scripts\run-code-coverage.ps1
```
**Target**: Maintain coverage above 80%

### 5. Example Project Verification
```bash
# Verify example projects still work
dotnet run --project examples/PlanningCenter.Api.Client.Console/
dotnet run --project examples/PlanningCenter.Api.Client.Fluent.Console/
```

### 6. Code Quality Checks
- **Nullable Reference Types**: Ensure no nullable warnings
- **XML Documentation**: Add/update XML docs for public APIs
- **Exception Handling**: Proper error handling with correlation IDs
- **Async Patterns**: All I/O operations are async with CancellationToken support

### 7. Git Operations (If Required)
```bash
# Stage changes
git add .

# Commit with descriptive message
git commit -m "descriptive commit message"

# Push if requested
git push
```

## Quality Gates

### Build Quality
- ✅ Zero compilation errors
- ✅ Zero compiler warnings
- ✅ All projects build successfully

### Test Quality
- ✅ All unit tests pass
- ✅ Integration tests pass (if applicable)
- ✅ No test failures or skipped tests
- ✅ Code coverage maintained above 80%

### Code Quality
- ✅ Follows established naming conventions
- ✅ Proper async/await patterns
- ✅ Comprehensive error handling
- ✅ XML documentation for public APIs
- ✅ Correlation ID tracking for operations

### Documentation Quality
- ✅ Code changes documented
- ✅ README.md updated if needed
- ✅ CLAUDE.md updated if development process changes

## Common Issues to Watch For

### Build Issues
- Missing using statements
- Nullable reference type warnings
- Project reference problems
- Package version conflicts

### Test Issues
- Mock setup problems
- Async test execution issues
- Test data generation failures
- Integration test credential problems

### Runtime Issues
- Dependency injection registration
- HTTP client configuration
- Authentication setup
- Exception handling gaps

## Verification Commands Summary
```bash
# Complete verification sequence
dotnet clean src/PlanningCenter.Api.sln
dotnet restore src/PlanningCenter.Api.sln
dotnet build src/PlanningCenter.Api.sln
dotnet test src/PlanningCenter.Api.Client.Tests/
```

**Only proceed with commit/deployment if ALL verification steps pass successfully.**