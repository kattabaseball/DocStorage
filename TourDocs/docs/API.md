# TourDocs API Documentation

## Overview

TourDocs API is a RESTful Web API built with .NET Core 8. All endpoints are versioned under `/api/v1/` and return responses in a consistent `ApiResponse<T>` wrapper format.

## Interactive Documentation

Swagger UI is available in development and staging environments:

- **Development:** `https://localhost:7001/swagger`
- **Staging:** `https://api-staging.tourdocs.io/swagger`

Swagger provides interactive documentation where you can explore endpoints, view request/response schemas, and test API calls directly.

## Base URL

| Environment  | URL                                  |
|-------------|--------------------------------------|
| Development | `https://localhost:7001/api/v1`      |
| Staging     | `https://api-staging.tourdocs.io/api/v1` |
| Production  | `https://api.tourdocs.io/api/v1`     |

## Authentication

All endpoints (except `/api/v1/auth/login` and `/api/v1/auth/register`) require a valid JWT bearer token.

```
Authorization: Bearer <access_token>
```

Tokens expire after 15 minutes. Use the refresh endpoint to obtain a new access token.

## Response Format

### Success Response

```json
{
  "success": true,
  "message": "Operation completed successfully.",
  "data": { },
  "errors": null
}
```

### Error Response

```json
{
  "success": false,
  "message": "Validation failed.",
  "data": null,
  "errors": {
    "FieldName": ["Error message 1", "Error message 2"]
  }
}
```

### Paginated Response

```json
{
  "success": true,
  "message": null,
  "data": {
    "items": [],
    "totalCount": 150,
    "pageNumber": 1,
    "pageSize": 20,
    "totalPages": 8,
    "hasPreviousPage": false,
    "hasNextPage": true
  },
  "errors": null
}
```

## Pagination

All list endpoints support pagination via query parameters:

| Parameter    | Type   | Default | Description                          |
|-------------|--------|---------|--------------------------------------|
| pageNumber  | int    | 1       | Page number (1-based)                |
| pageSize    | int    | 20      | Items per page (max 100)             |
| sortBy      | string | null    | Property name to sort by             |
| sortDirection | string | asc   | Sort direction: `asc` or `desc`      |
| searchTerm  | string | null    | Free-text search across key fields   |

## HTTP Status Codes

| Code | Meaning                    | When                                          |
|------|----------------------------|-----------------------------------------------|
| 200  | OK                         | Successful GET, PUT, DELETE                    |
| 201  | Created                    | Successful POST (resource created)             |
| 400  | Bad Request                | Validation error, malformed request            |
| 401  | Unauthorized               | Missing or invalid JWT token                   |
| 403  | Forbidden                  | Valid token but insufficient permissions        |
| 404  | Not Found                  | Resource does not exist                        |
| 409  | Conflict                   | Duplicate resource or concurrency conflict      |
| 422  | Unprocessable Entity       | Business rule violation                        |
| 500  | Internal Server Error      | Unexpected server error                        |

## Endpoint Groups

### Authentication (`/api/v1/auth`)

| Method | Endpoint                | Description                  | Auth Required |
|--------|------------------------|------------------------------|---------------|
| POST   | `/auth/login`          | Authenticate user            | No            |
| POST   | `/auth/register`       | Register new user            | No            |
| POST   | `/auth/refresh`        | Refresh access token         | No (refresh token in body) |
| POST   | `/auth/logout`         | Revoke refresh token         | Yes           |
| POST   | `/auth/forgot-password`| Send password reset email    | No            |
| POST   | `/auth/reset-password` | Reset password with token    | No            |
| PUT    | `/auth/change-password`| Change current user password | Yes           |

### Members (`/api/v1/members`)

| Method | Endpoint                      | Description                    | Roles                |
|--------|------------------------------|--------------------------------|----------------------|
| GET    | `/members`                   | List members (paginated)       | Admin, Manager, Handler, Viewer |
| GET    | `/members/{id}`              | Get member by ID               | Admin, Manager, Handler, Viewer |
| POST   | `/members`                   | Create new member              | Admin, Manager       |
| PUT    | `/members/{id}`              | Update member                  | Admin, Manager       |
| DELETE | `/members/{id}`              | Soft delete member             | Admin                |
| GET    | `/members/{id}/documents`    | List member's documents        | Admin, Manager, Handler, Viewer |
| GET    | `/members/{id}/cases`        | List member's cases            | Admin, Manager, Handler |
| POST   | `/members/import`            | Bulk import from CSV           | Admin, Manager       |
| GET    | `/members/export`            | Export members to CSV          | Admin, Manager       |

### Documents (`/api/v1/documents`)

| Method | Endpoint                          | Description                       | Roles                |
|--------|----------------------------------|-----------------------------------|----------------------|
| GET    | `/documents`                     | List documents (paginated)        | Admin, Manager, Handler, Viewer |
| GET    | `/documents/{id}`                | Get document details              | Admin, Manager, Handler, Viewer |
| POST   | `/documents`                     | Upload new document               | Admin, Manager       |
| POST   | `/documents/{id}/versions`       | Upload new version                | Admin, Manager       |
| GET    | `/documents/{id}/download`       | Download document file            | Admin, Manager, Handler, Viewer |
| GET    | `/documents/{id}/versions`       | List document versions            | Admin, Manager, Handler |
| PUT    | `/documents/{id}/verify`         | Mark document as verified         | Admin, Manager, Handler |
| PUT    | `/documents/{id}/reject`         | Reject document                   | Admin, Manager, Handler |
| DELETE | `/documents/{id}`                | Soft delete document              | Admin, Manager       |
| GET    | `/documents/expiring`            | List documents expiring soon      | Admin, Manager       |
| GET    | `/documents/{id}/audit-log`      | Document access audit trail       | Admin                |

### Cases (`/api/v1/cases`)

| Method | Endpoint                          | Description                       | Roles                |
|--------|----------------------------------|-----------------------------------|----------------------|
| GET    | `/cases`                         | List cases (paginated)            | Admin, Manager, Handler |
| GET    | `/cases/{id}`                    | Get case details                  | Admin, Manager, Handler |
| POST   | `/cases`                         | Create new case                   | Admin, Manager       |
| PUT    | `/cases/{id}`                    | Update case                       | Admin, Manager       |
| DELETE | `/cases/{id}`                    | Soft delete case                  | Admin                |
| POST   | `/cases/{id}/members`            | Assign members to case            | Admin, Manager       |
| DELETE | `/cases/{id}/members/{memberId}` | Remove member from case           | Admin, Manager       |
| GET    | `/cases/{id}/checklist`          | Get case checklist progress       | Admin, Manager, Handler |
| PUT    | `/cases/{id}/status`             | Update case status                | Admin, Manager       |

### Checklists (`/api/v1/checklists`)

| Method | Endpoint                          | Description                       | Roles          |
|--------|----------------------------------|-----------------------------------|----------------|
| GET    | `/checklists`                    | List checklist templates          | Admin, Manager |
| GET    | `/checklists/{id}`               | Get checklist template details    | Admin, Manager |
| POST   | `/checklists`                    | Create checklist template         | Admin, Manager |
| PUT    | `/checklists/{id}`               | Update checklist template         | Admin, Manager |
| DELETE | `/checklists/{id}`               | Delete checklist template         | Admin          |

### Hard Copies (`/api/v1/hard-copies`)

| Method | Endpoint                              | Description                        | Roles                |
|--------|--------------------------------------|------------------------------------|----------------------|
| GET    | `/hard-copies`                       | List hard copies (paginated)       | Admin, Manager, Handler |
| GET    | `/hard-copies/{id}`                  | Get hard copy details              | Admin, Manager, Handler |
| POST   | `/hard-copies`                       | Register new hard copy             | Admin, Manager       |
| POST   | `/hard-copies/{id}/transfer`         | Record custody transfer            | Admin, Manager, Handler |
| GET    | `/hard-copies/{id}/custody-chain`    | Get full custody chain             | Admin, Manager, Handler |

### Organizations (`/api/v1/organizations`)

| Method | Endpoint                          | Description                       | Roles          |
|--------|----------------------------------|-----------------------------------|----------------|
| GET    | `/organizations/current`         | Get current organization          | All            |
| PUT    | `/organizations/current`         | Update organization settings      | Admin          |
| GET    | `/organizations`                 | List all organizations            | SuperAdmin     |
| POST   | `/organizations`                 | Create new organization           | SuperAdmin     |

### Users (`/api/v1/users`)

| Method | Endpoint                      | Description                    | Roles          |
|--------|------------------------------|--------------------------------|----------------|
| GET    | `/users`                     | List organization users        | Admin          |
| GET    | `/users/{id}`                | Get user details               | Admin          |
| POST   | `/users`                     | Create/invite user             | Admin          |
| PUT    | `/users/{id}`                | Update user                    | Admin          |
| PUT    | `/users/{id}/role`           | Change user role               | Admin          |
| DELETE | `/users/{id}`                | Deactivate user                | Admin          |
| GET    | `/users/me`                  | Get current user profile       | All            |
| PUT    | `/users/me`                  | Update current user profile    | All            |

### Reports (`/api/v1/reports`)

| Method | Endpoint                          | Description                       | Roles          |
|--------|----------------------------------|-----------------------------------|----------------|
| GET    | `/reports/dashboard`             | Dashboard statistics              | All            |
| GET    | `/reports/document-status`       | Document status breakdown         | Admin, Manager |
| GET    | `/reports/expiry-forecast`       | Document expiry forecast          | Admin, Manager |
| GET    | `/reports/case-progress`         | Case completion progress          | Admin, Manager |
| GET    | `/reports/activity-log`          | User activity report              | Admin          |
| GET    | `/reports/hard-copy-tracking`    | Hard copy location report         | Admin, Manager |

### Audit (`/api/v1/audit`)

| Method | Endpoint                  | Description                    | Roles      |
|--------|--------------------------|--------------------------------|------------|
| GET    | `/audit`                 | Query audit logs (paginated)   | Admin      |
| GET    | `/audit/export`          | Export audit logs to CSV       | Admin      |

### Notifications (`/api/v1/notifications`)

| Method | Endpoint                          | Description                       | Roles |
|--------|----------------------------------|-----------------------------------|-------|
| GET    | `/notifications`                 | List user notifications           | All   |
| GET    | `/notifications/unread-count`    | Get unread notification count     | All   |
| PUT    | `/notifications/{id}/read`       | Mark notification as read         | All   |
| PUT    | `/notifications/read-all`        | Mark all notifications as read    | All   |

## Real-Time (SignalR)

### Hub Endpoints

| Hub             | URL                              | Description                        |
|----------------|----------------------------------|------------------------------------|
| NotificationHub | `/hubs/notifications`           | Real-time notification delivery    |
| DocumentHub     | `/hubs/documents`               | Live document status updates       |

### Events (Server -> Client)

| Event                    | Payload                        | Description                          |
|--------------------------|-------------------------------|--------------------------------------|
| `ReceiveNotification`    | `NotificationDto`             | New notification received            |
| `DocumentStatusChanged`  | `{ documentId, newStatus }`   | Document status updated              |
| `DocumentUploaded`       | `{ memberId, documentId }`    | New document uploaded for a member   |
| `HardCopyTransferred`    | `{ hardCopyId, newStatus }`   | Hard copy custody changed            |

## Health Check

| Endpoint           | Description                                      |
|-------------------|--------------------------------------------------|
| `GET /health`     | Basic health check (returns 200 if API is running) |
| `GET /health/ready`| Readiness check (verifies DB and Redis connectivity)|

## Rate Limiting

API endpoints are rate-limited per user per minute:

| Endpoint Group  | Limit           |
|----------------|-----------------|
| Authentication | 10 requests/min |
| File Upload    | 20 requests/min |
| All others     | 100 requests/min|
