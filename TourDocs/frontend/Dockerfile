# =============================================================================
# TourDocs Web - Multi-Stage Dockerfile
# Angular 18 SPA with nginx
# =============================================================================

# -----------------------------------------------------------------------------
# Stage 1: Install dependencies
# -----------------------------------------------------------------------------
FROM node:20-alpine AS dependencies

WORKDIR /app

# Copy package files for layer caching
COPY tourdocs-web/package.json tourdocs-web/package-lock.json ./

# Install dependencies with clean install
RUN npm ci --prefer-offline --no-audit

# -----------------------------------------------------------------------------
# Stage 2: Build Angular application
# -----------------------------------------------------------------------------
FROM dependencies AS build

ARG API_URL=http://localhost:7001
ARG BUILD_CONFIGURATION=production

WORKDIR /app

# Copy all source files
COPY tourdocs-web/ .

# Copy installed node_modules from dependencies stage
COPY --from=dependencies /app/node_modules ./node_modules

# Set environment variable for build
ENV API_URL=$API_URL

# Build Angular application for production
RUN npx ng build \
    --configuration $BUILD_CONFIGURATION \
    --output-path dist/tourdocs-web

# -----------------------------------------------------------------------------
# Stage 3: Serve with nginx
# -----------------------------------------------------------------------------
FROM nginx:1.25-alpine AS runtime

# Remove default nginx configuration
RUN rm -rf /etc/nginx/conf.d/default.conf /usr/share/nginx/html/*

# Copy custom nginx configuration
COPY nginx.conf /etc/nginx/nginx.conf

# Copy built Angular application from build stage
COPY --from=build /app/dist/tourdocs-web/browser /usr/share/nginx/html

# Create non-root user
RUN addgroup -g 1001 -S tourdocs && \
    adduser -u 1001 -S tourdocs -G tourdocs && \
    chown -R tourdocs:tourdocs /usr/share/nginx/html && \
    chown -R tourdocs:tourdocs /var/cache/nginx && \
    chown -R tourdocs:tourdocs /var/log/nginx && \
    touch /var/run/nginx.pid && \
    chown -R tourdocs:tourdocs /var/run/nginx.pid

# Switch to non-root user
USER tourdocs

# Expose port 80
EXPOSE 80

# Health check
HEALTHCHECK --interval=30s --timeout=10s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:80 || exit 1

# Start nginx in foreground
CMD ["nginx", "-g", "daemon off;"]
