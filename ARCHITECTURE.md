# Bupa.Books Solution

A Clean Architecture implementation for the Bupa.Books application with two separate Web API services (Public and Private).

## Solution Structure

```
Bupa.Books/
├── src/
│   ├── PublicApi/
│   │   └── Bupa.Books.PublicApi/          # Public-facing API
│   │       ├── Controllers/
│   │       ├── Program.cs
│   │       └── appsettings.json
│   │
│   ├── PrivateApi/
│   │   └── Bupa.Books.PrivateApi/         # Private/Administrative API
│   │       ├── Controllers/
│   │       ├── Program.cs
│   │       └── appsettings.json
│   │
│   └── Shared/
│       ├── Bupa.Books.Domain/             # Domain models and business logic
│       │   └── Entities/
│       │
│       ├── Bupa.Books.Application/        # Application services and handlers
│       │   └── Abstractions/
│       │
│       ├── Bupa.Books.Infrastructure/     # Data access and external services
│       │   └── Persistence/
│       │
│       └── Bupa.Books.Common/             # Shared utilities and constants
│           └── Constants/
│
└── Bupa.Books.sln                         # Visual Studio Solution file
```

## Architecture Layers

### 1. **Domain Layer** (`Bupa.Books.Domain`)
- Contains core business logic and domain entities
- Includes aggregate roots, value objects, and domain events
- Has no dependencies on other layers except Common
- **Location:** `src/Shared/Bupa.Books.Domain/`

### 2. **Application Layer** (`Bupa.Books.Application`)
- Implements use cases and business workflows (CQRS pattern)
- Contains command and query handlers
- Orchestrates between Domain and Infrastructure layers
- **Location:** `src/Shared/Bupa.Books.Application/`
- **Key Package:** MediatR

### 3. **Infrastructure Layer** (`Bupa.Books.Infrastructure`)
- Implements data persistence (Entity Framework Core)
- Handles external service integrations
- Provides repository implementations
- **Location:** `src/Shared/Bupa.Books.Infrastructure/`
- **Key Package:** Entity Framework Core 8.0

### 4. **Common Layer** (`Bupa.Books.Common`)
- Shared utilities, enums, and constants
- Cross-cutting concerns
- Helper functions used across layers
- **Location:** `src/Shared/Bupa.Books.Common/`

### 5. **Presentation Layers**

#### Public API (`Bupa.Books.PublicApi`)
- Customer-facing REST API endpoints
- Located at `src/PublicApi/Bupa.Books.PublicApi/`
- Uses Swagger/OpenAPI for documentation

#### Private API (`Bupa.Books.PrivateApi`)
- Internal/administrative REST API endpoints
- Located at `src/PrivateApi/Bupa.Books.PrivateApi/`
- Uses Swagger/OpenAPI for documentation

## Dependencies

### Public API & Private API
- Microsoft.AspNetCore.OpenApi
- Swashbuckle.AspNetCore (Swagger)
- Bupa.Books.Application
- Bupa.Books.Infrastructure
- Bupa.Books.Common

### Application Layer
- MediatR (for CQRS pattern implementation)
- Bupa.Books.Domain
- Bupa.Books.Common

### Infrastructure Layer
- Entity Framework Core 8.0
- Microsoft.Extensions.DependencyInjection
- Bupa.Books.Application
- Bupa.Books.Domain
- Bupa.Books.Common

### Domain Layer
- Bupa.Books.Common

## Getting Started

### Prerequisites
- .NET 8.0 SDK or later
- Visual Studio 2022 or later (or Visual Studio Code)
- SQL Server (or configure a different database in Infrastructure layer)

### Installation

1. Clone the repository
2. Open `Bupa.Books.sln` in Visual Studio
3. Restore NuGet packages: `dotnet restore`
4. Build the solution: `dotnet build`

### Running the Applications

#### Public API
```bash
cd src/PublicApi/Bupa.Books.PublicApi
dotnet run
```
Swagger UI: `https://localhost:<port>/swagger`

#### Private API
```bash
cd src/PrivateApi/Bupa.Books.PrivateApi
dotnet run
```
Swagger UI: `https://localhost:<port>/swagger`

## Key Design Principles

### Clean Architecture Rules
1. **Dependency Rule:** Source code dependencies only point inward (toward higher-level policies)
2. **Independence:** Each layer is independently deployable
3. **Testability:** Business logic layers are highly testable
4. **Database Agnostic:** Business rules are independent of the presentation framework or database

### CQRS Pattern
- Separates read and write operations
- Implemented via MediatR in the Application layer
- Command handlers for write operations
- Query handlers for read operations

### Dependency Injection
- Projects use Microsoft.Extensions.DependencyInjection
- Configured in Program.cs of each API

## Next Steps

1. **Define Domain Models:** Add entities to `Bupa.Books.Domain/Entities/`
2. **Implement Use Cases:** Add command/query handlers to `Bupa.Books.Application/`
3. **Create Data Access:** Implement repositories in `Bupa.Books.Infrastructure/`
4. **Build API Endpoints:** Add controllers to each API project
5. **Add Authentication/Authorization:** Implement security layers as needed

## Contributing

- Follow SOLID principles
- Maintain separation of concerns
- Write unit tests in dedicated test projects
- Keep domain logic independent of infrastructure details

## License

[Add your license here]

