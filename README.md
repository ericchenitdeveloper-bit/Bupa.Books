# Bupa.Books
Bupa coding test - service to fetch and group books by owner age category with sorting and filtering support

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