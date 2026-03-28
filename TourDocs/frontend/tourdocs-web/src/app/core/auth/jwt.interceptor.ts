import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { BehaviorSubject, throwError } from 'rxjs';
import { catchError, filter, switchMap, take } from 'rxjs/operators';
import { AuthService } from './auth.service';

let isRefreshing = false;
let refreshTokenSubject = new BehaviorSubject<string | null>(null);

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);

  if (req.url.includes('/auth/login') ||
      req.url.includes('/auth/register') ||
      req.url.includes('/auth/refresh-token') ||
      req.url.includes('/auth/forgot-password')) {
    return next(req);
  }

  const token = authService.getToken();
  if (token) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        if (isRefreshing) {
          // Another request is already refreshing the token — queue this one
          return refreshTokenSubject.pipe(
            filter(token => token !== null),
            take(1),
            switchMap(token => next(req.clone({
              setHeaders: { Authorization: `Bearer ${token}` }
            })))
          );
        }

        isRefreshing = true;
        refreshTokenSubject.next(null);

        return authService.refreshToken().pipe(
          switchMap(response => {
            isRefreshing = false;
            if (response.success) {
              const newToken = response.data.accessToken;
              refreshTokenSubject.next(newToken);
              return next(req.clone({
                setHeaders: { Authorization: `Bearer ${newToken}` }
              }));
            }
            authService.logout();
            return throwError(() => error);
          }),
          catchError(refreshError => {
            isRefreshing = false;
            refreshTokenSubject.next(null);
            authService.logout();
            return throwError(() => refreshError);
          })
        );
      }
      return throwError(() => error);
    })
  );
};
