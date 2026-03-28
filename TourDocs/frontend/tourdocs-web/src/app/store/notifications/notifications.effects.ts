import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { map, exhaustMap, catchError } from 'rxjs/operators';
import { ApiService } from '@core/services/api.service';
import { AppNotification } from '@core/services/notification.service';
import { NotificationActions } from './notifications.actions';

@Injectable()
export class NotificationsEffects {
  private readonly actions$ = inject(Actions);
  private readonly apiService = inject(ApiService);

  loadNotifications$ = createEffect(() =>
    this.actions$.pipe(
      ofType(NotificationActions.loadNotifications),
      exhaustMap(() =>
        this.apiService.get<AppNotification[]>('notifications').pipe(
          map(response => {
            if (response.success) {
              return NotificationActions.loadNotificationsSuccess({ notifications: response.data });
            }
            return NotificationActions.loadNotificationsFailure({ error: response.message });
          }),
          catchError(error =>
            of(NotificationActions.loadNotificationsFailure({
              error: error.error?.message || 'Failed to load notifications.'
            }))
          )
        )
      )
    )
  );

  markAsRead$ = createEffect(() =>
    this.actions$.pipe(
      ofType(NotificationActions.markAsRead),
      exhaustMap(({ id }) =>
        this.apiService.put<void>(`notifications/${id}/read`, {}).pipe(
          map(() => NotificationActions.markAsReadSuccess({ id })),
          catchError(() => of(NotificationActions.markAsReadSuccess({ id })))
        )
      )
    )
  );

  markAllAsRead$ = createEffect(() =>
    this.actions$.pipe(
      ofType(NotificationActions.markAllAsRead),
      exhaustMap(() =>
        this.apiService.put<void>('notifications/read-all', {}).pipe(
          map(() => NotificationActions.markAllAsReadSuccess()),
          catchError(() => of(NotificationActions.markAllAsReadSuccess()))
        )
      )
    )
  );
}
