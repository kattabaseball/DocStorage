## Summary

<!-- Briefly describe what this PR does and why. Link to the relevant issue if applicable. -->

Closes #<!-- issue number -->

## Type of Change

- [ ] Bug fix (non-breaking change that fixes an issue)
- [ ] New feature (non-breaking change that adds functionality)
- [ ] Enhancement (improvement to an existing feature)
- [ ] Breaking change (fix or feature that would cause existing functionality to change)
- [ ] Refactoring (no functional changes)
- [ ] Documentation update
- [ ] CI/CD or infrastructure change
- [ ] Database migration

## Changes Made

<!-- List the specific changes made in this PR. -->

-
-
-

## Screenshots / Recordings

<!-- If this PR includes UI changes, add screenshots or recordings. Remove this section if not applicable. -->

## Checklist

### General
- [ ] My code follows the project's coding standards
- [ ] I have performed a self-review of my code
- [ ] I have commented my code where necessary (complex logic only)
- [ ] My changes generate no new warnings or errors

### Testing
- [ ] I have added unit tests that cover my changes
- [ ] All new and existing unit tests pass locally
- [ ] I have tested this change manually (describe in Testing Notes below)

### Backend Specific
- [ ] API responses use `ApiResponse<T>` wrapper
- [ ] Business logic is in service layer, not controllers
- [ ] Multi-tenancy filtering is applied (organization_id)
- [ ] Custom exceptions are used (not generic `Exception`)
- [ ] EF Core migration is included (if schema changed)
- [ ] Audit logging is implemented for sensitive operations

### Frontend Specific
- [ ] New control flow syntax used (`@if`, `@for`, not `*ngIf`, `*ngFor`)
- [ ] Components are standalone (no new NgModules)
- [ ] `inject()` used instead of constructor injection
- [ ] Reactive forms used (not template-driven)
- [ ] NgRx patterns followed (actions, effects, reducer, selectors)
- [ ] Responsive design verified on mobile, tablet, and desktop

### Database
- [ ] Migration has been tested (up and down)
- [ ] No breaking changes to existing data
- [ ] Indexes added for frequently queried columns

## Testing Notes

<!-- Describe how you tested this change. Include steps to reproduce if relevant. -->

## Additional Notes

<!-- Any other context, deployment notes, or considerations for reviewers. -->
