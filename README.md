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

## Running Locally

### Prerequisites
- .NET 8.0 SDK or later
- Visual Studio 2022 or later (or Visual Studio Code)

### Installation
1. Clone the repository
2. Open `Bupa.Books.sln` in Visual Studio
3. Restore NuGet packages: `dotnet restore`

### Running the APIs

#### 1. Start the Private API
```bash
cd src/PrivateApi/Bupa.Books.PrivateApi
dotnet run
```
The Private API will start on `http://localhost:5001`

#### 2. Start the Public API
```bash
cd src/PublicApi/Bupa.Books.PublicApi
dotnet run
```
The Public API will start on `http://localhost:5000`

#### 3. Get JWT Token
Use the demo credentials to obtain a JWT token from the Public API:

```bash
curl -X POST http://localhost:5000/api/v1/auth/token \
  -H "Content-Type: application/json" \
  -d '{"username": "demo", "password": "demo123"}'
```

Response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "tokenType": "Bearer",
  "expiresIn": 3600
}
```

#### 4. Call the Books Endpoint
Use the JWT token to call the protected books endpoint:

```bash
curl -X GET "http://localhost:5000/api/v1/books" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE"
```

Optional: Filter for hardcover books only:
```bash
curl -X GET "http://localhost:5000/api/v1/books?hardcoverOnly=true" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE"
```

### API Documentation
Both APIs include Swagger UI for interactive documentation:
- Public API: `http://localhost:5000/swagger`
- Private API: `http://localhost:5001/swagger`

### Security Note
⚠️ **For Development Only**: This setup uses demo credentials (`demo`/`demo123`) and JWT secrets stored in configuration files for testing purposes. In a real production application, these sensitive values should be stored securely using AWS services such as:
- **AWS Secrets Manager** for JWT secrets and API keys
- **AWS Systems Manager Parameter Store** for configuration parameters
- **AWS Key Management Service (KMS)** for encryption keys

Never commit sensitive credentials to version control or store them in application configuration files in production environments.

## CI/CD & Docker

> **Demonstration only** — the GitHub Actions workflows (`.github/workflows/`) and Dockerfiles (`Dockerfile.PrivateApi`, `Dockerfile.PublicApi`) are included to show how the application could be containerised and deployed. They are not wired to any real infrastructure and are not intended to be run as-is.

# Assumptions
- A book is treated as hardcover when its `Type` is `Hardcover` (case-insensitive).
- Owners with null or empty book collections are treated as having no books and do not contribute any book entries to the output.
- Only categories that contain at least one matching book are included in the response.
- The upstream API response is treated as the source of truth for this exercise, and no local persistence is introduced.
- If the upstream API call fails or times out, the application returns an error response rather than partial results.
