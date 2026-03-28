# TourDocs Frontend - Project Intelligence

## Angular Project Structure

```
frontend/tourdocs-web/
├── src/
│   ├── app/
│   │   ├── core/                          # Singleton services (provided in root)
│   │   │   ├── services/
│   │   │   │   ├── api.service.ts         # Base API service (HTTP helpers)
│   │   │   │   ├── auth.service.ts        # Authentication logic
│   │   │   │   ├── token.service.ts       # Token storage and retrieval
│   │   │   │   ├── signalr.service.ts     # SignalR connection management
│   │   │   │   └── notification.service.ts # Toast notifications
│   │   │   ├── guards/
│   │   │   │   ├── auth.guard.ts          # Functional guard: check JWT validity
│   │   │   │   ├── role.guard.ts          # Functional guard: check user role
│   │   │   │   └── unsaved-changes.guard.ts # Prevent nav with unsaved form
│   │   │   ├── interceptors/
│   │   │   │   ├── jwt.interceptor.ts     # Attach bearer token to requests
│   │   │   │   ├── error.interceptor.ts   # Handle 401/403/500 responses
│   │   │   │   └── loading.interceptor.ts # Set global loading state
│   │   │   ├── models/
│   │   │   │   ├── api-response.model.ts  # ApiResponse<T> interface
│   │   │   │   ├── paged-response.model.ts # PagedResponse<T> interface
│   │   │   │   ├── user.model.ts
│   │   │   │   └── enums.ts              # Frontend enum mirrors
│   │   │   └── constants/
│   │   │       ├── api-endpoints.ts       # API URL constants
│   │   │       └── app-constants.ts       # App-wide constants
│   │   │
│   │   ├── shared/                        # Shared (reusable) components
│   │   │   ├── components/
│   │   │   │   ├── confirm-dialog/        # Reusable confirmation dialog
│   │   │   │   ├── data-table/            # Generic paginated table component
│   │   │   │   ├── file-upload/           # Drag-and-drop file upload
│   │   │   │   ├── search-input/          # Debounced search input
│   │   │   │   ├── status-badge/          # Document/case status badge
│   │   │   │   ├── empty-state/           # Empty state illustration + message
│   │   │   │   ├── loading-spinner/       # Global/local loading indicator
│   │   │   │   ├── page-header/           # Page title + breadcrumb + actions
│   │   │   │   ├── validation-message/    # Form field error display
│   │   │   │   └── avatar/               # User/member avatar component
│   │   │   ├── directives/
│   │   │   │   ├── click-outside.directive.ts
│   │   │   │   ├── debounce-click.directive.ts
│   │   │   │   └── role-visible.directive.ts  # Show/hide by role
│   │   │   ├── pipes/
│   │   │   │   ├── relative-time.pipe.ts  # "2 hours ago"
│   │   │   │   ├── file-size.pipe.ts      # "2.4 MB"
│   │   │   │   ├── truncate.pipe.ts
│   │   │   │   └── status-label.pipe.ts   # Enum to display string
│   │   │   └── validators/
│   │   │       ├── file-size.validator.ts
│   │   │       ├── file-type.validator.ts
│   │   │       └── date-range.validator.ts
│   │   │
│   │   ├── features/                      # Feature modules (lazy-loaded)
│   │   │   ├── auth/
│   │   │   │   ├── pages/
│   │   │   │   │   ├── login/
│   │   │   │   │   ├── register/
│   │   │   │   │   ├── forgot-password/
│   │   │   │   │   └── reset-password/
│   │   │   │   ├── auth.routes.ts
│   │   │   │   └── services/
│   │   │   │       └── auth-api.service.ts
│   │   │   │
│   │   │   ├── dashboard/
│   │   │   │   ├── pages/
│   │   │   │   │   └── dashboard/         # Main dashboard with widgets
│   │   │   │   ├── components/
│   │   │   │   │   ├── stats-card/
│   │   │   │   │   ├── expiring-docs-widget/
│   │   │   │   │   ├── recent-activity-widget/
│   │   │   │   │   └── case-summary-widget/
│   │   │   │   └── dashboard.routes.ts
│   │   │   │
│   │   │   ├── members/
│   │   │   │   ├── pages/
│   │   │   │   │   ├── member-list/       # Paginated list with search/filter
│   │   │   │   │   ├── member-detail/     # Profile + documents + cases tabs
│   │   │   │   │   └── member-form/       # Create/edit member form
│   │   │   │   ├── components/
│   │   │   │   │   ├── member-card/
│   │   │   │   │   ├── member-documents-tab/
│   │   │   │   │   └── member-cases-tab/
│   │   │   │   ├── services/
│   │   │   │   │   └── member-api.service.ts
│   │   │   │   ├── store/
│   │   │   │   │   ├── member.actions.ts
│   │   │   │   │   ├── member.effects.ts
│   │   │   │   │   ├── member.reducer.ts
│   │   │   │   │   └── member.selectors.ts
│   │   │   │   └── members.routes.ts
│   │   │   │
│   │   │   ├── documents/
│   │   │   │   ├── pages/
│   │   │   │   │   ├── document-list/
│   │   │   │   │   ├── document-detail/
│   │   │   │   │   └── document-upload/
│   │   │   │   ├── components/
│   │   │   │   │   ├── document-card/
│   │   │   │   │   ├── document-status-timeline/
│   │   │   │   │   ├── document-version-list/
│   │   │   │   │   └── document-preview/
│   │   │   │   ├── services/
│   │   │   │   │   └── document-api.service.ts
│   │   │   │   ├── store/
│   │   │   │   │   ├── document.actions.ts
│   │   │   │   │   ├── document.effects.ts
│   │   │   │   │   ├── document.reducer.ts
│   │   │   │   │   └── document.selectors.ts
│   │   │   │   └── documents.routes.ts
│   │   │   │
│   │   │   ├── cases/
│   │   │   │   ├── pages/
│   │   │   │   │   ├── case-list/
│   │   │   │   │   ├── case-detail/
│   │   │   │   │   └── case-form/
│   │   │   │   ├── components/
│   │   │   │   │   ├── case-card/
│   │   │   │   │   ├── case-checklist/
│   │   │   │   │   └── case-members/
│   │   │   │   ├── services/
│   │   │   │   │   └── case-api.service.ts
│   │   │   │   ├── store/
│   │   │   │   │   ├── case.actions.ts
│   │   │   │   │   ├── case.effects.ts
│   │   │   │   │   ├── case.reducer.ts
│   │   │   │   │   └── case.selectors.ts
│   │   │   │   └── cases.routes.ts
│   │   │   │
│   │   │   ├── checklists/
│   │   │   │   ├── pages/
│   │   │   │   │   ├── checklist-list/
│   │   │   │   │   └── checklist-form/
│   │   │   │   ├── services/
│   │   │   │   │   └── checklist-api.service.ts
│   │   │   │   └── checklists.routes.ts
│   │   │   │
│   │   │   ├── hard-copies/
│   │   │   │   ├── pages/
│   │   │   │   │   ├── hard-copy-list/
│   │   │   │   │   └── hard-copy-detail/
│   │   │   │   ├── components/
│   │   │   │   │   ├── custody-timeline/
│   │   │   │   │   └── transfer-dialog/
│   │   │   │   ├── services/
│   │   │   │   │   └── hard-copy-api.service.ts
│   │   │   │   └── hard-copies.routes.ts
│   │   │   │
│   │   │   ├── reports/
│   │   │   │   ├── pages/
│   │   │   │   │   └── reports/
│   │   │   │   ├── components/
│   │   │   │   │   ├── report-filters/
│   │   │   │   │   └── report-chart/
│   │   │   │   └── reports.routes.ts
│   │   │   │
│   │   │   ├── settings/
│   │   │   │   ├── pages/
│   │   │   │   │   ├── org-settings/
│   │   │   │   │   ├── profile-settings/
│   │   │   │   │   └── notification-settings/
│   │   │   │   └── settings.routes.ts
│   │   │   │
│   │   │   └── admin/
│   │   │       ├── pages/
│   │   │       │   ├── user-management/
│   │   │       │   ├── audit-log/
│   │   │       │   └── org-management/     # SuperAdmin only
│   │   │       └── admin.routes.ts
│   │   │
│   │   ├── layout/                        # Application shell
│   │   │   ├── shell/                     # Main layout container
│   │   │   │   └── shell.component.ts
│   │   │   ├── sidebar/                   # Dark sidebar navigation
│   │   │   │   └── sidebar.component.ts
│   │   │   ├── toolbar/                   # Top toolbar (search, user menu, notifications)
│   │   │   │   └── toolbar.component.ts
│   │   │   └── footer/
│   │   │       └── footer.component.ts
│   │   │
│   │   ├── store/                         # Root NgRx store
│   │   │   ├── auth/
│   │   │   │   ├── auth.actions.ts
│   │   │   │   ├── auth.effects.ts
│   │   │   │   ├── auth.reducer.ts
│   │   │   │   └── auth.selectors.ts
│   │   │   ├── ui/
│   │   │   │   ├── ui.actions.ts
│   │   │   │   ├── ui.reducer.ts
│   │   │   │   └── ui.selectors.ts
│   │   │   ├── notifications/
│   │   │   │   ├── notification.actions.ts
│   │   │   │   ├── notification.effects.ts
│   │   │   │   ├── notification.reducer.ts
│   │   │   │   └── notification.selectors.ts
│   │   │   └── app.state.ts               # Root state interface
│   │   │
│   │   ├── app.component.ts
│   │   ├── app.config.ts                  # Application configuration
│   │   └── app.routes.ts                  # Root routing with lazy loading
│   │
│   ├── assets/
│   │   ├── icons/
│   │   ├── images/
│   │   └── i18n/                          # Translation files (future)
│   │
│   ├── environments/
│   │   ├── environment.ts                 # Development config
│   │   └── environment.prod.ts            # Production config
│   │
│   ├── styles/
│   │   ├── _variables.scss                # Colors, spacing, breakpoints, shadows
│   │   ├── _mixins.scss                   # SCSS mixins (responsive, flex, truncate)
│   │   ├── _typography.scss               # Font stacks and text styles
│   │   ├── _theme.scss                    # Angular Material custom theme
│   │   ├── _reset.scss                    # CSS reset / normalize
│   │   ├── _utilities.scss                # Utility classes
│   │   └── styles.scss                    # Main stylesheet (imports all partials)
│   │
│   ├── index.html
│   └── main.ts                            # Bootstrap entry point
│
├── angular.json                           # Angular CLI configuration
├── tsconfig.json                          # TypeScript base config
├── tsconfig.app.json                      # App-specific TS config
├── tsconfig.spec.json                     # Test-specific TS config
├── package.json
├── .eslintrc.json                         # ESLint configuration
├── .prettierrc                            # Prettier configuration
├── karma.conf.js                          # Karma test runner config
├── cypress.config.ts                      # Cypress E2E config
└── nginx.conf                             # Production nginx config
```

## How to Add a New Feature Module

Follow these steps to create a complete feature module. Example: adding a "contracts" feature.

### Step 1: Create the Feature Directory Structure

```
src/app/features/contracts/
├── pages/
│   ├── contract-list/
│   │   ├── contract-list.component.ts
│   │   ├── contract-list.component.html
│   │   └── contract-list.component.scss
│   ├── contract-detail/
│   │   ├── contract-detail.component.ts
│   │   ├── contract-detail.component.html
│   │   └── contract-detail.component.scss
│   └── contract-form/
│       ├── contract-form.component.ts
│       ├── contract-form.component.html
│       └── contract-form.component.scss
├── components/                            # Feature-specific presentational components
│   └── contract-card/
│       ├── contract-card.component.ts
│       ├── contract-card.component.html
│       └── contract-card.component.scss
├── services/
│   └── contract-api.service.ts
├── store/
│   ├── contract.actions.ts
│   ├── contract.effects.ts
│   ├── contract.reducer.ts
│   └── contract.selectors.ts
├── models/
│   └── contract.model.ts
└── contracts.routes.ts
```

### Step 2: Define the Feature Routes

```typescript
// features/contracts/contracts.routes.ts
import { Routes } from '@angular/router';

export const contractRoutes: Routes = [
  {
    path: '',
    component: ContractListComponent,
    data: { title: 'Contracts', breadcrumb: 'Contracts' }
  },
  {
    path: 'new',
    component: ContractFormComponent,
    data: { title: 'New Contract', breadcrumb: 'New' }
  },
  {
    path: ':id',
    component: ContractDetailComponent,
    data: { title: 'Contract Details', breadcrumb: 'Details' }
  },
  {
    path: ':id/edit',
    component: ContractFormComponent,
    data: { title: 'Edit Contract', breadcrumb: 'Edit' }
  }
];
```

### Step 3: Register in App Routes

```typescript
// app.routes.ts
{
  path: 'contracts',
  loadChildren: () => import('./features/contracts/contracts.routes').then(m => m.contractRoutes),
  canActivate: [authGuard],
  data: { requiredRole: 'Manager' }
}
```

### Step 4: Create the API Service

```typescript
// features/contracts/services/contract-api.service.ts
import { Injectable, inject } from '@angular/core';
import { ApiService } from '../../../core/services/api.service';
import { Contract, CreateContractRequest, UpdateContractRequest } from '../models/contract.model';
import { ApiResponse, PagedResponse, PagedRequest } from '../../../core/models';

@Injectable({ providedIn: 'root' })
export class ContractApiService {
  private readonly api = inject(ApiService);
  private readonly baseUrl = '/api/v1/contracts';

  getAll(params: PagedRequest) {
    return this.api.get<ApiResponse<PagedResponse<Contract>>>(this.baseUrl, { params });
  }

  getById(id: string) {
    return this.api.get<ApiResponse<Contract>>(`${this.baseUrl}/${id}`);
  }

  create(request: CreateContractRequest) {
    return this.api.post<ApiResponse<Contract>>(this.baseUrl, request);
  }

  update(id: string, request: UpdateContractRequest) {
    return this.api.put<ApiResponse<Contract>>(`${this.baseUrl}/${id}`, request);
  }

  delete(id: string) {
    return this.api.delete<ApiResponse<void>>(`${this.baseUrl}/${id}`);
  }
}
```

### Step 5: Create NgRx Store (see NgRx Patterns section below)

### Step 6: Create the Components

```typescript
// features/contracts/pages/contract-list/contract-list.component.ts
import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Store } from '@ngrx/store';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { PageHeaderComponent } from '../../../../shared/components/page-header/page-header.component';
import { SearchInputComponent } from '../../../../shared/components/search-input/search-input.component';
import { ContractActions } from '../../store/contract.actions';
import { selectContracts, selectContractsLoading, selectContractsPagination } from '../../store/contract.selectors';

@Component({
  selector: 'app-contract-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatPaginatorModule,
    PageHeaderComponent,
    SearchInputComponent
  ],
  templateUrl: './contract-list.component.html',
  styleUrls: ['./contract-list.component.scss']
})
export class ContractListComponent implements OnInit {
  private readonly store = inject(Store);

  contracts$ = this.store.select(selectContracts);
  loading$ = this.store.select(selectContractsLoading);
  pagination$ = this.store.select(selectContractsPagination);

  ngOnInit(): void {
    this.store.dispatch(ContractActions.loadContracts({ page: 1, pageSize: 20 }));
  }

  onSearch(searchTerm: string): void {
    this.store.dispatch(ContractActions.loadContracts({ page: 1, pageSize: 20, searchTerm }));
  }

  onPageChange(event: PageEvent): void {
    this.store.dispatch(ContractActions.loadContracts({
      page: event.pageIndex + 1,
      pageSize: event.pageSize
    }));
  }
}
```

### Step 7: Add Navigation Entry

Add the feature to the sidebar navigation in `layout/sidebar/sidebar.component.ts`:

```typescript
{
  label: 'Contracts',
  icon: 'description',
  route: '/contracts',
  roles: ['Admin', 'Manager']
}
```

## How to Add a New Shared Component

### Step 1: Create Component Files

```typescript
// shared/components/status-badge/status-badge.component.ts
import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-status-badge',
  standalone: true,
  imports: [CommonModule],
  template: `
    <span class="status-badge" [class]="'status-badge--' + status.toLowerCase()">
      {{ label || status }}
    </span>
  `,
  styleUrls: ['./status-badge.component.scss']
})
export class StatusBadgeComponent {
  @Input({ required: true }) status!: string;
  @Input() label?: string;
}
```

### Rules for Shared Components

- Every shared component MUST be standalone
- Shared components are presentational only — no service injection, no store access
- All data comes in via `@Input()`, all events go out via `@Output()`
- Write unit tests for every shared component
- Export from a barrel file: `shared/components/index.ts`
- Use Angular Material components internally when applicable
- Document inputs/outputs with JSDoc comments

## NgRx Patterns

### Actions

```typescript
// store/contract.actions.ts
import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { Contract } from '../models/contract.model';
import { PagedResponse } from '../../../core/models';

export const ContractActions = createActionGroup({
  source: 'Contracts',
  events: {
    // Load
    'Load Contracts': props<{ page: number; pageSize: number; searchTerm?: string }>(),
    'Load Contracts Success': props<{ response: PagedResponse<Contract> }>(),
    'Load Contracts Failure': props<{ error: string }>(),

    // Load Single
    'Load Contract': props<{ id: string }>(),
    'Load Contract Success': props<{ contract: Contract }>(),
    'Load Contract Failure': props<{ error: string }>(),

    // Create
    'Create Contract': props<{ request: CreateContractRequest }>(),
    'Create Contract Success': props<{ contract: Contract }>(),
    'Create Contract Failure': props<{ error: string }>(),

    // Update
    'Update Contract': props<{ id: string; request: UpdateContractRequest }>(),
    'Update Contract Success': props<{ contract: Contract }>(),
    'Update Contract Failure': props<{ error: string }>(),

    // Delete
    'Delete Contract': props<{ id: string }>(),
    'Delete Contract Success': props<{ id: string }>(),
    'Delete Contract Failure': props<{ error: string }>(),

    // UI
    'Clear Contract Error': emptyProps(),
    'Select Contract': props<{ id: string }>(),
  }
});
```

### Effects

```typescript
// store/contract.effects.ts
import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { Router } from '@angular/router';
import { catchError, map, mergeMap, of, tap } from 'rxjs';
import { ContractActions } from './contract.actions';
import { ContractApiService } from '../services/contract-api.service';
import { NotificationService } from '../../../core/services/notification.service';

@Injectable()
export class ContractEffects {
  private readonly actions$ = inject(Actions);
  private readonly contractApi = inject(ContractApiService);
  private readonly notification = inject(NotificationService);
  private readonly router = inject(Router);

  loadContracts$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ContractActions.loadContracts),
      mergeMap(({ page, pageSize, searchTerm }) =>
        this.contractApi.getAll({ pageNumber: page, pageSize, searchTerm }).pipe(
          map(response => ContractActions.loadContractsSuccess({ response: response.data })),
          catchError(error => of(ContractActions.loadContractsFailure({ error: error.message })))
        )
      )
    )
  );

  createContract$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ContractActions.createContract),
      mergeMap(({ request }) =>
        this.contractApi.create(request).pipe(
          map(response => ContractActions.createContractSuccess({ contract: response.data })),
          catchError(error => of(ContractActions.createContractFailure({ error: error.message })))
        )
      )
    )
  );

  createContractSuccess$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ContractActions.createContractSuccess),
      tap(({ contract }) => {
        this.notification.showSuccess('Contract created successfully.');
        this.router.navigate(['/contracts', contract.id]);
      })
    ),
    { dispatch: false }
  );

  deleteContractSuccess$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ContractActions.deleteContractSuccess),
      tap(() => {
        this.notification.showSuccess('Contract deleted.');
        this.router.navigate(['/contracts']);
      })
    ),
    { dispatch: false }
  );

  showError$ = createEffect(() =>
    this.actions$.pipe(
      ofType(
        ContractActions.loadContractsFailure,
        ContractActions.createContractFailure,
        ContractActions.updateContractFailure,
        ContractActions.deleteContractFailure
      ),
      tap(({ error }) => this.notification.showError(error))
    ),
    { dispatch: false }
  );
}
```

### Reducer

```typescript
// store/contract.reducer.ts
import { createReducer, on } from '@ngrx/store';
import { ContractActions } from './contract.actions';
import { Contract } from '../models/contract.model';

export interface ContractState {
  contracts: Contract[];
  selectedContract: Contract | null;
  loading: boolean;
  error: string | null;
  pagination: {
    totalCount: number;
    pageNumber: number;
    pageSize: number;
    totalPages: number;
  };
}

const initialState: ContractState = {
  contracts: [],
  selectedContract: null,
  loading: false,
  error: null,
  pagination: {
    totalCount: 0,
    pageNumber: 1,
    pageSize: 20,
    totalPages: 0
  }
};

export const contractReducer = createReducer(
  initialState,

  on(ContractActions.loadContracts, (state) => ({
    ...state,
    loading: true,
    error: null
  })),

  on(ContractActions.loadContractsSuccess, (state, { response }) => ({
    ...state,
    contracts: response.items,
    loading: false,
    pagination: {
      totalCount: response.totalCount,
      pageNumber: response.pageNumber,
      pageSize: response.pageSize,
      totalPages: response.totalPages
    }
  })),

  on(ContractActions.loadContractsFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  })),

  on(ContractActions.loadContractSuccess, (state, { contract }) => ({
    ...state,
    selectedContract: contract,
    loading: false
  })),

  on(ContractActions.createContractSuccess, (state, { contract }) => ({
    ...state,
    contracts: [contract, ...state.contracts],
    loading: false
  })),

  on(ContractActions.updateContractSuccess, (state, { contract }) => ({
    ...state,
    contracts: state.contracts.map(c => c.id === contract.id ? contract : c),
    selectedContract: contract,
    loading: false
  })),

  on(ContractActions.deleteContractSuccess, (state, { id }) => ({
    ...state,
    contracts: state.contracts.filter(c => c.id !== id),
    loading: false
  })),

  on(ContractActions.clearContractError, (state) => ({
    ...state,
    error: null
  })),

  on(ContractActions.selectContract, (state, { id }) => ({
    ...state,
    selectedContract: state.contracts.find(c => c.id === id) || null
  }))
);
```

### Selectors

```typescript
// store/contract.selectors.ts
import { createFeatureSelector, createSelector } from '@ngrx/store';
import { ContractState } from './contract.reducer';

export const selectContractState = createFeatureSelector<ContractState>('contracts');

export const selectContracts = createSelector(selectContractState, state => state.contracts);
export const selectSelectedContract = createSelector(selectContractState, state => state.selectedContract);
export const selectContractsLoading = createSelector(selectContractState, state => state.loading);
export const selectContractsError = createSelector(selectContractState, state => state.error);
export const selectContractsPagination = createSelector(selectContractState, state => state.pagination);

// Derived selectors
export const selectContractById = (id: string) =>
  createSelector(selectContracts, contracts => contracts.find(c => c.id === id));

export const selectHasContracts = createSelector(selectContracts, contracts => contracts.length > 0);
```

### Registering the Store

In the feature routes file:

```typescript
import { provideState } from '@ngrx/store';
import { provideEffects } from '@ngrx/effects';
import { contractReducer } from './store/contract.reducer';
import { ContractEffects } from './store/contract.effects';

export const contractRoutes: Routes = [
  {
    path: '',
    providers: [
      provideState('contracts', contractReducer),
      provideEffects(ContractEffects)
    ],
    children: [
      // ... routes
    ]
  }
];
```

## Routing and Lazy Loading

### Root Routes (app.routes.ts)

```typescript
import { Routes } from '@angular/router';
import { ShellComponent } from './layout/shell/shell.component';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';

export const appRoutes: Routes = [
  {
    path: 'auth',
    loadChildren: () => import('./features/auth/auth.routes').then(m => m.authRoutes)
  },
  {
    path: '',
    component: ShellComponent,
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        loadChildren: () => import('./features/dashboard/dashboard.routes').then(m => m.dashboardRoutes)
      },
      {
        path: 'members',
        loadChildren: () => import('./features/members/members.routes').then(m => m.memberRoutes),
        canActivate: [roleGuard],
        data: { requiredRole: ['Admin', 'Manager', 'Handler', 'Viewer'] }
      },
      {
        path: 'documents',
        loadChildren: () => import('./features/documents/documents.routes').then(m => m.documentRoutes),
        canActivate: [roleGuard],
        data: { requiredRole: ['Admin', 'Manager', 'Handler', 'Viewer'] }
      },
      {
        path: 'cases',
        loadChildren: () => import('./features/cases/cases.routes').then(m => m.caseRoutes),
        canActivate: [roleGuard],
        data: { requiredRole: ['Admin', 'Manager', 'Handler'] }
      },
      {
        path: 'hard-copies',
        loadChildren: () => import('./features/hard-copies/hard-copies.routes').then(m => m.hardCopyRoutes),
        canActivate: [roleGuard],
        data: { requiredRole: ['Admin', 'Manager', 'Handler'] }
      },
      {
        path: 'checklists',
        loadChildren: () => import('./features/checklists/checklists.routes').then(m => m.checklistRoutes),
        canActivate: [roleGuard],
        data: { requiredRole: ['Admin', 'Manager'] }
      },
      {
        path: 'reports',
        loadChildren: () => import('./features/reports/reports.routes').then(m => m.reportRoutes),
        canActivate: [roleGuard],
        data: { requiredRole: ['Admin', 'Manager'] }
      },
      {
        path: 'settings',
        loadChildren: () => import('./features/settings/settings.routes').then(m => m.settingRoutes)
      },
      {
        path: 'admin',
        loadChildren: () => import('./features/admin/admin.routes').then(m => m.adminRoutes),
        canActivate: [roleGuard],
        data: { requiredRole: ['Admin', 'SuperAdmin'] }
      }
    ]
  },
  { path: '**', redirectTo: 'dashboard' }
];
```

### Guard Implementation

```typescript
// core/guards/auth.guard.ts
import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { map, take } from 'rxjs';
import { selectIsAuthenticated } from '../../store/auth/auth.selectors';

export const authGuard: CanActivateFn = () => {
  const store = inject(Store);
  const router = inject(Router);

  return store.select(selectIsAuthenticated).pipe(
    take(1),
    map(isAuthenticated => {
      if (!isAuthenticated) {
        router.navigate(['/auth/login']);
        return false;
      }
      return true;
    })
  );
};

// core/guards/role.guard.ts
export const roleGuard: CanActivateFn = (route) => {
  const store = inject(Store);
  const router = inject(Router);
  const requiredRoles = route.data['requiredRole'] as string[];

  return store.select(selectCurrentUser).pipe(
    take(1),
    map(user => {
      if (!user || !requiredRoles.includes(user.role)) {
        router.navigate(['/dashboard']);
        return false;
      }
      return true;
    })
  );
};
```

## Material Theme Customization

### Theme File (_theme.scss)

```scss
@use '@angular/material' as mat;

// Custom palette based on Primer-style dark sidebar
$tourdocs-primary: mat.m2-define-palette(mat.$m2-blue-grey-palette, 800, 600, 900);
$tourdocs-accent: mat.m2-define-palette(mat.$m2-teal-palette, 500, 300, 700);
$tourdocs-warn: mat.m2-define-palette(mat.$m2-red-palette, 500);

$tourdocs-theme: mat.m2-define-light-theme((
  color: (
    primary: $tourdocs-primary,
    accent: $tourdocs-accent,
    warn: $tourdocs-warn,
  ),
  typography: mat.m2-define-typography-config(
    $font-family: '"Inter", "Roboto", "Helvetica Neue", sans-serif',
  ),
  density: 0,
));

@include mat.all-component-themes($tourdocs-theme);

// Component-specific overrides
.mat-mdc-raised-button {
  border-radius: 6px !important;
  text-transform: none;
  font-weight: 500;
}

.mat-mdc-card {
  border-radius: 8px !important;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.12) !important;
}

.mat-mdc-dialog-container {
  border-radius: 12px !important;
}
```

## Layout System (Primer-Style)

### Variables (_variables.scss)

```scss
// Colors
$sidebar-bg: #263238;
$sidebar-text: #eceff1;
$sidebar-active: #37474f;
$sidebar-hover: #2e3d44;
$toolbar-bg: #ffffff;
$toolbar-border: #e0e0e0;
$content-bg: #fafafa;
$card-bg: #ffffff;
$primary: #546e7a;
$accent: #26a69a;
$warn: #ef5350;
$text-primary: #212121;
$text-secondary: #757575;
$text-hint: #9e9e9e;
$border-color: #e0e0e0;
$success: #66bb6a;
$info: #42a5f5;
$warning: #ffa726;
$error: #ef5350;

// Spacing
$spacing-xs: 4px;
$spacing-sm: 8px;
$spacing-md: 16px;
$spacing-lg: 24px;
$spacing-xl: 32px;
$spacing-xxl: 48px;

// Layout
$sidebar-width: 260px;
$sidebar-collapsed-width: 64px;
$toolbar-height: 64px;
$content-max-width: 1400px;
$content-padding: 24px;

// Breakpoints
$breakpoint-mobile: 768px;
$breakpoint-tablet: 1024px;
$breakpoint-desktop: 1280px;
$breakpoint-wide: 1440px;

// Shadows
$shadow-sm: 0 1px 2px rgba(0, 0, 0, 0.05);
$shadow-md: 0 1px 3px rgba(0, 0, 0, 0.12);
$shadow-lg: 0 4px 6px rgba(0, 0, 0, 0.1);
$shadow-xl: 0 10px 15px rgba(0, 0, 0, 0.1);

// Border radius
$radius-sm: 4px;
$radius-md: 6px;
$radius-lg: 8px;
$radius-xl: 12px;

// Transitions
$transition-fast: 150ms ease;
$transition-normal: 250ms ease;
$transition-slow: 350ms ease;

// Z-index layers
$z-sidebar: 100;
$z-toolbar: 200;
$z-overlay: 300;
$z-dialog: 400;
$z-toast: 500;
```

### Shell Layout

```scss
// layout/shell/shell.component.scss
@use '../../../styles/variables' as *;

.shell {
  display: flex;
  height: 100vh;
  overflow: hidden;

  &__sidebar {
    width: $sidebar-width;
    flex-shrink: 0;
    transition: width $transition-normal;
    z-index: $z-sidebar;

    &--collapsed {
      width: $sidebar-collapsed-width;
    }
  }

  &__main {
    flex: 1;
    display: flex;
    flex-direction: column;
    overflow: hidden;
  }

  &__toolbar {
    height: $toolbar-height;
    flex-shrink: 0;
    z-index: $z-toolbar;
  }

  &__content {
    flex: 1;
    overflow-y: auto;
    background: $content-bg;
    padding: $content-padding;

    &-inner {
      max-width: $content-max-width;
      margin: 0 auto;
    }
  }
}

@media (max-width: $breakpoint-mobile) {
  .shell {
    &__sidebar {
      position: fixed;
      left: -$sidebar-width;
      height: 100vh;
      transition: left $transition-normal;

      &--open {
        left: 0;
      }
    }

    &__content {
      padding: $spacing-md;
    }
  }
}
```

## Form Patterns

### Standard Reactive Form

```typescript
// features/members/pages/member-form/member-form.component.ts
import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { ValidationMessageComponent } from '../../../../shared/components/validation-message/validation-message.component';

@Component({
  selector: 'app-member-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatDatepickerModule,
    ValidationMessageComponent
  ],
  templateUrl: './member-form.component.html',
  styleUrls: ['./member-form.component.scss']
})
export class MemberFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly store = inject(Store);

  form!: FormGroup;
  isEdit = false;
  memberId: string | null = null;

  ngOnInit(): void {
    this.memberId = this.route.snapshot.paramMap.get('id');
    this.isEdit = !!this.memberId;

    this.form = this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(100)]],
      lastName: ['', [Validators.required, Validators.maxLength(100)]],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', [Validators.pattern(/^\+?[\d\s-]{7,15}$/)]],
      dateOfBirth: [null],
      category: ['', Validators.required],
      notes: ['', Validators.maxLength(2000)]
    });

    if (this.isEdit) {
      this.store.dispatch(MemberActions.loadMember({ id: this.memberId! }));
      this.store.select(selectSelectedMember).subscribe(member => {
        if (member) {
          this.form.patchValue(member);
        }
      });
    }
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const formValue = this.form.getRawValue();

    if (this.isEdit) {
      this.store.dispatch(MemberActions.updateMember({ id: this.memberId!, request: formValue }));
    } else {
      this.store.dispatch(MemberActions.createMember({ request: formValue }));
    }
  }

  onCancel(): void {
    this.router.navigate(['/members']);
  }
}
```

### Form Template

```html
<!-- member-form.component.html -->
<app-page-header [title]="isEdit ? 'Edit Member' : 'New Member'">
  <button mat-stroked-button (click)="onCancel()">Cancel</button>
  <button mat-raised-button color="primary" (click)="onSubmit()" [disabled]="form.invalid">
    {{ isEdit ? 'Update' : 'Create' }}
  </button>
</app-page-header>

<form [formGroup]="form" (ngSubmit)="onSubmit()">
  <mat-card>
    <mat-card-content>
      <div class="form-grid">
        <mat-form-field appearance="outline">
          <mat-label>First Name</mat-label>
          <input matInput formControlName="firstName" />
          <app-validation-message [control]="form.get('firstName')!" />
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Last Name</mat-label>
          <input matInput formControlName="lastName" />
          <app-validation-message [control]="form.get('lastName')!" />
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Email</mat-label>
          <input matInput formControlName="email" type="email" />
          <app-validation-message [control]="form.get('email')!" />
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Phone</mat-label>
          <input matInput formControlName="phone" />
          <app-validation-message [control]="form.get('phone')!" />
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Date of Birth</mat-label>
          <input matInput [matDatepicker]="dob" formControlName="dateOfBirth" />
          <mat-datepicker-toggle matSuffix [for]="dob" />
          <mat-datepicker #dob />
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Category</mat-label>
          <mat-select formControlName="category">
            <mat-option value="general">General</mat-option>
            <mat-option value="vip">VIP</mat-option>
            <mat-option value="archived">Archived</mat-option>
          </mat-select>
          <app-validation-message [control]="form.get('category')!" />
        </mat-form-field>
      </div>

      <mat-form-field appearance="outline" class="full-width">
        <mat-label>Notes</mat-label>
        <textarea matInput formControlName="notes" rows="4"></textarea>
        <mat-hint align="end">{{ form.get('notes')?.value?.length || 0 }} / 2000</mat-hint>
        <app-validation-message [control]="form.get('notes')!" />
      </mat-form-field>
    </mat-card-content>
  </mat-card>
</form>
```

### Validation Message Component

```typescript
// shared/components/validation-message/validation-message.component.ts
@Component({
  selector: 'app-validation-message',
  standalone: true,
  imports: [CommonModule],
  template: `
    @if (control && control.invalid && control.touched) {
      <mat-error>
        @if (control.hasError('required')) {
          This field is required.
        } @else if (control.hasError('email')) {
          Please enter a valid email address.
        } @else if (control.hasError('maxlength')) {
          Maximum {{ control.getError('maxlength').requiredLength }} characters allowed.
        } @else if (control.hasError('minlength')) {
          Minimum {{ control.getError('minlength').requiredLength }} characters required.
        } @else if (control.hasError('pattern')) {
          Invalid format.
        }
      </mat-error>
    }
  `
})
export class ValidationMessageComponent {
  @Input({ required: true }) control!: AbstractControl;
}
```

## How to Add a New API Service

All API services follow the same pattern and extend `BaseApiService`:

```typescript
// core/services/api.service.ts
import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiUrl;

  get<T>(path: string, options?: { params?: any }): Observable<T> {
    let httpParams = new HttpParams();
    if (options?.params) {
      Object.keys(options.params).forEach(key => {
        const value = options.params[key];
        if (value !== null && value !== undefined && value !== '') {
          httpParams = httpParams.set(key, value.toString());
        }
      });
    }
    return this.http.get<T>(`${this.baseUrl}${path}`, { params: httpParams });
  }

  post<T>(path: string, body: any): Observable<T> {
    return this.http.post<T>(`${this.baseUrl}${path}`, body);
  }

  put<T>(path: string, body: any): Observable<T> {
    return this.http.put<T>(`${this.baseUrl}${path}`, body);
  }

  delete<T>(path: string): Observable<T> {
    return this.http.delete<T>(`${this.baseUrl}${path}`);
  }

  upload<T>(path: string, formData: FormData): Observable<T> {
    return this.http.post<T>(`${this.baseUrl}${path}`, formData, {
      reportProgress: true,
      observe: 'body' as const
    });
  }

  download(path: string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}${path}`, {
      responseType: 'blob'
    });
  }
}
```

To create a new feature API service:

```typescript
// features/widgets/services/widget-api.service.ts
import { Injectable, inject } from '@angular/core';
import { ApiService } from '../../../core/services/api.service';

@Injectable({ providedIn: 'root' })
export class WidgetApiService {
  private readonly api = inject(ApiService);
  private readonly path = '/api/v1/widgets';

  getAll(params: PagedRequest) {
    return this.api.get<ApiResponse<PagedResponse<Widget>>>(this.path, { params });
  }

  getById(id: string) {
    return this.api.get<ApiResponse<Widget>>(`${this.path}/${id}`);
  }

  create(request: CreateWidgetRequest) {
    return this.api.post<ApiResponse<Widget>>(this.path, request);
  }

  update(id: string, request: UpdateWidgetRequest) {
    return this.api.put<ApiResponse<Widget>>(`${this.path}/${id}`, request);
  }

  delete(id: string) {
    return this.api.delete<ApiResponse<void>>(`${this.path}/${id}`);
  }
}
```

## State Management Conventions

1. **Root store** (`app/store/`) handles cross-cutting state: auth, UI, notifications
2. **Feature stores** (`features/{name}/store/`) handle feature-specific state
3. Feature stores are lazily registered via `provideState()` in feature routes
4. NEVER access store state directly — always use selectors
5. NEVER dispatch actions from effects (except for chained success/failure patterns)
6. Effects handle ALL side effects: API calls, navigation, toast messages, SignalR
7. Reducers are pure functions — no side effects, no async operations
8. Use `createActionGroup` for type-safe, concise action definitions
9. Normalize nested data in the store when entities reference each other
10. Use `@ngrx/entity` adapter when a store slice is a flat list of entities with CRUD

## Responsive Design Breakpoints

```scss
// Usage in component SCSS
@use '../../../styles/variables' as *;
@use '../../../styles/mixins' as *;

// Mixin definitions (_mixins.scss)
@mixin mobile {
  @media (max-width: #{$breakpoint-mobile - 1px}) { @content; }
}

@mixin tablet {
  @media (min-width: $breakpoint-mobile) and (max-width: #{$breakpoint-tablet - 1px}) { @content; }
}

@mixin tablet-up {
  @media (min-width: $breakpoint-mobile) { @content; }
}

@mixin desktop {
  @media (min-width: $breakpoint-tablet) { @content; }
}

@mixin wide {
  @media (min-width: $breakpoint-wide) { @content; }
}

// Usage
.member-grid {
  display: grid;
  gap: $spacing-md;
  grid-template-columns: 1fr;

  @include tablet-up {
    grid-template-columns: repeat(2, 1fr);
  }

  @include desktop {
    grid-template-columns: repeat(3, 1fr);
  }

  @include wide {
    grid-template-columns: repeat(4, 1fr);
  }
}
```

### Responsive Rules

- Mobile-first approach: base styles target mobile, then add breakpoints for larger screens
- Sidebar collapses to hamburger menu on mobile
- Data tables switch to card layout on mobile
- Form fields stack vertically on mobile, grid on desktop
- Dialogs go full-screen on mobile
- Touch targets minimum 44x44px on mobile
- Hide secondary information on mobile; show via expand/detail view
