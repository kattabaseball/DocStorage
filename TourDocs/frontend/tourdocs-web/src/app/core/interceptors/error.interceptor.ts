import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { NotificationService } from '../services/notification.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const notificationService = inject(NotificationService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let errorMessage = 'An unexpected error occurred';

      switch (error.status) {
        case 0:
          errorMessage = 'Unable to connect to the server. Please check your internet connection.';
          notificationService.showError(errorMessage);
          break;
        case 400:
          errorMessage = error.error?.message || 'Bad request. Please check your input.';
          notificationService.showError(errorMessage);
          break;
        case 401:
          // JWT interceptor handles refresh. If we still get 401, session expired.
          notificationService.showWarning('Your session has expired. Please log in again.');
          break;
        case 403:
          errorMessage = 'You do not have permission to perform this action.';
          notificationService.showWarning(errorMessage);
          break;
        case 404:
          errorMessage = error.error?.message || 'The requested resource was not found.';
          notificationService.showWarning(errorMessage);
          break;
        case 409:
          errorMessage = error.error?.message || 'A conflict occurred. The resource may have been modified.';
          notificationService.showWarning(errorMessage);
          break;
        case 422:
          errorMessage = error.error?.message || 'Validation failed. Please check your input.';
          notificationService.showError(errorMessage);
          break;
        case 429:
          errorMessage = 'Too many requests. Please wait and try again.';
          notificationService.showWarning(errorMessage);
          break;
        case 500:
          errorMessage = 'An internal server error occurred. Please try again later.';
          notificationService.showError(errorMessage);
          break;
        case 503:
          errorMessage = 'The service is temporarily unavailable. Please try again later.';
          notificationService.showError(errorMessage);
          break;
        default:
          notificationService.showError(errorMessage);
          break;
      }

      return throwError(() => error);
    })
  );
};
