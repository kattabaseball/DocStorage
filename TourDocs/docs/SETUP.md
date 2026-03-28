# TourDocs - Development Environment Setup

## Prerequisites

### Required Software

| Software              | Version  | Download                                              |
|----------------------|----------|-------------------------------------------------------|
| .NET SDK             | 8.0+     | https://dotnet.microsoft.com/download/dotnet/8.0      |
| Node.js              | 20 LTS   | https://nodejs.org/                                   |
| npm                  | 10+      | Included with Node.js                                 |
| Angular CLI          | 18+      | `npm install -g @angular/cli`                         |
| SQL Server           | 2022     | https://www.microsoft.com/sql-server (or Docker)      |
| Git                  | 2.40+    | https://git-scm.com/                                  |

### Optional but Recommended

| Software             | Purpose                    | Download                                    |
|---------------------|----------------------------|---------------------------------------------|
| Docker Desktop      | Containerized development  | https://www.docker.com/products/docker-desktop |
| Visual Studio 2022  | .NET IDE                   | https://visualstudio.microsoft.com/         |
| VS Code             | Frontend IDE               | https://code.visualstudio.com/              |
| Azure Data Studio   | SQL Server management      | https://aka.ms/azuredatastudio              |
| Postman             | API testing                | https://www.postman.com/                    |

### VS Code Extensions (Recommended)

- Angular Language Service
- C# Dev Kit
- ESLint
- Prettier
- SCSS IntelliSense
- EditorConfig for VS Code
- GitLens
- Thunder Client (API testing)

## Quick Start with Docker

The fastest way to get the full stack running:

```bash
# Clone the repository
git clone https://github.com/your-org/tourdocs.git
cd tourdocs

# Start all services
docker-compose up -d

# Verify services are running
docker-compose ps
```

Services will be available at:
- **Web App:** http://localhost:4200
- **API:** http://localhost:7001
- **Swagger:** http://localhost:7001/swagger
- **Seq (Logs):** http://localhost:5341
- **Hangfire Dashboard:** http://localhost:7001/hangfire

Default admin credentials (seeded):
- **Email:** admin@tourdocs.local
- **Password:** Admin@123456

## Manual Setup (Step by Step)

### Step 1: Clone the Repository

```bash
git clone https://github.com/your-org/tourdocs.git
cd tourdocs
```

### Step 2: Set Up SQL Server

**Option A: Docker (Recommended)**
```bash
docker run -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=TourDocs_Dev_P@ss2024!" \
  -p 1433:1433 \
  --name tourdocs-sqlserver \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

**Option B: Local SQL Server**
- Install SQL Server 2022 Developer Edition
- Enable TCP/IP in SQL Server Configuration Manager
- Ensure the instance is accessible on port 1433

### Step 3: Set Up Backend

```bash
cd backend/src/TourDocs.API

# Configure user secrets (never commit these)
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=TourDocs;User Id=sa;Password=TourDocs_Dev_P@ss2024!;TrustServerCertificate=True;MultipleActiveResultSets=True"
dotnet user-secrets set "Jwt:Secret" "YourDevelopmentSecretKeyThatIsAtLeast256BitsLong!"
dotnet user-secrets set "Jwt:Issuer" "TourDocs"
dotnet user-secrets set "Jwt:Audience" "TourDocs"

# Restore NuGet packages
dotnet restore

# Apply database migrations (creates the database and schema)
dotnet ef database update --project ../TourDocs.Data

# Seed initial data (runs automatically on first startup)
dotnet run
```

The API will start on `https://localhost:7001`. Swagger is available at `https://localhost:7001/swagger`.

### Step 4: Set Up Frontend

```bash
cd frontend/tourdocs-web

# Install dependencies
npm ci

# Start the development server
ng serve
```

The Angular app will start on `http://localhost:4200` and proxy API calls to `https://localhost:7001`.

### Step 5: Verify the Setup

1. Open `http://localhost:4200` in your browser
2. Log in with the default admin credentials:
   - Email: `admin@tourdocs.local`
   - Password: `Admin@123456`
3. Verify the dashboard loads with sample data
4. Check the API swagger at `https://localhost:7001/swagger`

## Database Migrations

### Creating a New Migration

```bash
cd backend/src/TourDocs.API

# Create migration
dotnet ef migrations add <MigrationName> --project ../TourDocs.Data --context ApplicationDbContext

# Apply migration
dotnet ef database update --project ../TourDocs.Data
```

### Rolling Back a Migration

```bash
# Revert to a specific migration
dotnet ef database update <PreviousMigrationName> --project ../TourDocs.Data

# Remove the last unapplied migration
dotnet ef migrations remove --project ../TourDocs.Data
```

### Resetting the Database

```bash
# Drop and recreate the database
dotnet ef database drop --project ../TourDocs.Data --force
dotnet ef database update --project ../TourDocs.Data
```

## Environment Variables

### Backend

Create a `.env` file in `backend/` (or use `dotnet user-secrets`):

| Variable                             | Description                  | Example                                                          |
|--------------------------------------|------------------------------|------------------------------------------------------------------|
| `ConnectionStrings__DefaultConnection` | SQL Server connection string | `Server=localhost;Database=TourDocs;User Id=sa;Password=...`    |
| `Jwt__Secret`                        | JWT signing key (256+ bits)  | `YourSuperSecretKeyThatIsAtLeast256BitsLong!`                   |
| `Jwt__Issuer`                        | JWT issuer                   | `TourDocs`                                                      |
| `Jwt__Audience`                      | JWT audience                 | `TourDocs`                                                      |
| `FileStorage__BasePath`              | File upload root directory   | `./Storage`                                                     |
| `Redis__ConnectionString`            | Redis connection string      | `localhost:6379`                                                |
| `Email__SendGridApiKey`              | SendGrid API key             | `SG.xxxxx`                                                     |
| `Seq__ServerUrl`                     | Seq logging server URL       | `http://localhost:5341`                                         |

### Frontend

Environment configuration is in `frontend/tourdocs-web/src/environments/`:

```typescript
// environment.ts (development)
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7001',
  signalRUrl: 'https://localhost:7001/hubs'
};

// environment.prod.ts (production)
export const environment = {
  production: true,
  apiUrl: 'https://api.tourdocs.io',
  signalRUrl: 'https://api.tourdocs.io/hubs'
};
```

## Running Tests

### Backend Tests

```bash
cd backend

# Run all tests
dotnet test TourDocs.sln

# Run unit tests only
dotnet test tests/TourDocs.UnitTests/TourDocs.UnitTests.csproj

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test class
dotnet test --filter "FullyQualifiedName~MemberServiceTests"
```

### Frontend Tests

```bash
cd frontend/tourdocs-web

# Run unit tests (watch mode)
ng test

# Run unit tests (single run, CI mode)
ng test --watch=false --browsers=ChromeHeadless

# Run with coverage
ng test --code-coverage

# Run E2E tests
npx cypress open    # Interactive mode
npx cypress run     # Headless mode
```

## Troubleshooting

### SQL Server Connection Refused

- Verify SQL Server is running: `docker ps` or check SQL Server service
- Check the port is accessible: `telnet localhost 1433`
- Ensure `TrustServerCertificate=True` is in the connection string
- If using Docker, ensure the container is healthy: `docker logs tourdocs-sqlserver`

### EF Core Migration Errors

- Ensure you are running commands from the API project directory
- Check that `--project ../TourDocs.Data` points to the correct project
- If the database is out of sync, try dropping and recreating: `dotnet ef database drop --force`

### Angular Build Errors

- Delete `node_modules` and `package-lock.json`, then run `npm install`
- Clear the Angular cache: `ng cache clean`
- Ensure you are using Node.js 20 LTS: `node --version`

### CORS Errors

- Verify the backend `Program.cs` includes the Angular dev server URL in CORS policy
- Check that the frontend `environment.ts` `apiUrl` matches the backend URL

### JWT Token Issues

- Ensure the JWT secret is at least 256 bits (32+ characters)
- Check that `Jwt:Issuer` and `Jwt:Audience` match between token generation and validation
- Clear `localStorage` in the browser and log in again

### Port Conflicts

If default ports are in use:

```bash
# Backend: change port in launchSettings.json or use:
dotnet run --urls "https://localhost:7002"

# Frontend: use a different port:
ng serve --port 4201

# Docker: change port mappings in docker-compose.yml
```

### Docker Build Failures

- Ensure Docker Desktop is running
- Check available disk space: `docker system df`
- Clean up unused resources: `docker system prune -a`
- Rebuild without cache: `docker-compose build --no-cache`
