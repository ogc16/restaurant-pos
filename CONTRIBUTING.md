# Contributing to RestaurantPOS

Thanks for your interest in improving RestaurantPOS! This document explains how to
propose changes, the standards we follow, and the workflow for getting your work merged.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Ways to Contribute](#ways-to-contribute)
- [Getting Started](#getting-started)
- [Project Architecture](#project-architecture)
- [Development Workflow](#development-workflow)
- [Coding Standards](#coding-standards)
- [Testing](#testing)
- [Commit Messages](#commit-messages)
- [Pull Requests](#pull-requests)

## Code of Conduct

This project adheres to a [Code of Conduct](CODE_OF_CONDUCT.md). By participating,
you are expected to uphold this code. Please report unacceptable behavior to the
maintainers.

## Ways to Contribute

- Reporting bugs and requesting features via issues
- Improving documentation
- Fixing bugs and adding tests
- Implementing new features
- Reviewing pull requests

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- A code editor such as Visual Studio 2022+, JetBrains Rider, or VS Code
- Git

### Setup

```bash
# Fork and clone the repository
git clone https://github.com/<your-username>/restaurant-pos.git
cd restaurant-pos

# Restore dependencies
dotnet restore RestaurantPOS.slnx

# Build the solution
dotnet build RestaurantPOS.slnx

# Run the tests
dotnet test RestaurantPOS.slnx
```

The API uses SQLite and creates its database automatically on first startup. No
external database server is required.

### Running Locally

```bash
# Terminal 1 - API (port 7001)
dotnet run --project src/RestaurantPOS.Api

# Terminal 2 - Blazor Web UI (port 5001)
dotnet run --project src/RestaurantPOS.Web
```

## Project Architecture

RestaurantPOS follows Clean Architecture with a strict dependency direction:
`Api` / `Web` -> `Application` -> `Core`, with `Infrastructure` implementing the
interfaces declared in `Core`.

| Project | Responsibility |
|---------|----------------|
| `RestaurantPOS.Core` | Domain entities, enums, DTOs, interfaces, common types |
| `RestaurantPOS.Application` | Business logic and FluentValidation validators |
| `RestaurantPOS.Infrastructure` | EF Core `AppDbContext`, repositories, Identity, seed data |
| `RestaurantPOS.Api` | REST controllers, JWT configuration, global error middleware |
| `RestaurantPOS.Web` | Blazor Server frontend and API client |

See the [README](README.md#architecture) for UML diagrams of the system.

**Keep the dependency rule intact.** `Core` must never reference `Infrastructure`
or `Application`. Business logic belongs in `Application` services, not controllers
or Blazor components.

## Development Workflow

1. **Find or open an issue.** Discuss non-trivial changes before writing code.
2. **Fork** the repository and create a branch off `master`:
   ```bash
   git checkout -b feat/short-description
   ```
   Use a prefix such as `feat/`, `fix/`, `docs/`, `test/`, or `refactor/`.
3. **Write your change** with tests where applicable.
4. **Run the full build and test suite** locally before pushing.
5. **Push** your branch and open a pull request against `master`.

## Coding Standards

- Follow the existing style and conventions in the codebase.
- Use `async`/`await` for all I/O and suffix methods with `Async`.
- Prefer constructor injection over service location.
- Return `Result<T>` / DTOs from the service layer; do not leak EF Core entities
  out of controllers.
- Validate incoming requests with FluentValidation validators in
  `RestaurantPOS.Application/Validation`.
- Do not add comments that merely restate the code. Explain *why*, not *what*.
- Keep methods and classes focused; extract shared helpers rather than duplicating.
- Never commit secrets, connection strings, or credentials.

### Naming

- PascalCase for types, methods, and public members.
- camelCase for locals and parameters.
- Interfaces are prefixed with `I` (e.g., `IOrderService`).

## Testing

We use xUnit, FluentAssertions, and `WebApplicationFactory` with the EF Core
InMemory provider.

```bash
# All tests
dotnet test RestaurantPOS.slnx

# Unit tests only
dotnet test tests/RestaurantPOS.Application.Tests/

# API integration tests only
dotnet test tests/RestaurantPOS.Api.Tests/
```

Guidelines:

- Every bug fix should include a regression test.
- New service methods should have unit tests covering the happy path and key
  failure cases.
- New or changed endpoints should have integration tests.
- Keep tests fast and deterministic; avoid relying on wall-clock time or ordering.

## Commit Messages

Write clear, imperative commit messages:

```
<type>: <short summary>

<body explaining what and why, if needed>
```

Common types: `feat`, `fix`, `docs`, `test`, `refactor`, `chore`, `perf`.

Examples:

```
fix: prevent render batch conflict during login transition
feat: add loyalty point redemption to customer service
docs: document report CSV export columns
```

## Pull Requests

A good pull request:

- Is scoped to a single concern and stays small enough to review.
- References the related issue (e.g., `Closes #42`).
- Includes a summary of the change and how it was tested.
- Keeps the build green and all tests passing.
- Updates documentation when behavior or public APIs change.

All pull requests require review before merging. Please be responsive to review
feedback and rebase on `master` if your branch falls behind.

## License

By contributing, you agree that your contributions are licensed under the
[GNU General Public License v3.0](LICENSE).
