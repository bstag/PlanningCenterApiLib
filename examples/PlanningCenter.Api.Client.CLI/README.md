# Planning Center CLI Tool

A command-line interface for interacting with the Planning Center API using Personal Access Tokens.

## Features

- **Authentication**: Secure Personal Access Token management
- **Multiple Output Formats**: JSON (default), CSV, XML, and Table formats
- **Complete Module Coverage**: People, Services, Registrations, Calendar, and Check-Ins modules
- **Configuration Management**: Store and manage CLI settings
- **Advanced Filtering**: Complex filtering, sorting, and pagination options
- **Flexible Output**: Property selection, file export, and customizable formatting
- **Error Handling**: User-friendly error messages and comprehensive logging

## Installation

1. Navigate to the CLI project directory:
   ```bash
   cd examples/PlanningCenter.Api.Client.CLI
   ```

2. Build the project:
   ```bash
   dotnet build
   ```

3. Run the CLI:
   ```bash
   dotnet run -- [command] [options]
   ```

## Quick Start

### 1. Set Your Personal Access Token

```bash
# Set your token (format: app_id:secret)
dotnet run -- config set-token "your_app_id:your_secret"

# Test the token
dotnet run -- config test-token
```

### 2. Get Your User Information

```bash
dotnet run -- people me
```

### 3. List People

```bash
# List first 10 people
dotnet run -- people list --page-size 10

# List people with JSON output
dotnet run -- people list --format json

# Search for people by name
dotnet run -- people search "John Doe"
```

## Available Modules

### People Module ✅
- List, search, and get people
- Manage households and field data
- User profile information
- Advanced filtering and sorting

### Services Module ✅
- List and get service plans
- Manage service types
- Song and team management
- Service scheduling

### Registrations Module ✅
- List events and registrations
- Get event details
- Manage attendees and signups
- Registration analytics

### Calendar Module ✅
- List and manage events
- Event scheduling and resources
- Calendar integration
- Event filtering and search

### Check-Ins Module ✅
- List events and check-ins
- Location management
- Attendance tracking
- Check-in analytics

## Commands

### Configuration Commands

#### `config set-token <token>`
Store your Personal Access Token securely.

```bash
dotnet run -- config set-token "abc123:def456789"
```

#### `config get-token`
Display the stored token (masked for security).

```bash
dotnet run -- config get-token
```

#### `config test-token [--token <token>]`
Test authentication with stored or provided token.

```bash
dotnet run -- config test-token
dotnet run -- config test-token --token "abc123:def456789"
```

#### `config show`
Display current configuration settings.

```bash
dotnet run -- config show
```

### People Commands

The People module provides comprehensive people management functionality.

#### `people list [options]`
List people with optional filtering and pagination.

**Options:**
- `--page-size <number>`: Number of items per page (default: 25)
- `--page <number>`: Page number to retrieve (default: 1)
- `--where <conditions>`: Filter conditions (e.g., 'first_name=John,status=active')
- `--order <field>`: Sort order (e.g., 'last_name', '-created_at')
- `--include <fields>`: Related resources to include
- `--format <format>`: Output format (table, json, csv, xml)
- `--include-props <props>`: Properties to include in output
- `--exclude-props <props>`: Properties to exclude from output
- `--output-file <file>`: File to write output to
- `--include-nulls`: Include null values in output

**Examples:**
```bash
# List first 50 people
dotnet run -- people list --page-size 50

# List people with specific filters
dotnet run -- people list --where "status=active,first_name=John"

# List people sorted by last name
dotnet run -- people list --order "last_name"

# Export to CSV
dotnet run -- people list --format csv --output-file people.csv

# Include only specific properties
dotnet run -- people list --include-props "id,first_name,last_name,email"
```

#### `people get <id> [options]`
Get a specific person by ID.

**Examples:**
```bash
# Get person by ID
dotnet run -- people get 12345

# Get person with related data
dotnet run -- people get 12345 --include "emails,phone_numbers"

# Get person as JSON
dotnet run -- people get 12345 --format json
```

#### `people search <query> [options]`
Search for people by name or email.

**Examples:**
```bash
# Search by name
dotnet run -- people search "John Doe"

# Search with limit
dotnet run -- people search "john@example.com" --limit 10

# Search and export to CSV
dotnet run -- people search "Smith" --format csv --output-file search_results.csv
```

#### `people me [options]`
Get information about the authenticated user.

**Examples:**
```bash
# Get your user info
dotnet run -- people me

# Get as JSON
dotnet run -- people me --format json
```

#### `people households [options]`
List households with optional filtering.

**Examples:**
```bash
# List households
dotnet run -- people households

# List with filters
dotnet run -- people households --where "name=Smith Family"
```

#### `people field-data <person-id> [options]`
Get field data for a specific person.

**Examples:**
```bash
# Get field data for person
dotnet run -- people field-data 12345

# Export field data to JSON
dotnet run -- people field-data 12345 --format json
```

### Services Commands

The Services module manages worship services, plans, and scheduling.

#### `services list-plans [options]`
List service plans with optional filtering.

**Examples:**
```bash
# List service plans
dotnet run -- services list-plans

# List plans for specific service type
dotnet run -- services list-plans --where "service_type_id=123"

# Export plans to JSON
dotnet run -- services list-plans --format json --output-file plans.json
```

#### `services get-plan <id> [options]`
Get a specific service plan by ID.

**Examples:**
```bash
# Get service plan
dotnet run -- services get-plan 12345

# Get plan with items included
dotnet run -- services get-plan 12345 --include "items"
```

#### `services list-service-types [options]`
List available service types.

**Examples:**
```bash
# List service types
dotnet run -- services list-service-types

# Filter by frequency
dotnet run -- services list-service-types --where "frequency=weekly"
```

#### `services list-songs [options]`
List songs in the song library.

**Examples:**
```bash
# List songs
dotnet run -- services list-songs

# Search songs by title
dotnet run -- services list-songs --where "title=Amazing Grace"
```

### Registrations Commands

The Registrations module manages event registrations and attendees.

#### `registrations list-events [options]`
List registration events.

**Examples:**
```bash
# List events
dotnet run -- registrations list-events

# List active events only
dotnet run -- registrations list-events --where "status=active"
```

#### `registrations get-event <id> [options]`
Get a specific registration event by ID.

**Examples:**
```bash
# Get event details
dotnet run -- registrations get-event 12345

# Get event with registrations
dotnet run -- registrations get-event 12345 --include "registrations"
```

### Calendar Commands

The Calendar module manages calendar events and resources.

#### `calendar list-events [options]`
List calendar events.

**Examples:**
```bash
# List events
dotnet run -- calendar list-events

# List events for specific date range
dotnet run -- calendar list-events --where "starts_at>2024-01-01"
```

#### `calendar get-event <id> [options]`
Get a specific calendar event by ID.

**Examples:**
```bash
# Get event details
dotnet run -- calendar get-event 12345

# Get event with resources
dotnet run -- calendar get-event 12345 --include "resource_bookings"
```

### Check-Ins Commands

The Check-Ins module manages event check-ins and attendance.

#### `checkins list-events [options]`
List check-in events.

**Examples:**
```bash
# List events
dotnet run -- checkins list-events

# List events with check-ins
dotnet run -- checkins list-events --include "check_ins"
```

#### `checkins get-event <id> [options]`
Get a specific check-in event by ID.

**Examples:**
```bash
# Get event details
dotnet run -- checkins get-event 12345

# Get event with locations
dotnet run -- checkins get-event 12345 --include "locations"
```

## Global Options

These options are available for all commands:

- `--token <token>`: Personal Access Token (overrides stored token)
- `--format <format>`: Output format (table, json, csv, xml)
- `--verbose`: Enable verbose logging

## Output Formats

### JSON (Default)
Structured JSON output, perfect for scripting and automation. This is now the default format.

```bash
dotnet run -- people list
# Outputs JSON by default
```

### Table
Human-readable table format with automatic column sizing.

```bash
dotnet run -- people list --format table
```

### CSV
Comma-separated values format for spreadsheet applications.

```bash
dotnet run -- people list --format csv --output-file people.csv
```

### XML
Structured XML output for systems that require XML format.

```bash
dotnet run -- people list --format xml
```

## Filtering and Searching

### Where Conditions
Use the `--where` option to filter results:

```bash
# Single condition
dotnet run -- people list --where "status=active"

# Multiple conditions
dotnet run -- people list --where "status=active,first_name=John"

# With quotes for values containing spaces
dotnet run -- people list --where "first_name='John Doe'"
```

### Ordering
Use the `--order` option to sort results:

```bash
# Ascending order
dotnet run -- people list --order "last_name"

# Descending order (prefix with -)
dotnet run -- people list --order "-created_at"

# Multiple fields
dotnet run -- people list --order "last_name,first_name"
```

### Including Related Data
Use the `--include` option to include related resources:

```bash
# Include emails and phone numbers
dotnet run -- people get 12345 --include "emails,phone_numbers"

# Include multiple related resources
dotnet run -- people list --include "households,field_data"
```

## Property Selection

### Include Specific Properties
Use `--include-props` to show only specific properties:

```bash
dotnet run -- people list --include-props "id,first_name,last_name,email"
```

### Exclude Properties
Use `--exclude-props` to hide specific properties:

```bash
dotnet run -- people list --exclude-props "created_at,updated_at"
```

## File Output

Save output to a file using the `--output-file` option:

```bash
# Save as JSON
dotnet run -- people list --format json --output-file people.json

# Save as CSV
dotnet run -- people list --format csv --output-file people.csv

# Save as XML
dotnet run -- people list --format xml --output-file people.xml
```

## Error Handling

The CLI provides user-friendly error messages for common issues:

- **Authentication errors**: Check your Personal Access Token
- **Permission errors**: Verify your account has the required permissions
- **Network errors**: Check your internet connection
- **Invalid parameters**: Review the command syntax and options

Use the `--verbose` flag to see detailed error information:

```bash
dotnet run -- people list --verbose
```

## Configuration File

The CLI stores configuration in your user profile directory:
- **Windows**: `%USERPROFILE%\.planningcenter\config.json`
- **macOS/Linux**: `~/.planningcenter/config.json`

This file contains:
- Encrypted Personal Access Token
- Default output format
- Default page size
- Logging preferences

## Security

- Personal Access Tokens are stored securely using system encryption
- Tokens are never logged or displayed in plain text
- Use the `config get-token` command to view a masked version of your stored token

## Troubleshooting

### Common Issues

1. **"No Personal Access Token provided"**
   - Run `config set-token` to store your token
   - Or use the `--token` parameter

2. **"Authentication failed"**
   - Verify your token with `config test-token`
   - Check that your token has the correct format: `app_id:secret`

3. **"Resource not found"**
   - Verify the ID exists and you have permission to access it
   - Check for typos in the ID parameter

4. **"Rate limit exceeded"**
   - Wait a moment and try again
   - Consider reducing the page size for large requests

### Getting Help

Use the `--help` option with any command to see detailed usage information:

```bash
dotnet run -- --help
dotnet run -- people --help
dotnet run -- people list --help
```

## Development

This CLI is built using:
- **.NET 8**: Modern C# runtime
- **System.CommandLine**: Command-line parsing
- **Microsoft.Extensions**: Dependency injection and configuration
- **Planning Center API Client**: Official API client library

### Building from Source

```bash
# Clone the repository
git clone <repository-url>

# Navigate to the CLI project
cd examples/PlanningCenter.Api.Client.CLI

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run tests (if available)
dotnet test
```

## License

This project is licensed under the same terms as the Planning Center API Client library.