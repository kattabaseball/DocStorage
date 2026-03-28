import { createFeatureSelector, createSelector } from '@ngrx/store';
import { AuthState } from './auth.reducer';

export const selectAuthState = createFeatureSelector<AuthState>('auth');

export const selectUser = createSelector(
  selectAuthState,
  (state) => state.user
);

export const selectIsAuthenticated = createSelector(
  selectAuthState,
  (state) => state.token !== null
);

export const selectAuthLoading = createSelector(
  selectAuthState,
  (state) => state.loading
);

export const selectAuthError = createSelector(
  selectAuthState,
  (state) => state.error
);

export const selectUserRoles = createSelector(
  selectUser,
  (user) => user?.roles || []
);

export const selectUserFullName = createSelector(
  selectUser,
  (user) => user ? user.fullName : ''
);

export const selectOrganizationName = createSelector(
  selectUser,
  (user) => user?.organizationName || ''
);
