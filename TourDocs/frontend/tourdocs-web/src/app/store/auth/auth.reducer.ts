import { createReducer, on } from '@ngrx/store';
import { UserProfile, TokenResponse } from '@core/auth/auth.models';
import { AuthActions } from './auth.actions';

export interface AuthState {
  user: UserProfile | null;
  token: TokenResponse | null;
  loading: boolean;
  error: string | null;
}

export const initialAuthState: AuthState = {
  user: null,
  token: null,
  loading: false,
  error: null
};

export const authReducer = createReducer(
  initialAuthState,
  on(AuthActions.login, (state) => ({
    ...state,
    loading: true,
    error: null
  })),
  on(AuthActions.loginSuccess, (state, { tokenResponse }) => ({
    ...state,
    token: tokenResponse,
    user: tokenResponse.userId ? {
      id: tokenResponse.userId,
      email: tokenResponse.email || '',
      firstName: (tokenResponse.fullName || '').split(' ')[0] || '',
      lastName: (tokenResponse.fullName || '').split(' ').slice(1).join(' ') || '',
      fullName: tokenResponse.fullName || '',
      avatarUrl: null,
      roles: [tokenResponse.role || ''],
      organizationId: tokenResponse.organizationId || '',
      organizationName: tokenResponse.organizationName || '',
      isActive: true,
      createdAt: '',
      lastLoginAt: ''
    } : null,
    loading: false,
    error: null
  })),
  on(AuthActions.loginFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  })),
  on(AuthActions.register, (state) => ({
    ...state,
    loading: true,
    error: null
  })),
  on(AuthActions.registerSuccess, (state, { tokenResponse }) => ({
    ...state,
    token: tokenResponse,
    user: tokenResponse.userId ? {
      id: tokenResponse.userId,
      email: tokenResponse.email || '',
      firstName: (tokenResponse.fullName || '').split(' ')[0] || '',
      lastName: (tokenResponse.fullName || '').split(' ').slice(1).join(' ') || '',
      fullName: tokenResponse.fullName || '',
      avatarUrl: null,
      roles: [tokenResponse.role || ''],
      organizationId: tokenResponse.organizationId || '',
      organizationName: tokenResponse.organizationName || '',
      isActive: true,
      createdAt: '',
      lastLoginAt: ''
    } : null,
    loading: false,
    error: null
  })),
  on(AuthActions.registerFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  })),
  on(AuthActions.initAuthComplete, (state, { token, user }) => ({
    ...state,
    token,
    user,
    loading: false,
    error: null
  })),
  on(AuthActions.logout, () => ({
    ...initialAuthState
  })),
  on(AuthActions.loadProfile, (state) => ({
    ...state,
    loading: true
  })),
  on(AuthActions.loadProfileSuccess, (state, { user }) => ({
    ...state,
    user: user || state.user,
    loading: false
  })),
  on(AuthActions.loadProfileFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  })),
  on(AuthActions.clearError, (state) => ({
    ...state,
    error: null
  }))
);
