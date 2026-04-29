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
- A book is treated as hardcover when its `Type` is `Hardcover` (case-insensitive).
- Owners with null or empty book collections are treated as having no books and do not contribute any book entries to the output.
- Only categories that contain at least one matching book are included in the response.
- The upstream API response is treated as the source of truth for this exercise, and no local persistence is introduced.
- If the upstream API call fails or times out, the application returns an error response rather than partial results.
