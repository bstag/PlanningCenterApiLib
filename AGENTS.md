# Repository Guidelines

## Project Structure & Modules
- `src/`: Solution and library code (e.g., `PlanningCenter.Api.sln`, `PlanningCenter.Api.*`).
- `tests/`: Unit/integration test projects ending with `.Tests`.
- `samples/` (optional): Minimal apps demonstrating SDK usage.
- `docs/` (optional): Additional documentation and assets if present.

## Build, Test, and Development Commands
- Restore: `dotnet restore src/PlanningCenter.Api.sln` — restore NuGet packages.
- Build: `dotnet build src/PlanningCenter.Api.sln` — compile all projects.
- Test: `dotnet test src/PlanningCenter.Api.sln` — run all tests.
- Format (optional): `dotnet format` — apply standard .NET formatting.
- Pack (if needed): `dotnet pack src/PlanningCenter.Api.sln -c Release` — create NuGet packages.

Prerequisite: .NET 9 SDK. Ensure `dotnet --version` reports a 9.x runtime.

## Coding Style & Naming Conventions
- Indentation: 4 spaces; file-scoped namespaces; prefer `readonly`/`sealed` where appropriate.
- Naming: PascalCase for types/namespaces; camelCase for locals/params; private fields `_camelCase`.
- Async: Suffix async methods with `Async` and use `CancellationToken` when practical.
- Nullability: Keep nullable reference types enabled and annotate intent (`?`, `!`).
- Public API: Provide XML docs for externally consumed types and members.

## Testing Guidelines
- Location: Place tests in `tests/ProjectName.Tests` mirroring source namespaces.
- Naming: `MethodName_ShouldExpectedBehavior_WhenCondition` for test methods.
- Scope: Favor fast, deterministic unit tests; add integration tests only where valuable.
- Coverage: Aim for meaningful coverage on changed code; include tests with feature/fix PRs.
- Running: `dotnet test src/PlanningCenter.Api.sln` (add `-v n` for normal verbosity during debugging).

## Commit & Pull Request Guidelines
- Commits: Use imperative mood; prefer Conventional Commits (`feat:`, `fix:`, `docs:`, `refactor:`, `test:`). Keep changes focused.
- PRs: Provide a clear description, motivation, and any breaking-change notes. Link related issues, attach screenshots for user-visible changes, and update docs/samples.
- CI: Ensure the solution builds and tests pass locally before opening or updating a PR.

## Configuration & Secrets
- Do not commit secrets. Use environment variables or `dotnet user-secrets` for local development.
  - Example: `dotnet user-secrets set PLANNINGCENTER_CLIENT_ID "..."`
- Prefer `appsettings.Development.json` for local config (git-ignored if present). Document required keys in README or samples.

