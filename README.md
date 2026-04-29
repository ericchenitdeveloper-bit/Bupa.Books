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

# Assumptions