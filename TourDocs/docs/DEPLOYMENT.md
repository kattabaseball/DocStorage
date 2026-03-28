# TourDocs - Deployment Guide

## Deployment Options

TourDocs supports multiple deployment strategies:

1. **Docker Compose** — Single server deployment (small to medium scale)
2. **Kubernetes** — Container orchestration (medium to large scale)
3. **Azure App Service** — PaaS deployment (managed infrastructure)
4. **Manual IIS + Kestrel** — Traditional Windows Server deployment

This guide covers Docker Compose as the primary deployment method.

---

## Docker Compose Deployment

### Prerequisites

- Linux server (Ubuntu 22.04 LTS recommended) or Windows Server 2022
- Docker Engine 24+ and Docker Compose V2
- Minimum 4 GB RAM, 2 CPU cores, 50 GB disk
- Domain name with DNS configured
- SSL certificate (Let's Encrypt recommended)

### Step 1: Prepare the Server

```bash
# Update system
sudo apt update && sudo apt upgrade -y

# Install Docker
curl -fsSL https://get.docker.com | sh
sudo usermod -aG docker $USER

# Install Docker Compose plugin
sudo apt install docker-compose-plugin

# Verify installation
docker --version
docker compose version
```

### Step 2: Clone and Configure

```bash
# Clone the repository
git clone https://github.com/your-org/tourdocs.git /opt/tourdocs
cd /opt/tourdocs

# Create production environment file
cp backend/.env.example .env

# Edit environment variables
nano .env
```

### Step 3: Configure Environment Variables

Edit the `.env` file with production values:

```bash
# Database
SA_PASSWORD=<strong-password-here>

# JWT (generate with: openssl rand -base64 64)
JWT_SECRET=<256-bit-secret-key>

# Email
SENDGRID_API_KEY=SG.xxxxxxxxxxxxxxxxxxxx

# Seq (optional)
SEQ_ADMIN_PASSWORD_HASH=<bcrypt-hash>

# Application
ASPNETCORE_ENVIRONMENT=Production
```

### Step 4: Build and Start

```bash
# Build images
docker compose build

# Start services in detached mode
docker compose up -d

# Verify all services are running
docker compose ps

# Check logs
docker compose logs -f tourdocs-api
```

### Step 5: Apply Database Migrations

```bash
# Run migrations inside the API container
docker compose exec tourdocs-api dotnet ef database update \
  --project /src/TourDocs.Data \
  --startup-project /src/TourDocs.API

# Or apply a pre-generated SQL script
docker compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "$SA_PASSWORD" -d TourDocs -i /migrations/migrations.sql -C
```

### Step 6: Verify Deployment

```bash
# Health check
curl http://localhost:7001/health

# Readiness check
curl http://localhost:7001/health/ready

# Check web app
curl http://localhost:4200
```

---

## Reverse Proxy Configuration (nginx)

For production, place an nginx reverse proxy in front of the application for SSL termination and routing.

### Install nginx

```bash
sudo apt install nginx certbot python3-certbot-nginx
```

### Configure nginx

```nginx
# /etc/nginx/sites-available/tourdocs
server {
    listen 80;
    server_name tourdocs.yourdomain.com;
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    server_name tourdocs.yourdomain.com;

    ssl_certificate /etc/letsencrypt/live/tourdocs.yourdomain.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/tourdocs.yourdomain.com/privkey.pem;

    # SSL configuration
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers HIGH:!aNULL:!MD5;
    ssl_prefer_server_ciphers on;

    # Security headers
    add_header Strict-Transport-Security "max-age=31536000; includeSubDomains" always;
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;

    # Frontend
    location / {
        proxy_pass http://localhost:4200;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }

    # API
    location /api/ {
        proxy_pass http://localhost:7001;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        client_max_body_size 50M;
    }

    # SignalR WebSocket
    location /hubs/ {
        proxy_pass http://localhost:7001;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection "upgrade";
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_read_timeout 3600;
    }

    # Swagger (restrict in production)
    location /swagger {
        # Allow only from internal network
        allow 10.0.0.0/8;
        allow 172.16.0.0/12;
        allow 192.168.0.0/16;
        deny all;
        proxy_pass http://localhost:7001;
    }

    # Hangfire Dashboard (restrict in production)
    location /hangfire {
        allow 10.0.0.0/8;
        allow 172.16.0.0/12;
        allow 192.168.0.0/16;
        deny all;
        proxy_pass http://localhost:7001;
    }
}
```

### Obtain SSL Certificate

```bash
sudo certbot --nginx -d tourdocs.yourdomain.com
```

---

## Environment Configuration

### Production Environment Variables

| Variable                               | Required | Description                                |
|----------------------------------------|----------|--------------------------------------------|
| `ConnectionStrings__DefaultConnection` | Yes      | SQL Server connection string               |
| `Jwt__Secret`                          | Yes      | JWT signing key (256+ bits)                |
| `Jwt__Issuer`                          | Yes      | JWT issuer (your domain)                   |
| `Jwt__Audience`                        | Yes      | JWT audience (your domain)                 |
| `FileStorage__BasePath`               | Yes      | Absolute path for file storage             |
| `Email__SendGridApiKey`               | Yes      | SendGrid API key                           |
| `Email__FromAddress`                  | Yes      | Sender email address                       |
| `Email__FromName`                     | Yes      | Sender display name                        |
| `Redis__ConnectionString`             | No       | Redis connection (improves performance)    |
| `Seq__ServerUrl`                      | No       | Seq URL for structured logging             |
| `Cors__AllowedOrigins`               | Yes      | Comma-separated allowed origins            |
| `Hangfire__DashboardEnabled`          | No       | Enable Hangfire dashboard (default: false) |

### Configuration Priority (highest to lowest)

1. Environment variables
2. `appsettings.Production.json`
3. `appsettings.json`

---

## Database Migration in Production

### Strategy: Migration Scripts

For production deployments, always use pre-generated SQL scripts rather than running `dotnet ef database update` directly.

```bash
# Generate idempotent migration script
cd backend/src/TourDocs.API
dotnet ef migrations script \
  --project ../TourDocs.Data \
  --idempotent \
  --output migrations.sql

# Review the script before applying
cat migrations.sql

# Apply via sqlcmd
sqlcmd -S production-server -U deploy_user -P password -d TourDocs -i migrations.sql
```

### Pre-Deployment Checklist for Migrations

1. Generate the migration script from the exact commit being deployed
2. Review the script for destructive operations (DROP TABLE, DROP COLUMN)
3. Test the script against a copy of production data
4. Back up the production database before applying
5. Apply during a maintenance window for destructive migrations
6. Verify the application starts correctly after migration

---

## File Storage Migration to Cloud

### Step 1: Implement Cloud Storage Service

Create a new `IFileStorageService` implementation (e.g., `AzureBlobStorageService`):

```csharp
public class AzureBlobStorageService : IFileStorageService
{
    private readonly BlobServiceClient _blobClient;
    // ... implementation
}
```

### Step 2: Migrate Existing Files

```bash
# Use AzCopy or a custom migration script
azcopy copy "Storage/*" "https://tourdocs.blob.core.windows.net/documents/*" --recursive
```

### Step 3: Update DI Registration

```csharp
// Change in ServiceCollectionExtensions.cs
services.AddScoped<IFileStorageService, AzureBlobStorageService>();
```

### Step 4: Deploy and Verify

1. Deploy the updated application
2. Verify new uploads go to cloud storage
3. Verify existing file downloads still work
4. Monitor for errors in Seq logs
5. Decommission local storage after verification period

---

## Health Check Endpoints

### Basic Health Check

```
GET /health
```
Returns 200 OK if the API process is running. Used by load balancers and container orchestrators for liveness probes.

### Readiness Check

```
GET /health/ready
```
Returns 200 OK if the API can serve requests (database connected, Redis available). Used for readiness probes.

### Response Format

```json
{
  "status": "Healthy",
  "checks": {
    "database": "Healthy",
    "redis": "Healthy",
    "fileStorage": "Healthy"
  },
  "duration": "00:00:00.1234567"
}
```

---

## Monitoring and Logging

### Structured Logging with Seq

All application logs are sent to Seq in structured format.

**Accessing Seq:**
- Development: `http://localhost:5341`
- Production: Configure Seq behind a reverse proxy with authentication

**Common Log Queries:**
```
# Find errors in the last hour
@Level = "Error" and @Timestamp > Now() - 1h

# Track a specific request
CorrelationId = "abc-123-def"

# Find slow requests
Elapsed > 1000

# Find failed logins
@MessageTemplate like "%login failed%"
```

### Application Metrics

Key metrics to monitor:
- **Request rate** — Requests per second
- **Error rate** — 5xx responses per minute
- **Response time** — P50, P95, P99 latency
- **Database connections** — Active connection count
- **Background jobs** — Hangfire job success/failure rate
- **File storage** — Disk usage percentage

### Alerting Recommendations

| Condition                      | Severity | Action                              |
|-------------------------------|----------|-------------------------------------|
| Error rate > 1% for 5 min    | Warning  | Investigate error logs              |
| Error rate > 5% for 5 min    | Critical | Immediate investigation required    |
| Response time P95 > 2s       | Warning  | Check database and slow queries     |
| Disk usage > 80%             | Warning  | Plan storage expansion or cleanup   |
| Disk usage > 95%             | Critical | Immediate cleanup or expansion      |
| Database connections > 80%   | Warning  | Check for connection leaks          |
| Hangfire failed jobs > 10    | Warning  | Review failed job details           |

---

## Backup Strategy

### Database Backups

```bash
# Full backup (daily)
docker compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "$SA_PASSWORD" -C \
  -Q "BACKUP DATABASE TourDocs TO DISK = '/var/opt/mssql/backup/TourDocs_$(date +%Y%m%d).bak'"

# Transaction log backup (hourly)
docker compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "$SA_PASSWORD" -C \
  -Q "BACKUP LOG TourDocs TO DISK = '/var/opt/mssql/backup/TourDocs_Log_$(date +%Y%m%d_%H%M).trn'"
```

### File Storage Backups

```bash
# Sync to backup location (daily)
rsync -avz /opt/tourdocs/Storage/ /backup/tourdocs-files/

# Or use cloud backup
rclone sync /opt/tourdocs/Storage/ remote:tourdocs-backup/files/
```

### Backup Retention

| Backup Type       | Frequency | Retention |
|------------------|-----------|-----------|
| Full DB backup   | Daily     | 30 days   |
| Log backup       | Hourly    | 7 days    |
| File storage     | Daily     | 30 days   |

---

## Rollback Procedures

### Application Rollback

```bash
# Roll back to previous version
docker compose pull  # If using tagged images
docker compose up -d --force-recreate

# Or specify exact image tags
docker compose up -d tourdocs-api --force-recreate
```

### Database Rollback

```bash
# Restore from backup
docker compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "$SA_PASSWORD" -C \
  -Q "RESTORE DATABASE TourDocs FROM DISK = '/var/opt/mssql/backup/TourDocs_YYYYMMDD.bak' WITH REPLACE"
```

### Emergency Procedures

1. **API Down:** Check `docker compose logs tourdocs-api` for errors
2. **Database Down:** Check `docker compose logs sqlserver` and verify disk space
3. **Out of Memory:** Restart services with `docker compose restart`
4. **Security Incident:** Rotate JWT secret, force all users to re-authenticate
