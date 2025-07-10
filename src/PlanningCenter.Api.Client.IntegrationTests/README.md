# PlanningCenter.Api.Client.IntegrationTests

This project contains integration tests for the Planning Center API Client library. These tests interact with the actual Planning Center API and verify that the client library works correctly against the real API.

## Configuration

The integration tests require valid Planning Center API credentials to run. You can configure these credentials in one of the following ways:

### Option 1: appsettings.local.json

Create an `appsettings.local.json` file in the project directory with the following structure:

```json
{
  "PlanningCenter": {
    "Authentication": {
      "Type": "PersonalAccessToken",
      "ApplicationId": "YOUR_APPLICATION_ID",
      "Secret": "YOUR_SECRET"
    }
  }
}
```

This file is excluded from source control via `.gitignore` to prevent credentials from being committed.

### Option 2: Environment Variables

Set the following environment variables:

```
PLANNING_CENTER_APP_ID=YOUR_APPLICATION_ID
PLANNING_CENTER_SECRET=YOUR_SECRET
```

## Running the Tests

### Using Visual Studio

1. Configure your credentials using one of the methods above
2. Open the solution in Visual Studio
3. Right-click on the `PlanningCenter.Api.Client.IntegrationTests` project and select "Run Tests"

### Using Command Line

```bash
cd src/PlanningCenter.Api.Client.IntegrationTests
dotnet test
```

## Test Categories

The integration tests are organized into the following categories:

1. **PeopleServiceIntegrationTests** - Tests basic CRUD operations for people
2. **PeopleServiceContactInfoIntegrationTests** - Tests address, email, and phone number management
3. **PeopleServiceErrorHandlingTests** - Tests error handling and edge cases
4. **ApiConnectionIntegrationTests** - Tests the core HTTP client functionality

## Best Practices

- Always clean up test data after tests to avoid cluttering the Planning Center database
- Use the `inactive` status for test people to avoid affecting active records
- Use random data for test records to avoid conflicts
- Use the test fixture for shared setup and configuration
- Implement proper error handling and assertions in tests
