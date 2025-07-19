# Planning Center .NET SDK

This document provides information about the Planning Center .NET SDK project.

## Project Setup

To build and test this project, you will need the .NET 9 SDK. You can install it using the following commands:

```bash
curl -L https://dot.net/v1/dotnet-install.sh -o dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh -c 9.0
export PATH="$HOME/.dotnet:$PATH"
```

## Building the Project

To build the solution, run the following command:

```bash
dotnet build src/PlanningCenter.Api.sln
```

## Running Tests

To run the unit tests, run the following command:

```bash
dotnet test src/PlanningCenter.Api.sln
```
