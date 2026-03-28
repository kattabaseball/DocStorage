# TourDocs - Contributing Guide

Thank you for contributing to TourDocs. This guide covers the development workflow, conventions, and quality standards.

## Getting Started

1. Read the [Setup Guide](SETUP.md) to configure your development environment
2. Read the root [CLAUDE.md](../CLAUDE.md) for project overview and coding standards
3. Read the relevant sub-project CLAUDE.md ([backend](../backend/CLAUDE.md) or [frontend](../frontend/CLAUDE.md))
4. Familiarize yourself with the [Architecture Decisions](ARCHITECTURE.md)

## Branch Naming Convention

All branches must follow this naming pattern:

```
{type}/TOUR-{issue-number}-{short-description}
```

### Branch Types

| Type      | Purpose                                | Base Branch |
|-----------|----------------------------------------|-------------|
| `feature` | New feature or functionality           | `develop`   |
| `bugfix`  | Bug fix                                | `develop`   |
| `hotfix`  | Urgent production fix                  | `main`      |
| `release` | Release preparation                    | `develop`   |
| `chore`   | Build, CI, dependency updates          | `develop`   |
| `docs`    | Documentation only changes             | `develop`   |

### Examples

```
feature/TOUR-42-member-bulk-import
bugfix/TOUR-108-document-upload-timeout
hotfix/TOUR-215-auth-token-refresh-loop
release/v1.2.0
chore/TOUR-300-upgrade-angular-18
docs/TOUR-55-api-pagination-examples
```

## Commit Message Format

We follow [Conventional Commits](https://www.conventionalcommits.org/):

```
<type>(<scope>): <description>

[optional body]

[optional footer]
```

### Types

| Type       | Description                                           |
|-----------|-------------------------------------------------------|
| `feat`    | New feature                                           |
| `fix`     | Bug fix                                               |
| `docs`    | Documentation changes                                 |
| `style`   | Formatting, white-space (no code change)              |
| `refactor`| Code restructuring (no behavior change)               |
| `perf`    | Performance improvement                               |
| `test`    | Adding or updating tests                              |
| `build`   | Build system or dependency changes                    |
| `ci`      | CI/CD configuration changes                           |
| `chore`   | Other changes that don't modify src or test files     |
| `revert`  | Revert a previous commit                              |

### Scopes

| Scope        | Description                            |
|-------------|----------------------------------------|
| `auth`      | Authentication and authorization       |
| `members`   | Member management                      |
| `documents` | Document management                    |
| `cases`     | Case management                        |
| `checklists`| Checklist management                   |
| `hard-copies`| Hard copy tracking                    |
| `reports`   | Reporting                              |
| `admin`     | Administration features                |
| `api`       | API layer (controllers, middleware)     |
| `core`      | Business logic (services, DTOs)        |
| `data`      | Data layer (repositories, migrations)  |
| `ui`        | Frontend UI components                 |
| `store`     | NgRx state management                  |
| `deps`      | Dependency updates                     |
| `ci`        | CI/CD pipeline                         |
| `docker`    | Docker configuration                   |

### Examples

```
feat(members): add CSV bulk import endpoint

Implement POST /api/v1/members/import that accepts a CSV file,
validates each row, and creates members in bulk.

Closes TOUR-42

---

fix(documents): resolve version conflict on concurrent upload

Use optimistic concurrency with row version to prevent duplicate
version numbers when two users upload simultaneously.

Closes TOUR-108

---

chore(deps): update Angular to 18.2.1

Update @angular/core, @angular/cli, and related packages
to version 18.2.1 for security patches.
```

## Development Workflow

### 1. Pick an Issue

- Assign yourself to the issue in the project board
- Move the issue to "In Progress"
- If no issue exists, create one before starting work

### 2. Create a Branch

```bash
git checkout develop
git pull origin develop
git checkout -b feature/TOUR-42-member-bulk-import
```

### 3. Develop

- Follow the coding standards in the relevant CLAUDE.md
- Write tests alongside your code
- Commit frequently with meaningful messages
- Keep commits atomic (one logical change per commit)

### 4. Test Locally

```bash
# Backend
cd backend
dotnet test TourDocs.sln

# Frontend
cd frontend/tourdocs-web
ng test --watch=false
ng lint
```

### 5. Push and Create PR

```bash
git push origin feature/TOUR-42-member-bulk-import
```

Create a Pull Request on GitHub targeting the `develop` branch. Fill out the PR template completely.

### 6. Address Review Feedback

- Respond to all review comments
- Push new commits to address feedback (do not force-push)
- Re-request review after addressing all comments

### 7. Merge

After approval:
- Squash and merge for feature/bugfix branches
- Merge commit for release branches
- Delete the source branch after merge

## Code Review Checklist

### General

- [ ] Code follows project coding standards (see CLAUDE.md)
- [ ] No unnecessary code changes (formatting, imports) outside the PR scope
- [ ] No commented-out code committed
- [ ] No hardcoded values that should be configuration
- [ ] Error handling is appropriate and consistent
- [ ] Logging is added for significant operations

### Backend

- [ ] Business logic is in service layer, not controllers
- [ ] Controllers are thin (validate, call service, return result)
- [ ] Multi-tenancy filtering is applied
- [ ] DTOs used for all API input/output (never raw entities)
- [ ] FluentValidation validators created for new request DTOs
- [ ] Custom exceptions used (not generic `Exception`)
- [ ] Async/await used throughout (no `.Result` or `.Wait()`)
- [ ] AutoMapper mappings added for new DTOs
- [ ] `[ProducesResponseType]` attributes on controller actions
- [ ] Audit logging for sensitive operations
- [ ] EF Core migration included if schema changed
- [ ] Migration tested (both up and down)

### Frontend

- [ ] Standalone components (no new NgModules)
- [ ] New control flow syntax (`@if`, `@for`, not `*ngIf`, `*ngFor`)
- [ ] `inject()` function used (not constructor injection)
- [ ] Reactive forms (not template-driven)
- [ ] NgRx pattern followed (actions, effects, reducer, selectors)
- [ ] Component split: smart (container) vs dumb (presentational)
- [ ] BEM naming for SCSS classes
- [ ] Responsive design verified (mobile, tablet, desktop)
- [ ] No hardcoded API URLs (use environment config)
- [ ] Accessibility: proper ARIA attributes, keyboard navigation

### Testing

- [ ] Unit tests cover new business logic
- [ ] Test naming follows convention (`MethodName_Scenario_ExpectedBehavior`)
- [ ] Edge cases tested (null, empty, boundary values)
- [ ] Tests are independent (no shared mutable state)
- [ ] No flaky tests introduced

### Security

- [ ] No secrets in code or configuration files
- [ ] Authorization checks in place for new endpoints
- [ ] Input validation prevents injection attacks
- [ ] File upload validation (type, size) enforced
- [ ] Sensitive data not logged or exposed in API responses

## Testing Requirements

### Minimum Coverage

| Project           | Target | Enforced |
|-------------------|--------|----------|
| TourDocs.Core     | 80%    | CI gate  |
| TourDocs.API      | 60%    | Advisory |
| TourDocs.Data     | 50%    | Advisory |
| Frontend (overall)| 70%    | CI gate  |

### What to Test

**Always test:**
- Service methods (business logic)
- Validators (validation rules)
- Reducers (state transitions)
- Effects (side effect handling)
- Guards (route protection)
- Pipes (data transformation)
- Utility functions

**Test when practical:**
- Controller actions (integration tests)
- Repository custom queries (integration tests with test DB)
- Component rendering (template tests)
- E2E flows (Cypress for critical paths)

**Do not test:**
- Auto-generated code (AutoMapper mappings, unless custom logic)
- Framework code (Angular lifecycle hooks without custom logic)
- Simple property access (getters/setters without logic)

### Test Naming Convention

**Backend (xUnit):**
```
MethodName_Scenario_ExpectedBehavior
```
Example: `GetByIdAsync_NonExistentId_ThrowsNotFoundException`

**Frontend (Jasmine):**
```
should [expected behavior] when [scenario]
```
Example: `should display error message when form is invalid`

## Pull Request Process

1. **Create PR** with the PR template filled out completely
2. **Automated checks** run (CI pipeline: build, lint, test)
3. **Request review** from at least one team member
4. **Address feedback** by pushing new commits
5. **All checks pass** and at least one approval received
6. **Squash and merge** into the target branch
7. **Delete** the source branch
8. **Move issue** to "Done" on the project board

### PR Size Guidelines

- Aim for PRs under 400 lines of changes (excluding auto-generated files)
- If a feature is large, break it into smaller PRs:
  1. Backend entity + repository + migration
  2. Backend service + DTOs + validators
  3. Backend controller + tests
  4. Frontend store + API service
  5. Frontend components + pages
- Each PR should be independently reviewable and deployable

### Labels

| Label            | Description                        |
|-----------------|------------------------------------|
| `backend`       | Changes to .NET backend            |
| `frontend`      | Changes to Angular frontend        |
| `breaking`      | Contains breaking changes          |
| `migration`     | Includes database migration        |
| `needs-review`  | Ready for code review              |
| `wip`           | Work in progress (do not merge)    |
| `blocked`       | Blocked by another issue/PR        |

## Release Process

1. Create a `release/v{major}.{minor}.{patch}` branch from `develop`
2. Update version numbers in `package.json` and `.csproj` files
3. Update CHANGELOG.md with release notes
4. Create PR from release branch to `main`
5. After merge to `main`, tag the release: `git tag v{major}.{minor}.{patch}`
6. Merge `main` back to `develop`
7. CD pipeline deploys to production

## Questions?

- Check existing documentation in the `docs/` folder
- Search closed issues and PRs for similar questions
- Ask in the team communication channel
- Create a discussion on GitHub if the question is broadly relevant
