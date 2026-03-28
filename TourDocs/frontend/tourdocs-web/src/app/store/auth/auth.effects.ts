import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of, EMPTY } from 'rxjs';
import { map, exhaustMap, catchError, tap, switchMap } from 'rxjs/operators';
import { AuthService } from '@core/auth/auth.service';
import { TokenResponse, UserProfile } from '@core/auth/auth.models';
import { AuthActions } from './auth.actions';

@Injectable()
export class AuthEffects {
  private readonly actions$ = inject(Actions);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  initAuth$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.initAuth),
      switchMap(() => {
        const token = this.authService.getToken();
        if (!token || !this.authService.isAuthenticated()) {
          return EMPTY;
        }
        const payload = this.decodeJwt(token);
        if (!payload) {
          return EMPTY;
        }
        const tokenResponse: TokenResponse = {
          accessToken: token,
          refreshToken: this.authService.getRefreshToken() || '',
          expiresAt: new Date(payload.exp * 1000).toISOString(),
          tokenType: 'Bearer'
        };
        const user: UserProfile = {
          id: payload.sub,
          email: payload.email || '',
          firstName: payload.given_name || '',
          lastName: payload.family_name || '',
          fullName: [payload.given_name, payload.family_name].filter(Boolean).join(' ') || payload.email || '',
          avatarUrl: null,
          roles: Array.isArray(payload.roles) ? payload.roles : (payload.roles ? [payload.roles] : []),
          organizationId: payload.org_id || '',
          organizationName: '',
          isActive: true,
          createdAt: '',
          lastLoginAt: ''
        };
        return of(AuthActions.initAuthComplete({ token: tokenResponse, user }));
      })
    )
  );

  initAuthComplete$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.initAuthComplete),
      map(() => AuthActions.loadProfile())
    )
  );

  login$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.login),
      exhaustMap(({ request }) =>
        this.authService.login(request).pipe(
          map(response => {
            if (response.success) {
              return AuthActions.loginSuccess({ tokenResponse: response.data });
            }
            return AuthActions.loginFailure({ error: response.message });
          }),
          catchError(error =>
            of(AuthActions.loginFailure({
              error: error.error?.message || 'Login failed. Please try again.'
            }))
          )
        )
      )
    )
  );

  loginSuccess$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.loginSuccess),
      tap(() => {
        this.router.navigate(['/dashboard']);
      }),
      map(() => AuthActions.loadProfile())
    )
  );

  register$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.register),
      exhaustMap(({ request }) =>
        this.authService.register(request).pipe(
          map(response => {
            if (response.success) {
              return AuthActions.registerSuccess({ tokenResponse: response.data });
            }
            return AuthActions.registerFailure({ error: response.message });
          }),
          catchError(error =>
            of(AuthActions.registerFailure({
              error: error.error?.message || 'Registration failed. Please try again.'
            }))
          )
        )
      )
    )
  );

  registerSuccess$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.registerSuccess),
      tap(() => {
        this.router.navigate(['/dashboard']);
      }),
      map(() => AuthActions.loadProfile())
    )
  );

  loadProfile$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.loadProfile),
      exhaustMap(() =>
        this.authService.getCurrentUser().pipe(
          map(response => {
            if (response.success) {
              return AuthActions.loadProfileSuccess({ user: response.data });
            }
            return AuthActions.loadProfileFailure({ error: response.message });
          }),
          catchError(error =>
            of(AuthActions.loadProfileFailure({
              error: error.error?.message || 'Failed to load profile.'
            }))
          )
        )
      )
    )
  );

  logout$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.logout),
      tap(() => {
        this.authService.logout();
      })
    ),
    { dispatch: false }
  );

  private decodeJwt(token: string): any {
    try {
      const payload = token.split('.')[1];
      return JSON.parse(atob(payload));
    } catch {
      return null;
    }
  }
}
