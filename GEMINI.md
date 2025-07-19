# Gemini Code Assistant Documentation

This document provides instructions and guidelines for using the Gemini Code Assistant with the Planning Center API SDK for .NET.

## 1. Project Overview

This repository contains a comprehensive, production-ready .NET SDK for the [Planning Center API](https://developer.planning.center/docs/#/overview/). It provides a clean, intuitive interface for interacting with all of Planning Center's services.

### Key Features:

*   **Complete API Coverage**: Full implementation of all 9 Planning Center modules.
*   **Multiple Authentication Methods**: Personal Access Tokens (PAT), OAuth 2.0, and Access Tokens.
*   **Fluent API**: LINQ-like interface for intuitive query building.
*   **Automatic Pagination**: Built-in pagination helpers for large datasets.
*   **Dependency Injection**: Full support for .NET dependency injection.
*   **Async/Await**: Modern async patterns throughout.
*   **Strongly Typed**: Rich type system with comprehensive models.

## 2. Getting Started

### 2.1. Prerequisites

*   [.NET 9](https://dotnet.microsoft.com/download/dotnet/9.0)

### 2.2. Building the Project

To build the solution, run the following command from the root directory:

```bash
dotnet build src
```

### 2.3. Running Tests

#### Unit Tests

To run the unit tests, use the following command:

```bash
dotnet test src
```

#### Integration Tests

To run the integration tests, you will first need to create an `appsettings.local.json` file in the `src/PlanningCenter.Api.Client.IntegrationTests` directory. You can use the `appsettings.local.template.json` file as a starting point.

**`appsettings.local.json` structure:**

```json
{
  "PlanningCenter": {
    "Authentication": {
      "Type": "PersonalAccessToken",
      "ApplicationId": "YOUR_APPLICATION_ID_HERE",
      "Secret": "YOUR_SECRET_HERE"
    }
  }
}
```

Once you have created and configured the `appsettings.local.json` file, you can run the integration tests using the same command as the unit tests:

```bash
dotnet test src
```

## 3. Development Guidelines

### 3.1. Coding Style

This project follows standard .NET coding conventions and emphasizes the following principles:

*   **DRY (Don't Repeat Yourself)**: Avoid code duplication by abstracting common functionality into reusable components.
*   **SOLID**: Adhere to the five SOLID principles of object-oriented design.

### 3.2. Project Structure

The solution is organized into the following projects:

*   `PlanningCenter.Api.Client`: The main client library.
*   `PlanningCenter.Api.Client.Abstractions`: Contains the interfaces and abstractions used by the client.
*   `PlanningCenter.Api.Client.Models`: Contains the data models for the Planning Center API.
*   `PlanningCenter.Api.Client.Tests`: Unit tests for the client library.
*   `PlanningCenter.Api.Client.IntegrationTests`: Integration tests for the client library.
*   `examples/`: Contains example console applications demonstrating how to use the SDK.

## 4. Workflow Commands

### 4.1. Common Tasks

*   **Build the solution:** `dotnet build src`
*   **Run all tests:** `dotnet test src`
*   **Run code coverage:** `.\scripts\run-code-coverage.ps1`
