# Planning Center API SDK for .NET - Project Overview

## Project Purpose
The Planning Center API SDK for .NET is a comprehensive, production-ready .NET SDK for the Planning Center API. It provides a clean, intuitive interface for interacting with all 9 Planning Center services (People, Giving, Calendar, Check-Ins, Groups, Registrations, Services, Publishing, Webhooks).

## Key Features
- **Complete API Coverage**: Full implementation of all 9 Planning Center modules
- **ServiceBase Architecture**: Unified service pattern with correlation ID management and performance monitoring
- **Multiple Authentication Methods**: Personal Access Tokens (PAT), OAuth 2.0, and Access Tokens
- **Automatic Pagination**: Built-in pagination helpers eliminate manual pagination logic
- **Memory-Efficient Streaming**: Stream large datasets without loading everything into memory
- **Fluent API**: LINQ-like interface for intuitive query building across all modules
- **Comprehensive Error Handling**: Detailed exceptions with proper error context and correlation tracking
- **Built-in Caching**: Configurable response caching for improved performance
- **Dependency Injection**: Full support for .NET dependency injection
- **CLI Tool**: Complete command-line interface for all 9 modules with advanced features

## Project Structure
- **src/PlanningCenter.Api.Client**: Core SDK implementation with services and authentication
- **src/PlanningCenter.Api.Client.Models**: Domain models, DTOs, and request/response models
- **src/PlanningCenter.Api.Client.Abstractions**: Interfaces and contracts
- **src/PlanningCenter.Api.Client.Tests**: Unit tests using xUnit, Moq, and FluentAssertions
- **src/PlanningCenter.Api.Client.IntegrationTests**: Integration tests for API endpoints
- **examples/**: Working examples for different scenarios including CLI tool

## Current Status
This is a production-ready SDK with complete implementation across all 9 Planning Center modules. The solution builds successfully with zero compilation errors and comprehensive test coverage.