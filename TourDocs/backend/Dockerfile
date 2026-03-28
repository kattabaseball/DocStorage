# =============================================================================
# TourDocs API - Multi-Stage Dockerfile
# .NET Core 8 Web API
# =============================================================================

# -----------------------------------------------------------------------------
# Stage 1: Restore dependencies
# -----------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS restore

WORKDIR /src

# Copy solution and project files first for layer caching
COPY TourDocs.sln ./
COPY src/TourDocs.API/TourDocs.API.csproj src/TourDocs.API/
COPY src/TourDocs.Core/TourDocs.Core.csproj src/TourDocs.Core/
COPY src/TourDocs.Data/TourDocs.Data.csproj src/TourDocs.Data/
COPY src/TourDocs.Domain/TourDocs.Domain.csproj src/TourDocs.Domain/
COPY src/TourDocs.Infrastructure/TourDocs.Infrastructure.csproj src/TourDocs.Infrastructure/

# Restore NuGet packages
RUN dotnet restore TourDocs.sln

# -----------------------------------------------------------------------------
# Stage 2: Build and publish
# -----------------------------------------------------------------------------
FROM restore AS build

ARG BUILD_CONFIGURATION=Release

# Copy all source code
COPY . .

# Build the solution
RUN dotnet build TourDocs.sln \
    -c $BUILD_CONFIGURATION \
    --no-restore

# Run tests (optional - uncomment to include in build)
# COPY tests/ tests/
# RUN dotnet test TourDocs.sln \
#     -c $BUILD_CONFIGURATION \
#     --no-build \
#     --verbosity normal

# Publish the API project
RUN dotnet publish src/TourDocs.API/TourDocs.API.csproj \
    -c $BUILD_CONFIGURATION \
    --no-build \
    -o /app/publish \
    /p:UseAppHost=false

# -----------------------------------------------------------------------------
# Stage 3: Runtime
# -----------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Install curl for health checks
RUN apt-get update && apt-get install -y --no-install-recommends curl && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Create non-root user for security
RUN groupadd -r tourdocs && useradd -r -g tourdocs -s /bin/false tourdocs

# Create storage and logs directories
RUN mkdir -p /app/storage /app/logs && \
    chown -R tourdocs:tourdocs /app/storage /app/logs

# Copy published output from build stage
COPY --from=build /app/publish .

# Set ownership of application files
RUN chown -R tourdocs:tourdocs /app

# Switch to non-root user
USER tourdocs

# Expose port
EXPOSE 7001

# Set environment variables
ENV ASPNETCORE_URLS=http://+:7001
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_EnableDiagnostics=0

# Health check
HEALTHCHECK --interval=30s --timeout=10s --retries=3 --start-period=60s \
    CMD curl -f http://localhost:7001/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "TourDocs.API.dll"]
